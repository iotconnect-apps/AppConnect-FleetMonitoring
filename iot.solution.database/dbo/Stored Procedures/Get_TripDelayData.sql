/*******************************************************************      
EXEC [dbo].[Get_TripDelayData]      
*******************************************************************/      
CREATE PROCEDURE [dbo].[Get_TripDelayData]      
--(      
--  @output   SMALLINT   OUTPUT      
-- ,@fieldName   VARCHAR(255)  OUTPUT      
--)      
AS      
BEGIN      
    SET NOCOUNT ON      
      
    DECLARE @dt DATETIME = GETUTCDATE();    
     --   DECLARE @Param XML                                 
     --   SELECT @Param =                                 
     --   (                                
     --       SELECT 'Get_TripDelayData' AS '@procName'                                             
     --        , CONVERT(nvarchar(MAX),@output) AS '@output'                               
     --        , @fieldName AS '@fieldName'                                     
     --       FOR XML PATH('Params')                                
     --)       
    BEGIN TRY          
    SELECT    
    T.[guid],T.[fleetGuid],T.[sourceLocation],T.[destinationLocation],T.[startDateTime],F.[fleetId] AS [fleetName],U.[email] as ownerEmail,U.[firstName] +' '+U.[lastName] as ownerName,D.[email] as driverEmail,D.[firstName] +' '+D.[lastName] as driverName,
     
    DATEDIFF(minute, [startDateTime], @dt) as delayInMin    
    FROM     
    [dbo].[Trip] as T WITH (NOLOCK)  
 inner Join [dbo].[Fleet] AS F WITH (NOLOCK) ON F.[guid] = T.[fleetGuid] AND F.[isDeleted] = 0    
 inner Join [dbo].[Driver] AS D WITH (NOLOCK) ON D.[fleetGuid] = T.[fleetGuid] AND D.[isDeleted] = 0    
 inner Join [dbo].[User] AS U WITH (NOLOCK) ON U.[guid] = T.[createdBy] AND U.[isDeleted] = 0    
    WHERE     
    T.[startDateTime]< @dt and T.[isActive]=1    
    and T.[isDeleted]=0 and [isStarted]=0 and [isCompleted]=0 and @dt<(SELECT top 1 endDateTime from [dbo].[TripStops] (NOLOCK) WHERE [tripGuid] = T.[Guid] AND [isDeleted] = 0 ORDER BY [endDateTime] DESC)    
    ORDER BY     
    T.[startDateTime] desc     
        
    --SET @output = 1        
    --SET @fieldname = 'Success'           
   END TRY         
   BEGIN CATCH         
    DECLARE @errorReturnMessage nvarchar(MAX)        
        
    --SET @output = 0        
        
    SELECT @errorReturnMessage =         
     ISNULL(@errorReturnMessage, '') +  SPACE(1)   +         
     'ErrorNumber:'  + ISNULL(CAST(ERROR_NUMBER() as nvarchar), '')  +         
     'ErrorSeverity:'  + ISNULL(CAST(ERROR_SEVERITY() as nvarchar), '') +         
     'ErrorState:'  + ISNULL(CAST(ERROR_STATE() as nvarchar), '') +         
     'ErrorLine:'  + ISNULL(CAST(ERROR_LINE () as nvarchar), '') +         
     'ErrorProcedure:'  + ISNULL(CAST(ERROR_PROCEDURE() as nvarchar), '') +         
     'ErrorMessage:'  + ISNULL(CAST(ERROR_MESSAGE() as nvarchar(max)), '')        
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