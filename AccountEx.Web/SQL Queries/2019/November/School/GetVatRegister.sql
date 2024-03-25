SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
GO
-- Stored Procedure





ALTER PROCEDURE [dbo].[GetVatRegister]

@FromDate			DATETIME,
@ToDate					DATETIME,
@CompanyId				INT,
@FiscalId				INT,
@SalePersonId			INT=0


as
SELECT
	S.[Date],
	S.VoucherNumber AS InvNumber,
	S.InvoiceNumber AS BookNo,
	S.AccountCode + '-' + S.AccountName AS AccountName,
	ISNULL(s.SalesmanName,'') AS SalesmanName,
	S.TransactionType,
	SI.Quantity,
	SI.Rate,
	SI.Amount AS GrossTotal,
	SI.GSTAmount AS GstAmountTotal,
	SI.NetAmount AS NetTotal

FROM dbo.Sales AS S INNER JOIN dbo.SaleItems SI ON S.Id=SI.SaleId
WHERE S.ISDELETED = 0
AND S.CompanyId = @COMPANYID
AND s.FiscalId=@FiscalId
AND S.TransactionType IN(25,26,66)
AND	((@SalePersonId=0 AND S.TransactionType<>66) OR	@SalePersonId>0)
AND CAST(S.[DATE] AS DATE) >= @FROMDATE AND CAST(S.[DATE] AS DATE)<= @TODATE
AND	(@SalePersonId=0 OR	S.SalesmanId=@SalePersonId)
ORDER BY s.[Date]




--Calculdate Stock Summary

-- Constants
DECLARE @EntryType_StockOut INT = 17
DECLARE @EntryType_StockIn INT = 18


-- Sale Purchase Opening Balance
SELECT DISTINCT
	SI.ItemId AS AccountId,
	SUM(CASE WHEN TT.IsStockIn=1 THEN SI.Quantity ELSE 0 END) -
	SUM(CASE WHEN TT.IsStockOut=1 THEN SI.Quantity ELSE 0 END) AS Quantity
INTO #OPENINGBALANCES	 -- Sale Purchase Opening Balance
FROM dbo.Sales AS S
	JOIN dbo.SaleItems AS SI ON S.Id = SI.SaleId 
	JOIN dbo.TransactionTypes TT ON TT.Id = S.TransactionType
WHERE S.CompanyId = @CompanyId AND S.TransactionType IN(25,26,66) AND S.FiscalId = @FiscalId AND S.[Date] < @FromDate 
AND	(@SalePersonId=0 OR	S.SalesmanId=@SalePersonId)
GROUP BY SI.ItemId



-- Sale/Purchase stock
SELECT 
	AD.AccountId,
	SUM(ISNULL(CASE WHEN TT2.IsStockOut=1	THEN OI.Quantity ELSE 0 END, 0)) AS SaleQty,
	SUM(ISNULL(CASE WHEN TT2.IsStockIn=1	THEN OI.Quantity ELSE 0 END, 0)) AS PurchaseQty
INTO #SalePurchaseStock
FROM dbo.AccountDetails AS AD 
	LEFT JOIN dbo.SaleItems AS OI ON OI.ItemId = AD.AccountId
	LEFT JOIN dbo.Sales AS S
	 ON S.Id = OI.SaleId AND S.FiscalId = @FiscalId AND S.[Date] BETWEEN @FromDate AND @ToDate
	LEFT JOIN dbo.TransactionTypes TT2 ON TT2.Id = S.TransactionType
WHERE AD.CompanyId = @CompanyId AND S.TransactionType IN(25,26,66) AND AD.AccountDetailFormId = 5
AND	(@SalePersonId=0 OR	S.SalesmanId=@SalePersonId)
GROUP BY AD.AccountId





SELECT 
	AD.AccountId,
	AD.Name,
	SUM(ISNULL(OB.Quantity,0)) AS OBQty,
	SUM(ISNULL(SPS.SaleQty,0)) AS SaleQty,
	SUM(ISNULL(SPS.PurchaseQty,0)) AS PurchaseQty,
	ISNULL(SUM(ISNULL(OB.Quantity,0) + ISNULL(SPS.PurchaseQty,0) - ISNULL(SPS.SaleQty,0)),0.0) AS Balance
FROM dbo.AccountDetails AS AD 
	LEFT JOIN #OPENINGBALANCES AS OB ON OB.AccountId = AD.AccountId
    LEFT JOIN #SalePurchaseStock AS SPS ON SPS.AccountId = AD.AccountId
WHERE 
AD.CompanyId = @CompanyId 
			AND AD.AccountDetailFormId = 5
				
			--AND AD.AccountId=91869
GROUP BY AD.AccountId,AD.Name
HAVING ISNULL(SUM(ISNULL(OB.Quantity,0) + ISNULL(SPS.PurchaseQty,0) - ISNULL(SPS.SaleQty,0)),0.0)<>0.0000







GO