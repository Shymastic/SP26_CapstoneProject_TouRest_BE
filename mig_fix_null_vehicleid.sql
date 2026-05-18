-- Fix NULL VehicleId in itinerary_stops (causes SqlNullValueException in EF Core)
-- VehicleId is declared non-nullable in entity but may be NULL in DB for old rows

BEGIN TRANSACTION;
GO

-- 1. Fill NULL VehicleId with empty Guid so EF can read the column
IF COL_LENGTH('itinerary_stops', 'VehicleId') IS NOT NULL
BEGIN
    UPDATE [itinerary_stops]
    SET [VehicleId] = '00000000-0000-0000-0000-000000000000'
    WHERE [VehicleId] IS NULL;
END
GO

-- 2. If the column is currently nullable, make it NOT NULL
IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('itinerary_stops')
      AND name = 'VehicleId'
      AND is_nullable = 1
)
BEGIN
    ALTER TABLE [itinerary_stops] ALTER COLUMN [VehicleId] uniqueidentifier NOT NULL;
END
GO

-- 3. Also fix ItineraryId1 shadow column: fill NULLs if it exists (leftover EF artifact)
IF COL_LENGTH('itinerary_stops', 'ItineraryId1') IS NOT NULL
BEGIN
    UPDATE [itinerary_stops] ist
    SET ist.[ItineraryId1] = ist.[ItineraryId]
    WHERE ist.[ItineraryId1] IS NULL;
END
GO

COMMIT;
GO
