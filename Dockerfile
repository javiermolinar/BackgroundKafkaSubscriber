FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["BackgroundKafkaSubscriber.csproj", "./"]
RUN dotnet restore "./BackgroundKafkaSubscriber.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "BackgroundKafkaSubscriber.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BackgroundKafkaSubscriber.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BackgroundKafkaSubscriber.dll"]
