@echo off
:: Determine python path by inspecting registry: HKCU/HKLM SOFTWARE\Python\PythonCore\${PYTHON_VERSION}\InstallPath
:: And then try some default locations: c:\python27amd64 , c:\python27
python -B cefglue_interop_gen.py --cpp-header-dir include --cefglue-dir ..\CefGlue\ --no-backup
pause
