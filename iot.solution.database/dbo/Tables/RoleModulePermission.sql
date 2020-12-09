CREATE TABLE [dbo].[RoleModulePermission] (
    [guid]        UNIQUEIDENTIFIER NOT NULL,
    [roleGuid]    UNIQUEIDENTIFIER NOT NULL,
    [companyGuid] UNIQUEIDENTIFIER NOT NULL,
    [moduleGuid]  UNIQUEIDENTIFIER NOT NULL,
    [permission]  INT              NULL,
    [createdDate] DATETIME         NULL,
    [createdBy]   UNIQUEIDENTIFIER NULL,
    PRIMARY KEY CLUSTERED ([guid] ASC)
);

