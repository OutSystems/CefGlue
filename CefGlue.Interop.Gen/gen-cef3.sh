#!/bin/sh

# @echo off
/usr/bin/python3 -B cefglue_interop_gen.py --cpp-header-dir include --cefglue-dir ../CefGlue/ --no-backup
read -p "Press any key to resume ..."
