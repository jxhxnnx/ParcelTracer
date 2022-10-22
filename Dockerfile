#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/PaPl.SKS.Package.Services/PaPl.SKS.Package.Services.csproj", "src/PaPl.SKS.Package.Services/"]
COPY ["PaPl.SKS.Package.Services.Interfaces/PaPl.SKS.Package.Services.Interfaces.csproj", "PaPl.SKS.Package.Services.Interfaces/"]
COPY ["PaPl.SKS.Package.Services.DTOs/PaPl.SKS.Package.Services.DTOs.csproj", "PaPl.SKS.Package.Services.DTOs/"]
RUN dotnet restore "src/PaPl.SKS.Package.Services/PaPl.SKS.Package.Services.csproj"

COPY . .
WORKDIR "/src/src/PaPl.SKS.Package.Services"
RUN dotnet build "PaPl.SKS.Package.Services.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaPl.SKS.Package.Services.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaPl.SKS.Package.Services.dll"]