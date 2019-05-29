FROM microsoft/dotnet:2.1-sdk as builder

WORKDIR /src
COPY *.sln ./
COPY ManageATenancyAPI/ManageATenancyAPI.csproj ManageATenancyAPI/
COPY Hackney.InterfaceStubs/Hackney.InterfaceStubs.csproj Hackney.InterfaceStubs/
COPY Hackney.ServiceLocator/Hackney.ServiceLocator.csproj Hackney.ServiceLocator/
COPY ManageATenancyAPI.Database/ManageATenancyAPI.Database.csproj ManageATenancyAPI.Database/
RUN dotnet restore ManageATenancyAPI/ManageATenancyAPI.csproj

# copy all other files in the repo
COPY . .
RUN dotnet publish ./ManageATenancyAPI/ManageATenancyAPI.csproj -c Release -o out

# swap to a smallar image to reduce file size
FROM microsoft/dotnet:2.1-runtime

# set required envs for New Relic
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=./out/newrelic \
CORECLR_PROFILER_PATH=./out/newrelic/libNewRelicProfiler.so \
NEW_RELIC_LICENSE_KEY="${NEW_RELIC_LICENSE_KEY}" \
NEW_RELIC_APP_NAME="${NEW_RELIC_APP_NAME}"

WORKDIR /src/
# copy over our compiled .net app from the previouse image
COPY --from=builder /src/ManageATenancyAPI/out .

EXPOSE ${PORT:-3000}
ENV ASPNETCORE_URLS=http://+:${PORT:-3000}
CMD dotnet ./ManageATenancyAPI.dll