FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/Estoque/Estoque.Domain/Estoque.Domain.csproj src/Estoque/Estoque.Domain/
COPY src/Estoque/Estoque.Application/Estoque.Application.csproj src/Estoque/Estoque.Application/
COPY src/Estoque/Estoque.Infrastructure/Estoque.Infrastructure.csproj src/Estoque/Estoque.Infrastructure/
COPY src/Estoque/Estoque.API/Estoque.API.csproj src/Estoque/Estoque.API/

RUN dotnet restore src/Estoque/Estoque.API/Estoque.API.csproj

COPY src/Estoque src/Estoque
RUN dotnet publish src/Estoque/Estoque.API/Estoque.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Estoque.API.dll"]
