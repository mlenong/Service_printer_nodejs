@echo off
set "DIST_DIR=Service Print 3000"

if not exist "%DIST_DIR%" (
    mkdir "%DIST_DIR%"
)

echo Collecting files into "%DIST_DIR%"...

copy /Y ServicePrintInstaller.exe "%DIST_DIR%\"
copy /Y ServicePrintTray.exe "%DIST_DIR%\"
copy /Y service-backend.exe "%DIST_DIR%\"
copy /Y SumatraPDF.exe "%DIST_DIR%\"
copy /Y printer.ico "%DIST_DIR%\"
copy /Y check_status.bat "%DIST_DIR%\"
copy /Y "cara install servis print.txt" "%DIST_DIR%\"

if not exist "%DIST_DIR%\files" (
    mkdir "%DIST_DIR%\files"
)

echo Files collected successfully in "%DIST_DIR%".
pause
