DROP TABLE IF EXISTS Movies CASCADE;
DROP TABLE IF EXISTS Stars CASCADE;
DROP TABLE IF EXISTS Stars_In_Movies CASCADE;
DROP TABLE IF EXISTS Genres CASCADE;
DROP TABLE IF EXISTS Genres_In_Movies CASCADE;
DROP TABLE IF EXISTS Customers CASCADE;
DROP TABLE IF EXISTS Sales CASCADE;
DROP TABLE IF EXISTS Credit_Cards CASCADE;
DROP TABLE IF EXISTS Ratings CASCADE;

CREATE TABLE Movies (
  id TEXT PRIMARY KEY,
  title TEXT NOT NULL,
  year INT NOT NULL,
  director TEXT NOT NULL,
  banner_url TEXT NOT NULL
);

CREATE TABLE Stars (
  id TEXT PRIMARY KEY,
  name TEXT NOT NULL,
  birth_year INT,
  headshot TEXT NOT NULL
);

CREATE TABLE Stars_In_Movies (
  star_id TEXT REFERENCES Stars(id),
  movie_id TEXT REFERENCES Movies(id)
);

CREATE TABLE Genres (
  id INT PRIMARY KEY,
  name TEXT NOT NULL
);

CREATE TABLE Genres_In_Movies (
  genre_id INT REFERENCES Genres(id),
  movie_id TEXT REFERENCES Movies(id)
);

CREATE TABLE Credit_Cards (
  id TEXT PRIMARY KEY,
  first_name TEXT NOT NULL,
  last_name TEXT NOT NULL,
  expiration_date DATE NOT NULL
);

CREATE TABLE Customers (
  id INT PRIMARY KEY,
  first_name TEXT NOT NULL,
  last_name TEXT NOT NULL,
  cc_id TEXT REFERENCES Credit_Cards(id),
  address TEXT NOT NULL,
  email TEXT NOT NULL UNIQUE,
  password TEXT NOT NULL
);

CREATE TABLE Sales (
  id INT PRIMARY KEY,
  customer_id INT REFERENCES Customers(id),
  movie_id TEXT REFERENCES Movies(id),
  sale_date DATE NOT NULL
);

CREATE TABLE Ratings (
  movie_id TEXT REFERENCES Movies(id),
  rating FLOAT NOT NULL,
  num_votes INT NOT NULL
);
