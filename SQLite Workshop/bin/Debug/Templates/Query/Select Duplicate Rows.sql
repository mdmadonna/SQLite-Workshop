--***************************************************************************************************************************
--
--     SQL Template for a Query to display duplicate records. The list of Columns should include
--       all columns to include  in the dup search and the GROUP BY should include all listed columns.
--
--***************************************************************************************************************************

SELECT	"ColumnName1",
	"ColumnName2",
	"ColumnName3...",
	SUM(1) AS Count
	FROM "TableName"
	GROUP By "ColumnName1", "ColumnName2", "ColumnName3..."
	HAVING Count > 1
