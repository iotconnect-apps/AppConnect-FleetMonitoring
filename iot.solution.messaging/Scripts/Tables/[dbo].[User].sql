SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[User](
	[guid] [uniqueidentifier] NOT NULL,
	[userid] [nvarchar](100) NOT NULL,
	[firstname] [nvarchar](50) NOT NULL,
	[lastname] [nvarchar](50) NOT NULL,
	[entityGuid] [uniqueidentifier] NULL,
	[companyGuid] [uniqueidentifier] NULL,
	[isActive] [bit] NOT NULL,
	[isDeleted] [bit] NOT NULL CONSTRAINT [DF_User_isDeleted]  DEFAULT ((0)),
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[guid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO