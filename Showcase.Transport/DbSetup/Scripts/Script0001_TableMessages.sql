CREATE TABLE [dbo].[Messages] (
    [Id] uniqueidentifier primary key not null,
    [CorrelationId] uniqueidentifier not null,
    [Direction] nvarchar(1) not null,
    [Content] nvarchar(max) not null,
    [CreateTime] datetimeoffset default sysdatetimeoffset() not null,
    [UpdateTime] datetimeoffset default sysdatetimeoffset() not null,
);

CREATE INDEX IX_Messages_CorrelationId
    ON Messages (CorrelationId)
    GO 

CREATE INDEX IX_Messages_CreateTime
    ON Messages (CreateTime)
    GO

CREATE TRIGGER T_Messages_UpdateTime_Automatic
    ON Messages
    AFTER UPDATE AS
        UPDATE [Messages]
        SET [UpdateTime] = sysdatetimeoffset()
        WHERE Id in (SELECT DISTINCT Id FROM Inserted)
    GO
