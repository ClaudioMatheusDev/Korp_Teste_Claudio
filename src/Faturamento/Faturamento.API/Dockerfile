FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/Faturamento/Faturamento.Domain/Faturamento.Domain.csproj src/Faturamento/Faturamento.Domain/
COPY src/Faturamento/Faturamento.Application/Faturamento.Application.csproj src/Faturamento/Faturamento.Application/
COPY src/Faturamento/Faturamento.Infrastructure/Faturamento.Infrastructure.csproj src/Faturamento/Faturamento.Infrastructure/
COPY src/Faturamento/Faturamento.API/Faturamento.API.csproj src/Faturamento/Faturamento.API/

RUN dotnet restore src/Faturamento/Faturamento.API/Faturamento.API.csproj

COPY src/Faturamento src/Faturamento
RUN dotnet publish src/Faturamento/Faturamento.API/Faturamento.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Faturamento.API.dll"]
