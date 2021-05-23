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
  bannerUrl TEXT NULL
);

CREATE TABLE Stars (
  id TEXT PRIMARY KEY,
  name TEXT NOT NULL,
  birthYear INT,
  headshot TEXT NULL
);

CREATE TABLE Stars_In_Movies (
  starId TEXT REFERENCES Stars(id),
  movieId TEXT REFERENCES Movies(id)
);

CREATE TABLE Genres (
  id INT PRIMARY KEY,
  name TEXT NOT NULL
);

CREATE TABLE Genres_In_Movies (
  genreId INT REFERENCES Genres(id),
  movieId TEXT REFERENCES Movies(id)
);

CREATE TABLE Credit_Cards (
  id TEXT PRIMARY KEY,
  firstName TEXT NOT NULL,
  lastName TEXT NOT NULL,
  expirationDate DATE NOT NULL
);

CREATE TABLE Customers (
  id INT PRIMARY KEY,
  firstName TEXT NOT NULL,
  lastName TEXT NOT NULL,
  ccId TEXT REFERENCES Credit_Cards(id),
  address TEXT NOT NULL,
  email TEXT NOT NULL UNIQUE,
  password TEXT NOT NULL
);

CREATE TABLE Sales (
  id INT PRIMARY KEY,
  customerId INT REFERENCES Customers(id),
  movieId TEXT REFERENCES Movies(id),
  saleDate DATE NOT NULL
);

CREATE TABLE Ratings (
  movieId TEXT REFERENCES Movies(id),
  rating FLOAT NOT NULL,
  numVotes INT NOT NULL
);
