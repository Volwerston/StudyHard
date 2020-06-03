DECLARE @UniqueConName NVARCHAR(MAX) = NULL;

SET @UniqueConName = (SELECT TC.Constraint_Name from information_schema.table_constraints TC
	inner join information_schema.constraint_column_usage CC on TC.Constraint_Name = CC.Constraint_Name
where TC.constraint_type = 'Unique' AND TC.TABLE_NAME = 'Course' AND COLUMN_NAME = 'Name')

IF (@UniqueConName IS NOT NULL)
BEGIN
	EXECUTE ('ALTER TABLE [Course] DROP CONSTRAINT '+ @UniqueConName)
END