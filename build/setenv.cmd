@echo off

:: Always using x86 toolset.
goto x86

if "%PROCESSOR_ARCHITECTURE%" == "x86" goto :x86
if "%PROCESSOR_ARCHITECTURE%" == "AMD64" goto :x64

echo.ERROR^^^! Unknown processor architecture.
exit /b 1

:x86
set path=%PATH%;"%WINDIR%\Microsoft.NET\Framework\v4.0.30319"
goto :eof

:x64
set path=%PATH%;"%WINDIR%\Microsoft.NET\Framework64\v4.0.30319"
goto :eof
