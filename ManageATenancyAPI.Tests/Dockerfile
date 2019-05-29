FROM mcr.microsoft.com/dotnet/core/sdk:2.2

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./ManageATenancyAPI/ManageATenancyAPI.csproj ManageATenancyAPI/
COPY ./Hackney.InterfaceStubs/Hackney.InterfaceStubs.csproj Hackney.InterfaceStubs/
COPY ./Hackney.ServiceLocator/Hackney.ServiceLocator.csproj Hackney.ServiceLocator/
COPY ./ManageATenancyAPI.Database/ManageATenancyAPI.Database.csproj ManageATenancyAPI.Database/
COPY ./ManageATenancyAPI.Tests/ManageATenancyAPI.Tests.csproj ManageATenancyAPI.Tests/

RUN dotnet restore ./ManageATenancyAPI/ManageATenancyAPI.csproj
RUN dotnet restore ./Hackney.InterfaceStubs/Hackney.InterfaceStubs.csproj
RUN dotnet restore ./Hackney.ServiceLocator/Hackney.ServiceLocator.csproj
RUN dotnet restore ./ManageATenancyAPI.Database/ManageATenancyAPI.Database.csproj
RUN dotnet restore ./ManageATenancyAPI.Tests/ManageATenancyAPI.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out ManageATenancyAPI.Tests/ManageATenancyAPI.Tests.csproj

CMD dotnet test