# ----- 第一階段: 建構階段 (Build Stage) -----
# 使用 .NET SDK 映像作為基礎，用於編譯您的應用程式
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

# 設定容器內的工作目錄為 /app
WORKDIR /app

# 將所有專案檔案和原始碼複製到容器的 /app 目錄
# 這個 . 表示將 Docker build context (您執行 docker build 的當前目錄) 的所有內容複製到 /app
COPY . .

# 進入您的專案目錄 (如果您的 .csproj 在 /app 下的子目錄中)
# 由於您執行 docker build 的目錄是 UserManagementAPI 專案的根目錄，
# 所以 UserManagementAPI.csproj 會直接在 /app 下。
# 因此，這裡不需要額外的 WORKDIR 到子目錄。
# 如果您的 .csproj 檔案名稱是 UserManagementAPI.csproj，它會在 /app/UserManagementAPI.csproj
# 如果您的專案結構是 UserManagementSolution/UserManagementAPI/UserManagementAPI.csproj
# 並且您在 UserManagementAPI 資料夾執行 docker build，那麼 COPY . . 會把 UserManagementAPI 資料夾的內容複製到 /app
# 所以，UserManagementAPI.csproj 會在 /app/UserManagementAPI.csproj

# 為了確保 dotnet publish 能找到 .csproj 檔案，我們需要確保路徑正確。
# 假設您在 UserManagementAPI 專案的根目錄下執行 docker build .
# 那麼 COPY . . 會將該目錄下的所有內容複製到 /app
# 所以 UserManagementAPI.csproj 將位於 /app/UserManagementAPI.csproj

# 發布 (Publish) 應用程式到 'out' 目錄
# dotnet publish 會自動處理 restore 和 build
RUN dotnet publish "UserManagementAPI.csproj" -c Release -o /app/out

# ----- 第二階段: 最終運行階段 (Final Run Stage) -----
# 使用 .NET ASP.NET Runtime 映像作為基礎，用於運行您的應用程式
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final

# 設定容器內的工作目錄為 /app
WORKDIR /app

# 從建構階段複製發布的應用程式檔案到最終運行階段的 /app 目錄
# --from=build: 指定從名為 'build' 的階段複製
# /app/out: 建構階段的輸出路徑
# .: 複製到當前階段的工作目錄 (即 /app)
COPY --from=build /app/out .

# 暴露容器的埠號
EXPOSE 80
EXPOSE 443

# 定義容器啟動時執行的命令
ENTRYPOINT ["dotnet", "UserManagementAPI.dll"]