FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["int3306/int3306.csproj", "int3306/"]
RUN dotnet restore "int3306/int3306.csproj"
COPY . .
WORKDIR "/src/int3306"
RUN dotnet build "int3306.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "int3306.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "int3306.dll"]
