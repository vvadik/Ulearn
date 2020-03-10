FROM mcr.microsoft.com/dotnet/core/sdk

RUN mkdir /app

COPY ./app/ /app/

WORKDIR app

RUN dotnet build packages.csproj