SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DebugInfo](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[data] [nvarchar](max) NOT NULL,
	[dt] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[DebugInfo] ADD  DEFAULT (getutcdate()) FOR [dt]
GO