CREATE TABLE [dbo].[Notifications] (
    [Id] uniqueidentifier primary key not null,
    [CorrelationId] uniqueidentifier not null,
    [NotificationType] nvarchar(50) not null,
    [Content] nvarchar(MAX) not null,
    [CreateTime] datetimeoffset default sysdatetimeoffset() not null
)

CREATE INDEX IX_Notifications_CorrelationId
    ON Notifications (CorrelationId)
    GO

CREATE INDEX IX_Notifications_CreateTime
    ON Notifications (CreateTime)
    GO