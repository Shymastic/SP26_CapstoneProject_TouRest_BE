BEGIN TRANSACTION;
GO

-- wallets.PendingBalance → bigint
DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[wallets]') AND [c].[name] = N'PendingBalance');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [wallets] DROP CONSTRAINT [' + @var0 + '];');
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('wallets') AND name = 'PendingBalance' AND system_type_id = TYPE_ID('decimal'))
    ALTER TABLE [wallets] ALTER COLUMN [PendingBalance] bigint NOT NULL;
GO

-- wallets.Balance → bigint
DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[wallets]') AND [c].[name] = N'Balance');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [wallets] DROP CONSTRAINT [' + @var1 + '];');
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('wallets') AND name = 'Balance' AND system_type_id = TYPE_ID('decimal'))
    ALTER TABLE [wallets] ALTER COLUMN [Balance] bigint NOT NULL;
GO

-- wallet_transactions.Amount → bigint
DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[wallet_transactions]') AND [c].[name] = N'Amount');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [wallet_transactions] DROP CONSTRAINT [' + @var2 + '];');
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('wallet_transactions') AND name = 'Amount' AND system_type_id = TYPE_ID('decimal'))
    ALTER TABLE [wallet_transactions] ALTER COLUMN [Amount] bigint NOT NULL;
GO

-- refunds.TotalRefundAmount → bigint
DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[refunds]') AND [c].[name] = N'TotalRefundAmount');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [refunds] DROP CONSTRAINT [' + @var3 + '];');
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('refunds') AND name = 'TotalRefundAmount' AND system_type_id = TYPE_ID('decimal'))
    ALTER TABLE [refunds] ALTER COLUMN [TotalRefundAmount] bigint NOT NULL;
GO

-- payouts.Amount → bigint
DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[payouts]') AND [c].[name] = N'Amount');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [payouts] DROP CONSTRAINT [' + @var4 + '];');
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('payouts') AND name = 'Amount' AND system_type_id = TYPE_ID('decimal'))
    ALTER TABLE [payouts] ALTER COLUMN [Amount] bigint NOT NULL;
GO

-- Payments.FinalAmount → bigint
DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Payments]') AND [c].[name] = N'FinalAmount');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Payments] DROP CONSTRAINT [' + @var5 + '];');
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Payments') AND name = 'FinalAmount' AND system_type_id = TYPE_ID('decimal'))
    ALTER TABLE [Payments] ALTER COLUMN [FinalAmount] bigint NOT NULL;
GO

-- Payments.DiscountAmount → bigint
DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Payments]') AND [c].[name] = N'DiscountAmount');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Payments] DROP CONSTRAINT [' + @var6 + '];');
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Payments') AND name = 'DiscountAmount' AND system_type_id = TYPE_ID('decimal'))
    ALTER TABLE [Payments] ALTER COLUMN [DiscountAmount] bigint NOT NULL;
GO

-- Payments.Amount → bigint
DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Payments]') AND [c].[name] = N'Amount');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Payments] DROP CONSTRAINT [' + @var7 + '];');
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Payments') AND name = 'Amount' AND system_type_id = TYPE_ID('decimal'))
    ALTER TABLE [Payments] ALTER COLUMN [Amount] bigint NOT NULL;
GO

-- Add VehicleId as NULLABLE to avoid FK conflict with existing rows (no matching Vehicle rows yet)
IF COL_LENGTH('itinerary_stops', 'VehicleId') IS NULL
    ALTER TABLE [itinerary_stops] ADD [VehicleId] uniqueidentifier NULL;
GO

-- itinerary_activities.Price → bigint
DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[itinerary_activities]') AND [c].[name] = N'Price');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [itinerary_activities] DROP CONSTRAINT [' + @var8 + '];');
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('itinerary_activities') AND name = 'Price' AND system_type_id = TYPE_ID('int'))
    ALTER TABLE [itinerary_activities] ALTER COLUMN [Price] bigint NOT NULL;
