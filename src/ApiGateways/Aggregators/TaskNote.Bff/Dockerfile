#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/ApiGateways/Aggregators/TaskNote.Bff/TaskNote.Bff.csproj", "src/ApiGateways/Aggregators/TaskNote.Bff/"]
RUN dotnet restore "src/ApiGateways/Aggregators/TaskNote.Bff/TaskNote.Bff.csproj"
COPY . .
WORKDIR "/src/src/ApiGateways/Aggregators/TaskNote.Bff"
RUN dotnet build "TaskNote.Bff.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskNote.Bff.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskNote.Bff.dll"]