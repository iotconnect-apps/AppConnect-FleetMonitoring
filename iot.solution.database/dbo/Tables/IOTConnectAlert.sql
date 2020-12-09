CREATE TABLE [dbo].[IOTConnectAlert]
(
	[guid] [uniqueidentifier] NOT NULL,
	[message] [nvarchar](500) NULL,
	[companyGuid] [uniqueidentifier] NULL,
	[condition] [nvarchar](2000) NULL,
	[deviceGuid] [uniqueidentifier] NULL,
	[entityGuid] [uniqueidentifier] NULL,
	[eventDate] [datetime] NULL,
	[uniqueId] [nvarchar](500) NULL,
	[audience] [nvarchar](2000) NULL,
	[eventId] [nvarchar](50) NULL,
	[refGuid] [uniqueidentifier] NULL,
	[severity] [nvarchar](200) NULL,
	[ruleName] [nvarchar](200) NULL,
	[data] [nvarchar](4000) NULL,
	PRIMARY KEY ([guid])
)
