@echo off & setlocal enableextensions enabledelayedexpansion

call "%~dp0\build\setenv"
if "%errorlevel%" neq "0" goto errSetEnv

:: build
MSBuild %* "build.proj"
if "%errorlevel%" neq "0" goto errBuild
goto :eof

:: errors
:errBuild
echo.Error: build error.
exit /b 1

:errSetEnv
echo.Error: setenv error.
exit /b 1
