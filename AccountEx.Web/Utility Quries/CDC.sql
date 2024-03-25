

USE master 
GO 
SELECT [name], database_id, is_cdc_enabled  
FROM sys.databases       
GO   

EXEC sp_changedbowner 'sa'

GO
EXEC sys.sp_cdc_enable_db
GO

GO
EXEC sys.sp_cdc_disable_db
GO


GO 
SELECT [name], is_tracked_by_cdc  
FROM sys.tables 
WHERE is_tracked_by_cdc = 1


EXEC sys.sp_cdc_enable_table @source_schema = N'dbo', @source_name   = N'Transactions', @role_name     = NULL 
EXEC sys.sp_cdc_enable_table @source_schema = N'dbo', @source_name   = N'Accounts', @role_name     = NULL 
EXEC sys.sp_cdc_enable_table @source_schema = N'dbo', @source_name   = N'AccountDetails', @role_name     = NULL 

EXEC sys.sp_cdc_enable_table @source_schema = N'dbo', @source_name   = N'Sales', @role_name     = NULL 
EXEC sys.sp_cdc_enable_table @source_schema = N'dbo', @source_name   = N'SaleItems', @role_name     = NULL 

EXEC sys.sp_cdc_enable_table @source_schema = N'dbo', @source_name   = N'Vouchers', @role_name     = NULL 
EXEC sys.sp_cdc_enable_table @source_schema = N'dbo', @source_name   = N'VoucherItems', @role_name     = NULL 

EXEC sys.sp_cdc_enable_table @source_schema = N'dbo', @source_name   = N'Users', @role_name     = NULL 




UPDATE Transactions SET ModifiedAt = GETDATE() WHERE ID = 129390

delete FROM Transactions WHERE ID = 129390

 SELECT TOP 10  * FROM Transactions ORDER BY 1 DESC 


SELECT  * FROM CDC.DBO_TRANSACTIONS_CT WHERE ID = 129390