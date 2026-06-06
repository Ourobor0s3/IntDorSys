@echo off
:: Проверка прав администратора
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo Requesting administrative privileges...
    powershell -Command "Start-Process '%~f0' -Verb RunAs"
    exit /b
)
:: Теперь с правами администратора
taskkill /f /im IntDorSys.Web.Api.exe >nul 2>&1
if %errorlevel% equ 0 (
    echo Stopped successfully.
) else (
    echo Failed to stop process (not running or access denied).
)
pause
exit /b