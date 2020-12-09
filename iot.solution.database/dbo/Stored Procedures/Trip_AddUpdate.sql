/*******************************************************************                        
DECLARE @output INT = 0                        
 ,@fieldName nvarchar(255)                         
 ,@newid  UNIQUEIDENTIFIER                        
EXEC [dbo].[Trip_AddUpdate]                         
 @companyGuid = '74B126BE-1139-4135-8766-3E56A0125D09'                        
 --,@Guid   = '1667C67D-65C1-4B2D-9BE1-EC3CD9F2E821'                        
 ,@fleetGuid ='FC3C84A2-7E5A-4052-864E-4E6589592A15'                   
 ,@tripId='T-MP1211010101'                
 ,@sourceLocation ='Ujjain Mahakal'                     
 ,@sourceLatitude='25.654'                    
 ,@sourceLongitude='21.354'                    
 ,@destinationLocation ='Indore Omkareshwar'                     
 ,@destinationLatitude='63.245'                    
 ,@destinationLongitude='59.245'                    
 ,@materialTypeGuid  = 'B6890D1C-4E7C-4EFF-9A3B-252226722DF2'                        
 ,@weight = '21'                        
 ,@startDateTime    = '2020-10-29 13:10:04.620'                        
 ,@stopData    = '<stops><stop><guid>83699C91-1170-4AC9-86BE-A36979F945C9</guid><stopName>Indore Omkareshwar</stopName><latitude>25.321</latitude><longitude>24.658</longitude><endDateTime>2020-10-28 13:10:04.620</endDateTime></stop></stops>'             
 ,@totalMiles ='236'        
 ,@invokingUser = 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'                                      
 ,@version  = 'v1'                                      
 ,@newid   = @newid  OUTPUT                        
 ,@output  = @output  OUTPUT                        
 ,@fieldName  = @fieldName OUTPUT                         
                        
SELECT @output status, @fieldName fieldName, @newid newid                      
                        
                        
*******************************************************************/                        
CREATE PROCEDURE [dbo].[Trip_AddUpdate]                            
( @companyGuid UNIQUEIDENTIFIER                            
 ,@guid   UNIQUEIDENTIFIER   =NULL                             
 ,@fleetGuid UNIQUEIDENTIFIER                        
 ,@tripId nvarchar(150)                      
 ,@sourceLocation nvarchar(250)                           
 ,@sourceLatitude nvarchar(50)                        
 ,@sourceLongitude nvarchar(50)                        
 ,@destinationLocation nvarchar(250)                          
 ,@destinationLatitude nvarchar(50)                          
 ,@destinationLongitude nvarchar(50)                         
 ,@totalMiles int                        
 ,@materialTypeGuid uniqueidentifier = NULL                            
 ,@weight nvarchar(100)                            
 ,@startDateTime Datetime= NULL             
 ,@stopData   XML     = NULL                          
 ,@invokingUser UNIQUEIDENTIFIER                            
 ,@version  nvarchar(10)                                
 ,@newid   UNIQUEIDENTIFIER OUTPUT                            
 ,@output  SMALLINT   OUTPUT                                
 ,@fieldName  nvarchar(100)  OUTPUT                               
 ,@culture  nvarchar(10)  = 'en-Us'                            
 ,@enableDebugInfo CHAR(1)   = '0'                            
)                             
AS                            
BEGIN                            
 DECLARE @bool BIT=0;                        
 SET @enableDebugInfo = 1                            
 SET NOCOUNT ON                            
