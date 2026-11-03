FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY GiftOfTheGivers.Helpers/GiftOfTheGivers.Helpers.csproj GiftOfTheGivers.Helpers/
COPY Service/Service.csproj Service/
COPY Web/Web.csproj Web/
RUN dotnet restore Web/Web.csproj
COPY GiftOfTheGivers.Helpers/ GiftOfTheGivers.Helpers/
COPY Service/ Service/
COPY Web/ Web/
RUN dotnet publish Web/Web.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
ENTRYPOINT ["dotnet", "Web.dll"]
