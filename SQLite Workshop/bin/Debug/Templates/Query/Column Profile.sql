--***************************************************************************************************************************
--
--     SQL Template to Profile a column. It returns all values in a column along with the number of
--       occurrences.
--
--***************************************************************************************************************************

SELECT 	"ColumnName"
	,Sum(1) AS Occurrences
	FROM "TableName"
	GROUP BY "ColumnName";
