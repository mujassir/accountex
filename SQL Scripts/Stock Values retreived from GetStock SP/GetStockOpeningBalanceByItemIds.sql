

ALTER PROCEDURE [dbo].[GetStockOpeningBalanceByItemIds]
@ItemIds			varchar(2000),
@CompanyId			INT,
@FiscalId			INT

as



--DECLARE
--@COMPANYID int = 85,
--@FiscalId int = 157,
--@ItemIds  varchar(max) = '57267,57268'



DECLARE @FromDate	DATE = (SELECT FromDate FROM Fiscals WHERE ID = @FiscalId)

DECLARE @ToDate		DATE = (SELECT Convert(date, GetDate()))

DECLARE @TEMPTABLE table(

	[AccountId] [int] NOT NULL,
	[GroupName] [varchar](500) NULL,
	[Code] [varchar](50) NULL,
	[Name] [varchar](500) NULL,
	[ArticleNo] [varchar](50) NULL,
	[Location] [varchar](500) NULL,
	[SalePrice] [decimal](18, 2) NOT NULL,
	[AvgRate] [decimal](38, 6) NOT NULL,
	[Initial] [decimal](18, 4) NULL,
	[StockIn] [decimal](18, 4) NULL,
	[StockOut] [decimal](18, 4) NULL,
	[Balance] [decimal](38, 4) NOT NULL,
	[StockValue] [decimal](38, 6) NOT NULL,
	[BookValue] [decimal](38, 6) NOT NULL,
	[UnitValue] [decimal](38, 6) NOT NULL,
	[UnitPrice] [decimal](18, 4) NOT NULL,
	[AvgPurchaseRate] [decimal](38, 6) NOT NULL,
	[AvgSaleRate] [decimal](38, 6) NOT NULL
)
INSERT @TEMPTABLE
EXEC [dbo].[GetStock] @FromDate = @FromDate, @ToDate = @ToDate,@FiscalId = @FiscalId, @CompanyId = @CompanyId


SELECT 
		AccountId,
		Balance 
FROM	@TEMPTABLE 
WHERE	AccountId IN (select * from dbo.fnStringList2Table(@ItemIds))


