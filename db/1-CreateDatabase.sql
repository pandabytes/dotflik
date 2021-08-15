CREATE USER dot_admin WITH PASSWORD 'password';
ALTER ROLE dot_admin WITH CREATEROLE;

DROP DATABASE IF EXISTS dotflik;

CREATE DATABASE dotflik
  WITH 
  OWNER = dot_admin
  ENCODING = 'UTF8'
  -- LC_COLLATE = 'English_United States.1252'
  -- LC_CTYPE = 'English_United States.1252'
  TABLESPACE = pg_default
  CONNECTION LIMIT = 10;

