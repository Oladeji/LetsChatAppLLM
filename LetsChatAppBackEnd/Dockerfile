FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["LetsChatAppBackEnd/LetsChatAppBackEnd.csproj", "LetsChatAppBackEnd/"]
RUN dotnet restore "LetsChatAppBackEnd/LetsChatAppBackEnd.csproj"
COPY . .
WORKDIR "/src/LetsChatAppBackEnd"
RUN dotnet build "LetsChatAppBackEnd.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LetsChatAppBackEnd.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create directories for uploads and database
RUN mkdir -p AllDbDocumentsUploads

ENTRYPOINT ["dotnet", "LetsChatAppBackEnd.dll"]
