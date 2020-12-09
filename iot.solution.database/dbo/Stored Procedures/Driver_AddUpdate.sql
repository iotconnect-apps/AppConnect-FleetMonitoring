/*******************************************************************        
DECLARE @output INT = 0        
 ,@fieldName nvarchar(255)         
 ,@newid  UNIQUEIDENTIFIER        
EXEC [dbo].[Driver_AddUpdate]         
  @companyGuid = 'AB469212-2488-49AD-BC94-B3A3F45590D2'        
 ,@Guid   = '5CAD4F30-C848-49ED-AF85-2391425C489F'        
 ,@fleetGuid='98611812-0DB2-4183-B352-C3FEC9A3D1A4'      
 ,@firstName   = 'Rocky'        
 ,@lastName   = 'Lee'        
 ,@email   = 'sunil12@test.com'        
 ,@contactNo = '9893598935'      
 ,@address  = 'Indore'        
 ,@city   = 'Indore'        
 ,@stateGuid  = 'dd3df070-597a-4248-b5d5-8e6a47e4be9b'         
 ,@countryGuid = '4bd21ec1-7fe3-4d26-8e41-f142933c4e54'         
 ,@zipCode  = '111111'         
 ,@image   = NULL        
 ,@licenceNo = 'MP142015K1'      
 ,@licenceImage   = NULL    
 ,@driverId = 'D-10114'  
 ,@invokingUser = 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'                      
 ,@version  = 'v1'                      
 ,@newid   = @newid  OUTPUT        
 ,@output  = @output  OUTPUT        
 ,@fieldName  = @fieldName OUTPUT         
        
SELECT @output status, @fieldName fieldName, @newid newid        
        
001 SFM-1 28-11-2019 [Sunil Bhawsar] Added Initial Version to Add Driver          
*******************************************************************/        
Create PROCEDURE [dbo].[Driver_AddUpdate]        
( @companyGuid UNIQUEIDENTIFIER        
 ,@guid   UNIQUEIDENTIFIER        
 ,@fleetGuid UNIQUEIDENTIFIER        
 ,@firstName   NVARCHAR(50)        
 ,@lastName  NVARCHAR(50)       
 ,@email     NVARCHAR(100)        
 ,@contactNo NVARCHAR(25)      
 ,@licenceNo NVARCHAR (25)      
 ,@address  NVARCHAR(500)        
 ,@city   NVARCHAR(50)  = NULL        
 ,@stateGuid  UNIQUEIDENTIFIER = NULL        
 ,@countryGuid UNIQUEIDENTIFIER = NULL        
 ,@zipCode  NVARCHAR(10)  = NULL        
 ,@image   NVARCHAR(250)  = NULL        
 ,@licenceImage   NVARCHAR(250)  = NULL      
 ,@driverId NVARCHAR(150)  
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
 DECLARE @dt DATETIME = GETUTCDATE()        
    IF (@enableDebugInfo = 1)        
 BEGIN        
        DECLARE @Param XML         
        SELECT @Param =         
        (        
            SELECT 'Driver_AddUpdate' AS '@procName'                     
             , CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid'         
    , CONVERT(nvarchar(MAX),@fleetGuid) AS '@fleetGuid'         
             , CONVERT(nvarchar(MAX),@guid) AS '@guid'         
      
    , @firstName AS '@firstName'         
 , @lastName AS '@lastName'       
    , @email AS '@email'        
    , @contactNo AS '@contactNo'         
 , @licenceNo AS '@licenceNo'       
    , @address AS '@address'        
    , @city AS '@city'         
    , CONVERT(nvarchar(MAX),@stateGuid) AS '@stateGuid'         
    , CONVERT(nvarchar(MAX),@countryGuid) AS '@countryGuid'         
    , @zipCode AS '@zipCode'        
    , @image AS '@image'        
 , @licenceImage AS '@licenceImage'  
 , @driverId AS '@driverId'  
    , CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'        
             , CONVERT(nvarchar(MAX),@version) AS '@version'         
             , CONVERT(nvarchar(MAX),@output) AS '@output'         
             , @fieldName AS '@fieldName'           
            FOR XML PATH('Params')        
     )         
     INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), @dt)        
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
        
  IF NOT EXISTS(SELECT TOP 1 1 FROM dbo.[Driver] (NOLOCK) WHERE [guid] = @guid and [companyGuid] = @companyGuid and [isdeleted] = 0)                  
  BEGIN                  
   SET @bool = 1;                  
  END 

 BEGIN TRY        
   IF @bool = 1                  
  BEGIN                  
    IF EXISTS (SELECT TOP 1 1 FROM dbo.[Driver] (NOLOCK) WHERE [companyguid] = @companyguid AND [isdeleted]=0 AND [email]=@email)        
   BEGIN        
    SET @output = -3        
    SET @fieldname = 'EmailIdAlreadyExists'           
    RETURN;        
   END                 
  END 
         
  IF @bool = 1
  BEGIN        
   IF EXISTS (SELECT TOP 1 1 FROM dbo.[Driver] (NOLOCK) WHERE [companyguid] = @companyguid AND [isdeleted]=0 AND [licenceNo]=@licenceNo)        
   BEGIN        
    SET @output = -2        
    SET @fieldname = 'LicenceNoAlreadyExists'           
    RETURN;        
   END        
  END     
   
  IF @bool = 1
  BEGIN      
   IF EXISTS (SELECT TOP 1 1 FROM dbo.[Driver] (NOLOCK) WHERE [companyguid] = @companyguid AND [isdeleted]=0 AND [driverId]=@driverId)      
   BEGIN      
    SET @output = -1      
    SET @fieldname = 'DriverIdAlreadyExists'         
    RETURN;      
   END      
  END      
 BEGIN TRAN         
  IF @bool = 1
  BEGIN         
   INSERT INTO dbo.[Driver](        
    [guid]         
    ,[companyGuid]        
 ,[fleetGuid]      
 ,[firstName]      
 ,[lastName]      
 ,[email]      
 ,[contactNo]      
    ,[city]        
    ,[stateGuid]        
    ,[countryGuid]        
    ,[zipCode]        
    ,[image]        
 ,[address]      
 ,[licenceNo]      
 ,[licenceImage]      
    ,[isActive]        
    ,[isDeleted]        
    ,[createddate]        
    ,[createdby]        
    ,[updatedDate]        
    ,[updatedBy]  
 ,[driverId]  
    )        
   VALUES(@guid         
    ,@companyGuid       
 ,@fleetGuid      
 ,@firstName      
 ,@lastName      
 ,@email      
 ,@contactNo      
    ,@city       
    ,@stateGuid       
    ,@countryGuid      
    ,@zipCode       
    ,@image      
 ,@address      
 ,@licenceNo      
 ,@licenceImage      
    ,1        
    ,0           
    ,@dt        
    ,@invokingUser        
    ,@dt        
    ,@invokingUser   
 ,@driverId  
   )        
  END        
  ELSE        
  BEGIN        
   UPDATE dbo.[Driver]        
   SET fleetGuid=@fleetGuid      
 ,firstName=@firstName      
 ,lastName=@lastName      
 ,email=@email      
 ,contactNo=@contactNo      
    ,city=@city      
    ,stateGuid=@stateGuid        
    ,countryGuid=@countryGuid      
    ,zipCode=@zipCode        
    ,image =@image        
 ,address=@address      
 ,licenceNo=@licenceNo      
 ,licenceImage=@licenceImage      
    ,[updatedDate] = @dt        
    ,[updatedBy] = @invokingUser   
 ,[driverId]=@driverId  
   WHERE        
    [guid] = @guid        
    AND [companyGuid] = @companyGuid        
    AND [isDeleted] = 0        
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