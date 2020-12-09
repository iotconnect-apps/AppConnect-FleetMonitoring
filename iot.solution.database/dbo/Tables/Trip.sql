CREATE TABLE [dbo].[Trip](
	[guid] [uniqueidentifier] NOT NULL,
	[companyGuid] [uniqueidentifier] NOT NULL,
	[fleetGuid] [uniqueidentifier] NOT NULL,
	[sourceLocation] [nvarchar](250) NOT NULL,
	[destinationLocation] [nvarchar](250) NULL,
	[materialTypeGuid] [uniqueidentifier] NULL,
	[weight] [nvarchar](100) NOT NULL,
	[startDateTime] [datetime] NULL,
	[isActive] [bit] NOT NULL,
	[isDeleted] [bit] NOT NULL,
	[createdDate] [datetime] NULL,
	[createdBy] [uniqueidentifier] NULL,
	[updatedDate] [datetime] NULL,
	[updatedBy] [uniqueidentifier] NULL,
	[sourceLatitude] [nvarchar](50) NULL,
	[sourceLongitude] [nvarchar](50) NULL,
	[destinationLatitude] [nvarchar](50) NULL,
	[destinationLongitude] [nvarchar](50) NULL,
	[totalMiles] [int] NULL,
	[tripId] [nvarchar](100) NULL,
	[isCompleted] [bit] NOT NULL,
	[completedDate] [datetime] NULL,
	[isStarted] [bit] NOT NULL,
	[actualStartDateTime] [datetime] NULL,
	[etaEndDateTime] [datetime] NULL,
	[aggressiveAcceleration] [int] NOT NULL,
	[harshBraking] [int] NOT NULL,
	[overSpeed] [int] NOT NULL,
	[idleTime] [int] NOT NULL,
	[coveredMiles] [int] NULL,
	[odometer] [bigint] NULL
 CONSTRAINT [PK__Trip__497F6CB475C7652E] PRIMARY KEY CLUSTERED 
(
	[guid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Trip] ADD  DEFAULT ((1)) FOR [isActive]
GO

ALTER TABLE [dbo].[Trip] ADD  DEFAULT ((0)) FOR [isDeleted]
GO

ALTER TABLE [dbo].[Trip] ADD  DEFAULT ((0)) FOR [totalMiles]
GO

ALTER TABLE [dbo].[Trip] ADD  CONSTRAINT [trip_status]  DEFAULT ((0)) FOR [isCompleted]
GO
ALTER TABLE [dbo].[Trip] ADD  DEFAULT ((0)) FOR [isStarted]
GO
ALTER TABLE [dbo].[Trip] ADD  DEFAULT ((0)) FOR [odometer]
GO

ALTER TABLE [dbo].[Trip] ADD  DEFAULT ((0)) FOR [aggressiveAcceleration]
GO

ALTER TABLE [dbo].[Trip] ADD  DEFAULT ((0)) FOR [harshBraking]
GO

ALTER TABLE [dbo].[Trip] ADD  DEFAULT ((0)) FOR [overSpeed]
GO

ALTER TABLE [dbo].[Trip] ADD  DEFAULT ((0)) FOR [idleTime]
GO