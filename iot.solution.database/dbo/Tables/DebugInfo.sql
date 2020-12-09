CREATE TABLE [dbo].[DebugInfo] (
    [id]   INT            IDENTITY (1, 1) NOT NULL,
    [data] NVARCHAR (MAX) NOT NULL,
    [dt]   DATETIME       DEFAULT (getutcdate()) NULL
);

