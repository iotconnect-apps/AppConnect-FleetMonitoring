SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DeviceTemplate](
	[guid] [uniqueidentifier] NOT NULL,
	[code] [nvarchar](10) NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[firmwareGuid] [uniqueidentifier] NULL,
	[companyGuid] [uniqueidentifier] NOT NULL,
	[isDeleted] [bit] NOT NULL CONSTRAINT [DF_DeviceTemplate_isDeleted]  DEFAULT ((0)),
	[isEdgeSupport] [bit] NOT NULL DEFAULT ((0)),
	[isType2Support] [bit] NOT NULL DEFAULT ((0)),
	[tag] [nvarchar](50) NULL,
 CONSTRAINT [PK_DeviceTemplate] PRIMARY KEY CLUSTERED 
(
	[guid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO