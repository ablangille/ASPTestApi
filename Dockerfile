FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 5186

ENV ASPNETCORE_URLS=http://+:5186
ENV ASPNETCORE_ENVIRONMENT=Development

# docker build --build-arg connection=%TestApi_ConnectionString% 
# --build-arg key=%TestApi_JWT_KEY% --build-arg subject=%TestApi_JWT_SUBJECT% 
# --build-arg issuer=%TestApi_JWT_ISSUER% --build-arg audience=%TestApi_JWT_AUDIENCE% -t netcore .
ARG connection
ARG key
ARG subject
ARG issuer
ARG audience

ENV TestApi_ConnectionString=${connection}
ENV TestApi_JWT_KEY=${key}
ENV TestApi_JWT_SUBJECT=${subject}
ENV TestApi_JWT_ISSUER=${issuer}
ENV TestApi_JWT_AUDIENCE=${audience}

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["TestApi.csproj", "./"]
RUN dotnet restore "TestApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "TestApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestApi.dll"]
