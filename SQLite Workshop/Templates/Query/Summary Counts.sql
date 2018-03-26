--***************************************************************************************************************************
--
--     SQL Template for a Query to produce summary totals by Account Number. The
--       Where Clause should be customized as needed.
--
--***************************************************************************************************************************

SELECT 	"AcctNum"
	,Sum(1) AS Count
	FROM "TableName"
	WHERE "TransactionDate" Like '2018%'
	GROUP BY "AcctNum";
