-- Script to fix Endzeit for existing appointments
-- This sets Endzeit to DatumUhrzeit + 60 minutes for all records with invalid Endzeit

UPDATE "Termine"
SET "Endzeit" = "DatumUhrzeit" + INTERVAL '60 minutes'
WHERE "Endzeit" < "DatumUhrzeit" OR "Endzeit" = '0001-01-01 00:00:00';

-- Verify the update
SELECT "Id", "DatumUhrzeit", "Endzeit",
       EXTRACT(EPOCH FROM ("Endzeit" - "DatumUhrzeit"))/60 AS "DurationMinutes"
FROM "Termine"
ORDER BY "DatumUhrzeit";
