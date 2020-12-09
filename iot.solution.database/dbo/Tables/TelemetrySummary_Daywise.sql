CREATE TABLE [dbo].[TelemetrySummary_Daywise] (
    [guid]       UNIQUEIDENTIFIER NOT NULL,
    [deviceGuid] UNIQUEIDENTIFIER NOT NULL,
    [date]       DATE             NOT NULL,
    [attribute]  NVARCHAR (1000)  NULL,
    [min]        DECIMAL(18, 2)              NULL,
    [max]        DECIMAL(18, 2)              NULL,
    [avg]        DECIMAL(18, 2)              NULL,
    [latest]     DECIMAL(18, 2)              NULL,
    [sum]        DECIMAL(18, 2)           NULL,
    PRIMARY KEY CLUSTERED ([guid] ASC)
);

