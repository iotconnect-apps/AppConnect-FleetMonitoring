SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Role](
	[guid] [uniqueidentifier] NOT NULL,
	[companyGuid] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](100) NOT NULL,
	[description] [nvarchar](500) NULL,
	[isAdminRole] [bit] NULL DEFAULT ((0)),
	[isactive] [bit] NOT NULL DEFAULT ((1)),
	[isdeleted] [bit] NOT NULL DEFAULT ((0)),
	[createdDate] [datetime] NULL,
	[createdBy] [uniqueidentifier] NULL,
	[updatedDate] [datetime] NULL,
	[updatedBy] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[guid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


