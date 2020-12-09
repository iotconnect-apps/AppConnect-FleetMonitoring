/*******************************************************************        
EXEC [dbo].[Get_UserSubscriptionEndData]        
*******************************************************************/        
CREATE PROCEDURE [dbo].[Get_UserSubscriptionEndData]        
AS        
BEGIN        
    SET NOCOUNT ON        
        
    DECLARE @dt DATETIME = CONVERT(DATE, DATEADD(DAY, 7, GETUTCDATE()))     
    BEGIN TRY            
    SELECT      
     T.[guid],T.[email],cast(T.[subscriptionEndDate] as Date) as expiryDate,T.[firstName] +' '+T.[lastName] as customerName
    FROM       
    [dbo].[User] as T WITH (NOLOCK)    
    WHERE       
    cast(T.[subscriptionEndDate] as Date)=@dt and T.[isActive]=1     
    and T.[isDeleted]=0 and T.[subscriptionEndDate] is not NULL       
   END TRY           
   BEGIN CATCH           
    DECLARE @errorReturnMessage nvarchar(MAX)          
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