
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;




DECLARE @CompanyId INT= 91;
DECLARE @CreateBy INT= 3245;

SELECT * FROM dbo.CRMCustomers WHERE CompanyId=@CompanyId
DELETE FROM dbo.CRMCustomers WHERE CompanyId=@CompanyId


SELECT * FROM dbo.CRMCustomerSalePersons WHERE CompanyId=@CompanyId
DELETE FROM dbo.CRMCustomerSalePersons WHERE CompanyId=@CompanyId





INSERT INTO dbo.CRMCustomers
(
    Name,
    RegionId,
    UOMId,
    Capicty,
    IndustryTypeId,
    EmailDomain,
    Fax,
    NTN,
    GSTRN,
    CellNo,
    OfficeNo,
    HomeNo,
    Email,
    SecondayEmail,
    Web,
    Company,
    ShippingAddress,
    ShippingProvinceId,
    ShippingRegionId,
    BillingAddress,
    BillingProvinceId,
    BillingRegionId,
    ImportAddress,
    ImportProvinceId,
    ImportRegionId,
    IsAllowPortal,
    IsActive,
    IsDeleted,
    CompanyId,
    CreatedBy,
    CreatedAt,
    ModifiedBy,
    ModifiedAt,
    ClusterTypeId,
    CityId
)
SELECT  [Organization Name],
       (SELECT TOP(1) Id FROM dbo.Regions WHERE LTRIM(RTRIM(LOWER(Name))) =  LTRIM(RTRIM(LOWER(LOWER(Region)))) AND CompanyId=@CompanyId ORDER BY Id) ,
       (SELECT TOP(1) Id FROM dbo.UOMS WHERE LTRIM(RTRIM(LOWER(Name))) =  LTRIM(RTRIM(LOWER(LOWER(UOM)))) AND CompanyId=@CompanyId ORDER BY Id),
	   [Total Production Capacity-Monthly],
	   NULL,
	   NULL,
	   NULL,
	   NULL,
	   NULL,
	   NULL,
	   NULL,
	   NULL,
	   [Primary Email],
	   NULL,
	   NULL,
	   NULL,
	   NULL,
	   NULL,
       (SELECT TOP(1) Id FROM dbo.Regions WHERE LTRIM(RTRIM(LOWER(Name))) =  LTRIM(RTRIM(LOWER(LOWER([Shipping City])))) AND CompanyId=@CompanyId ORDER BY Id),
	   NULL,
	   NULL,
	   NULL,
	   NULL,
	   NULL,
	   NULL,
	   0,
	   1,
	   0,      -- IsDeleted - bit
    @CompanyId,         -- CompanyId - int
    @CreateBy,         -- CreatedBy - int
    GETDATE(), -- CreatedAt - datetime
    NULL,         -- ModifiedBy - int
    NULL,  -- ModifiedAt - datetime,
	(SELECT TOP(1) Id FROM dbo.ClusterTypes WHERE LTRIM(RTRIM(LOWER(Name))) =  LTRIM(RTRIM(LOWER(LOWER(Cluster)))) AND CompanyId=@CompanyId ORDER BY Id),
	 ISNULL( (SELECT TOP(1) Id FROM dbo.Regions WHERE LTRIM(RTRIM(LOWER(Name))) =  LTRIM(RTRIM(LOWER(LOWER([Sales City])))) AND CompanyId=@CompanyId ORDER BY Id),0)
 FROM dbo.CRMTempCustomerImport



DECLARE db_cursor CURSOR
FOR
    SELECT Id,Name FROM dbo.CRMCustomers
DECLARE  @CustomerId INT,
@Name VARCHAR(MAX);
OPEN db_cursor;
FETCH NEXT FROM db_cursor INTO  @CustomerId,@Name;
WHILE @@FETCH_STATUS = 0
BEGIN


DECLARE @SalePerson VARCHAR(1000)='';
SET @SalePerson=(SELECT TOP(1) [user name] FROM dbo.CRMTempCustomerImport WHERE LTRIM(RTRIM(LOWER([Organization Name]))) =  LTRIM(RTRIM(LOWER(LOWER(@Name)))) ORDER BY [user name])
INSERT INTO dbo.CRMCustomerSalePersons
(
    CRMCustomerId,
    UserId,
    CreatedAt,
    CreatedBy,
    ModifiedAt,
    ModifiedBy,
    CompanyId,
    IsDeleted,
    CategroyId
)

