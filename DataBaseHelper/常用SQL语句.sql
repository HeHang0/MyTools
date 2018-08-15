--将数据库改为多用户模式
USE master;
GO
DECLARE @SQL VARCHAR(MAX);
SET @SQL=''
SELECT @SQL=@SQL+'; KILL '+RTRIM(SPID)
FROM master..sysprocesses
WHERE dbid=DB_ID('DataBaseName');
EXEC(@SQL);
GO

ALTER DATABASE DataBaseName SET MULTI_USER;