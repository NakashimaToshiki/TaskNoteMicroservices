#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/MicroServices/Job/Job.WebApi/Job.WebApi.csproj", "src/MicroServices/Job/Job.WebApi/"]
RUN dotnet restore "src/MicroServices/Job/Job.WebApi/Job.WebApi.csproj"
COPY . .
WORKDIR "/src/src/MicroServices/Job/Job.WebApi"
RUN dotnet build "Job.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Job.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Job.WebApi.dll"]