USE xameer_accountex

SELECT * FROM dbo.CRMVendors WHERE Name='COLOURTEX'


SELECT * FROM dbo.RudolfProducts



SELECT DISTINCT(Category) FROM dbo.RudolfProducts


SELECT * FROM dbo.ProductCategories WHERE Name='DYES'
SELECT * FROM dbo.Divisions WHERE Name='Dyes'
SELECT * FROM dbo.CRMVendors WHERE Name='COLOURTEX'



SELECT DISTINCT(Division) FROM dbo.RudolfProducts

SELECT DISTINCT(Vendor) FROM dbo.RudolfProducts

SELECT DISTINCT([Group]) FROM dbo.RudolfProducts

SELECT DISTINCT ([Sub Group]) FROM dbo.RudolfProducts


SELECT COUNT(Id) FROM dbo.CRMProducts




DECLARE @CompanyId INT= 91;
DECLARE @CreateBy INT= 3245;
DECLARE db_cursor CURSOR
FOR
    SELECT Name,[Group],[Sub Group] FROM dbo.RudolfProducts
DECLARE @Name VARCHAR(MAX);
DECLARE @pgroup VARCHAR(MAX);
DECLARE @psubgroup VARCHAR(MAX);
OPEN db_cursor;
FETCH NEXT FROM db_cursor INTO @Name,@pgroup,@psubgroup;
WHILE @@FETCH_STATUS = 0
BEGIN


INSERT INTO dbo.CRMProducts
(
    Name,
    CategoryId,
    DivisionId,
    GroupId,
    SubGroupId,
    VendorId,
    TDSFileUrl,
    TDSFileName,
    MSDSFileUrl,
    MSDSFileName,
    IsOwnProduct,
    IsDeleted,
    CompanyId,
    CreatedBy,
    CreatedAt,
    ModifiedBy,
    ModifiedAt,
    HSCode
)
SELECT  @Name,
       1,
      3,
       (SELECT TOP(1) Id FROM dbo.ProductGroups WHERE LTRIM(RTRIM(LOWER(Name))) =  LTRIM(RTRIM(LOWER(LOWER(@pgroup)))) ORDER BY Id),
       (SELECT TOP(1) Id FROM dbo.ProductSubGroups WHERE LTRIM(RTRIM(LOWER(Name))) =  LTRIM(RTRIM(LOWER(LOWER(@psubgroup)))) ORDER BY Id),
	    369,
       '',
	   '',
	   '',
	   '',
	   0,
	   0,      -- IsDeleted - bit
    @CompanyId,         -- CompanyId - int
    @CreateBy,         -- CreatedBy - int
    GETDATE(), -- CreatedAt - datetime
    NULL,         -- ModifiedBy - int
    NULL,  -- ModifiedAt - datetime,
	''

	FETCH NEXT FROM db_cursor INTO @Name,@pgroup,@psubgroup;

	END

CLOSE db_cursor;
DEALLOCATE db_cursor;

GO
