USE amex_dev



SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO



DECLARE @CompanyId INT= 88;
DECLARE @UserId INT= 3229;
DECLARE db_cursor CURSOR
FOR
    SELECT  Item
    FROM    dbo.SkyeSeedProducts 
DECLARE @Name VARCHAR(MAX);
OPEN db_cursor;
FETCH NEXT FROM db_cursor INTO @Name;
  DECLARE @ParentId INT=10
  DECLARE @Counter INT=1001;
WHILE @@FETCH_STATUS = 0
    BEGIN  

     

        
		 DECLARE @Code VARCHAR(500)='PT-'+CONVERT(varchar(10), @Counter);

        INSERT  INTO [dbo].[Accounts]
                ( [ParentId] ,
                  [Name] ,
                  [DisplayName] ,
                  [AccountCode] ,
                  [Level] ,
                  [IsDeleted] ,
                  [CompanyId] ,
                  [CreatedBy] ,
                  [CreatedAt]
                )
                SELECT  @ParentId [ParentId] ,
                        @Name ,
                        @Name ,
                        @Code,
                        4 ,
                        0 ,
                        @CompanyId [CompanyId] ,
                        @UserId [CreatedBy] ,
                        GETDATE() [CreatedAt]


	 ------------------------------------Get Auto generated Id-----------------------------------------------------
        DECLARE @AUTOID INT;
        SELECT  @AUTOID = SCOPE_IDENTITY();
  --------------------------------------------------------------------------------------------------------------


        INSERT  INTO AccountDetails
                ( [AccountId] ,
                  [AccountDetailFormId] ,
                  [Code] ,
                  [Name] ,
				  BrandName,
                  [IsDeleted] ,
                  [CompanyId] ,
                  [CreatedBy] ,
                  [CreatedAt]
	            )
                SELECT  @AUTOID ,
                        5,
                        @Code,
                        @Name,
						NULL,
                        0 ,
                        @CompanyId [CompanyId] ,
                        @UserId [CreatedBy] ,
                        GETDATE() [CreatedAt]




						SET @Counter=@Counter+1;
       FETCH NEXT FROM db_cursor INTO @Name;
    END;
CLOSE db_cursor;
DEALLOCATE db_cursor;











	
  
 