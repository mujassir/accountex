USE amex_dev

DELETE FROM dbo.CRMProjects

DBCC CHECKIDENT ('[CRMProjects]', RESEED, 0);


Alter Table TempImportCRMProjects
Add Id Int Identity(1, 1)
Go


DECLARE @CompanyId INT = 91;
DECLARE @CreateBy INT = 3245;
DECLARE @FiscalId INT = 168;
DECLARE @VoucherNo INT = 1;
DECLARE db_cursor CURSOR LOCAL FOR
SELECT Id,[Organization Name],[PMC Replace],[WITH]
FROM dbo.TempImportCRMProjects
WHERE [Organization Name] IS NOT NULL
AND year( CONVERT(DateTime, LTRIM(RTRIM( [Expected Close Date])), 105))=2017
--GROUP BY [Organization Name],[PMC Replace],[WITH];
DECLARE @Customer VARCHAR(MAX),
 @PmcProduct VARCHAR(MAX),
 @Id INT,
 @ProjectProduct VARCHAR(MAX);;
 DECLARE @PmcDate DATETIME;

SET @VoucherNo = 1;
OPEN db_cursor;
FETCH NEXT FROM db_cursor
INTO  @Id,@Customer,@PmcProduct,@ProjectProduct;
WHILE @@FETCH_STATUS = 0
BEGIN
DECLARE @Sn INT=1


 --SELECT TOP (1)
 --                  @PmcDate=[pmc close date]
 --           FROM dbo.PMCTempDataExport
 --           WHERE LTRIM(RTRIM(LOWER([Organization Name]))) = LTRIM(RTRIM(LOWER(LOWER(@Customer))))
 --           ORDER BY [Organization Name]

DECLARE @CustomerId INT=0,
   @PmcProductId INT=0,
    @ProjectProductId INT=0,
	@PmcItemId INT=0,
	@ActalPotential DECIMAL(18,2)=0.0;
DECLARE @strPmcDate VARCHAR(500);


 SET @CustomerId=ISNULL(
        (
            SELECT TOP (1)
                   Id
            FROM dbo.CRMCustomers
            WHERE LTRIM(RTRIM(LOWER(Name))) = LTRIM(RTRIM(LOWER(LOWER(@Customer))))
            ORDER BY Id
        ),0)

		 SET @PmcProductId=ISNULL(
        (
            SELECT TOP (1)
                   Id
            FROM dbo.CRMProducts
            WHERE LTRIM(RTRIM(LOWER(Name))) = LTRIM(RTRIM(LOWER(LOWER(@PmcProduct))))
            ORDER BY Id
        ),0)

		 SET @ProjectProductId=ISNULL(
        (
            SELECT TOP (1)
                   Id
            FROM dbo.CRMProducts
            WHERE LTRIM(RTRIM(LOWER(Name))) = LTRIM(RTRIM(LOWER(LOWER(@ProjectProduct))))
            ORDER BY Id
        ),0)


		SELECT TOP(1) @PmcItemId=pmci.Id,@ActalPotential=ISNULL(pmci.AnnualPotential,0.0) FROM dbo.PMCs pmc INNER JOIN dbo.PMCItems pmci ON pmci.PMCId = pmc.Id
		WHERE pmc.FiscalId=@FiscalId AND pmc.CustomerId=@CustomerId AND pmci.ProductId=@PmcProductId
		ORDER BY pmc.Id


		IF(@CustomerId=0) BEGIN
		PRINT('Missing Customer:'+ @Customer)
		END

 --IF(year(@PmcDate)=2017) BEGIN

 INSERT INTO dbo.CRMProjects
 (
     VoucherNumber,
     PMCItemId,
     CustomerId,
     ProductId,
     CurrencyId,
     ExcRate,
     Price,
     Quantity,
     StatusId,
     LostReasonId,
     Remarks,
     IsDeleted,
     CompanyId,
     CreatedBy,
     CreatedAt,
     ModifiedBy,
     ModifiedAt,
     Date,
     Potential,
     PotentialPercent,
     FiscalId,
     SalePersonId
 )
 SELECT @VoucherNo,
          @PmcItemId,
           @CustomerId,
			@ProjectProductId,
		   (
               SELECT TOP (1)
                      Id
               FROM dbo.Currencies
               WHERE  CompanyId=@CompanyId AND LTRIM(RTRIM(LOWER(Unit))) = LTRIM(RTRIM(LOWER(LOWER([PMC Currency]))))
               ORDER BY Id
           ),
		    [Ex rate],
            CAST(ISNULL([Price],0) AS DECIMAL(18,3)),
           ISNULL([Targeted Quantity],0),
           (
               SELECT TOP (1)
                      Id
               FROM dbo.ProductStatuses
               WHERE  CompanyId=@CompanyId AND LTRIM(RTRIM(LOWER(Name))) = LTRIM(RTRIM(LOWER(LOWER([Sales Stage]))))
               ORDER BY Id
           ),
           0,
           '',
           0,          -- IsDeleted - bit
           @CompanyId, -- CompanyId - int
           @CreateBy,  -- CreatedBy - int
           GETDATE(),  -- CreatedAt - datetime
           NULL,       -- ModifiedBy - int
           NULL,       -- ModifiedAt - datetime,
		  '2017-12-31',
		 ISNULL([Targeted Revenue in Rs#],0.0),
		CASE WHEN @PmcItemId>0 THEN CAST( (ISNULL([Targeted Revenue in Rs#],0.0)/@ActalPotential)*100 AS DECIMAL(18,2)) ELSE 0.0 END,
		 @FiscalId,
		  ISNULL((SELECT TOP(1) UserId FROM dbo.CRMCustomerSalePersons WHERE CRMCustomerId=@CustomerId),0)
    FROM dbo.TempImportCRMProjects
    WHERE Id=@Id
   
	SET @VoucherNo = @VoucherNo+1;
    FETCH NEXT FROM db_cursor
    INTO @Id,@Customer,@PmcProduct,@ProjectProduct;;
END
CLOSE db_cursor;
DEALLOCATE db_cursor;

GO