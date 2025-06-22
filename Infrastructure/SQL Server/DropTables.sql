USE [PropertyManagement];
GO

-- Step 1: Drop all foreign key constraints
DECLARE @fkSql NVARCHAR(MAX) = N'';

SELECT @fkSql += 
    'ALTER TABLE [' + SCHEMA_NAME(t.schema_id) + '].[' + t.name + '] ' +
    'DROP CONSTRAINT [' + fk.name + '];' + CHAR(13) + CHAR(10)
FROM sys.foreign_keys fk
JOIN sys.tables t ON fk.parent_object_id = t.object_id;

PRINT '-- Dropping Foreign Keys --';
PRINT @fkSql;
EXEC sp_executesql @fkSql;

-- Step 2: Drop all user-defined tables in dbo schema
DECLARE @dropSql NVARCHAR(MAX) = N'';

SELECT @dropSql += 
    'DROP TABLE IF EXISTS [dbo].[' + t.name + '];' + CHAR(13) + CHAR(10)
FROM sys.tables t
WHERE SCHEMA_NAME(t.schema_id) = 'dbo';

PRINT '-- Dropping Tables --';
PRINT @dropSql;
EXEC sp_executesql @dropSql;