﻿IF NOT EXISTS (SELECT * FROM information_schema.tables WHERE table_name='DatabaseVersion' AND table_schema = 'dbo')
    SELECT 0
ELSE
    SELECT MAX(Id) as [Version]
    FROM [dbo].[DatabaseVersion]
