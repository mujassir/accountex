
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO




--DELETE FROM dbo.ProductGroups WHERE CompanyId=91
--DELETE FROM dbo.ProductSubGroups WHERE CompanyId=91


DECLARE @CompanyId INT= 91;
DECLARE @CreateBy INT= 3245;
DECLARE db_cursor CURSOR
FOR
    SELECT [Group] FROM dbo.CRMTempImportedProducts
	WHERE [Group] IS NOT NULL
	GROUP BY [Group]
DECLARE @Group VARCHAR(MAX);
OPEN db_cursor;
FETCH NEXT FROM db_cursor INTO @Group;
WHILE @@FETCH_STATUS = 0
BEGIN
IF(NOT EXISTS(SELECT TOP(1) * FROM dbo.ProductGroups WHERE LOWER(Name)=LOWER(@Group)))
BEGIN
DECLARE @DivisionId INT=0
  SET @DivisionId=ISNULL((SELECT TOP(1) Id FROM dbo.Divisions WHERE LOWER(Name)=LOWER((SELECT TOP(1) Division FROM dbo.CRMTempImportedProducts WHERE LOWER([Group])=LOWER(@Group)))),0)
INSERT INTO dbo.ProductGroups
(
    Name,
    IsDeleted,
    CompanyId,
    CreatedBy,
    CreatedAt,
    ModifiedBy,
    ModifiedAt,
	DivisionId
)
VALUES
(   @Group,        -- Name - varchar(500)
    0,      -- IsDeleted - bit
    @CompanyId,         -- CompanyId - int
    @CreateBy,         -- CreatedBy - int
    GETDATE(), -- CreatedAt - datetime
    NULL,         -- ModifiedBy - int
    NULL,  -- ModifiedAt - datetime,
	@DivisionId
    )

	END
	FETCH NEXT FROM db_cursor INTO @Group;

	END

CLOSE db_cursor;
DEALLOCATE db_cursor;

GO



GO




DECLARE @CompanyId INT= 91;
DECLARE @CreateBy INT= 3245;
DECLARE db_cursor CURSOR
FOR
    SELECT [Sub-Group] FROM dbo.CRMTempImportedProducts
	WHERE [Sub-Group] IS NOT NULL
	GROUP BY [Sub-Group]
DECLARE @SubGroup VARCHAR(MAX);
OPEN db_cursor;
FETCH NEXT FROM db_cursor INTO @SubGroup;
WHILE @@FETCH_STATUS = 0
BEGIN
IF(NOT EXISTS(SELECT TOP(1) * FROM dbo.ProductSubGroups WHERE LOWER(Name)=LOWER(@SubGroup)))
BEGIN
DECLARE @GroupId INT=0
  SET @GroupId=ISNULL((SELECT TOP(1) Id FROM dbo.ProductGroups WHERE LOWER(Name)=LOWER((SELECT TOP(1) [Group] FROM dbo.CRMTempImportedProducts WHERE LOWER([Sub-Group])=LOWER(@SubGroup)))),0)
INSERT INTO dbo.ProductSubGroups
(
    Name,
	GroupId,
    IsDeleted,
    CompanyId,
    CreatedBy,
    CreatedAt,
    ModifiedBy,
    ModifiedAt
	
)
VALUES
(   @SubGroup,        -- Name - varchar(500)
	@GroupId,
    0,      -- IsDeleted - bit
    @CompanyId,         -- CompanyId - int
    @CreateBy,         -- CreatedBy - int
    GETDATE(), -- CreatedAt - datetime
    NULL,         -- ModifiedBy - int
    NULL  -- ModifiedAt - datetime,
	
    )

	END
	FETCH NEXT FROM db_cursor INTO @SubGroup;

	END

CLOSE db_cursor;
DEALLOCATE db_cursor;




GO



GO

--SELECT * FROM dbo.CRMVendors WHERE CompanyId=91
--DELETE FROM dbo.CRMVendors WHERE CompanyId=91


DECLARE @CompanyId INT= 91;
DECLARE @CreateBy INT= 3245;
DECLARE db_cursor CURSOR
FOR
    SELECT [Vendor Name] FROM dbo.CRMTempImportedProducts
	WHERE [Vendor Name] IS NOT NULL
	GROUP BY [Vendor Name]
DECLARE @Vendor VARCHAR(MAX);
OPEN db_cursor;
FETCH NEXT FROM db_cursor INTO @Vendor;
WHILE @@FETCH_STATUS = 0
BEGIN
IF(NOT EXISTS(SELECT TOP(1) * FROM dbo.CRMVendors WHERE LOWER(Name)=LOWER(@Vendor)))
BEGIN
INSERT INTO dbo.CRMVendors
(
    Name,
    ContactPerson,
    UOMId,
    CellNo,
    Email,
    Fax,
    Web,
    NTN,
    Address,
    IsDeleted,
    CompanyId,
    CreatedBy,
    CreatedAt,
    ModifiedBy,
    ModifiedAt,
    Supplier,
    Type
)
VALUES
(   @Vendor,        -- Name - varchar(50)
    '',        -- ContactPerson - varchar(50)
    0,         -- UOMId - int
    '',        -- CellNo - varchar(50)
    '',        -- Email - varchar(50)
    '',        -- Fax - varchar(50)
    '',        -- Web - varchar(50)
    '',        -- NTN - varchar(50)
    '',        -- Address - varchar(500)
    0,      -- IsDeleted - bit
    @CompanyId,         -- CompanyId - int
    @CreateBy,         -- CreatedBy - int
    GETDATE(), -- CreatedAt - datetime
    NULL,         -- ModifiedBy - int
    NULL,  -- ModifiedAt - datetime,
    '',        -- Supplier - varchar(500)
    0          -- Type - tinyint
    )

	END
	FETCH NEXT FROM db_cursor INTO @Vendor;

	END

