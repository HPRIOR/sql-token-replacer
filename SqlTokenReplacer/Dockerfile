FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SqlTokenReplacer/SqlTokenReplacer.fsproj", "SqlTokenReplacer/"]
RUN dotnet restore "SqlTokenReplacer/SqlTokenReplacer.fsproj"
COPY . .
WORKDIR "/src/SqlTokenReplacer"
RUN dotnet build "SqlTokenReplacer.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SqlTokenReplacer.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SqlTokenReplacer.dll"]
