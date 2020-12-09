CREATE TABLE [dbo].[KitTypeAttribute] (
    [guid]                        UNIQUEIDENTIFIER NOT NULL,
    [parentTemplateAttributeGuid] UNIQUEIDENTIFIER NULL,
    [templateGuid]                UNIQUEIDENTIFIER NOT NULL,
    [localName]                   NVARCHAR (100)   NOT NULL,
    [code]                        NVARCHAR (50)    NULL,
    [tag]                         NVARCHAR (50)    NULL,
    [description]                 NVARCHAR (100)   NULL,
    PRIMARY KEY CLUSTERED ([guid] ASC)
);

