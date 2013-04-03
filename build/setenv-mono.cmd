@echo off
if exist "%ProgramFiles(x86)%\Mono-2.10.8\bin\setmonopath.bat" (
call "%ProgramFiles(x86)%\Mono-2.10.8\bin\setmonopath"
) else if exist "%ProgramFiles%\Mono-2.10.8\bin\setmonopath.bat" (
call "%ProgramFiles%\Mono-2.10.8\bin\setmonopath.bat"
) else (
exit /b 1
)
exit /b 0
