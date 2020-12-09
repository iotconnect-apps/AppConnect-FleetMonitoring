  
/*******************************************************************  
BEGIN TRAN  
  
DECLARE @output INT = 0   
 ,@status nvarchar(50)  
 ,@fieldname nvarchar(100)  
     
EXEC [dbo].[ShipmentFiles_UpdateStatus]    
 @tripGuid = '651B311A-40D7-4304-B3C3-7C2DC4E8F505'   
 ,@guid   = '651B311A-40D7-4304-B3C3-7C2DC4E8F505'   
 ,@status  = 0  
 ,@invokinguser  = 'FF221908-486A-4E5A-843A-C68EB413F6EA'  
 ,@output  = @output OUTPUT    
 ,@version  = 'v1'  
 ,@fieldname  = @fieldname OUTPUT  
  
SELECT @output status, @fieldname fieldName  
  
ROLLBACK TRAN  
  
*******************************************************************/  
  
CREATE PROCEDURE [dbo].[ShipmentFiles_UpdateStatus]  
  @tripGuid  UNIQUEIDENTIFIER  
  ,@guid    UNIQUEIDENTIFIER = NULL  
  ,@status   BIT  
  ,@invokinguser  UNIQUEIDENTIFIER = NULL  
  ,@version   nvarchar(10)      
  ,@output   SMALLINT   OUTPUT      
  ,@fieldname   nvarchar(100)  OUTPUT     
  ,@culture   nvarchar(10)  = 'en-Us'  
  ,@enabledebuginfo CHAR(1)    = '0'  
AS  
BEGIN  
 SET NOCOUNT ON  
  
    IF (@enabledebuginfo = 1)  
 BEGIN  
        DECLARE @Param XML   
        SELECT @Param =   
        (  
            SELECT 'ShipmentFiles_UpdateStatus' AS '@procName'   
            , CONVERT(nvarchar(MAX),@tripGuid) AS '@tripGuid'      
   , CONVERT(nvarchar(MAX),@guid) AS '@guid'      
            , CONVERT(nvarchar(MAX),@status) AS '@status'   
            , CONVERT(nvarchar(MAX),@invokinguser) AS '@invokinguser'      
            , CONVERT(nvarchar(MAX),@version) AS '@version'   
            , CONVERT(nvarchar(MAX),@output) AS '@output'   
            , CONVERT(nvarchar(MAX),@fieldname) AS '@fieldname'     
            FOR XML PATH('Params')  
     )   
     INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())  
    END         
   
 DECLARE @dt DATETIME = GETUTCDATE()  
  
 SET @output = 1  
 SET @fieldname = 'Success'  
  
 BEGIN TRY    
  IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[ShipmentFiles] (NOLOCK) WHERE [tripGuid] = @tripGuid AND [guid] = ISNULL(@guid,[guid]) AND [isdeleted]=0)  
  BEGIN  
   SET @output = -2  
   SET @fieldname = 'ShipmantFileNotFound'  
  
   RETURN;  
  END      
    
  BEGIN TRAN  
  
   UPDATE [dbo].[ShipmentFiles]   
   SET [isDeleted]   = @status  
    ,[updateddate]  = @dt  
    ,[updatedby]  = @invokinguser  
   WHERE [tripGuid] = @tripGuid   
    AND [guid] = ISNULL(@guid,[guid])  
    AND [isdeleted]=0   
  
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