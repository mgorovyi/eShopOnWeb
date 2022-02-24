#https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-6.0

dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\aspnetapp.pfx -p "{hty$Ve[hty1"
dotnet dev-certs https --trust


docker run --rm -it -p 5001:80 -p 44315:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_ENVIRONMENT=Development -e ASPNETCORE_HTTPS_PORT=44315 -e ASPNETCORE_Kestrel__Certificates__Default__Password="{hty$Ve[hty1" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v $env:USERPROFILE\.aspnet\https:/https/ vivethereeshop:latest
docker run --rm -it -p 5098:80 -p 5099:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_ENVIRONMENT=Development -e ASPNETCORE_HTTPS_PORT=5099 -e ASPNETCORE_Kestrel__Certificates__Default__Password="{hty$Ve[hty1" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v $env:USERPROFILE\.aspnet\https:/https/ vivethereapi:latest