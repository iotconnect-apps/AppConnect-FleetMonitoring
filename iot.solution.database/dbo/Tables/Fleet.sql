CREATE TABLE [dbo].[Fleet](
	[guid] [uniqueidentifier] NOT NULL,
	[companyGuid] [uniqueidentifier] NOT NULL,
	[fleetId] [nvarchar](100) NOT NULL,
	[registrationNo] [nvarchar](100) NULL,
	[loadingCapacity] [nvarchar](100) NULL,
	[typeGuid] [uniqueidentifier] NULL,
	[materialTypeGuid] [uniqueidentifier] NULL,
	[image] [nvarchar](250) NULL,
	[speedLimit] [nvarchar](250) NULL,
	[latitude] nvarchar(50) null,
	[longitude] nvarchar(50) null,
	[radius] int null default(0),
	[totalMiles] int null default(0),
	[isActive] [bit] NOT NULL,
	[isDeleted] [bit] NOT NULL,
	[createdDate] [datetime] NULL,
	[createdBy] [uniqueidentifier] NULL,
	[updatedDate] [datetime] NULL,
	[updatedBy] [uniqueidentifier] NULL,
 CONSTRAINT [PK__Fleet__497F6CB475C7652E] PRIMARY KEY CLUSTERED 
(
	[guid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Fleet] ADD  DEFAULT ((1)) FOR [isActive]
GO

ALTER TABLE [dbo].[Fleet] ADD  DEFAULT ((0)) FOR [isDeleted]
GO


