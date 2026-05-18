BEGIN TRANSACTION;
GO

ALTER TABLE [bookings] DROP CONSTRAINT [FK_bookings_users_UserId];
GO

ALTER TABLE [refunds] DROP CONSTRAINT [FK_refunds_bookings_BookingId];
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bookings]') AND [c].[name] = N'PaymentStatus');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [bookings] DROP CONSTRAINT [' + @var0 + '];');
IF COL_LENGTH('bookings', 'PaymentStatus') IS NOT NULL
    ALTER TABLE [bookings] DROP COLUMN [PaymentStatus];
GO

IF COL_LENGTH('refunds', 'AdminNote') IS NULL
    ALTER TABLE [refunds] ADD [AdminNote] nvarchar(max) NULL;
GO

IF COL_LENGTH('refunds', 'InitiatedBy') IS NULL
    ALTER TABLE [refunds] ADD [InitiatedBy] int NOT NULL DEFAULT 0;
GO

IF COL_LENGTH('refunds', 'PaymentId') IS NULL
    ALTER TABLE [refunds] ADD [PaymentId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
GO

IF COL_LENGTH('refunds', 'RefundedAt') IS NULL
    ALTER TABLE [refunds] ADD [RefundedAt] datetime2 NULL;
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bookings]') AND [c].[name] = N'TotalAmount');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [bookings] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [bookings] ALTER COLUMN [TotalAmount] decimal(18,2) NOT NULL;
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[booking_itineraries]') AND [c].[name] = N'Price');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [booking_itineraries] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [booking_itineraries] ALTER COLUMN [Price] decimal(18,2) NOT NULL;
GO

IF COL_LENGTH('booking_itineraries', 'BookingId1') IS NULL
    ALTER TABLE [booking_itineraries] ADD [BookingId1] uniqueidentifier NULL;
GO

IF COL_LENGTH('booking_itineraries', 'FinalPrice') IS NULL
    ALTER TABLE [booking_itineraries] ADD [FinalPrice] decimal(18,2) NOT NULL DEFAULT 0.0;
GO

IF OBJECT_ID(N'[AuditLogs]', 'U') IS NULL
BEGIN
    CREATE TABLE [AuditLogs] (
        [Id] uniqueidentifier NOT NULL,
        [ActorUserId] uniqueidentifier NOT NULL,
        [Action] nvarchar(max) NOT NULL,
        [TargetUserId] uniqueidentifier NULL,
        [EntityType] nvarchar(100) NULL,
        [EntityId] uniqueidentifier NULL,
        [Description] nvarchar(500) NULL,
        [IpAddress] nvarchar(45) NULL,
        [OldValue] nvarchar(max) NULL,
        [NewValue] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AuditLogs_users_ActorUserId] FOREIGN KEY ([ActorUserId]) REFERENCES [users] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AuditLogs_users_TargetUserId] FOREIGN KEY ([TargetUserId]) REFERENCES [users] ([Id]) ON DELETE SET NULL
    );
END
GO

IF OBJECT_ID(N'[Payments]', 'U') IS NULL
BEGIN
    CREATE TABLE [Payments] (
        [Id] uniqueidentifier NOT NULL,
        [BookingId] uniqueidentifier NOT NULL,
        [OrderCode] bigint NOT NULL,
        [Amount] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NOT NULL,
        [FinalAmount] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        [PayOSPaymentLinkId] nvarchar(max) NULL,
        [TransactionReference] nvarchar(max) NULL,
        [PaidAt] datetime2 NULL,
        [ExpiredAt] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Payments_bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [bookings] ([Id]) ON DELETE NO ACTION
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_refunds_PaymentId' AND object_id = OBJECT_ID('refunds'))
    CREATE UNIQUE INDEX [IX_refunds_PaymentId] ON [refunds] ([PaymentId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_booking_itineraries_BookingId1' AND object_id = OBJECT_ID('booking_itineraries'))
    CREATE INDEX [IX_booking_itineraries_BookingId1] ON [booking_itineraries] ([BookingId1]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AuditLogs_ActorUserId' AND object_id = OBJECT_ID('AuditLogs'))
    CREATE INDEX [IX_AuditLogs_ActorUserId] ON [AuditLogs] ([ActorUserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AuditLogs_TargetUserId' AND object_id = OBJECT_ID('AuditLogs'))
    CREATE INDEX [IX_AuditLogs_TargetUserId] ON [AuditLogs] ([TargetUserId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Payments_BookingId' AND object_id = OBJECT_ID('Payments'))
    CREATE INDEX [IX_Payments_BookingId] ON [Payments] ([BookingId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Payments_OrderCode' AND object_id = OBJECT_ID('Payments'))
    CREATE UNIQUE INDEX [IX_Payments_OrderCode] ON [Payments] ([OrderCode]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_booking_itineraries_bookings_BookingId1')
    ALTER TABLE [booking_itineraries] ADD CONSTRAINT [FK_booking_itineraries_bookings_BookingId1] FOREIGN KEY ([BookingId1]) REFERENCES [bookings] ([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_bookings_users_UserId')
    ALTER TABLE [bookings] ADD CONSTRAINT [FK_bookings_users_UserId] FOREIGN KEY ([UserId]) REFERENCES [users] ([Id]) ON DELETE NO ACTION;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_refunds_Payments_PaymentId')
    ALTER TABLE [refunds] ADD CONSTRAINT [FK_refunds_Payments_PaymentId] FOREIGN KEY ([PaymentId]) REFERENCES [Payments] ([Id]) ON DELETE NO ACTION;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_refunds_bookings_BookingId')
    ALTER TABLE [refunds] ADD CONSTRAINT [FK_refunds_bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [bookings] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260417102534_20260417', N'8.0.23');
GO

COMMIT;
GO
