-- Add missing ImageUrls column to reports table
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'reports' AND COLUMN_NAME = 'ImageUrls'
)
BEGIN
    ALTER TABLE [reports] ADD [ImageUrls] NVARCHAR(MAX) NULL;
    PRINT 'Column ImageUrls added to reports table.';
END
ELSE
BEGIN
    PRINT 'Column ImageUrls already exists in reports table. Skipped.';
END