DECLARE @dt DATETIME = GETUTCDATE();                       
    IF (@enableDebugInfo = 1)                            
 BEGIN                            
        DECLARE @Param XML                             
        SELECT @Param =                             
        (                            
            SELECT 'Trip_AddUpdate' AS '@procName'                                         
             , CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid'                             
             , CONVERT(nvarchar(MAX),@guid) AS '@guid'                        
             , @fleetGuid AS '@fleetGuid'                             
             , @sourceLocation AS '@sourceLocation'                          
    , @sourceLatitude AS '@sourceLatitude'                        
    , @sourceLongitude AS '@sourceLongitude'             
             , @destinationLocation AS '@destinationLocation'                             
    , @destinationLatitude AS '@destinationLatitude'                      
    , @destinationLongitude AS '@destinationLongitude'                         
             , CONVERT(nvarchar(MAX),@materialTypeGuid) AS '@materialTypeGuid'                               
             , @weight AS '@weight'                                 
             , CONVERT(nvarchar(50),@startDateTime) AS '@startDateTime'                                
    , CONVERT(nvarchar(MAX),@stopData) AS '@stopData'                            
             , CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'                          
             , CONVERT(nvarchar(MAX),@version) AS '@version'                           
             , CONVERT(nvarchar(MAX),@output) AS '@output'                           
             , @fieldName AS '@fieldName'                                 
            FOR XML PATH('Params')                            
     )                             
   INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())                          
    END                                   
                             
 DECLARE @poutput  SMALLINT                            
   ,@pFieldName nvarchar(100)                            
                                
   IF(@poutput!=1)                            
      BEGIN                            
        SET @output = @poutput                            
        SET @fieldName = @pfieldName                            
        RETURN;                            
      END                           
 SET @newid = @guid                            
                            
 SET @output = 1                            
 SET @fieldName = 'Success'                 
           
  IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[Trip] (NOLOCK) WHERE [guid] =@guid and [companyGuid] = @companyGuid and [isdeleted] = 0)                          
  BEGIN                          
   SET @bool = 1;                          
  END              
                            
 BEGIN TRY                            
                         
                              
  IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[Fleet] (NOLOCK) where [companyGuid] = @companyGuid AND [guid] = @fleetGuid AND [isDeleted] = 0 AND [isActive] = 1)                          
  BEGIN                          
   SET @output = -3                          
   SET @fieldname = 'FleetNotExists'                           
   RETURN;                          
  END           
  IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[Driver] (NOLOCK) where [fleetGuid] = @fleetGuid AND [isDeleted] = 0 AND [isActive] = 1)                          
  BEGIN                          
   SET @output = -3                          
   SET @fieldname = 'DriverNotExist'                           
   RETURN;                          
  END         
                         
    IF EXISTS(SELECT TOP 1 1 FROM [dbo].[Fleet] (NOLOCK) where [guid] = @fleetGuid AND [isDeleted] = 0 and CAST([loadingCapacity] AS float)<(SELECT CAST(@weight AS float)))                          
   BEGIN                          
    SET @output = -3                          
    SET @fieldname = 'WeightIsGreaterThenFleetLoadCapacity'                           
    RETURN;                          
   END                         
           
          
  --IF @bool = 1                          
  --BEGIN                          
  --  IF EXISTS(SELECT TOP 1 1 FROM [dbo].[Trip] (NOLOCK) where [companyGuid] = @companyGuid AND [fleetGuid] = @fleetGuid and [materialTypeGuid]=@materialTypeGuid                           
  -- AND (cast([startDateTime] AS date) = cast(@startDateTime AS date)) AND [isDeleted] = 0)                          
  -- BEGIN                          
  --  SET @output = -3                 
  --  SET @fieldname = 'TripAlreadyExists'                           
  --  RETURN;                          
  -- END                         
  --END             
            
 IF @bool = 1              
 BEGIN              
    IF EXISTS(SELECT TOP 1 1 FROM [dbo].[Trip] (NOLOCK) where [companyGuid] = @companyGuid and [tripId] = @tripId AND [isDeleted] = 0)                          
     BEGIN                          
  SET @output = -3                          
  SET @fieldname = 'TripIdAlreadyExists'                           
  RETURN;                          
     END                         
 END              
                  
    IF OBJECT_ID ('tempdb..#Stop_Data') IS NOT NULL DROP TABLE #Stop_Data                          
                          
  CREATE TABLE #Stop_Data                          
  (                
    [guid] uniqueidentifier                
   ,[stopName]  NVARCHAR(200)                         
   ,[latitude] NVARCHAR(50)            
   ,[longitude] NVARCHAR(50)                        
   ,[endDateTime] DATETIME                          
                             
  )                          
  INSERT INTO #Stop_Data                          
  SELECT DISTINCT                 
  x.R.query('./guid').value('.', 'uniqueidentifier') AS [guid]                 
     ,x.R.query('./stopName').value('.', 'NVARCHAR(200)') AS [stopName]                         
  ,x.R.query('./latitude').value('.', 'NVARCHAR(50)') AS [latitude]                          
  ,x.R.query('./longitude').value('.', 'NVARCHAR(50)') AS [longitude]                         
    , x.R.query('./endDateTime').value('.', 'Datetime') AS [endDateTime]                          
                              
   FROM @stopData.nodes('//stops/stop') as x(R)           
        
   IF EXISTS(SELECT TOP 1 1 FROM #Stop_Data (NOLOCK) where @startDateTime>(SELECT top 1 endDateTime from #Stop_Data ORDER BY [endDateTime] DESC))                          
   BEGIN                          
    SET @output = -2                         
    SET @fieldname = 'StartDateIsGreaterThenEndDate'                           
    RETURN;                          
   END         
        
IF @bool = 1                          
  BEGIN                          
   IF EXISTS(SELECT TOP 1 1 FROM [dbo].[DeviceMaintenance] (NOLOCK) where [deviceGuid] = @fleetGuid    
   AND ISNULL([isCompleted],0)=0
  AND ((@startDateTime BETWEEN [startDateTime] AND [endDateTime]) OR ((SELECT top 1 endDateTime from #Stop_Data ORDER BY [endDateTime] DESC) BETWEEN [startDateTime] AND [endDateTime])))                          
   BEGIN                          
    SET @output = -2                          
    SET @fieldname = 'DeviceMaintenenceExists'                           
    RETURN;                          
   END                         
END           
          
 IF @bool = 1              
 BEGIN              
  DECLARE @recordCount int=(SELECT count(1) FROM [dbo].[Trip] as T WITH (NOLOCK) INNER JOIN [dbo].[TripStops] as TS WITH (NOLOCK) ON T.[guid] = TS.[tripGuid] where T.[fleetGuid]=@fleetGuid and T.[isDeleted]=0 and T.[isCompleted]=0 and ((@startDateTime BETWEEN T.[startDateTime] AND (SELECT top 1 endDateTime from [dbo].[TripStops] WITH (NOLOCK) where [isDeleted] =0 and [tripGuid] = T.[guid] ORDER BY [endDateTime] DESC)) OR ((SELECT top 1 endDateTime from #Stop_Data ORDER BY [endDateTime] DESC) BETWEEN T.[startDateTime] AND (SELECT top 1 endDateTime from [dbo].[TripStops] WITH (NOLOCK) where [isDeleted]=0 and [tripGuid] = T.[guid] ORDER BY [endDateTime] DESC))));                   
    IF @recordCount >0          
    BEGIN          
     SET @output = -2                          
     SET @fieldname = 'TripAlreadyExistsBetweenDateRange'                           
     RETURN;             
    END          
 END            
          
 BEGIN TRAN                             
 IF @bool = 1          
 BEGIN                             
     SET @newid = NEWID()                             
     INSERT INTO dbo.[Trip](                            
   [guid]                                 
   ,[companyGuid]                                
   ,[fleetGuid]                            
   ,[sourceLocation]                          
   ,[destinationLocation]                        
   ,[materialTypeGuid]                             
   ,[weight]                             
   ,[startDateTime]                            
   ,[isActive]                            
   ,[isDeleted]                            
   ,[createddate]                            
   ,[createdby]                            
   ,[updatedDate]                            
   ,[updatedBy]                            
   ,[sourceLatitude]                        
   ,[sourceLongitude]                        
   ,[destinationLatitude]                        
   ,[destinationLongitude]                        
   ,[totalMiles]                     
   ,[tripId]                    
   )                            
     VALUES(@newid                                 
   ,@companyGuid                            
    ,@fleetGuid                              
    ,@sourceLocation                        
    ,@destinationLocation                         
    ,@materialTypeGuid                            
    ,@weight                            
    ,@startDateTime                            
   ,1                            
   ,0                               
   ,@dt                            
   ,@invokingUser                            
   ,@dt                            
   ,@invokingUser                          
   ,@sourceLatitude                        
 ,@sourceLongitude                        
 ,@destinationLatitude                        
 ,@destinationLongitude                        
 ,@totalMiles                     
 ,@tripId                     
     )                            
     INSERT INTO [dbo].[TripStops]                          
        ([guid]                          
        ,[tripGuid]                          
        ,[stopName]                          
        ,[endDateTime]                          
        ,[isDeleted]                        
        ,[createdDate]                          
        ,[createdBy]                          
        ,[updatedDate]                          
        ,[updatedBy]                          
  ,[latitude]                        
  ,[longitude]                        
      )                          
     SELECT                          
         [guid]                          
        ,@newid                          
        ,[stopName]               
        ,[endDateTime]                          
        ,0                          
        ,@dt                          
        ,@invokingUser                                 
        ,@dt                          
        ,@invokingUser                          
  ,[latitude]                        
  ,[longitude]                        
    FROM #Stop_Data                          
  END                            
  ELSE                            
  BEGIN                            
     UPDATE dbo.[Trip]                            
     SET  [fleetGuid]=@fleetGuid                            
   ,[sourceLocation]=@sourceLocation                        
   ,[sourceLatitude]=@sourceLatitude                        
   ,[sourceLongitude]=@sourceLongitude                        
   ,[destinationLocation] =@destinationLocation                        
   ,[destinationLatitude] =@destinationLatitude                        
   ,[destinationLongitude]=@destinationLongitude                        
   ,[materialTypeGuid] =@materialTypeGuid                       
   ,[weight] =@weight                         
   ,[totalMiles]=@totalMiles                      
   ,[tripId]=@tripId                     
   ,[startDateTime]=@startDateTime                            
   ,[updatedDate] = @dt                            
   ,[updatedBy] = @invokingUser                               
     WHERE                            
   [guid] = @guid                            
   AND [companyGuid] = @companyGuid                            
   AND [isDeleted] = 0                            
                          
MERGE dbo.[TripStops] DA USING #Stop_Data A                          
    ON DA.[tripGuid] = @guid AND DA.[isDeleted] = 0 AND DA.[guid] = A.[guid]                       
    WHEN MATCHED                   
 THEN UPDATE SET                 
  DA.[stopName] = A.[stopName],                          
     DA.[latitude]=A.[latitude],                        
     DA.[longitude]=A.[longitude],                        
     DA.[endDateTime] = A.[endDateTime]                          
    WHEN NOT MATCHED BY TARGET                   
 THEN INSERT ([guid],[tripGuid],[stopName],[endDateTime],[isDeleted],[createdDate],[createdBy],[updatedDate],[updatedBy],[latitude],[longitude])                          
     VALUES (A.[guid], @guid, A.[stopName], A.[endDateTime], 0, @dt, @invokingUser, @dt, @invokingUser,A.[latitude],A.[longitude])                          
    WHEN NOT MATCHED BY SOURCE AND DA.[tripGuid] = @guid                 
    THEN DELETE;                           
  END                            
 COMMIT TRAN                             
 END TRY                               
 BEGIN CATCH                            
 SET @output = 0                            
 DECLARE @errorReturnMessage nvarchar(MAX)                            
                            
 SELECT                            
  @errorReturnMessage = ISNULL(@errorReturnMessage, ' ') + SPACE(1) +                            
  'ErrorNumber:' + ISNULL(CAST(ERROR_NUMBER() AS nvarchar), ' ') +                            
  'ErrorSeverity:' + ISNULL(CAST(ERROR_SEVERITY() AS nvarchar), ' ') +                            
  'ErrorState:' + ISNULL(CAST(ERROR_STATE() AS nvarchar), ' ') +                            
  'ErrorLine:' + ISNULL(CAST(ERROR_LINE() AS nvarchar), ' ') +                            
  'ErrorProcedure:' + ISNULL(CAST(ERROR_PROCEDURE() AS nvarchar), ' ') +                            
  'ErrorMessage:' + ISNULL(CAST(ERROR_MESSAGE() AS nvarchar(MAX)), ' ')                            
                            
 RAISERROR (@errorReturnMessage                            
 , 11                            
 , 1                            
 )                   
                            
 IF (XACT_STATE()) = -1 BEGIN                            
  ROLLBACK TRANSACTION                            
 END                            
 IF (XACT_STATE()) = 1 BEGIN                            
  ROLLBACK TRANSACTION                            
 END                            
 END CATCH                            
END   
  