--***************************************************************************************************************************
--
--     SQL Template for a Trigger to update a LastUpdate Date with current date and time.
--    
--       This trigger will track last update but please note that it does not fire when 
--       the row is inserted.  If you need to track Insert time, use a default value of
--       datetime('now', 'localtime') when creating the table.
--
--***************************************************************************************************************************

CREATE TRIGGER "TriggerName"
	After Update On "TableName"
	For Each Row
	Begin
		UPDATE "TableName SET LastUpdateColumnName = (datetime('now','localtime')) WHERE Id = old.Id;
	End