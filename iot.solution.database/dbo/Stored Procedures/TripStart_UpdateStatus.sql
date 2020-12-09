/*******************************************************************      
BEGIN TRAN      
      
DECLARE @output INT = 0       
 ,@fieldname nvarchar(100)      
         
EXEC [dbo].[TripStart_UpdateStatus]        
  @tripId   = '7B49233C-F1D5-4641-B5E3-3794AC5EE94D' 
 ,@fleetId='A8A288DF-2E0D-4010-9AAE-C35B388CD92A'
 ,@companyGuid = '388127DE-D5A3-49A5-9569-C4ADB0CB5ABB'       
 ,@actualStartDateTime  = '2020-09-30 08:45:08.237'      
 ,@etaEndDateTime  = '2020-09-30 10:40:08.237'      
 ,@invokinguser  = 'FF221908-486A-4E5A-843A-C68EB413F6EA'      
 ,@output  = @output OUTPUT        
 ,@version  = 'v1'      
 ,@fieldname  = @fieldname OUTPUT      
      
SELECT @output status, @fieldname fieldName      
      
ROLLBACK TRAN      
    
*******************************************************************/      
  CREATE PROCEDURE [dbo].[TripStart_UpdateStatus]      
   @tripId     UNIQUEIDENTIFIER   
  ,@fleetId     UNIQUEIDENTIFIER
  ,@companyGuid   UNIQUEIDENTIFIER       
  ,@actualStartDateTime   DATETIME    = NULL          
  ,@etaEndDateTime   DATETIME    = NULL     
  ,@odometer BIGINT =0
  ,@invokinguser   UNIQUEIDENTIFIER = NULL      
  ,@version    nvarchar(10)          
  ,@output    SMALLINT   OUTPUT          
  ,@fieldname    nvarchar(100)  OUTPUT         
  ,@culture    nvarchar(10)  = 'en-Us'      
  ,@enabledebuginfo  CHAR(1)    = '0'      
AS      
BEGIN      
 SET NOCOUNT ON      
   
    IF (@enabledebuginfo = 1)      
 BEGIN      
        DECLARE @Param XML       
        SELECT @Param =       
        (      
            SELECT 'TripStart_UpdateStatus' AS '@procName'       
            , CONVERT(nvarchar(MAX),@tripId) AS '@tripId'       
     , CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid'         
     , CONVERT(nvarchar(100),@actualStartDateTime) AS '@actualStartDateTime'      
     , CONVERT(nvarchar(100),@etaEndDateTime) AS '@etaEndDateTime'     
            , CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser'       
            , CONVERT(nvarchar(MAX),@version) AS '@version'       
            , CONVERT(nvarchar(MAX),@output) AS '@output'       
            , CONVERT(nvarchar(MAX),@fieldname) AS '@fieldname'         
            FOR XML PATH('Params')      
     )       
     INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())      
    END             
       
 SET @output = 1      
 SET @fieldname = 'Success'      
      
 BEGIN TRY        
  IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[Trip] (NOLOCK) WHERE [guid] = @tripId AND [companyGuid] = @companyGuid AND [isDeleted] = 0)      
  BEGIN      
   SET @output = -2      
   SET @fieldname = 'TripNotFound'      
   RETURN;      
  END    
  
 
  
  IF EXISTS (SELECT top 1 1 FROM [dbo].[Trip] (NOLOCK) where fleetGuid=@fleetId and [guid] !=@tripId and isStarted=1 and isCompleted=0 and isDeleted=0)      
  BEGIN      
   SET @output = -2      
   SET @fieldname = 'TripAlreadyRunningForFleet'      
   RETURN;      
  END 
   IF EXISTS (SELECT top 1 1 FROM [dbo].[Trip] (NOLOCK) where fleetGuid=@fleetId and [guid] =@tripId and isStarted=1 and isCompleted=0 and isDeleted=0)      
  BEGIN      
   SET @output = -2      
   SET @fieldname = 'TripAlreadyRunning'      
   RETURN;      
  END 
   IF NOT EXISTS (SELECT top 1 1 FROM [dbo].[Trip] as T WITH (NOLOCK) INNER JOIN [dbo].[TripStops] as TS WITH (NOLOCK) ON T.[guid] = TS.[tripGuid] where T.[guid]=@tripId and T.[isDeleted]=0 and T.[isCompleted]=0 and ((@actualStartDateTime BETWEEN T.[startDateTime] AND (SELECT top 1 endDateTime from [dbo].[TripStops] WITH (NOLOCK) where [isDeleted] =0 and [tripGuid] = T.[guid] ORDER BY [endDateTime] DESC))))      
  BEGIN  
		DECLARE @newTripGuid UNIQUEIDENTIFIER =null
		SET @newTripGuid = (SELECT top 1 T.[guid] FROM [dbo].[Trip] as T WITH (NOLOCK) INNER JOIN [dbo].[TripStops] as TS WITH (NOLOCK) ON T.[guid] = TS.[tripGuid] where T.[fleetGuid]=@fleetId and T.[isDeleted]=0 and T.[isCompleted]=0 and ((@actualStartDateTime BETWEEN T.[startDateTime] AND (SELECT top 1 endDateTime from [dbo].[TripStops] WITH (NOLOCK) where [isDeleted] =0 and [tripGuid] = T.[guid] ORDER BY [endDateTime] DESC))))       
     IF @newTripGuid IS NULL      
	 BEGIN   
		   SET @output = -2      
		   SET @fieldname = 'TripNotFoundForCurrentDate'      
		   RETURN;      
	 END
	 ELSE 
	 BEGIN
		SET @tripId = @newTripGuid
	 END
  END
  BEGIN TRAN     
      UPDATE [dbo].[Trip]      
      SET [isStarted] = 1      
     ,[actualStartDateTime] = @actualStartDateTime      
     ,[etaEndDateTime] = @etaEndDateTime
     ,[odometer]=@odometer
	 ,[updatedDate] =  @actualStartDateTime                        
     ,[updatedBy] = @invokingUser  
     WHERE [guid] = @tripId AND [companyGuid] = @companyGuid AND isStarted=0 AND [isDeleted] = 0    
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
    
    