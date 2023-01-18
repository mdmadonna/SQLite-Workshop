--***************************************************************************************************************************
--
--     SQL Template for a Query to substitute for SELECT INTO, which is not supported by SQLite. Note that  
--       you may need to modify column data types after table creation. Use one of the following formats or 
--       create your own.
--
--***************************************************************************************************************************


CREATE TABLE "NewTableName" AS 
  SELECT * 
  FROM "OldTableName";

or

CREATE TABLE "NewTableName" AS 
  SELECT "ColumnName1", "ColumnName2", "ColumnName3..."
  FROM "OldTableName";

or

CREATE TABLE "NewTableName" AS 
  SELECT  DISTINCT "ColumnName1", "ColumnName2", "ColumnName3..."
  FROM "OldTableName";