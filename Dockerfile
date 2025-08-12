FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["E-Menu.API/E-Menu.API.csproj", "E-Menu.API/"]
COPY ["E-Menu.Engine.Standart/E-Menu.Engine.Standart.csproj", "E-Menu.Engine.Standart/"]
COPY ["Shared.Kernel.Standart/Shared.Kernel.Standart.csproj", "Shared.Kernel.Standart/"]
COPY ["E-Menu.Internal/E-Menu.Internal.csproj", "E-Menu.Internal/"]
COPY ["E-Menu.JobScheduler/E-Menu.JobScheduler.csproj", "E-Menu.JobScheduler/"]
COPY ["E-Menu.Logging/E-Menu.Logging.csproj", "E-Menu.Logging/"]
COPY ["E-Menu.Caching/E-Menu.Caching.csproj", "E-Menu.Caching/"]
COPY ["E-Menu.Chatbot/E-Menu.Chatbot.csproj", "E-Menu.Chatbot/"]

RUN dotnet restore "E-Menu.API/E-Menu.API.csproj"

COPY . .
WORKDIR "/src/E-Menu.API"
RUN dotnet publish "E-Menu.API.csproj" -c Release -o /app/publish  

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /src/E-Menu.API/appsettings.json .
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "E-Menu.API.dll"]