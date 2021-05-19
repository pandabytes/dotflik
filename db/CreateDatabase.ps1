$ErrorActionPreference = "Stop"

psql -U "postgres" -f CreateDatabase.sql
