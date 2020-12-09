CREATE TABLE [dbo].[AdminRule] (
    [guid]              UNIQUEIDENTIFIER NOT NULL,
    [templateGuid]      UNIQUEIDENTIFIER NOT NULL,
    [attributeGuid]     XML              NULL,
    [ruleType]          SMALLINT         DEFAULT ((1)) NOT NULL,
    [name]              NVARCHAR (100)   NULL,
    [description]       NVARCHAR (1000)  NULL,
    [conditionText]     NVARCHAR (1000)  NOT NULL,
    [conditionValue]    NVARCHAR (1000)  NOT NULL,
    [isActive]          BIT              DEFAULT ((1)) NOT NULL,
    [isDeleted]         BIT              DEFAULT ((0)) NOT NULL,
    [createdDate]       DATETIME         NULL,
    [createdBy]         UNIQUEIDENTIFIER NULL,
    [updatedDate]       DATETIME         NULL,
    [updatedBy]         UNIQUEIDENTIFIER NULL,
    [severityLevelGuid] UNIQUEIDENTIFIER NULL,
    [notificationType]  BIGINT           NULL,
    [commandText]       NVARCHAR (500)   NULL,
    [commandValue]      NVARCHAR (100)   NULL,
    PRIMARY KEY CLUSTERED ([guid] ASC)
);