GO

-- itineraries.Price → bigint
DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[itineraries]') AND [c].[name] = N'Price');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [itineraries] DROP CONSTRAINT [' + @var9 + '];');
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('itineraries') AND name = 'Price' AND system_type_id = TYPE_ID('int'))
    ALTER TABLE [itineraries] ALTER COLUMN [Price] bigint NOT NULL;
GO

IF COL_LENGTH('itineraries', 'MaxCapacity') IS NULL
    ALTER TABLE [itineraries] ADD [MaxCapacity] int NOT NULL DEFAULT 0;
GO

IF COL_LENGTH('itineraries', 'TourGuideId') IS NULL
    ALTER TABLE [itineraries] ADD [TourGuideId] uniqueidentifier NULL;
GO

IF COL_LENGTH('images', 'PublicByUserId') IS NULL
    ALTER TABLE [images] ADD [PublicByUserId] uniqueidentifier NULL;
GO

-- bookings.TotalAmount → bigint
DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[bookings]') AND [c].[name] = N'TotalAmount');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [bookings] DROP CONSTRAINT [' + @var10 + '];');
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('bookings') AND name = 'TotalAmount' AND system_type_id = TYPE_ID('decimal'))
    ALTER TABLE [bookings] ALTER COLUMN [TotalAmount] bigint NOT NULL;
GO

-- booking_itineraries.Price → bigint
DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[booking_itineraries]') AND [c].[name] = N'Price');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [booking_itineraries] DROP CONSTRAINT [' + @var11 + '];');
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('booking_itineraries') AND name = 'Price' AND system_type_id = TYPE_ID('decimal'))
    ALTER TABLE [booking_itineraries] ALTER COLUMN [Price] bigint NOT NULL;
GO

-- booking_itineraries.FinalPrice → bigint
DECLARE @var12 sysname;
SELECT @var12 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[booking_itineraries]') AND [c].[name] = N'FinalPrice');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [booking_itineraries] DROP CONSTRAINT [' + @var12 + '];');
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('booking_itineraries') AND name = 'FinalPrice' AND system_type_id = TYPE_ID('decimal'))
    ALTER TABLE [booking_itineraries] ALTER COLUMN [FinalPrice] bigint NOT NULL;
GO

IF OBJECT_ID(N'[Vehicle]', 'U') IS NULL
BEGIN
    CREATE TABLE [Vehicle] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(max) NULL,
        [Capacity] int NOT NULL,
        [Type] int NOT NULL,
        [AgencyId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Vehicle] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Vehicle_agencies_AgencyId] FOREIGN KEY ([AgencyId]) REFERENCES [agencies] ([Id]) ON DELETE CASCADE
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_itinerary_stops_VehicleId' AND object_id = OBJECT_ID('itinerary_stops'))
    CREATE INDEX [IX_itinerary_stops_VehicleId] ON [itinerary_stops] ([VehicleId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_itineraries_TourGuideId' AND object_id = OBJECT_ID('itineraries'))
    CREATE INDEX [IX_itineraries_TourGuideId] ON [itineraries] ([TourGuideId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Vehicle_AgencyId' AND object_id = OBJECT_ID('Vehicle'))
    CREATE INDEX [IX_Vehicle_AgencyId] ON [Vehicle] ([AgencyId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_itineraries_agency_users_TourGuideId')
    ALTER TABLE [itineraries] ADD CONSTRAINT [FK_itineraries_agency_users_TourGuideId] FOREIGN KEY ([TourGuideId]) REFERENCES [agency_users] ([Id]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_itinerary_stops_Vehicle_VehicleId')
    ALTER TABLE [itinerary_stops] ADD CONSTRAINT [FK_itinerary_stops_Vehicle_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicle] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260425174805_20260421', N'8.0.23');
GO

COMMIT;
GO
