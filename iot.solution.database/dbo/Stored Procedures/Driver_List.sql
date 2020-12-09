/*******************************************************************      
DECLARE @count INT      
      ,@output INT = 0      
  ,@fieldName VARCHAR(255)      
      
EXEC [dbo].[Driver_List]      
  @companyGuid = 'DD73217B-CD29-4799-A6ED-59DD263EA968'       
 ,@pageSize  = 10      
 ,@search=null    
 ,@pageNumber = 1      
 ,@orderby  = NULL      
 ,@count   = @count OUTPUT      
 ,@invokingUser  = 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'      
 ,@version  = 'v1'      
 ,@output  = @output OUTPUT      
 ,@fieldName  = @fieldName OUTPUT      
      
SELECT @count count, @output status, @fieldName fieldName      
      
      
*******************************************************************/      
CREATE PROCEDURE [dbo].[Driver_List]          
(   @companyGuid  UNIQUEIDENTIFIER   
 ,@currentDate  DATETIME   = NULL    
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
          
    IF (@enableDebugInfo = 1)          
 BEGIN          
        DECLARE @Param XML          
        SELECT @Param =          
        (          
            SELECT 'Driver_List' AS '@procName'          
             , CONVERT(VARCHAR(MAX),@companyGuid) AS '@companyGuid'              
             , CONVERT(VARCHAR(MAX),@search) AS '@search'    
     , CONVERT(VARCHAR(50),@currentDate) as '@currentDate'    
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
  SET @currentDate = ISNULL(@currentDate,GETUTCDATE())     
  IF OBJECT_ID('tempdb..#temp_Driver') IS NOT NULL DROP TABLE #temp_Driver          
           
  CREATE TABLE #temp_Driver          
  (  [guid]     UNIQUEIDENTIFIER        
    ,[companyguid]   UNIQUEIDENTIFIER        
    ,[fleetGuid] UNIQUEIDENTIFIER        
 ,[fleetName]    NVARCHAR(150)         
   ,[firstname]   NVARCHAR(50)          
   ,[lastname]    NVARCHAR(50)         
    ,[name]     NVARCHAR(150)          
   ,[email]    NVARCHAR(100)          
   ,[contactno]   NVARCHAR(25)             
   ,[licenceNo]  NVARCHAR(100)         
   ,[licenceImage]  NVARCHAR(150)          
   ,[image] NVARCHAR(150)         
   ,[isactive]    BIT        
   ,[driverId] NVARCHAR(150)   
   ,[isEditDelete] BIT  
   ,[row_num]    INT          
  )          
          
  IF LEN(ISNULL(@orderby, '')) = 0          
  SET @orderby = 'firstName asc'          
          
  DECLARE @Sql nvarchar(MAX) = ''          
          
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
    , f.[fleetId] AS [fleetName]      
  ,u.[firstname]             
   ,u.[lastname]             
   ,(u.[firstname] + '' '' + u.[lastname]) AS name     
   ,u.[email] AS [email]                  
   ,u.[contactno]           
   ,u.[licenceNo]          
   ,u.[licenceImage]        
      ,u.[image]        
   ,u.[isactive]           
   ,u.[driverId]   
   ,'''' as [isEditDelete]  
   FROM [dbo].[Driver] AS u WITH (NOLOCK)             
   LEFT JOIN [dbo].[Fleet] AS f WITH (NOLOCK) ON f.[guid] = u.[fleetGuid] AND f.[isDeleted] = 0          
   WHERE u.companyguid = @companyguid AND u.[isdeleted] = 0'           
   + CASE WHEN @search IS NULL THEN '' ELSE          
   ' AND (u.firstname LIKE ''%' + @search + '%'' OR u.lastname LIKE ''%' + @search + '%''          
     OR (u.firstname + '' '' + u.lastname) LIKE ''%' + @search + '%''          
     OR u.email LIKE ''%' + @search + '%''         
   OR u.driverId LIKE ''%' + @search + '%''          
     OR f.[registrationNo] LIKE ''%' + @search + '%''          
     OR u.[contactno] LIKE ''%' + @search + '%''          
  OR u.[licenceNo] LIKE ''%' + @search + '%''        
   ) '          
    END +          
  ') data '         
            
  INSERT INTO #temp_Driver          
  EXEC sp_executesql           
     @Sql          
   , N'@orderby VARCHAR(100), @companyGuid UNIQUEIDENTIFIER  '          
   , @orderby  = @orderby             
   , @companyGuid = @companyGuid             
            
  SET @count = @@ROWCOUNT          
          ;WITH CTE_Trips  
  AS (   
   SELECT  Dr.[guid], D.[startDateTime]   
   ,(SELECT top 1 [endDateTime] from [dbo].[TripStops] (NOLOCK) WHERE [tripGuid] = D.[Guid] AND [isDeleted] = 0 and [endDateTime]<=@currentDate ORDER BY [endDateTime] DESC) AS [endDateTime]   
   ,D.[isStarted],D.[isCompleted]  
    FROM dbo.[Trip] D (NOLOCK)  
    INNER JOIN dbo.[Driver] Dr ON Dr.[fleetGuid] = D.[fleetGuid] AND Dr.[isDeleted] = 0  
    INNER JOIN dbo.[Fleet] E ON Dr.[fleetGuid] = E.[guid] AND E.[isDeleted] = 0       
    WHERE D.[companyGuid] = @companyGuid AND D.[isDeleted] = 0     
  )  
  --, CTE_Maintenance  
  --AS (   
  -- SELECT DM.[deviceGuid] AS [fleetGuid]  
  --   , DM.[guid] AS [guid]  
  --   ,DM.[startDateTime],DM.[endDateTime]  
  --    , DM.[isCompleted]   
  --  FROM dbo.[DeviceMaintenance] DM (NOLOCK)   
  --  INNER JOIN dbo.[Driver] Dr ON Dr.[fleetGuid] = DM.[deviceGuid] AND Dr.[isDeleted] = 0  
  --  INNER JOIN [dbo].[Fleet] E ON Dr.[fleetGuid] = E.[guid] AND E.[isDeleted] = 0  
  --  WHERE DM.[companyGuid] = @companyGuid   
  --  AND DM.[IsDeleted]=0   
  -- )  
   --SELECT * FROM CTE_Trips  
   UPDATE E   
  SET [isEditDelete]=ISNULL((SELECT TOP 1  CASE WHEN (CTR.[isStarted]=1 AND CTR.[isCompleted]=0)  
      THEN  0   
       ELSE CASE WHEN (CTR.[isCompleted]=1)   
      THEN 1     
      ELSE CASE WHEN CTR.[endDateTime] < @currentDate  
      THEN 1    
      ELSE CASE WHEN @currentDate >=CTR.[startDateTime]   
      THEN 1                
      END END END END FROM CTE_Trips CTR   
  WHERE CTR.[guid]=E.[guid] ORDER BY CTR.[startDateTime] DESC  
 ),1 )  
  FROM #temp_Driver E       
  --LEFT JOIN CTE_Trips CTR ON E.[guid] = CTR.[guid]  
  --LEFT JOIN CTE_Maintenance CTM ON E.[fleetGuid] = CTM.[fleetGuid]   
  IF(@pageSize <> -1 AND @pageNumber <> -1)          
   BEGIN          
    SELECT           
     d.[guid]     
  ,d.[companyguid]    
  ,d.[fleetGuid]        
  ,d.[fleetName]     
   ,d.[firstname]          
    ,d.[lastname]        
    ,d.[name]       
    ,d.[email]     
  ,d.[contactno]     
  ,d.[licenceNo]    
  ,d.[licenceImage]     
  ,d.[image]      
   ,d.[isactive]     
    ,d.[driverId]     
 ,d.[isEditDelete]  
    FROM #temp_Driver d        
    WHERE row_num BETWEEN ((@pageNumber - 1) * @pageSize) + 1 AND (@pageSize * @pageNumber)             
   END          
  ELSE          
   BEGIN          
     SELECT           
     d.[guid]     
  ,d.[companyguid]    
  ,d.[fleetGuid]        
  ,d.[fleetName]     
   ,d.[firstname]          
    ,d.[lastname]        
    ,d.[name]       
    ,d.[email]     
  ,d.[contactno]     
  ,d.[licenceNo]    
  ,d.[licenceImage]     
  ,d.[image]      
   ,d.[isactive]     
    ,d.[driverId]  
 ,d.[isEditDelete]  
    FROM #temp_Driver d        
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