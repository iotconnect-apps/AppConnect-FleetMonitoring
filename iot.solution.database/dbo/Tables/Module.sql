CREATE TABLE [dbo].[Module] (
    [guid]           UNIQUEIDENTIFIER NOT NULL,
    [name]           NVARCHAR (50)    NULL,
    [permission]     INT              NULL,
    [applyTo]        INT              NULL,
    [categoryName]   NVARCHAR (200)   NULL,
    [moduleSequence] FLOAT (53)       NULL,
    [isAdminModule]  BIT              DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([guid] ASC)
);