CLOSE db_cursor;
DEALLOCATE db_cursor;




GO


GO




DECLARE @CompanyId INT= 91;
DECLARE @CreateBy INT= 3245;


SELECT * FROM dbo.CRMProducts WHERE CompanyId=@CompanyId
DELETE FROM dbo.CRMProducts WHERE CompanyId=@CompanyId


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
SELECT  [Product Name],
       (SELECT TOP(1) Id FROM dbo.ProductCategories WHERE LTRIM(RTRIM(LOWER(Name))) =  LTRIM(RTRIM(LOWER(LOWER([Product Category])))) ORDER BY Id),
       (SELECT TOP(1) Id FROM dbo.Divisions WHERE LTRIM(RTRIM(LOWER(Name))) =  LTRIM(RTRIM(LOWER(LOWER(Division)))) ORDER BY Id),
       (SELECT TOP(1) Id FROM dbo.ProductGroups WHERE LTRIM(RTRIM(LOWER(Name))) =  LTRIM(RTRIM(LOWER(LOWER([Group])))) ORDER BY Id),
       (SELECT TOP(1) Id FROM dbo.ProductSubGroups WHERE LTRIM(RTRIM(LOWER(Name))) =  LTRIM(RTRIM(LOWER(LOWER([Sub-Group])))) ORDER BY Id),
	    (SELECT TOP(1) Id FROM dbo.CRMVendors WHERE LTRIM(RTRIM(LOWER(Name))) =  LTRIM(RTRIM(LOWER(LOWER([Vendor Name])))) ORDER BY Id),
       '',
	   '',
	   '',
	   '',
	   CASE WHEN LOWER([Vendor Name]) LIKE'%rppl%' THEN 1 ELSE 0 END,
	   0,      -- IsDeleted - bit
    @CompanyId,         -- CompanyId - int
    @CreateBy,         -- CreatedBy - int
    GETDATE(), -- CreatedAt - datetime
    NULL,         -- ModifiedBy - int
    NULL,  -- ModifiedAt - datetime,
	''
 FROM dbo.CRMTempImportedProducts



GO




USE amex_dev



SELECT [Vendor Name] FROM dbo.CRMTempImportedProducts WHERE [Vendor Name] LIKE '%rppl%'


SELECT * FROM dbo.CRMProducts WHERE IsOwnProduct=1

SELECT * FROM dbo.CRMProducts WHERE DivisionId IS  NULL
SELECT * FROM dbo.CRMTempImportedProducts WHERE [Product Name] IN(SELECT Name FROM dbo.CRMProducts WHERE DivisionId IS  NULL) ORDER BY Division
SELECT DISTINCT(Division) FROM dbo.CRMTempImportedProducts
SELECT * FROM dbo.Divisions


SELECT * FROM dbo.CRMProducts WHERE CategoryId IS  NULL
SELECT * FROM dbo.CRMTempImportedProducts WHERE [Product Name] IN(SELECT Name FROM dbo.CRMProducts WHERE CategoryId IS  NULL) ORDER BY [Product Category]
SELECT DISTINCT([Product Category]) FROM dbo.CRMTempImportedProducts
SELECT * FROM dbo.ProductCategories


SELECT * FROM dbo.CRMProducts WHERE GroupId IS NULL 
SELECT * FROM dbo.CRMTempImportedProducts WHERE [Product Name] IN(SELECT Name FROM dbo.CRMProducts WHERE [GroupId] IS   NULL) ORDER BY [Group]
SELECT DISTINCT([Group]) FROM dbo.CRMTempImportedProducts
SELECT * FROM dbo.ItemGroups


SELECT * FROM dbo.CRMProducts WHERE GroupId IS NOT  NULL



SELECT * FROM dbo.CRMProducts WHERE SubGroupId IS  NULL
SELECT * FROM dbo.CRMTempImportedProducts WHERE [Product Name] IN(SELECT Name FROM dbo.CRMProducts WHERE SubGroupId IS  NULL) ORDER BY [Sub-Group]
SELECT DISTINCT([Sub-Group]) FROM dbo.CRMTempImportedProducts
SELECT * FROM dbo.ProductSubGroups

SELECT * FROM dbo.CRMProducts WHERE VendorId IS  NULL
SELECT * FROM dbo.CRMTempImportedProducts WHERE [Product Name] IN(SELECT Name FROM dbo.CRMProducts WHERE VendorId IS  NULL) ORDER BY [Vendor Name]
SELECT DISTINCT([Vendor Name]) FROM dbo.CRMTempImportedProducts
SELECT * FROM dbo.CRMVendors WHERE Name='ALKA CHEMICALS'


SELECT TOP(1) Id FROM dbo.ProductGroups WHERE LTRIM(RTRIM(LOWER(Name))) = LTRIM(RTRIM(LOWER(LOWER('AFTERTREATING AGENTS/FASTNESS IMPROVING'))))

SELECT TOP(1) Id FROM dbo.CRMVendors WHERE LTRIM(RTRIM(LOWER(Name))) LIKE'%'+LTRIM(RTRIM(LOWER(LOWER('AFTERTREATING AGENTS/FASTNESS IMPROVING'))))+'%' 

SELECT TOP(1) Id FROM dbo.ProductGroups WHERE Name LIKE '%AFTERTREATING AGENTS/FASTNESS IMPROVING%'

SELECT * FROM dbo.CRMVendors WHERE Id IN(SELECT VendorId FROM dbo.CRMProducts)



SELECT * FROM dbo.ProductSubGroups WHERE Id IN(SELECT SubGroupId FROM dbo.CRMProducts)















	
  
 