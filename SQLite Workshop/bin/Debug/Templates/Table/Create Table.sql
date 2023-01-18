--***************************************************************************************************************************
--
--     SQL Template to Drop and Create a Table
--
--***************************************************************************************************************************

Drop Table if Exists "TableName";

CREATE TABLE IF NOT EXISTS "TableName" (
    "Column1" integer PRIMARY KEY
    ,"Column2" char(12) UNIQUE
    ,"Column3" text
    ,"Column4" varchar(50) NULL
    ,"Column5" real NOT NULL
    ,"Column6" blob
);