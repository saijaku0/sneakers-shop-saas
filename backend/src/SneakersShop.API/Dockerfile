FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["SneakersShop.API/SneakersShop.API.csproj", "SneakersShop.API/"]
COPY ["SneakersShop.Application/SneakersShop.Application.csproj", "SneakersShop.Application/"]
COPY ["SneakersShop.Domain/SneakersShop.Domain.csproj", "SneakersShop.Domain/"]
COPY ["SneakersShop.Infrastructure/SneakersShop.Infrastructure.csproj", "SneakersShop.Infrastructure/"]

RUN dotnet restore "SneakersShop.API/SneakersShop.API.csproj"

COPY . .
WORKDIR "/src/SneakersShop.API"
RUN dotnet build "SneakersShop.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SneakersShop.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SneakersShop.API.dll"]