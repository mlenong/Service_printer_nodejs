@echo off
setlocal
echo ==========================================
echo Service Print Status Checker
echo ==========================================

echo [1/3] Checking Tray Application Process...
tasklist /FI "IMAGENAME eq ServicePrintTray.exe" 2>NUL | find /I "ServicePrintTray.exe" >NUL
if "%ERRORLEVEL%"=="0" (
    echo [OK] ServicePrintTray.exe is RUNNING.
) else (
    echo [ERROR] ServicePrintTray.exe is NOT RUNNING.
)

echo.
echo [2/3] Checking Backend Service Process...
tasklist /FI "IMAGENAME eq service-backend.exe" 2>NUL | find /I "service-backend.exe" >NUL
if "%ERRORLEVEL%"=="0" (
    echo [OK] service-backend.exe is RUNNING.
) else (
    echo [ERROR] service-backend.exe is NOT RUNNING.
)

echo.
echo [3/3] Pinging API (http://localhost:3000/ping)...
powershell -Command "try { $response = Invoke-RestMethod -Uri 'http://localhost:3000/ping' -ErrorAction Stop; Write-Host '[OK] API Response:' $response.message } catch { Write-Host '[ERROR] API Request Failed:' $_.Exception.Message }"

echo.
echo ==========================================
echo If [OK] across the board, the service is working.
echo If [ERROR], please run the installer again or check logs.
echo ==========================================
pause
