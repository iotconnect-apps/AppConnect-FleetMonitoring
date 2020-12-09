CREATE TABLE [dbo].[FleetIdleStatus](
	[guid] [uniqueidentifier] NOT NULL,
	[companyGuid] [uniqueidentifier] NOT NULL,
	[fleetGuid] [uniqueidentifier] NOT NULL,
	[tripGuid] [uniqueidentifier] NOT NULL,
	[driverGuid] [uniqueidentifier] NOT NULL,
	[idleStartDateTime] [datetime] NULL,
	[idleEndDateTime] [datetime] NULL,	
	[lastUpdatedIdleDateTime] [datetime] NOT NULL 
 CONSTRAINT [PK__FleetIdleStatus__497F6CB475C7652E] PRIMARY KEY CLUSTERED 
(
	[guid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO