FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/TskMngmntSys.Web.Host/TskMngmntSys.Web.Host.csproj", "src/TskMngmntSys.Web.Host/"]
COPY ["src/TskMngmntSys.Web.Core/TskMngmntSys.Web.Core.csproj", "src/TskMngmntSys.Web.Core/"]
COPY ["src/TskMngmntSys.Application/TskMngmntSys.Application.csproj", "src/TskMngmntSys.Application/"]
COPY ["src/TskMngmntSys.Core/TskMngmntSys.Core.csproj", "src/TskMngmntSys.Core/"]
COPY ["src/TskMngmntSys.EntityFrameworkCore/TskMngmntSys.EntityFrameworkCore.csproj", "src/TskMngmntSys.EntityFrameworkCore/"]
WORKDIR "/src/src/TskMngmntSys.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/TskMngmntSys.Web.Host", "src/TskMngmntSys.Web.Host"]
COPY ["src/TskMngmntSys.Web.Core", "src/TskMngmntSys.Web.Core"]
COPY ["src/TskMngmntSys.Application", "src/TskMngmntSys.Application"]
COPY ["src/TskMngmntSys.Core", "src/TskMngmntSys.Core"]
COPY ["src/TskMngmntSys.EntityFrameworkCore", "src/TskMngmntSys.EntityFrameworkCore"]
WORKDIR "/src/src/TskMngmntSys.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "TskMngmntSys.Web.Host.dll"]
