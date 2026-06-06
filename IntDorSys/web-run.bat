@echo off
cd /d "%~dp0"

echo Запуск приложений...

:: Запуск первого приложения на порту 5050
set ASPNETCORE_URLS=http://localhost:5050
start "IntDorSys.Api" /D "%~dp0intdorsys" cmd /c "IntDorSys.Web.Api.exe"

@REM start "" cmd /k "cd /d "%~dp0ui" && npm start"

echo Приложения запущены:
echo  - IntDorSys.Api → http://localhost:5050

exit /b