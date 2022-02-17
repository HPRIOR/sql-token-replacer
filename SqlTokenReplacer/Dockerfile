FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["/SqlTokenReplacer.fsproj", "SqlTokenReplacer/"]
RUN dotnet restore "SqlTokenReplacer/SqlTokenReplacer.fsproj"
COPY . .
RUN dotnet build "SqlTokenReplacer.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SqlTokenReplacer.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SqlTokenReplacer.dll"]
