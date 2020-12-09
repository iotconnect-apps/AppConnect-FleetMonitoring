CREATE PROCEDURE [dbo].[DeviceUsage_Get]
(	
	@companyguid UNIQUEIDENTIFIER = NULL
	, @entityguid UNIQUEIDENTIFIER = NULL
	, @hardwarekitguid UNIQUEIDENTIFIER = NULL
)
AS
BEGIN
    SET NOCOUNT ON

              SELECT 'G001' AS [Name], '10' AS [Value] 
	UNION ALL SELECT 'G002' , '18' AS [Value] 
	UNION ALL SELECT 'G003' , '17' AS [Value] 
	UNION ALL SELECT 'G004' , '35' AS [Value] 
	UNION ALL SELECT 'G005' , '20' AS [Value] 

END