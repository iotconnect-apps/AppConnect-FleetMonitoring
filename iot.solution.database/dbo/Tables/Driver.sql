CREATE TABLE [dbo].[Driver](
	[guid] [uniqueidentifier] NOT NULL,
	[companyGuid] [uniqueidentifier] NOT NULL,
	[fleetGuid] [uniqueidentifier] NOT NULL,
	[firstName] [nvarchar](50) NOT NULL,
	[lastName] [nvarchar](50) NOT NULL,
	[email] [nvarchar](100) NOT NULL,
	[contactNo] [nvarchar](25) NULL,
	[city] [nvarchar](50) NULL,
	[zipcode] [nvarchar](10) NULL,
	[stateGuid] [uniqueidentifier] NULL,
	[countryGuid] [uniqueidentifier] NULL,
	[image] [nvarchar](250) NULL,
	[address] [nvarchar](500) NULL,
	[licenceNo] [nvarchar](25) NOT NULL,
	[licenceImage] [nvarchar](250) NULL,
	[isActive] [bit] NOT NULL,
	[isDeleted] [bit] NOT NULL,
	[createdDate] [datetime] NULL,
	[createdBy] [uniqueidentifier] NULL,
	[updatedDate] [datetime] NULL,
	[updatedBy] [uniqueidentifier] NULL,
	[driverId] [nvarchar](150) NULL,
	[aggressiveAcceleration] [int] NOT NULL,
	[overSpeed] [int] NOT NULL,
	[harshBraking] [int] NOT NULL,
	[idleTime] [int] NOT NULL,
	[haltTime] [int] NOT NULL
 CONSTRAINT [PK__Driver__497F6CB4FD41A318] PRIMARY KEY CLUSTERED 
(
	[guid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Driver] ADD  CONSTRAINT [DF__Driver__isactive__49C3F6B7]  DEFAULT ((1)) FOR [isActive]
GO

ALTER TABLE [dbo].[Driver] ADD  CONSTRAINT [DF__Driver__isdeleted__4AB81AF0]  DEFAULT ((0)) FOR [isDeleted]
GO

ALTER TABLE [dbo].[Driver] ADD  DEFAULT ((0)) FOR [aggressiveAcceleration]
GO

ALTER TABLE [dbo].[Driver] ADD  DEFAULT ((0)) FOR [overSpeed]
GO

ALTER TABLE [dbo].[Driver] ADD  DEFAULT ((0)) FOR [harshBraking]
GO

ALTER TABLE [dbo].[Driver] ADD  DEFAULT ((0)) FOR [idleTime]
GO

ALTER TABLE [dbo].[Driver] ADD  DEFAULT ((0)) FOR [haltTime]
GO