SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Entity](
	[guid] [uniqueidentifier] NOT NULL,
	[companyGuid] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](500) NOT NULL,
	[description] [nvarchar](1000) NULL,
	[address] [nvarchar](500) NOT NULL,
	[address2] [nvarchar](500) NULL,
	[city] [nvarchar](50) NULL,
	[zipcode] [nvarchar](10) NULL,
	[stateGuid] [uniqueidentifier] NULL,
	[countryGuid] [uniqueidentifier] NULL,
	[image] [nvarchar](250) NULL,
	[isactive] [bit] NOT NULL DEFAULT ((1)),
	[isdeleted] [bit] NOT NULL DEFAULT ((0)),
	[createdDate] [datetime] NULL,
	[createdBy] [uniqueidentifier] NULL,
	[updatedDate] [datetime] NULL,
	[updatedBy] [uniqueidentifier] NULL,
	[parentEntityGuid] [uniqueidentifier] NULL,
 CONSTRAINT [PK__Entity__497F6CB475C7652E] PRIMARY KEY CLUSTERED 
(
	[guid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


