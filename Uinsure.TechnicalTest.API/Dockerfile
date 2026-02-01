FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Uinsure.TechnicalTest.sln ./
COPY Uinsure.TechnicalTest.API/Uinsure.TechnicalTest.API.csproj Uinsure.TechnicalTest.API/
COPY Uinsure.TechnicalTest.Application/Uinsure.TechnicalTest.Application.csproj Uinsure.TechnicalTest.Application/
COPY Uinsure.TechnicalTest.Domain/Uinsure.TechnicalTest.Domain.csproj Uinsure.TechnicalTest.Domain/
COPY Uinsure.TechnicalTest.Infrastructure.EntityFramework/Uinsure.TechnicalTest.Infrastructure.EntityFramework.csproj Uinsure.TechnicalTest.Infrastructure.EntityFramework/

RUN dotnet restore Uinsure.TechnicalTest.API/Uinsure.TechnicalTest.API.csproj

COPY . .
WORKDIR /src/Uinsure.TechnicalTest.API
RUN dotnet publish Uinsure.TechnicalTest.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Uinsure.TechnicalTest.API.dll"]
