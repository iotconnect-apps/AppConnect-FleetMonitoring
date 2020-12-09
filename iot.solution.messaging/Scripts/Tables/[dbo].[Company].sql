SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Company](
	[guid] [uniqueidentifier] NOT NULL,
	[entityGuid] [uniqueidentifier] NOT NULL,
	[parentguid] [uniqueidentifier] NULL,
	[adminUserGuid] [uniqueidentifier] NULL,
	[name] [nvarchar](100) NOT NULL,
	[cpId] [nvarchar](200) NOT NULL,
	[contactNo] [nvarchar](25) NULL,
	[address] [nvarchar](500) NULL,
	[countryGuid] [uniqueidentifier] NULL,
	[timezoneGuid] [uniqueidentifier] NULL,
	[image] [nvarchar](250) NULL,
	[isactive] [bit] NOT NULL DEFAULT ((1)),
	[isdeleted] [bit] NOT NULL DEFAULT ((0)),
	[createddate] [datetime] NULL,
	[createdby] [uniqueidentifier] NULL,
	[updateddate] [datetime] NULL,
	[updatedby] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[guid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO