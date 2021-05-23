# Database

This directory contain scripts that create the tables and insert data to those tables. The database used here is PostgresSQL.

# Backup tables
```sql
-- Export table to csv file
COPY movies TO 'C:\dev\dotflik\db\movies_db.csv' DELIMITER ',' CSV HEADER;

-- Import csv file to table
COPY movies(id, title, year, director, bannerUrl)
FROM 'C:\dev\dotflik\db\movies_db.csv'
DELIMITER ','
CSV HEADER;

-- Dump entire database to sql file
pg_dump.exe --column-inserts -a -U dot_admin -h localhost dotflik > db\db.sql
```


