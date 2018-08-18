FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Miqo.License/*.csproj ./Miqo.Config/
COPY Miqo.License.CreateKeys/*.csproj ./Miqo.License.CreateKeys/
COPY Miqo.License.Tests/*.csproj ./Miqo.License.Tests/
RUN dotnet restore

# copy and build everything else
COPY Miqo.License/. ./Miqo.License/
COPY Miqo.License.CreateKeys/. ./Miqo.License.CreateKeys/
COPY Miqo.License.Tests/. ./Miqoo.License.Tests/
RUN dotnet build

FROM build AS testrunner
WORKDIR /app/Miqo.License.Tests
COPY Miqo.License.Tests/. .
RUN dotnet test
ENTRYPOINT ["dotnet", "test","--logger:trx"]
