-- Fix missing SpotLeft on itineraries and NumberOfGuests rename on booking_itineraries
BEGIN TRANSACTION;
GO

-- 1. Add SpotLeft to itineraries (was skipped as no-op in FixModelSync_20260501)
IF COL_LENGTH('itineraries', 'SpotLeft') IS NULL
    ALTER TABLE [itineraries] ADD [SpotLeft] int NOT NULL DEFAULT 0;
GO

-- 2. Rename NumberOfPeople -> NumberOfGuests in booking_itineraries
--    (entity uses NumberOfGuests; migration AddNumberOfPeopleToBookingItinerary added NumberOfPeople)
IF COL_LENGTH('booking_itineraries', 'NumberOfPeople') IS NOT NULL
   AND COL_LENGTH('booking_itineraries', 'NumberOfGuests') IS NULL
BEGIN
    EXEC sp_rename 'booking_itineraries.NumberOfPeople', 'NumberOfGuests', 'COLUMN';
END
GO

COMMIT;
GO
