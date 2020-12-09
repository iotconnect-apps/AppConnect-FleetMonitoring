
/*******************************************************************
DECLARE @output INT = 0
	,@fieldName	nvarchar(255)	
	,@newid		UNIQUEIDENTIFIER
EXEC [dbo].[Fleet_AddUpdate]	
	@companyGuid	= 'FD2374F2-EF4F-4AF7-95BB-95185422E16D'
	,@Guid			= '98611812-0DB2-4183-B352-C3FEC9A3D1A4'
	,@fleetId ='ID101' 
	,@registrationNo ='ABCD 123' 
	,@loadingCapacity ='25'
	,@typeGuid  = 'C4DEABFD-77BE-443C-8D62-27D8839F2650'
	,@materialTypeGuid  = 'B6890D1C-4E7C-4EFF-9A3B-252226722DF2'
	,@speedLimit = ''
	,@image				= ''
	,@deviceData				= '<deviceInfos><deviceInfo><templateGuid>12A5CD86-F6C6-455F-B27A-EFE587ED410D</templateGuid><deviceGuid>12A5CD86-F6C6-455F-B27A-EFE587ED410D</deviceGuid></deviceInfo>                                       
                               </deviceInfos>'
	,@invokingUser	= 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'              
	,@version		= 'v1'              
	,@newid			= @newid		OUTPUT
	,@output		= @output		OUTPUT
	,@fieldName		= @fieldName	OUTPUT	

SELECT @output status, @fieldName fieldName, @newid newid


*******************************************************************/
CREATE PROCEDURE [dbo].[Fleet_AddUpdate]
(	@companyGuid	UNIQUEIDENTIFIER
	,@guid			UNIQUEIDENTIFIER 		=NULL 
	,@fleetId nvarchar(100) 
	,@registrationNo nvarchar(100) 
	,@loadingCapacity nvarchar(100)
	,@typeGuid uniqueidentifier = NULL
	,@materialTypeGuid uniqueidentifier = NULL
	,@speedLimit nvarchar(250)= NULL
	,@image			NVARCHAR(250)		= NULL
	,@latitude nvarchar(50) =null
	,@longitude nvarchar(50) =null
	,@radius int =0
	,@deviceData			XML					= NULL
	,@totalMiles int =NULL
	,@invokingUser	UNIQUEIDENTIFIER
	,@version		nvarchar(10)    
	,@newid			UNIQUEIDENTIFIER	OUTPUT
	,@output		SMALLINT			OUTPUT    
	,@fieldName		nvarchar(100)		OUTPUT   
	,@culture		nvarchar(10)		= 'en-Us'
	,@enableDebugInfo	CHAR(1)			= '0'
)	
AS
BEGIN

	SET @enableDebugInfo = 1
	SET NOCOUNT ON
	DECLARE @dt DATETIME = GETUTCDATE()
    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'Fleet_AddUpdate' AS '@procName'             
            	, CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid' 
            	, CONVERT(nvarchar(MAX),@guid) AS '@guid' 				
				, @fleetId AS '@fleetId' 
				, @registrationNo	AS '@registrationNo'
				, @loadingCapacity AS '@loadingCapacity' 
				,  CONVERT(nvarchar(MAX),@typeGuid) AS '@typeGuid'
				,  CONVERT(nvarchar(MAX),@materialTypeGuid) AS '@materialTypeGuid'			
				, @speedLimit AS '@speedLimit'					
				, @image AS '@image'	
				,@latitude AS '@latitude'
				,@longitude AS '@longitude'
				,@radius AS '@radius'	
				, CONVERT(nvarchar(MAX),@deviceData) AS '@deviceData'		
				, CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
            	, CONVERT(nvarchar(MAX),@version) AS '@version' 
            	, CONVERT(nvarchar(MAX),@output) AS '@output' 
            	, @fieldName AS '@fieldName'   
            FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), @dt)
    END       
	
	DECLARE @poutput		SMALLINT
			,@pFieldName	nvarchar(100)
    
	  IF(@poutput!=1)
      BEGIN
        SET @output = @poutput
        SET @fieldName = @pfieldName
        RETURN;
      END
	SET @newid = @guid

	SET @output = 1
	SET @fieldName = 'Success'

	BEGIN TRY

		
			IF EXISTS (SELECT TOP 1 1 FROM [Fleet] (NOLOCK) WHERE @guid is null and [companyguid] = @companyguid AND [isdeleted]=0 AND [registrationNo]=@registrationNo)
			BEGIN
				SET @output = -3
				SET @fieldname = 'RegistrationNoAlreadyExists'		 
				RETURN;
			END
		
		IF EXISTS (SELECT TOP 1 1 FROM [Fleet] (NOLOCK) WHERE @guid is null and [companyguid] = @companyguid AND [isdeleted]=0 AND [fleetId]=@fleetId)
			BEGIN
				SET @output = -2
				SET @fieldname = 'FleetIdAlreadyExists'		 
				RETURN;
			END
			IF EXISTS (SELECT TOP 1 1 FROM [Fleet] (NOLOCK) WHERE @guid is not null and [companyguid] = @companyguid AND [isdeleted]=0)
			BEGIN
				IF OBJECT_ID('tempdb..#tripsFAU') IS NOT NULL BEGIN DROP TABLE #tripsFAU END
				CREATE TABLE #tripsFAU ( [fleetGuid] UNIQUEIDENTIFIER, [startDateTime] DATETIME,[endDateTime] DATETIME,[isStarted] BIT,[isCompleted] BIT) 
				INSERT INTO #tripsFAU ([fleetGuid], [startDateTime],[endDateTime],[isStarted] ,[isCompleted]) 
				SELECT [fleetGuid], [startDateTime],			
				(SELECT top 1 endDateTime from [dbo].[TripStops] (NOLOCK) WHERE [tripGuid] = T.[Guid] AND [isDeleted] = 0 ORDER BY [endDateTime] DESC) AS [endDateTime] 			
					,[isStarted] ,[isCompleted] FROM [dbo].[Trip] T (NOLOCK) 
					WHERE [fleetGuid] = @guid AND [isDeleted] = 0 
				IF EXISTS (SELECT 1 from #tripsFAU WHERE ([isStarted]=1 AND [isCompleted]=0) and [fleetGuid]=@guid)--(@dt >= [startDateTime] AND @dt <= [endDateTime])
				BEGIN
					SET @output = -3
					SET @fieldname = 'OnGoingTripExists'						
					RETURN;
				END

				IF EXISTS (SELECT 1 from dbo.[DeviceMaintenance] DM (NOLOCK) WHERE [isDeleted] = 0 AND @dt >= [startDateTime] AND @dt <= [endDateTime] AND ISNULL([isCompleted],0)=0 and [deviceGuid]=@guid)
				BEGIN
					SET @output = -1
					SET @fieldname = 'OnGoingMaintenanceExists'						
					RETURN;
				END
			END
		IF OBJECT_ID ('tempdb..#Device_Data') IS NOT NULL DROP TABLE #Device_Data

		CREATE TABLE #Device_Data
		(	[templateGuid]		UNIQUEIDENTIFIER
			,[deviceGuid]			UNIQUEIDENTIFIER
			
		)
		INSERT INTO #Device_Data
		SELECT DISTINCT 
				 x.R.query('./templateGuid').value('.', 'NVARCHAR(50)') AS [templateGuid]
				, x.R.query('./deviceGuid').value('.', 'NVARCHAR(50)') AS [deviceGuid]
				
			FROM @deviceData.nodes('/deviceInfos/deviceInfo') as x(R)
	BEGIN TRAN	
		IF NOT EXISTS(SELECT TOP 1 1 FROM [Fleet] (NOLOCK) where [registrationNo] = @registrationNo and companyGuid = @companyGuid AND isdeleted = 0 )
		BEGIN	
			SET @newid = NEWID() 
			INSERT INTO dbo.[Fleet](
				[guid]					
				,[companyGuid]				
				,[fleetId]
				,[registrationNo] 
				,[loadingCapacity] 
				,[typeGuid] 
				,[materialTypeGuid] 
				,[image] 
				,[speedLimit]
				,[latitude]
				,[longitude] 
				,[radius]
				,[totalMiles]
				,[isActive]
				,[isDeleted]
				,[createddate]
				,[createdby]
				,[updatedDate]
				,[updatedBy]
				)
			VALUES(@newid					
				,@companyGuid
				,@fleetId
				,@registrationNo
				,@loadingCapacity
				,@typeGuid
				,@materialTypeGuid
				,@image
				,@speedLimit
				,@latitude
				,@longitude
				,@radius
				,ISNULL(@totalMiles,0)
				,1
				,0			
				,@dt
				,@invokingUser
				,@dt
				,@invokingUser
			)
			UPDATE D 
			SET [fleetGuid]= @newid
			FROM dbo.[Device] AS D (NOLOCK) INNER JOIN #device_Data AS A
			 ON D.[templateGuid]=A.[templateGuid] AND D.[Guid]= A.[deviceGuid] 			

		END
		ELSE
		BEGIN
			UPDATE dbo.[Fleet]
			SET --[fleetId]=@fleetId
				--,[registrationNo]=@registrationNo
				[loadingCapacity] =@loadingCapacity
				,[typeGuid] =@typeGuid
				,[materialTypeGuid] =@materialTypeGuid
				,[image] =@image
				,[speedLimit]=@speedLimit
				,[latitude]=@latitude
				,[longitude]=@longitude
				,[radius]=@radius
				,[totalMiles]=ISNULL(totalMiles,0) + ISNULL(@totalMiles,ISNULL(totalMiles,0))
				,[updatedDate]	= @dt
				,[updatedBy]	= @invokingUser			
			WHERE
				[guid] = @guid
				AND [companyGuid] = @companyGuid
				AND [isDeleted] = 0
				UPDATE D 
			SET D.[fleetGuid]= NULL 
			FROM dbo.[Device] AS D (NOLOCK) WHERE D.[fleetGuid]=@guid

			UPDATE D 
			SET [fleetGuid]= @guid
			FROM dbo.[Device] AS D (NOLOCK) INNER JOIN #device_Data AS A
			 ON D.[templateGuid]=A.[templateGuid] AND D.[Guid]= A.[deviceGuid] 		
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