SELECT 
	@CustomerId,         -- CRMCustomerId - int
   ISNULL( (SELECT TOP(1) Id FROM dbo.Users WHERE LTRIM(RTRIM(LOWER(Username))) =  LTRIM(RTRIM(LOWER(LOWER(@SalePerson)))) AND CompanyId=@CompanyId ORDER BY Id),0),         -- UserId - int
	GETDATE(), -- CreatedAt - datetime
    @CreateBy,         -- CreatedBy - int
    NULL,         -- ModifiedBy - int
    NULL,  -- ModifiedAt - datetime,
	@CompanyId,         -- CompanyId - int
	0,
    Id          -- CategroyId - int FROM dbo.ProductCategories
	FROM dbo.ProductCategories
WHERE Name IN('DYES' , 'CHEM', 'INKS-WATERBASED');


	FETCH NEXT FROM db_cursor INTO  @CustomerId,@Name;

	END

CLOSE db_cursor;
DEALLOCATE db_cursor;






GO




--USE amex_dev


--SELECT * FROM dbo.CRMCustomers WHERE RegionId IS  NULL
--SELECT * FROM dbo.CRMTempCustomerImport WHERE [Organization Name] IN(SELECT Name FROM dbo.CRMCustomers WHERE RegionId IS  NULL) ORDER BY Region
--SELECT DISTINCT(dbo.Regions) FROM dbo.CRMTempCustomerImport
--SELECT * FROM dbo.Regions WHERE CompanyId=91


--SELECT * FROM dbo.CRMCustomers WHERE UOMId IS  NULL
--SELECT * FROM dbo.CRMTempCustomerImport WHERE [Organization Name] IN(SELECT Name FROM dbo.CRMCustomers WHERE UOMId IS  NULL) ORDER BY UOM
--SELECT DISTINCT(UOM) FROM dbo.CRMTempCustomerImport
--SELECT * FROM dbo.UOMS WHERE CompanyId=91


--SELECT * FROM dbo.CRMCustomers WHERE ShippingRegionId IS  NULL
--SELECT * FROM dbo.CRMTempCustomerImport WHERE [Organization Name] IN(SELECT Name FROM dbo.CRMCustomers WHERE ShippingRegionId IS  NULL) ORDER BY [Shipping City]
--SELECT DISTINCT([Shipping City]) FROM dbo.CRMTempCustomerImport
--SELECT * FROM dbo.Regions WHERE CompanyId=91


--SELECT * FROM dbo.CRMCustomers WHERE ClusterTypeId IS  NULL
--SELECT * FROM dbo.CRMTempCustomerImport WHERE [Organization Name] IN(SELECT Name FROM dbo.CRMCustomers WHERE ClusterTypeId IS  NULL) ORDER BY Cluster
--SELECT DISTINCT([Shipping City]) FROM dbo.CRMTempCustomerImport
--SELECT * FROM dbo.Regions WHERE CompanyId=91

--SELECT * FROM dbo.CRMCustomers WHERE CityId IS  NULL
--SELECT * FROM dbo.CRMTempCustomerImport WHERE [Organization Name] IN(SELECT Name FROM dbo.CRMCustomers WHERE ClusterTypeId IS  NULL) ORDER BY Cluster
--SELECT DISTINCT([Shipping City]) FROM dbo.CRMTempCustomerImport
--SELECT * FROM dbo.Regions WHERE CompanyId=91


----SELECT DISTINCT(CRMCustomerId) FROM dbo.CRMCustomerSalePersons
----SELECT * FROM dbo.CRMCustomers WHERE Id NOT IN(SELECT DISTINCT(CRMCustomerId) FROM dbo.CRMCustomerSalePersons)

--SELECT * FROM dbo.CRMCustomerSalePersons WHERE UserId=0


--SELECT * FROM dbo.CRMCustomers WHERE CityId IS  NULL
--SELECT * FROM dbo.CRMTempCustomerImport WHERE [Organization Name] IN(SELECT Name FROM dbo.CRMCustomers WHERE Id IN (SELECT CRMCustomerId FROM dbo.CRMCustomerSalePersons WHERE UserId=0)) ORDER BY [user name]
--SELECT DISTINCT([Shipping City]) FROM dbo.CRMTempCustomerImport

--SELECT * FROM dbo.vw_CRMUsers WHERE CompanyId=91 AND UserTypeId=5



















	
  
 