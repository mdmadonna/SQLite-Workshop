--***************************************************************************************************************************
--
--     SQL Template for a simple Trigger
--
--***************************************************************************************************************************
Create Trigger If Not Exists "TriggerName"
	Before|After|Instead Of Delete|Insert|Update On "TableName"(
	When Expression,
	Begin
		Update Statement
		Insert Statement
		Delete Statement
		Select Statement
		;
	End 
