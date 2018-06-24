--***************************************************************************************************************************
--
--     SQL Template to Drop and Create an Index
--
--***************************************************************************************************************************

Drop Index if Exists "IndexName";

CREATE UNIQUE INDEX IF NOT EXISTS "IndexName" ON "TableName" (
    "Column1"
    ,"Column2"
)
WHERE "Column1" = expression;