FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /src

RUN dotnet tool install --global dotnet-ef --version 10.0.*
ENV PATH="$PATH:/root/.dotnet/tools"

COPY . .

RUN dotnet restore "SneakersShop.API/SneakersShop.API.csproj"

ENTRYPOINT ["dotnet", "ef", "database", "update", "--project", "SneakersShop.Migrations/SneakersShop.Migrations.csproj", "--startup-project", "SneakersShop.API/SneakersShop.API.csproj"]