FROM postgres:13

# Insert sample data to db
WORKDIR /db_scripts

COPY ./db .

# These env vars replace the need of 1-CreateDatabase.sql
ENV POSTGRES_USER=dot_admin
ENV POSTGRES_PASSWORD=password
ENV POSTGRES_DB=dotflik

ADD ./db/2-CreateTables.sql /docker-entrypoint-initdb.d
ADD ./db/3-InsertSampleData.sql /docker-entrypoint-initdb.d
ADD ./db/4-CreateIndexes.sql /docker-entrypoint-initdb.d
