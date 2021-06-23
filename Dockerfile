FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["ShoeStorePayments/ShoeStorePayments.csproj", "ShoeStorePayments/"]
RUN dotnet restore "ShoeStorePayments/ShoeStorePayments.csproj"
COPY . .
WORKDIR "/src/ShoeStorePayments"
RUN dotnet build "ShoeStorePayments.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ShoeStorePayments.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ShoeStorePayments.dll"]