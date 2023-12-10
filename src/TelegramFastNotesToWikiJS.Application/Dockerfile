FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["./src/TelegramFastNotesToWikiJS.Application/", "TelegramFastNotesToWikiJS.Application/"]
COPY ["./src/TelegramFastNotesToWikiJS.Infrastructure.Abstractions/", "TelegramFastNotesToWikiJS.Infrastructure.Abstractions/"]
COPY ["./src/TelegramFastNotesToWikiJS.Infrastructure.Implementation.Telegram/", "TelegramFastNotesToWikiJS.Infrastructure.Implementation.Telegram/"]
COPY ["./src/TelegramFastNotesToWikiJS.Infrastructure.Implementation.WikiJs/", "TelegramFastNotesToWikiJS.Infrastructure.Implementation.WikiJs/"]

RUN dotnet restore "TelegramFastNotesToWikiJS.Application/TelegramFastNotesToWikiJS.Application.csproj"
COPY . .
WORKDIR "/src/TelegramFastNotesToWikiJS.Application"
RUN dotnet build "TelegramFastNotesToWikiJS.Application.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelegramFastNotesToWikiJS.Application.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelegramFastNotesToWikiJS.Application.dll"]