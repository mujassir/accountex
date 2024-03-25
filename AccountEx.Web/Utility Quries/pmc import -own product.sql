

--DELETE FROM dbo.PMCs
--DELETE FROM dbo.PMCItems

--DBCC CHECKIDENT ('[PMCs]', RESEED, 0);
--DBCC CHECKIDENT ('[PMCItems]', RESEED, 0);

DECLARE @CompanyId INT = 91;
DECLARE @CreateBy INT = 3245;
DECLARE @FiscalId INT = 168;
DECLARE @VoucherNo INT = 1;
DECLARE db_cursor CURSOR FOR
SELECT [Organization Name]
FROM dbo.PMCTempDataExport
WHERE [Organization Name] IS NOT NULL
GROUP BY [Organization Name];
DECLARE @Customer VARCHAR(MAX);

SET @VoucherNo = 1;
OPEN db_cursor;
FETCH NEXT FROM db_cursor
INTO @Customer;
WHILE @@FETCH_STATUS = 0
BEGIN
DECLARE @Sn INT=1

DECLARE @CustomerId INT=0
 DECLARE @PmcId INT=0

SET @CustomerId= (SELECT TOP (1)
                   Id
            FROM dbo.CRMCustomers
            WHERE LTRIM(RTRIM(LOWER(Name))) = LTRIM(RTRIM(LOWER(LOWER(@Customer))))
            ORDER BY Id)

IF(NOT EXISTS(SELECT TOP(1) * FROM dbo.PMCs WHERE FiscalId=@FiscalId AND CompanyId=@CompanyId AND CustomerId=@CustomerId))
BEGIN	
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
        '2017-12-31', -- Date - datetime
        ISNULL(
        (
            SELECT TOP (1)
                   Id
            FROM dbo.CRMCustomers
            WHERE LTRIM(RTRIM(LOWER(Name))) = LTRIM(RTRIM(LOWER(LOWER(@Customer))))
            ORDER BY Id
        ),
        0
              ),      -- CustomerId - int
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

	END
	ELSE
    BEGIN
	PRINT('Customer Id:'+CONVERT(VARCHAR(50),@CustomerId))
	SELECT @PmcId=(SELECT TOP(1) Id FROM dbo.PMCs WHERE FiscalId=@FiscalId AND CompanyId=@CompanyId AND CustomerId=@CustomerId ORDER BY Id DESC)

	END
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
           ISNULL( [Avg Price],0),
           ISNULL( [Annual QTY],0),
           [Annual Potential PKR],
           0,
           CASE WHEN [Counter]='Yes' THEN 1 ELSE 0 END,
           1,
           0,          -- IsDeleted - bit
           @CompanyId, -- CompanyId - int
           @CreateBy,  -- CreatedBy - int
           GETDATE(),  -- CreatedAt - datetime
           NULL,       -- ModifiedBy - int
           NULL,       -- ModifiedAt - datetime,
		  [Annual RPPL Potential],
		 ISNULL((SELECT TOP(1) UserId FROM dbo.CRMCustomerSalePersons WHERE CRMCustomerId=@CustomerId),0)
    FROM dbo.PMCTempDataExport
    WHERE [Organization Name] = @Customer;
	SET @VoucherNo = @VoucherNo+1;
    FETCH NEXT FROM db_cursor
    INTO @Customer;
END;
CLOSE db_cursor;
DEALLOCATE db_cursor;
GO