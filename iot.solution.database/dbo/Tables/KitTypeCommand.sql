CREATE TABLE [dbo].[KitTypeCommand] (
    [guid]          UNIQUEIDENTIFIER NOT NULL,
    [templateGuid]  UNIQUEIDENTIFIER NOT NULL,
    [name]          NVARCHAR (100)   NOT NULL,
    [command]       NCHAR (100)      NULL,
    [requiredParam] BIT              CONSTRAINT [DF_KitTypeCommand_requiredParam] DEFAULT ((0)) NOT NULL,
    [requiredAck]   BIT              CONSTRAINT [DF_KitTypeCommand_requiredAck] DEFAULT ((0)) NOT NULL,
    [isOTACommand]  BIT              CONSTRAINT [DF_KitTypeCommand_isOTACommand] DEFAULT ((0)) NOT NULL,
    [tag]           NCHAR (100)      NULL
);

