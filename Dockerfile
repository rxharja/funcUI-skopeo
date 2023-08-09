FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY . ./
RUN dotnet restore
RUN dotnet publish -r linux-x64 -c Release -p:IncludeNativeLibrariesForSelfExtract=true --self-contained -o out

FROM mcr.microsoft.com/dotnet/runtime:6.0
RUN apt update && apt install -y podman skopeo libx11-6 libice6 libsm6 libfreetype6 libfontconfig1
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "NewApp.dll"]
