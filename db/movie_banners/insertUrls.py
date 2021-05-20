import sys
import psycopg2
import psycopg2.extras
from imdb import IMDb
from getpass import getpass

def insertBanner(dbName: str, user: str, password: str, 
                 host: str = "localhost", port: int = 5432):
  with psycopg2.connect(database=dbName, user=user, host=host, password=password, port=port) as connection:
    cur = connection.cursor(cursor_factory=psycopg2.extras.DictCursor)
    cur.execute("SELECT id FROM movies")
    rows = cur.fetchall()

    ia = IMDb()
    for i, r in enumerate(rows):
      movieId = r["id"]
      
      # Ignore the first 2 characters because IMDb doesn't use those characters
      movie = ia.get_movie(movieId[2:])

      if "cover url" in movie:
        bannerUrl = movie["cover url"]
        cur.execute(f"UPDATE movies SET banner_url = '{bannerUrl}' where id = '{movieId}'")
        print(f"{i+1} - Updated movie \"{movieId}\" {bannerUrl}")
      else:
        print(f"{i+1} - Movie \"{movieId}\" has no banner")

def insertHeadshot(dbName: str, user: str, password: str, 
                   host: str = "localhost", port: int = 5432):
  with psycopg2.connect(database=dbName, user=user, host=host, password=password, port=port) as connection:
    cur = connection.cursor(cursor_factory=psycopg2.extras.DictCursor)
    cur.execute("SELECT id FROM stars")
    rows = cur.fetchall()

    ia = IMDb()
    for i, r in enumerate(rows):
      starId = r["id"]
      
      # Ignore the first 2 characters because IMDb doesn't use those characters
      star = ia.get_person(starId[2:])

      if "headshot" in star:
        headshotUrl = star["headshot"]
        cur.execute(f"UPDATE stars SET headshot = '{headshotUrl}' where id = '{starId}'")
        print(f"{i+1} - Updated star \"{starId}\" {headshotUrl}")
      else:
        print(f"{i+1} - Star \"{starId}\" has no headshot")

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