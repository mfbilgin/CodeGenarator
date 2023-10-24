FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ConsoleUI/ConsoleUI.csproj", "ConsoleUI/"]
RUN dotnet restore "ConsoleUI/ConsoleUI.csproj"
COPY . .
WORKDIR "/src/ConsoleUI"
RUN dotnet build "ConsoleUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ConsoleUI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConsoleUI.dll"]
