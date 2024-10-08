


Create PROCEDURE [dbo].[GetVehicleActiveStockValue] 
@CompanyId			INT,
@BranchId			INT = NULL,
@Date				DATETIME = NULL
AS 

--DECLARE 
--@CompanyId			INT = 63,
--@BranchId			INT = NULL,
--@Date				DATETIME = '2017-06-17'

IF @Date IS NULL SET @Date = GETDATE()

SELECT 
	VS.FileNo,
	VS.ChassisNo,
	VS.RegNo,
	VS.VehicleName,
	VS.SupplierName,
	VS.BLReceivedDate,
	VS.Consignee,
	VS.EngineNo,
	VS.Color,
	VS.[Year],
	VS.CC,
	VS.ClearingAgent,
	VS.PurchasePrice,
	VS.DutyTaxes,
	VS.BLCharges,
	VS.MiscCharges,
	VS.TotalCost,
	VS.[Status],
	VS.Id,
	VS.BranchName
INTO #TempStock
FROM dbo.vw_VehicleStock VS
WHERE (VS.AccountId IS NULL OR VS.RecoveryStatus IN	(3,11) OR VS.RecoveryStatus IS NULL OR (VS.RecoveryStatus=0 AND VS.AccountId IS NULL))
	AND VS.VehicleStatus NOT IN(3)
	AND VS.CompanyId=@CompanyId
	AND (@BranchId IS NULL OR VS.BranchId =@BranchId)
	AND (VS.[Date] IS NULL OR VS.[Date] <= @Date)

DELETE TS
FROM #TempStock TS
	JOIN dbo.VehicleSales VS ON VS.VehicleId  = TS.Id
WHERE VS.RecoveryStatus NOT IN (3,11)


SELECT 
	
	SUM(VS.TotalCost) AS TotalCost
FROM #TempStock VS

--
DROP TABLE #TempStock







