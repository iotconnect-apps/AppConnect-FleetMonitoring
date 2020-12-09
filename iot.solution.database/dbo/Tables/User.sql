CREATE TABLE [dbo].[User] (
    [guid]         UNIQUEIDENTIFIER NOT NULL,
    [email]        NVARCHAR (100)   NOT NULL,
    [companyGuid]  UNIQUEIDENTIFIER NOT NULL,
    [entityGuid]   UNIQUEIDENTIFIER NOT NULL,
    [roleGuid]     UNIQUEIDENTIFIER NULL,
    [firstName]    NVARCHAR (50)    NOT NULL,
    [lastName]     NVARCHAR (50)    NOT NULL,
    [timeZoneGuid] UNIQUEIDENTIFIER NULL,
    [imageName]    NVARCHAR (100)   NULL,
    [contactNo]    NVARCHAR (25)    NULL,
    [isActive]     BIT              CONSTRAINT [DF__User__isactive__49C3F6B7] DEFAULT ((1)) NOT NULL,
    [isDeleted]    BIT              CONSTRAINT [DF__User__isdeleted__4AB81AF0] DEFAULT ((0)) NOT NULL,
    [createdDate]  DATETIME         NULL,
    [createdBy]    UNIQUEIDENTIFIER NULL,
    [updatedDate]  DATETIME         NULL,
    [updatedBy]    UNIQUEIDENTIFIER NULL,
    [subscriptionEndDate]  DATETIME         NULL,
    CONSTRAINT [PK__User__497F6CB4FD41A318] PRIMARY KEY CLUSTERED ([guid] ASC)
);

