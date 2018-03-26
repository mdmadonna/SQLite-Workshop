--***************************************************************************************************************************
--
--     SQL Template for a simple view
--
--***************************************************************************************************************************
Create View If Not Exists "ViewName"
(
	"ColumnName 1",
	"ColumnName 2",
	"ColumnName ..."
)
As 
Select 
	"ColumnName 1",
	"ColumnName 2",
	"ColumnName ..."
From 
	"TableName" 
Where "ColumnName 1" = "columninfo"
Order By "ColumnName 1" Asc, "ColumnName 2" Asc