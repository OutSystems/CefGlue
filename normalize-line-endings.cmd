@echo off
"%~dp0\tools\xifixeol" "%~dp0\" *.cs lf utf8
"%~dp0\tools\xifixeol" "%~dp0\" *.csproj lf utf8
"%~dp0\tools\xifixeol" "%~dp0\" *.h lf ascii
"%~dp0\tools\xifixeol" "%~dp0\" *.py lf ascii
