# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /source
COPY . .
WORKDIR /source/API
# ファイル名を指定する
RUN dotnet restore "./API.csproj" --disable-parallel
# release で configure。app フォルダにビルドしたバイナリを出力
RUN dotnet publish "./API.csproj" -c release -o /app --no-restore


# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal
WORKDIR /app
# build stage の app フォルダの中身をカレントフォルダにコピー
COPY --from=build /app ./

EXPOSE 5000 80 443

ENTRYPOINT ["dotnet", "API.dll"]