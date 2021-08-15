DROP INDEX IF EXISTS idx_movies_title;
DROP INDEX IF EXISTS idx_movies_year;
DROP INDEX IF EXISTS idx_stars_name;
DROP INDEX IF EXISTS idx_stars_birth_year;
DROP INDEX IF EXISTS idx_customers_email;
DROP INDEX IF EXISTS idx_ratings_rating;

CREATE INDEX idx_movies_year ON Movies USING BTREE
(
  year
);

CREATE INDEX idx_movies_title ON Movies USING HASH
(
  title
);

CREATE INDEX idx_stars_name ON Stars USING HASH
(
  name
);

CREATE INDEX idx_stars_birth_year ON Stars USING HASH
(
  birthYear
);

CREATE INDEX idx_customers_email ON Customers USING HASH
(
  email
);

CREATE INDEX idx_ratings_rating ON Ratings USING BTREE
(
  rating
);


