# Базовий образ для рантайму
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Образ для збірки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копіюємо файли проектів для відновлення залежностей
COPY ["FindFi/FindFi.csproj", "FindFi/"]
COPY ["FindFi.Bll/FindFi.Bll.csproj", "FindFi.Bll/"]
COPY ["FindFi.Dal/FindFi.Dal.csproj", "FindFi.Dal/"]
COPY ["FindFi.Domain/FindFi.Domain.csproj", "FindFi.Domain/"]

# Відновлюємо залежності
RUN dotnet restore "FindFi/FindFi.csproj"

# Копіюємо всі інші файли
COPY . .
WORKDIR "/src/FindFi"

# Збираємо проект
RUN dotnet build "FindFi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Публікуємо проект
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FindFi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Фінальний образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FindFi.dll"]
