#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/MicroServices/Job/Job.WebApp/Job.WebApp.csproj", "src/MicroServices/Job/Job.WebApp/"]
RUN dotnet restore "src/MicroServices/Job/Job.WebApp/Job.WebApp.csproj"
COPY . .
WORKDIR "/src/src/MicroServices/Job/Job.WebApp"
RUN dotnet build "Job.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Job.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Job.WebApp.dll"]