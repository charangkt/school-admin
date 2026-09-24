# School Admin - web demo (Blazor) for Render / any Docker host.
# The WPF desktop project is not part of this image.

# ---- Build ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first so this layer is cached when only code changes
COPY SchoolAdmin/src/SchoolAdmin.Data/SchoolAdmin.Data.csproj SchoolAdmin/src/SchoolAdmin.Data/
COPY SchoolAdmin/src/SchoolAdmin.Web/SchoolAdmin.Web.csproj SchoolAdmin/src/SchoolAdmin.Web/
RUN dotnet restore SchoolAdmin/src/SchoolAdmin.Web/SchoolAdmin.Web.csproj

COPY SchoolAdmin/src/SchoolAdmin.Data/ SchoolAdmin/src/SchoolAdmin.Data/
COPY SchoolAdmin/src/SchoolAdmin.Web/ SchoolAdmin/src/SchoolAdmin.Web/
# No --no-restore here: the Blazor framework assets (_framework/blazor.web.js) are
# only added once the .razor files are present, so restore must run again.
RUN dotnet publish SchoolAdmin/src/SchoolAdmin.Web/SchoolAdmin.Web.csproj -c Release -o /app

# ---- Run ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

# Render routes traffic to port 10000. The demo uses a throw-away SQLite file
# in /tmp (writable by the non-root container user) and fills it with demo data.
ENV ASPNETCORE_HTTP_PORTS=10000 \
    Database__Provider=Sqlite \
    ConnectionStrings__SchoolDb="Data Source=/tmp/school-demo.db" \
    DemoMode=true
EXPOSE 10000

ENTRYPOINT ["dotnet", "SchoolAdmin.Web.dll"]
