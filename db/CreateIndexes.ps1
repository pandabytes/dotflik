$ErrorActionPreference = "Stop"

psql -U "dot_admin" -f CreateIndexes.sql dotflik
