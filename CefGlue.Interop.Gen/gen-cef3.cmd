@echo off
:: Determine python path by inspecting registry: HKCU/HKLM SOFTWARE\Python\PythonCore\${PYTHON_VERSION}\InstallPath
:: And then try some default locations: c:\python27amd64 , c:\python27
c:\python27amd64\python.exe -B cefglue_interop_gen.py --schema cef3 --cpp-header-dir include --cefglue-dir ..\CefGlue\ --no-backup
pause
