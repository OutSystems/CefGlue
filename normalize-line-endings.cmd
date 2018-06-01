@echo off
"%~dp0\tools\xifixeol" "%~dp0\" *.cs crlf utf8
"%~dp0\tools\xifixeol" "%~dp0\" *.csproj crlf utf8
"%~dp0\tools\xifixeol" "%~dp0\" *.h crlf ascii
"%~dp0\tools\xifixeol" "%~dp0\" *.py crlf ascii
