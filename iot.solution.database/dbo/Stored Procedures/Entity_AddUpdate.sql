
/*******************************************************************
DECLARE @output INT = 0
	,@fieldName	nvarchar(255)	
	,@newid		UNIQUEIDENTIFIER
EXEC [dbo].[Entity_AddUpdate]	
	@companyGuid	= 'AB469212-2488-49AD-BC94-B3A3F45590D2'
	,@Guid			= '98611812-0DB2-4183-B352-C3FEC9A3D1A4'
	,@parentEntityGuid	= '98611812-0DB2-4183-B352-C3FEC9A3D1A4'	
	,@name			= 'Chicago 7'
	,@type			= 'indoor'
	,@description	= 'Chicago 5 Description'
	,@address		= 'Chicago 5'
	,@address2		= 'Chicago 5'
	,@city			= 'Chicago 5'
	,@stateGuid		= 'dd3df070-597a-4248-b5d5-8e6a47e4be9b'	
	,@countryGuid	= '4bd21ec1-7fe3-4d26-8e41-f142933c4e54'	
	,@zipCode		= '111111'	
	,@image			= NULL
	,@latitude		= NULL
	,@longitude		= NULL
	,@invokingUser	= 'C1596B8C-7065-4D63-BFD0-4B835B93DFF2'              
	,@version		= 'v1'              
	,@newid			= @newid		OUTPUT
	,@output		= @output		OUTPUT
	,@fieldName		= @fieldName	OUTPUT	

SELECT @output status, @fieldName fieldName, @newid newid

001	SAQM-1 28-11-2019 [Nishit Khakhi]	Added Initial Version to Add Entity
002 SAQM-1 17-03-2020 [Nishit Khakhi]	Updated to add column 'type', parent entity guid
*******************************************************************/
CREATE PROCEDURE [dbo].[Entity_AddUpdate]
(	@companyGuid	UNIQUEIDENTIFIER
	,@guid			UNIQUEIDENTIFIER
	,@parentEntityGuid UNIQUEIDENTIFIER	= NULL
	,@name			NVARCHAR(500)
	,@type			NVARCHAR(100)		= NULL
	,@description	NVARCHAR(1000)	
	,@address		NVARCHAR(500)
	,@address2		NVARCHAR(500)		= NULL
	,@city			NVARCHAR(50)		= NULL
	,@stateGuid		UNIQUEIDENTIFIER	= NULL
	,@countryGuid	UNIQUEIDENTIFIER	= NULL
	,@zipCode		NVARCHAR(10)		= NULL
	,@latitude		NVARCHAR(50)		= NULL
	,@longitude		NVARCHAR(50)		= NULL
	,@image			NVARCHAR(250)		= NULL
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
            SELECT 'Entity_AddUpdate' AS '@procName'             
            	, CONVERT(nvarchar(MAX),@companyGuid) AS '@companyGuid' 
            	, CONVERT(nvarchar(MAX),@guid) AS '@guid' 
				, CONVERT(nvarchar(MAX),@parentEntityGuid) AS '@parentEntityGuid' 
				, @name AS '@name' 
				, @type	AS '@type'
				, @description AS '@description' 
				, @address AS '@address'
				, @address2 AS '@address2'			
				, @city AS '@city'	
				, CONVERT(nvarchar(MAX),@stateGuid) AS '@stateGuid'	
				, CONVERT(nvarchar(MAX),@countryGuid) AS '@countryGuid'	
				, @zipCode AS '@zipCode'
				, @image AS '@image'
				, @latitude AS '@latitude'
				, @longitude AS '@longitude'
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

		IF NOT EXISTS(SELECT TOP 1 1 FROM [Entity] (NOLOCK) WHERE [guid] = @guid and [companyGuid] = @companyGuid and [isdeleted] = 0)
		BEGIN
			IF EXISTS (SELECT TOP 1 1 FROM [Entity] (NOLOCK) WHERE [companyguid] = @companyguid AND [isdeleted]=0 AND [name]=@name)
			BEGIN
				SET @output = -3
				SET @fieldname = 'EntityNameAlreadyExists'		 
				RETURN;
			END
		END		
		
	BEGIN TRAN	
		IF NOT EXISTS(SELECT TOP 1 1 FROM [Entity] where [guid] = @guid and companyGuid = @companyGuid AND isdeleted = 0 )
		BEGIN	
			INSERT INTO dbo.[Entity](
				[guid]	
				,[parentEntityGuid]	
				,[name]
				,[type]
				,[description]
				,[companyGuid]
				,[address]
				,[address2]
				,[city]
				,[stateGuid]
				,[countryGuid]
				,[zipCode]
				,[image]
				,[latitude]
				,[longitude]
				,[isActive]
				,[isDeleted]
				,[createddate]
				,[createdby]
				,[updatedDate]
				,[updatedBy]
				)
			VALUES(@guid	
				,@parentEntityGuid		
				,@name
				,@type
				,@description
				,@companyGuid
				,@address
				,@address2
				,@city
				,@stateGuid
				,@countryGuid
				,@zipCode
				,@image
				,@latitude
				,@longitude
				,1
				,0			
				,@dt
				,@invokingUser
				,@dt
				,@invokingUser
			)
		END
		ELSE
		BEGIN
			UPDATE dbo.[Entity]
			SET [parentEntityGuid]	= @parentEntityGuid
				,[name] 			= @name
				,[type]			= @type
				,[description]	= @description
				,[address]		= @address
				,[address2]		= @address2
				,[city]			= @city
				,[stateGuid]	= @stateGuid
				,[countryGuid]	= @countryGuid
				,[zipCode]		= @zipCode
				,[image]		= @image
				,[latitude]		= @latitude
				,[longitude]	= @longitude
				,[updatedDate]	= @dt
				,[updatedBy]	= @invokingUser			
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