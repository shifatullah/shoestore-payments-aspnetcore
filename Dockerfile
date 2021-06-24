FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ShoeStorePayments/ShoeStorePayments.csproj", "ShoeStorePayments/"]
RUN dotnet restore "ShoeStorePayments/ShoeStorePayments.csproj"
COPY . .
WORKDIR "/src/ShoeStorePayments"
RUN dotnet build "ShoeStorePayments.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShoeStorePayments.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShoeStorePayments.dll"]