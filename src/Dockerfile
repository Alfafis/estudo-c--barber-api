FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /src

EXPOSE 8080

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV DOTNET_USE_POLLING_FILE_WATCHER=1

CMD ["dotnet", "watch", "--project", "/src/Barber.Api.csproj", "run"]