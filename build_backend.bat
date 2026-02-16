@echo off
echo Packaging Node.js Backend...

call npx pkg . --targets node18-win-x64 --output service-backend.exe --public

if %errorlevel% neq 0 (
    echo Backend Packaging Failed!
    exit /b %errorlevel%
)
echo Backend Packaging Successful. service-backend.exe created.
