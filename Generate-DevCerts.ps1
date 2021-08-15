
# Generate a dev cert for Docker container to use
& dotnet dev-certs https -ep .\.aspnet\https\aspnetapp.pfx -p password
& dotnet dev-certs https --trust