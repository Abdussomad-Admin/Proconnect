FROM ://microsoft.com AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080 8081 10000

FROM ://microsoft.com AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Host/Host.csproj", "Host/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]

RUN dotnet restore "Host/Host.csproj"
COPY . .
WORKDIR "/src/Host" 
RUN dotnet build "Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:10000
ENTRYPOINT ["dotnet", "Host.dll"]
