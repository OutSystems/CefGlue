@echo off & setlocal enableextensions enabledelayedexpansion

call "%~dp0\build\setenv-mono"
if "%errorlevel%" neq "0" goto errSetEnv

:: build
call xbuild %* "build.proj"
if "%errorlevel%" neq "0" goto errBuild
goto :eof

:: errors
:errBuild
echo.Error: build error.
exit /b 1

:errSetEnv
echo.Error: setenv-mono reports error.
exit /b 1
