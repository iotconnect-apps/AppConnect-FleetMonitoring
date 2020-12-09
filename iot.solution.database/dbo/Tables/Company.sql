CREATE TABLE [dbo].[Company] (
    [guid]          UNIQUEIDENTIFIER NOT NULL,
    [entityGuid]    UNIQUEIDENTIFIER NOT NULL,
    [parentGuid]    UNIQUEIDENTIFIER NULL,
    [adminUserGuid] UNIQUEIDENTIFIER NULL,
    [name]          NVARCHAR (100)   NOT NULL,
    [cpId]          NVARCHAR (200)   NOT NULL,
    [contactNo]     NVARCHAR (25)    NULL,
    [address]       NVARCHAR (500)   NULL,
    [countryGuid]   UNIQUEIDENTIFIER NULL,
    [timezoneGuid]  UNIQUEIDENTIFIER NULL,
    [image]         NVARCHAR (250)   NULL,
    [isActive]      BIT              DEFAULT ((1)) NOT NULL,
    [isDeleted]     BIT              DEFAULT ((0)) NOT NULL,
    [createdDate]   DATETIME         NULL,
    [createdBy]     UNIQUEIDENTIFIER NULL,
    [updatedDate]   DATETIME         NULL,
    [updatedBy]     UNIQUEIDENTIFIER NULL,
    [stateGuid]     UNIQUEIDENTIFIER NULL,
    [city]          NVARCHAR (50)    NULL,
    [postalCode]    NVARCHAR (30)    NULL,
    PRIMARY KEY CLUSTERED ([guid] ASC)
);

