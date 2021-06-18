import sys
import psycopg2
import psycopg2.extras
import logging
from imdb import IMDb, IMDbDataAccessError
from getpass import getpass

# Disable imdbpy logging
logger = logging.getLogger('imdbpy');
logger.disabled = True

def insertBanner(dbName: str, user: str, password: str, 
                 host: str = "localhost", port: int = 5432):
  with psycopg2.connect(database=dbName, user=user, host=host, password=password, port=port) as connection:
    cur = connection.cursor(cursor_factory=psycopg2.extras.DictCursor)
    cur.execute("SELECT id FROM movies")
    rows = cur.fetchall()

    coverUrlKey = "cover url"
    fullSizeUrlKey = "full-size cover url"
    ia = IMDb(reraiseExceptions=True)

    for i, r in enumerate(rows):
      movieId = r["id"]

      print(f"Id: {movieId[2:]} - {movieId}")     
      try:
        # Ignore the first 2 characters because IMDb doesn't use those characters
        movie = ia.get_movie(movieId[2:])

        bannerUrl = "NULL"
        if fullSizeUrlKey in movie.keys():
          bannerUrl = f"'{movie[fullSizeUrlKey]}'"
        elif coverUrlKey in movie.keys():
          bannerUrl = f"'{movie[coverUrlKey]}'"

        cur.execute(f"UPDATE movies SET bannerUrl = {bannerUrl} WHERE id = '{movieId}'")
        print(f"  {i+1} - Updated movie \"{movieId}\" {bannerUrl}")
      except IMDbDataAccessError as ex:
        print(str(ex))

def insertHeadshot(dbName: str, user: str, password: str, 
                   host: str = "localhost", port: int = 5432):
  with psycopg2.connect(database=dbName, user=user, host=host, password=password, port=port) as connection:
    cur = connection.cursor(cursor_factory=psycopg2.extras.DictCursor)
    cur.execute("SELECT id FROM stars")
    rows = cur.fetchall()

    headshotKey = "headshot"
    fullSizeKey = "full-size headshot"
    ia = IMDb(reraiseExceptions=True)

    for i, r in enumerate(rows):
      starId = r["id"]
      
      print(f"Id: {starId[2:]} - {starId}")
      try:
        # Ignore the first 2 characters because IMDb doesn't use those characters
        star = ia.get_person(starId[2:])

        headshotUrl = "NULL"
        if fullSizeKey in star.keys():
          headshotUrl = f"'{star[fullSizeKey]}'"
        elif headshotKey in star.keys():
          headshotUrl = f"'{star[headshotKey]}'"

        cur.execute(f"UPDATE stars SET headshot = {headshotUrl} WHERE id = '{starId}'")
        print(f"{i+1} - Updated star \"{starId}\" {headshotUrl}")
      except IMDbDataAccessError as ex:
        print(str(ex))

if __name__ == "__main__":
  args = sys.argv[1:]
  if len(args) != 1 or ("banner" != args[0] and "headshot" != args[0]):
    raise ValueError("Required argument must be 'banner' OR 'headshot'")

  host = input("Enter host [localhost]: ") or "localhost"
  dbName = input("Enter database name [dotflik]: ") or "dotflik"
  port = int(input("Enter port [5432]: ") or 5432)
  user = input("Enter username [dot_admin]: ") or "dot_admin"
  password = getpass(f"Enter password for user {user}: ")

  if "banner" == args[0] :
    insertBanner(dbName, user, password, host, port)
  elif "headshot" == args[0] :
    insertHeadshot(dbName, user, password, host, port)