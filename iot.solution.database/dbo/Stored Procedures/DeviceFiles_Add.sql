
/*******************************************************************
DECLARE @output INT = 0
	,@fieldName	nvarchar(255)

EXEC [dbo].[DeviceFiles_Add]
	@deviceGuid			= '895019CF-1D3E-420C-828F-8971253E5784'
	,@files					= '<files>
									<file>
										<path>path1</path>
										<desc>desc1</desc>
										<type>M</type>
									</file>
									<file>
										<path>path2</path>
										<desc>desc2</desc>
										<type>D</type>
									</file>
									<file>
										<path>path3</path>
										<desc>desc3</desc>
										<type>I</type>
									</file>
								</files>'
	,@invokingUser			= '200EDCFA-8FF1-4837-91B1-7D5F967F5129'
	,@version				= 'v1'
	,@output				= @output		OUTPUT
	,@fieldName				= @fieldName	OUTPUT
	
SELECT @output status, @fieldName fieldname

001	SG-18	14-02-2020	[Nishit Khakhi]	Added Initial Version to Add Device File
002	SAM-138	06/07/2020	[Nishit Khakhi]	Added type field
*******************************************************************/

CREATE PROCEDURE [dbo].[DeviceFiles_Add]
(	@deviceGuid		UNIQUEIDENTIFIER
	,@files 			XML	
	,@invokingUser		UNIQUEIDENTIFIER
	,@version			NVARCHAR(10)
	,@output			SMALLINT			OUTPUT
	,@fieldName			NVARCHAR(100)		OUTPUT
	,@culture			NVARCHAR(10)		= 'en-Us'
	,@enableDebugInfo	 CHAR(1)			= '0'
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @dt DATETIME = GETUTCDATE()
    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML
        SELECT @Param =
        (
            SELECT 'DeviceFiles_Add' AS '@procName'
			, CONVERT(nvarchar(MAX),@deviceGuid) AS '@deviceGuid'
			, CONVERT(nvarchar(MAX),@files) AS '@files'
			, CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
            , CONVERT(nvarchar(MAX),@version) AS '@version'
            , CONVERT(nvarchar(MAX),@output) AS '@output'
            , CONVERT(nvarchar(MAX),@fieldName) AS '@fieldName'
			FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), @dt)
    END

	BEGIN TRY
	
	IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.[Device] (NOLOCK) WHERE [guid] = @deviceGuid AND [isDeleted] = 0)
	BEGIN
		SET @output = -3
		SET @fieldName = 'DeviceNotExists'
	END

	IF OBJECT_ID ('tempdb..#files') IS NOT NULL DROP TABLE #files

	SELECT x.XmlCol.value('./path[1]','NVARCHAR(500)') [path]
			,x.XmlCol.value('./desc[1]','NVARCHAR(200)') [desc]
			,x.XmlCol.value('./type[1]','NVARCHAR(5)') [type]
	INTO #files
	FROM @files.nodes('//files/file') x(XmlCol)

	--select * from #files
	BEGIN TRAN
		INSERT INTO [dbo].[DeviceFiles]
	           ([guid]
	           ,[deviceGuid]
	           ,[filePath]
			   ,[description]
			   ,[isDeleted]
	           ,[createdDate]
	           ,[createdBy]
	           ,[updatedDate]
	           ,[updatedBy]
			   ,[type]
				)
	     SELECT
	           NEWID()
	           ,@deviceGuid
	           ,[path]
	           ,[desc]
	           ,0
	           ,@dt
	           ,@invokingUser				   
			   ,@dt
			   ,@invokingUser
			   ,[type]
		FROM #files
	
	COMMIT TRAN
		
	SET @output = 1
	SET @fieldName = 'Success'
	
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