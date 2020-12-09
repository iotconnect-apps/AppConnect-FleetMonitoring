
/*******************************************************************      
DECLARE 
   @output INT = 0      
  ,@fieldName VARCHAR(255)      
      
EXEC [dbo].[Get_TripStatus]      
  @companyGuid = 'DD73217B-CD29-4799-A6ED-59DD263EA968'
  ,@tripGuid='168F2A2C-E8CC-4E45-9D8B-F03F91598F34'
 ,@invokingUser  = 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'      
 ,@version  = 'v1'      
 ,@output  = @output OUTPUT      
 ,@fieldName  = @fieldName OUTPUT      
      
SELECT @output status, @fieldName fieldName      
      
      
*******************************************************************/      
Create PROCEDURE [dbo].[Get_TripStatus]        
(  @companyGuid  UNIQUEIDENTIFIER        
  ,@tripGuid  UNIQUEIDENTIFIER = NULL 
  ,@status varchar(100)= NULL  
  ,@invokingUser  UNIQUEIDENTIFIER        
  ,@version   VARCHAR(10)        
  ,@culture   VARCHAR(10)   = 'en-Us'        
  ,@output   SMALLINT   OUTPUT        
  ,@fieldName   VARCHAR(255)  OUTPUT        
  ,@enableDebugInfo CHAR(1)    = '0'        
)        
AS        
BEGIN        
    SET NOCOUNT ON        
      DECLARE @dt DATETIME = GETUTCDATE() 
    IF (@enableDebugInfo = 1)        
 BEGIN        
        DECLARE @Param XML        
        SELECT @Param =        
        (        
            SELECT 'Get_TripStatus' AS '@procName'        
             , CONVERT(VARCHAR(MAX),@companyGuid) AS '@companyGuid'  
			 , CONVERT(VARCHAR(MAX),@tripGuid) AS '@tripGuid'
   , CONVERT(VARCHAR(MAX),@version) AS '@version'        
             , CONVERT(VARCHAR(MAX),@invokingUser) AS '@invokingUser'        
            FOR XML PATH('Params')        
     )        
     INSERT INTO DebugInfo(data, dt) VALUES(Convert(VARCHAR(MAX), @Param), GETDATE())        
    END        
            
      BEGIN TRY        
        
  SET @output = 1        
         
  IF OBJECT_ID('tempdb..#temp_TripStatus') IS NOT NULL DROP TABLE #temp_TripStatus        
         
  CREATE TABLE #temp_TripStatus        
  (  [guid]     UNIQUEIDENTIFIER      
    ,[status]    NVARCHAR(150)       
  )        
  
  DECLARE @Sql nvarchar(MAX) = ''      
    
  IF OBJECT_ID('tempdb..#tripsStatus') IS NOT NULL BEGIN DROP TABLE #tripsStatus END  
  CREATE TABLE #tripsStatus ([Guid] UNIQUEIDENTIFIER, [fleetGuid] UNIQUEIDENTIFIER, [startDateTime] DATETIME,[endDateTime] DATETIME,[status] NVARCHAR(100))   
    
   INSERT INTO #tripsStatus ([guid],[fleetGuid], [startDateTime],[endDateTime])   
   SELECT [guid],[fleetGuid], [startDateTime],     
   (SELECT top 1 endDateTime from [dbo].[TripStops] (NOLOCK) WHERE [tripGuid] = T.[Guid] AND [isDeleted] = 0 ORDER BY [endDateTime] DESC) AS [endDateTime]   
    FROM [dbo].[Trip] T (NOLOCK)   
    WHERE [companyGuid] = @companyGuid AND [isDeleted] = 0   
       
    UPDATE T set [status]=CASE WHEN @dt >= T.[startDateTime] AND @dt <= T.[endDateTime]  
        THEN 'On Going'  
        ELSE CASE WHEN T.[startDateTime] < @dt AND T.[endDateTime] < @dt   
        THEN 'Completed'  
        ELSE 'Upcoming'  
        END  
        END FROM #tripsStatus T   
        
  SET @Sql = '  
   SELECT         
     *        
    FROM        
    (        
     SELECT           
     u.[guid]       
   ,u_end.[status] AS [status]   
     FROM [dbo].[Trip] AS u WITH (NOLOCK)        
     INNER JOIN #tripsStatus AS u_end ON u.[guid]=u_end.[guid]   
     WHERE u.companyguid = @companyguid AND u.[isdeleted] = 0 
       AND u_end.[status]=ISNULL(@status,u_end.[status]) '  
	   + CASE WHEN @tripGuid IS NULL THEN '' ELSE ' AND u.[guid] = @tripGuid ' END +       
    ') data '       
          
    INSERT INTO #temp_TripStatus       
    EXEC sp_executesql         
    @Sql        
     , N'@dt DATETIME, @companyGuid UNIQUEIDENTIFIER,@tripGuid UNIQUEIDENTIFIER,@status NVARCHAR(100)  '        
     ,@dt=@dt      
     , @companyGuid = @companyGuid        
	, @tripGuid = @tripGuid
    ,@status=@status  
     BEGIN        
   SELECT         
    d.[guid]    
   ,d.[status]  
   FROM #temp_TripStatus d      
     END        
    SET @output = 1        
    SET @fieldName = 'Success'        
  END TRY         
 BEGIN CATCH          
  DECLARE @errorReturnMessage VARCHAR(MAX)        
        
  SET @output = 0        
        
  SELECT @errorReturnMessage =         
   ISNULL(@errorReturnMessage, '') +  SPACE(1)   +         
   'ErrorNumber:'  + ISNULL(CAST(ERROR_NUMBER() as VARCHAR), '')  +         
   'ErrorSeverity:'  + ISNULL(CAST(ERROR_SEVERITY() as VARCHAR), '') +         
   'ErrorState:'  + ISNULL(CAST(ERROR_STATE() as VARCHAR), '') +         
   'ErrorLine:'  + ISNULL(CAST(ERROR_LINE () as VARCHAR), '') +         
   'ErrorProcedure:'  + ISNULL(CAST(ERROR_PROCEDURE() as VARCHAR), '') +         
   'ErrorMessage:'  + ISNULL(CAST(ERROR_MESSAGE() as VARCHAR(max)), '')        
  RAISERROR (@errorReturnMessage, 11, 1)          
         
  IF (XACT_STATE()) = -1          
  BEGIN           
   ROLLBACK TRANSACTION         
  END           
  IF (XACT_STATE()) = 1          
  BEGIN              
   ROLLBACK TRANSACTION          
  END           
  RAISERROR (@errorReturnMessage, 11, 1)           
 END CATCH     
END