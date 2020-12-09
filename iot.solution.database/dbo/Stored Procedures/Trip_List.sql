/*******************************************************************            
DECLARE @count INT              
      ,@output INT = 0              
  ,@fieldName VARCHAR(255)              
              
EXEC [dbo].[Trip_List]              
  @companyGuid = '74B126BE-1139-4135-8766-3E56A0125D09' 
  ,@currentDate  ='2020-10-21 09:19:59.637'   
	 --,@startDate  ='2020-09-01 09:19:59.637'     
	 --,@endDate  ='2020-10-25 09:19:59.637'     
     ,@status = 'On Going' 
 ,@pageSize  = 20             
 ,@search=''            
 ,@pageNumber = 1              
 ,@orderby  = NULL              
 ,@count   = @count OUTPUT              
 ,@invokingUser  = 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'              
 ,@version  = 'v1'              
 ,@output  = @output OUTPUT              
 ,@fieldName  = @fieldName OUTPUT              
              
SELECT @count count, @output status, @fieldName fieldName              
            
*******************************************************************/            
CREATE PROCEDURE [dbo].[Trip_List]                  
(    @companyGuid  UNIQUEIDENTIFIER                  
     ,@fleetGuid  UNIQUEIDENTIFIER = NULL           
     ,@driverGuid  UNIQUEIDENTIFIER = NULL           
     ,@currentDate Datetime =NULL     
  ,@startDate Datetime =NULL       
  ,@endDate Datetime =NULL       
     ,@status varchar(100)= NULL            
     ,@search   VARCHAR(100)  = NULL                  
      ,@pageSize   INT                  
      ,@pageNumber  INT                  
      ,@orderby   VARCHAR(100)  = NULL                  
      ,@invokingUser  UNIQUEIDENTIFIER                  
      ,@version   VARCHAR(10)                  
      ,@culture   VARCHAR(10)   = 'en-Us'                  
      ,@output   SMALLINT   OUTPUT                  
      ,@fieldName   VARCHAR(255)  OUTPUT                  
      ,@count    INT     OUTPUT                  
      ,@enableDebugInfo CHAR(1)    = '0'                  
)                  
AS                  
BEGIN                  
    SET NOCOUNT ON                  
      DECLARE @dt DATETIME = @currentDate;            
    IF (@enableDebugInfo = 1)                  
 BEGIN                  
        DECLARE @Param XML                  
        SELECT @Param =                  
        (                  
            SELECT 'Trip_List' AS '@procName'                  
             , CONVERT(VARCHAR(MAX),@companyGuid) AS '@companyGuid'                      
             , CONVERT(VARCHAR(MAX),@search) AS '@search'                  
               , CONVERT(VARCHAR(MAX),@pageSize) AS '@pageSize'                  
               , CONVERT(VARCHAR(MAX),@pageNumber) AS '@pageNumber'                  
               , CONVERT(VARCHAR(MAX),@orderby) AS '@orderby'                  
               , CONVERT(VARCHAR(MAX),@version) AS '@version'                  
             , CONVERT(VARCHAR(MAX),@invokingUser) AS '@invokingUser'                  
            FOR XML PATH('Params')                  
     )                  
     INSERT INTO DebugInfo(data, dt) VALUES(Convert(VARCHAR(MAX), @Param), GETDATE())                  
    END                  
                      
      BEGIN TRY                  
                  
  SET @output = 1                  
  SET @count = -1                  
                   
  IF OBJECT_ID('tempdb..#temp_Trip') IS NOT NULL DROP TABLE #temp_Trip                  
                   
  CREATE TABLE #temp_Trip                  
  (          
        [guid]     UNIQUEIDENTIFIER                
        ,[companyguid]   UNIQUEIDENTIFIER                
        ,[fleetGuid] UNIQUEIDENTIFIER             
        ,[driverGuid] UNIQUEIDENTIFIER        
        ,[fleetName]    NVARCHAR(150)                 
        ,[sourceLocation]   NVARCHAR(250)                  
        ,[destinationLocation]    NVARCHAR(250)          
        ,[sourceLatitude] NVARCHAR(50)        
        ,[sourceLongitude] NVARCHAR(50)        
        ,[destinationLatitude] NVARCHAR(50)        
        ,[destinationLongitude] NVARCHAR(50)        
        ,[materialType]     NVARCHAR(150)                  
        ,[weight]    NVARCHAR(100)                  
        ,[startDateTime]   datetime               
         ,[endDateTime]   datetime               
         ,[status]    NVARCHAR(150)                 
         ,[totalMiles] int            
         ,[tripId] NVARCHAR(150)          
          ,[driverName]    NVARCHAR(150)          
    ,[uniqueId]    NVARCHAR(500)      
 ,[coveredMiles] int    
    ,[isStarted] BIT    
    ,[isCompleted] BIT    
        ,[row_num]    INT                  
  )                  
                  
  IF LEN(ISNULL(@orderby, '')) = 0                  
  SET @orderby = 'tripId asc'                  
       SET @orderby = REPLACE(@orderby,'weight','CONVERT(INT,weight)')           
  DECLARE @Sql nvarchar(MAX) = ''                
              
  IF OBJECT_ID('tempdb..#trips') IS NOT NULL BEGIN DROP TABLE #trips END           
  CREATE TABLE #trips ([Guid] UNIQUEIDENTIFIER, [fleetGuid] UNIQUEIDENTIFIER, [startDateTime] DATETIME,[isStarted] BIT,[isCompleted] BIT, [endDateTime] DATETIME,[status] NVARCHAR(100))             
              
   INSERT INTO #trips ([guid],[fleetGuid], [startDateTime], [isStarted],[isCompleted],[endDateTime])             
   SELECT [guid],[fleetGuid], [startDateTime], [isStarted],[isCompleted],              
   (SELECT top 1 endDateTime from [dbo].[TripStops] (NOLOCK) WHERE [tripGuid] = T.[Guid] AND [isDeleted] = 0 ORDER BY [endDateTime] DESC) AS [endDateTime]             
    FROM [dbo].[Trip] T (NOLOCK)             
    WHERE [companyGuid] = @companyGuid AND [isDeleted] = 0             
                 
   UPDATE T set [status]=CASE WHEN (T.[isStarted]=1 AND T.[isCompleted]=0)        
        THEN 'On Going'            
        ELSE CASE WHEN T.[isCompleted]=1             
        THEN 'Completed'            
        ELSE CASE WHEN T.[endDateTime] < @currentDate    
  THEN 'Overdue'     
  ELSE 'Upcoming'            
        END            
        END END FROM #trips T                
                  
  SET @Sql = '            
   SELECT                   
     *                  
     ,ROW_NUMBER() OVER (ORDER BY '+@orderby+') AS row_num                  
    FROM                  
    (                  
     SELECT                     
         u.[guid]                 
         , u.[companyGuid]                
         , u.[fleetGuid]          
         ,d.[guid] AS [driverGuid]        
         ,f.[fleetId] AS [fleetName]              
       ,u.[sourceLocation]              
       ,u.[destinationLocation]            
       ,u.[sourceLatitude],u.[sourceLongitude]        
       ,u.[destinationLatitude],u.[destinationLongitude]        
       ,m.[name] AS [materialType]               
       ,u.[weight]              
       ,u.[startDateTime]              
       ,u_end.[endDateTime] AS [endDateTime]            
       ,u_end.[status] AS [status]             
       ,u.[totalMiles]          
       ,u.[tripId]       
       ,(d.[firstname] + '' '' + d.[lastname]) AS [driverName]        
    ,dv.[uniqueId]     
 ,u.[coveredMiles]    
    ,u.[isStarted]    
    ,u.[isCompleted]    
     FROM [dbo].[Trip] AS u WITH (NOLOCK)                  
     INNER JOIN #trips AS u_end ON u.[guid]=u_end.[guid]             
     LEFT JOIN [dbo].[Fleet] AS f WITH (NOLOCK) ON f.[guid] = u.[fleetGuid] AND f.[isDeleted] = 0                
     LEFT JOIN [dbo].[Driver] AS d WITH (NOLOCK) ON d.[fleetGuid] = u.[fleetGuid] AND d.[isDeleted] = 0               
     LEFT JOIN [dbo].[FleetMaterialType] AS m WITH (NOLOCK) ON m.[guid] = u.[materialTypeGuid] AND m.[isDeleted] = 0          
  LEFT JOIN [dbo].[Device] AS dv WITH (NOLOCK) ON dv.[fleetGuid] = u.[fleetGuid] AND dv.[isDeleted] = 0       
     WHERE u.companyguid = @companyguid AND u.[isdeleted] = 0 AND f.[Guid]=ISNULL(@fleetGuid,f.[Guid]) AND d.[Guid]=ISNULL(@driverGuid,d.[Guid])  '           
      + ' AND u_end.[status]=ISNULL(@status,u_end.[status]) '  
    + CASE WHEN @startDate IS NULL THEN '' ELSE ' AND u.[startDateTime] >= @startDate' END +  
   +CASE WHEN @endDate IS NULL THEN '' ELSE ' AND u_end.[endDateTime] <= @endDate ' END +  
     + CASE WHEN @search IS NULL THEN '' ELSE                  
     ' AND (u.[sourceLocation] LIKE ''%' + @search + '%''               
    OR u.[destinationLocation] LIKE ''%' + @search + '%''            
  OR (u.tripId) LIKE ''%' + @search + '%''          
   OR (d.firstname + '' '' + d.lastname) LIKE ''%' + @search + '%''        
    OR (f.fleetId) LIKE ''%' + @search + '%''                  
    OR m.[name] LIKE ''%' + @search + '%''                  
    OR f.[fleetId] LIKE ''%' + @search + '%'' 
	OR u_end.[status] LIKE ''%' + @search + '%'' 
     ) '                  
   END +                  
    ') data '                 
                    
    INSERT INTO #temp_Trip                 
    EXEC sp_executesql                   
    @Sql                  
     , N'@orderby VARCHAR(100),@dt DATETIME,@companyGuid UNIQUEIDENTIFIER,@fleetGuid UNIQUEIDENTIFIER,@driverGuid UNIQUEIDENTIFIER,@status NVARCHAR(100),@startDate DATETIME,@endDate DATETIME '                  
     , @orderby  = @orderby          
     , @dt=@dt        
     , @companyGuid = @companyGuid                  
    , @fleetGuid = @fleetGuid            
    ,@driverGuid = @driverGuid        
    ,@status=@status   
 ,@startDate=@startDate  
 ,@endDate=@endDate  
    SET @count = @@ROWCOUNT                  
               
    IF(@pageSize <> -1 AND @pageNumber <> -1)                  
     BEGIN                  
   SELECT                   
        d.[guid]              
       ,d.[companyguid]              
       ,d.[fleetGuid]                
       ,d.[fleetName]            
       ,d.[driverGuid]        
       ,d.[sourceLocation]                  
       ,d.[destinationLocation]           
       ,d.[sourceLatitude]        
       ,d.[sourceLongitude]        
       ,d.[destinationLatitude]        
       ,d.[destinationLongitude]         
       ,d.[materialType]                
       ,d.[weight]                
       ,d.[startDateTime]                
       ,d.[endDateTime]                 
       ,d.[status]            
       ,d.[totalMiles]            
       ,d.[tripId]            
       ,d.[driverName]      
    ,d.[uniqueId]    
 ,d.[coveredMiles]    
    ,d.[isStarted]    
     ,d.[isCompleted]    
   FROM #temp_Trip d                
   WHERE row_num BETWEEN ((@pageNumber - 1) * @pageSize) + 1 AND (@pageSize * @pageNumber)              
               
     END                  
    ELSE                  
     BEGIN                  
    SELECT                   
        d.[guid]              
       ,d.[companyguid]              
       ,d.[fleetGuid]                
       ,d.[fleetName]            
       ,d.[driverGuid]        
       ,d.[sourceLocation]                  
       ,d.[destinationLocation]           
       ,d.[sourceLatitude]        
       ,d.[sourceLongitude]        
       ,d.[destinationLatitude]        
       ,d.[destinationLongitude]         
       ,d.[materialType]                
       ,d.[weight]                
       ,d.[startDateTime]                
       ,d.[endDateTime]                 
       ,d.[status]            
       ,d.[totalMiles]            
       ,d.[tripId]            
       ,d.[driverName]        
    ,d.[uniqueId]     
 ,d.[coveredMiles]    
    ,d.[isStarted]    
     ,d.[isCompleted]    
   FROM #temp_Trip d              
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
   'ErrorState:'  + ISNULL(CAST(ERROR_STATE() as VARCHAR), '') +              'ErrorLine:'  + ISNULL(CAST(ERROR_LINE () as VARCHAR), '') +                   
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