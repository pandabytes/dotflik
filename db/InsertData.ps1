$ErrorActionPreference = "Stop"

psql -U "dot_admin" -f InsertData.sql dotflik
