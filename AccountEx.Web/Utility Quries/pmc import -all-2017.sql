USE amex_dev

DELETE FROM dbo.PMCs
DELETE FROM dbo.PMCItems

DBCC CHECKIDENT ('[PMCs]', RESEED, 0);
DBCC CHECKIDENT ('[PMCItems]', RESEED, 0);

DECLARE @CompanyId INT = 91;
DECLARE @CreateBy INT = 3245;
DECLARE @FiscalId INT = 168;
DECLARE @VoucherNo INT = 1;
DECLARE db_cursor CURSOR FOR
SELECT [Organization Name]
FROM dbo.PMCTempDataExport
WHERE [Organization Name] IS NOT NULL
AND year([pmc close date])=2017
GROUP BY [Organization Name];
DECLARE @Customer VARCHAR(MAX);
 DECLARE @PmcDate DATETIME;

SET @VoucherNo = 1;
OPEN db_cursor;
FETCH NEXT FROM db_cursor
INTO @Customer;
WHILE @@FETCH_STATUS = 0
BEGIN
DECLARE @Sn INT=1


 --SELECT TOP (1)
 --                  @PmcDate=[pmc close date]
 --           FROM dbo.PMCTempDataExport
 --           WHERE LTRIM(RTRIM(LOWER([Organization Name]))) = LTRIM(RTRIM(LOWER(LOWER(@Customer))))
 --           ORDER BY [Organization Name]

DECLARE @CustomerId INT=0
DECLARE @strPmcDate VARCHAR(500);


 SET @CustomerId=ISNULL(
        (
            SELECT TOP (1)
                   Id
            FROM dbo.CRMCustomers
            WHERE LTRIM(RTRIM(LOWER(Name))) = LTRIM(RTRIM(LOWER(LOWER(@Customer))))
            ORDER BY Id
        ),0)


		IF(@CustomerId=0) BEGIN
		PRINT('Missing:'+ @Customer)
		END

 DECLARE @PmcId INT=0
 --IF(year(@PmcDate)=2017) BEGIN
 SET @FiscalId=168;
 SELECT @strPmcDate='2017-12-31'
 INSERT INTO dbo.PMCs
    (
        VoucherNumber,
        InvoiceNumber,
        Date,
        CustomerId,
        SalePersonId,
        RegionId,
        Comments,
        IsDeleted,
        FiscalId,
        CompanyId,
        CreatedBy,
        CreatedAt,
        ModifiedBy,
        ModifiedAt
    )
    VALUES
    (   @VoucherNo,   -- VoucherNumber - int
        @VoucherNo,   -- InvoiceNumber - int
        @strPmcDate, -- Date - datetime
       @CustomerId,      -- CustomerId - int
        0,            -- SalePersonId - int
        0,            -- RegionId - int
        '',           -- Comments - varchar(max)
        0,            -- IsDeleted - bit
        @FiscalId,    -- FiscalId - int
        @CompanyId,   -- CompanyId - int
        @CreateBy,    -- CreatedBy - int
        GETDATE(),    -- CreatedAt - datetime
        NULL,         -- ModifiedBy - int
        NULL          -- ModifiedAt - datetime
        );
    SET @PmcId = SCOPE_IDENTITY();
    INSERT INTO dbo.PMCItems
    (
        PMCId,
        SN,
        ProductId,
        CurrencyId,
        ExcRate,
        Price,
        AnnualQty,
        AnnualPotential,
        IsAnnualRPLPotential,
        IsCounter,
        IsActive,
        IsDeleted,
        CompanyId,
        CreatedBy,
        CreatedAt,
        ModifiedBy,
        ModifiedAt,
        AnnualRPLPotential,
        SalePersonId
    )
    SELECT @PmcId,
            ROW_NUMBER() OVER (ORDER BY product),
           (
               SELECT TOP (1)
                      Id
               FROM dbo.CRMProducts
               WHERE LTRIM(RTRIM(LOWER(Name))) = LTRIM(RTRIM(LOWER(LOWER(product))))
               ORDER BY Id
           ),
           (
               SELECT TOP (1)
                      Id
               FROM dbo.Currencies
               WHERE  CompanyId=@CompanyId AND LTRIM(RTRIM(LOWER(Unit))) = LTRIM(RTRIM(LOWER(LOWER(currency))))
               ORDER BY Id
           ),
           [Ex rate],
           ISNULL( [Price],0),
           ISNULL( [Annual QTY],0),
           [Annual Potential Rs],
           0,
           CASE WHEN [Counter]='Yes' THEN 1 ELSE 0 END,
           1,
           0,          -- IsDeleted - bit
           @CompanyId, -- CompanyId - int
           @CreateBy,  -- CreatedBy - int
           GETDATE(),  -- CreatedAt - datetime
           NULL,       -- ModifiedBy - int
           NULL,       -- ModifiedAt - datetime,
		  ISNULL( [Annual RPPL Potential],0.0),
		 ISNULL((SELECT TOP(1) UserId FROM dbo.CRMCustomerSalePersons WHERE CRMCustomerId=@CustomerId),0)
    FROM dbo.PMCTempDataExport
    WHERE [Organization Name] = @Customer AND year([pmc close date])=2017
	SET @VoucherNo = @VoucherNo+1;
    FETCH NEXT FROM db_cursor
    INTO @Customer;
END
CLOSE db_cursor;
DEALLOCATE db_cursor;

GO