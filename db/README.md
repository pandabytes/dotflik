# Database

This directory contain scripts that create the tables and insert data to those tables. The database used here is PostgresSQL.

The schema and data are taken from [here](https://grape.ics.uci.edu/wiki/public/wiki/cs122b-2019-winter-project1#Task4:CreateaMySQLDatabase).

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


