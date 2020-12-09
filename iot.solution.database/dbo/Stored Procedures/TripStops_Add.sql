
/*******************************************************************
DECLARE @output INT = 0
	,@fieldName	nvarchar(255)

EXEC [dbo].[TripStops_Add]
	@tripGuid			= 'DCFC6AED-759B-425C-A969-61CC49631918'
	,@trips					= '<trips>
									<trip>
										<stopName>Shamgarh</stopName>
										<endDateTime>2020-08-21 15:10:04.620</endDateTime>
									</trip>
									<trip>
										<stopName>Ujjain</stopName>
										<endDateTime>2020-08-22 15:10:04.620</endDateTime>
									</trip>
									<trip>
										<stopName>Indore</stopName>
										<endDateTime>2020-08-22 15:10:04.620</endDateTime>
									</trip>
								</trips>'
	,@invokingUser			= '200EDCFA-8FF1-4837-91B1-7D5F967F5129'
	,@version				= 'v1'
	,@output				= @output		OUTPUT
	,@fieldName				= @fieldName	OUTPUT
	
SELECT @output status, @fieldName fieldname

001	SG-18	14-02-2020	[Sunil Bhawsar]	Added Initial Version to Add Shipment trips 

*******************************************************************/

Create PROCEDURE [dbo].[TripStops_Add]
(	@tripGuid		UNIQUEIDENTIFIER
	,@trips 			XML	
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
            SELECT 'TripStops_Add' AS '@procName'
			, CONVERT(nvarchar(MAX),@tripGuid) AS '@deviceGuid'
			, CONVERT(nvarchar(MAX),@trips) AS '@trips'
			, CONVERT(nvarchar(MAX),@invokingUser) AS '@invokingUser'
            , CONVERT(nvarchar(MAX),@version) AS '@version'
            , CONVERT(nvarchar(MAX),@output) AS '@output'
            , CONVERT(nvarchar(MAX),@fieldName) AS '@fieldName'
			FOR XML PATH('Params')
	    )
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), @dt)
    END

	BEGIN TRY
	
	IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.[Trip] (NOLOCK) WHERE [guid] = @tripGuid AND [isDeleted] = 0)
	BEGIN
		SET @output = -3
		SET @fieldName = 'TripNotExists'
	END

	IF OBJECT_ID ('tempdb..#trips') IS NOT NULL DROP TABLE #trips

	SELECT x.XmlCol.value('./stopName[1]','NVARCHAR(200)') [stopName]
			,x.XmlCol.value('./endDateTime[1]','Datetime') [endDateTime]
	INTO #trips
	FROM @trips.nodes('//trips/trip') x(XmlCol)

	--select * from #trips
	BEGIN TRAN
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
				)
	     SELECT
	           NEWID()
	           ,@tripGuid
	           ,[stopName]
	           ,[endDateTime]
	           ,0
	           ,@dt
	           ,@invokingUser				   
			   ,@dt
			   ,@invokingUser
		FROM #trips
	
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