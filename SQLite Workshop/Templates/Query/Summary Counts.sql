--***************************************************************************************************************************
--
--     SQL Template for a Query to produce summary totals by Column Number. The
--       Where Clause should be removed or customized as needed.
--
--***************************************************************************************************************************

SELECT 	"Column1"
	,Sum(1) AS Count
	FROM "TableName"
	WHERE "Column" Like expression
	GROUP BY "Column1";
