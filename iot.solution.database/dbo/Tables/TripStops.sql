CREATE TABLE [dbo].[TripStops](
	[guid] [uniqueidentifier] NOT NULL,
	[tripGuid] [uniqueidentifier] NOT NULL,
	[stopName] [nvarchar](200) NULL,
	[endDateTime] [datetime] NULL,
	[isDeleted] [bit] NOT NULL,
	[createdDate] [datetime] NOT NULL,
	[createdBy] [uniqueidentifier] NOT NULL,
	[updatedDate] [datetime] NULL,
	[updatedBy] [uniqueidentifier] NULL,
	[Latitude] [nvarchar](50) NULL,
	[Longitude] [nvarchar](50) NULL,
 CONSTRAINT [PK__TripStops__GUID] PRIMARY KEY CLUSTERED 
(
	[guid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO