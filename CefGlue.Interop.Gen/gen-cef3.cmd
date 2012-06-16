@echo off
c:\python27\python.exe -B cefglue_interop_gen.py --schema cef3 --cpp-header-dir include --cefglue-dir ..\CefGlue\ --no-backup
pause
