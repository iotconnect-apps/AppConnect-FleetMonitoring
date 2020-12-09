/*******************************************************************               
begin tran

DECLARE 
	@ComapnyXml XML =N'<items><item><guid>60c3e321-fa42-44e2-9bec-64edd03ed5a5</guid><cpId>2B363A73DECE449D83F442BBBD65AA1C</cpId><entityGuid>5458c79a-bd1f-43df-907c-4e8cf4ecd270</entityGuid><name>AQ1stApr01</name><isDeleted>0</isDeleted><isActive>1</isActive><adminUserGuid>4620f82f-20d9-4f83-9bb7-6cfa0efe5738</adminUserGuid><countryGuid>4bd21ec1-7fe3-4d26-8e41-f142933c4e54</countryGuid><stateGuid>dd3df070-597a-4248-b5d5-8e6a47e4be9b</stateGuid><contactNo>45-6456464564</contactNo><address>address</address><timezoneGuid>8ecba46e-5951-475a-beb4-c242a575ef19</timezoneGuid></item></items>'

EXEC [dbo].[IotConnect_ManageCompany]
	  @companyGuid	= 'C51D63B4-5B6F-461A-B7FC-F55EF734F779'
     ,@ComapnyXml	= @ComapnyXml
	 ,@action		= 'update'

rollback

001	sgh-1	21-11-2019 [Nishit Khakhi]	Manage multiple Company
002 SGH-145	25-03-2020	[Nishit Khakhi] Updated to insert/update company details
003 SAQM-51	06-04-2020	[Nishit Khakhi] Updated ContactNo to remove '+' sign
*******************************************************************/ 
CREATE PROCEDURE [dbo].[IotConnect_ManageCompany]
(              
	 @companyGuid		UNIQUEIDENTIFIER 	
	,@action			VARCHAR(20)		
	,@enableDebugInfo	CHAR(1)	= '0'
	,@ComapnyXml		XML			
)AS              
BEGIN
    SET NOCOUNT ON

    IF (@enableDebugInfo = 1)
	BEGIN
        DECLARE @Param XML 
        SELECT @Param = 
        (
            SELECT 'Company_ManageRef' AS '@procName'
			, CONVERT(nvarchar(MAX), @action) AS '@action'
			, CONVERT(nvarchar(MAX), @companyGuid) AS '@companyGuid'
			, CONVERT(nvarchar(MAX), @ComapnyXml) AS '@CompanyXml'
			FOR XML PATH('Params')
	    ) 
	    INSERT INTO DebugInfo(data, dt) VALUES(Convert(nvarchar(MAX), @Param), GETUTCDATE())
    END         
	BEGIN TRY
		
		IF OBJECT_ID('tempdb..#tempCompany') IS NOT NULL DROP TABLE #tempCompany
		
		SELECT 
			b.value('guid[1]', 'UNIQUEIDENTIFIER') [guid]
			,b.exist('guid[1]') hasGuid

			,b.value('name[1]', 'NVARCHAR(100)') [name]
			,b.exist('name[1]') hasName

			,b.value('cpId[1]', 'NVARCHAR(500)') cpId
			,b.exist('cpId[1]') hasCpId

			,b.value('entityGuid[1]', 'UNIQUEIDENTIFIER') entityGuid
			,b.exist('entityGuid[1]') hasEntityGuid			

			,b.value('isActive[1]', 'BIT') isActive
			,b.value('isDeleted[1]', 'BIT') isDeleted
			
			,b.value('adminUserGuid[1]', 'UNIQUEIDENTIFIER') adminUserGuid
			,b.exist('adminUserGuid[1]') hasAdminUserGuid
			
			,b.value('countryGuid[1]', 'UNIQUEIDENTIFIER') countryGuid
			,b.exist('countryGuid[1]') hasCountryGuid	

			,b.value('timezoneGuid[1]', 'UNIQUEIDENTIFIER') timezoneGuid
			,b.exist('timezoneGuid[1]') hasTimezoneGuid	

			,b.value('stateGuid[1]', 'UNIQUEIDENTIFIER') stateGuid
			,b.exist('stateGuid[1]') hasStateGuid	

			,b.value('contactNo[1]', 'NVARCHAR(25)') contactNo
			,b.exist('contactNo[1]') hasContactNo

			,b.value('address[1]', 'NVARCHAR(500)') address
			,b.exist('address[1]') hasAddress
		INTO #tempCompany
		FROM @ComapnyXml.nodes('/items/item') a(b)

		BEGIN TRAN
		IF NOT EXISTS(SELECT 1 from dbo.[Company] d (NOLOCK) INNER JOIN #tempCompany te ON te.[guid] = d.[guid] WHERE te.hasGuid = 1) AND @action='update'
				BEGIN
					 SET @action='insert'
				END 
			IF(@action = 'insert') 
			BEGIN
				;WITH ExistingCompany AS
				(
					SELECT [guid] FROM dbo.[Company] d (NOLOCK)
				)
			
				INSERT INTO dbo.[Company]([guid], [cpId], [name], [entityGuid], [isActive], [isDeleted],[createdDate],[createdBy],[contactNo]
										,[address],[countryGuid],[timezoneGuid],[stateGuid],[adminUserGuid])
				SELECT DISTINCT [guid], cpId, [name], entityGuid, isActive, isDeleted, GETUTCDATE(),'00000000-0000-0000-0000-000000000000'
							,REPLACE([contactNo],'+',''),[address],[countryGuid],[timezoneGuid],[stateGuid],[adminUserGuid]
				FROM #tempCompany tc WHERE NOT EXISTS ( SELECT 1 FROM ExistingCompany eu WHERE tc.[guid] = eu.[guid] )		
			END
			
			IF(@action = 'update') 
			BEGIN
				UPDATE d
				SET  [cpId]	= CASE WHEN tc.hascpId = 1 THEN tc.cpId ELSE d.[cpId] END
					,[name]	= CASE WHEN tc.hasname = 1 THEN tc.[name] ELSE d.[name] END
					,[entityGuid]	= CASE WHEN tc.hasEntityguid = 1 THEN tc.entityguid ELSE d.[entityGuid] END
					,[contactNo]	= CASE WHEN tc.hascontactNo = 1 THEN REPLACE(tc.[contactNo],'+','') ELSE d.[contactNo] END
					,[address]	= CASE WHEN tc.hasaddress = 1 THEN tc.[address] ELSE d.[address] END
					,[countryGuid]	= CASE WHEN tc.hasCountryGuid = 1 THEN tc.[countryGuid] ELSE d.[countryGuid] END
					,[timezoneGuid]	= CASE WHEN tc.hasTimezoneGuid = 1 THEN tc.[timezoneGuid] ELSE d.[timezoneGuid] END
					,[stateGuid]	= CASE WHEN tc.hasStateGuid = 1 THEN tc.[stateGuid] ELSE d.[stateGuid] END
					,[adminUserGuid]	= CASE WHEN tc.hasAdminUserGuid = 1 THEN tc.[adminUserGuid] ELSE d.[adminUserGuid] END
				FROM dbo.[Company] d (NOLOCK)    
				INNER JOIN #tempCompany tc
					ON tc.[guid] = d.[guid]    
				WHERE tc.hasGuid = 1
			END  
			
			IF(@action = 'delete') 
			BEGIN
				UPDATE c
					SET [isDeleted] = tc.isDeleted , [isActive] = 0
				FROM dbo.[Company] AS c (NOLOCK)
				INNER JOIN #tempCompany tc
					ON tc.[guid] = c.[guid]
				WHERE tc.hasGuid = 1

				UPDATE u
					SET [isDeleted] = tc.isDeleted , [isActive] = 0
				FROM dbo.[User] AS u (NOLOCK)
				INNER JOIN #tempCompany tc
					ON tc.[guid] = u.[companyGuid]
				WHERE tc.hasGuid = 1
			END
			
			IF(@action = 'status')
			BEGIN
				UPDATE c
					SET [isActive] = tc.isActive
				FROM dbo.[Company] AS c (NOLOCK)
				INNER JOIN #tempCompany tc
					ON tc.[guid] = c.[guid]
				WHERE tc.hasGuid = 1
			END

		COMMIT
	END TRY                
	BEGIN CATCH              
	               
		DECLARE @errorReturnMessage nvarchar(MAX)        
	        
		SELECT @errorReturnMessage =        
			ISNULL(@errorReturnMessage, '') + SPACE(1) +        
			'ErrorNumber:' + ISNULL(CAST(ERROR_NUMBER() AS nvarchar), '') +        
			'ErrorSeverity:' + ISNULL(CAST(ERROR_SEVERITY() AS nvarchar), '') +        
			'ErrorState:' + ISNULL(CAST(ERROR_STATE() AS nvarchar), '') +        
			'ErrorLine:' + ISNULL(CAST(ERROR_LINE() AS nvarchar), '') +        
			'ErrorProcedure:' + ISNULL(CAST(ERROR_PROCEDURE() AS nvarchar), '') +        
			'ErrorMessage:' + ISNULL(CAST(ERROR_MESSAGE() AS nvarchar(max)), '')   
			
		RAISERROR (@errorReturnMessage, 11, 1)

		IF (XACT_STATE()) = -1 BEGIN
			ROLLBACK TRANSACTION
		END
		IF (XACT_STATE()) = 1 BEGIN
			ROLLBACK TRANSACTION
		END
	END CATCH        
END