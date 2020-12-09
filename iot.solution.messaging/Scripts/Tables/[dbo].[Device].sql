SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TABLE [dbo].[Device](
	[guid] [uniqueidentifier] NOT NULL,
	[companyGuid] [uniqueidentifier] NOT NULL,
	[entityGuid] [uniqueidentifier] NOT NULL,
	[type] [tinyint] NULL,
	[uniqueId] [nvarchar](500) NOT NULL,
	[name] [nvarchar](500) NOT NULL,
	[note] [nvarchar](1000) NOT NULL,
	[tag] [nvarchar](50) NULL,
	[image] [nvarchar](200) NULL,
	[isactive] [bit] NOT NULL,
	[isdeleted] [bit] NOT NULL,
	[createdDate] [datetime] NULL,
	[createdBy] [uniqueidentifier] NULL,
	[updatedDate] [datetime] NULL,
	[updatedBy] [uniqueidentifier] NULL,
	[deviceTemplateGuid] [uniqueidentifier] NULL,
	[parentDeviceGuid] [uniqueidentifier] NULL,
	[isConnected] [bit] NULL,
	[isAcquired] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[guid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Device] ADD  DEFAULT ((1)) FOR [isactive]
GO

ALTER TABLE [dbo].[Device] ADD  DEFAULT ((0)) FOR [isdeleted]
GO
