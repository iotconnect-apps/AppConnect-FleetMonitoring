CREATE TABLE [dbo].[HardwareKit] (
    [guid]          UNIQUEIDENTIFIER NOT NULL,
    [kitTypeGuid]   UNIQUEIDENTIFIER NOT NULL,
    [kitCode]       NVARCHAR (50)    NOT NULL,
    [companyGuid]   UNIQUEIDENTIFIER NULL,
    [entityGuid]    UNIQUEIDENTIFIER NULL,
    [uniqueId]      NVARCHAR (500)   NOT NULL,
    [name]          NVARCHAR (500)   NOT NULL,
    [note]          NVARCHAR (1000)  NOT NULL,
    [tagGuid]       UNIQUEIDENTIFIER NULL,
    [isProvisioned] BIT              CONSTRAINT [DF_HardwareKit_isProvisioned] DEFAULT ((1)) NOT NULL,
    [isActive]      BIT              CONSTRAINT [DF__HardwareK__isAct__60A75C0F] DEFAULT ((1)) NOT NULL,
    [isDeleted]     BIT              CONSTRAINT [DF__HardwareK__isDel__619B8048] DEFAULT ((0)) NOT NULL,
    [createdDate]   DATETIME         NULL,
    [createdBy]     UNIQUEIDENTIFIER NULL,
    [updatedDate]   DATETIME         NULL,
    [updatedBy]     UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__Hardware__497F6CB4048EB41D] PRIMARY KEY CLUSTERED ([guid] ASC)
);

