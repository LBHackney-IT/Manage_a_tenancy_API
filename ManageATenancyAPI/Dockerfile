#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.1-sdk-nanoserver-sac2016 AS base
WORKDIR /app
EXPOSE 80

#TCENV

FROM microsoft/dotnet:2.1-sdk-nanoserver-sac2016 AS build
WORKDIR /src
COPY ["ManageATenancyAPI/ManageATenancyAPI.csproj", "ManageATenancyAPI/"]
COPY ["Hackney.InterfaceStubs/Hackney.InterfaceStubs.csproj", "Hackney.InterfaceStubs/"]
COPY ["Hackney.ServiceLocator/Hackney.ServiceLocator.csproj", "Hackney.ServiceLocator/"]
RUN dotnet restore "ManageATenancyAPI/ManageATenancyAPI.csproj"
COPY . .
WORKDIR "/src/ManageATenancyAPI"
RUN dotnet build "ManageATenancyAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ManageATenancyAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ManageATenancyAPI.dll"]