FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY API/API.csproj ./
ADD nginx.conf.sigil ./

RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out API.csproj

# Build runtime image
FROM microsoft/aspnetcore:2.0
RUN apt-get update \
    && apt-get install -y \
    texlive-full \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*
RUN apt-get update && apt-get install -y sendmail 
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=build-env /app/nginx.conf.sigil .
ENTRYPOINT ["dotnet", "API.dll"]