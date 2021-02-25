# Copyright (c) 2009 The Chromium Embedded Framework Authors. All rights
# reserved. Use of this source code is governed by a BSD-style license that
# can be found in the LICENSE file.

import sys
from cef_parser import *
from make_interop import write_interop
from optparse import OptionParser


# cannot be loaded as a module
if __name__ != "__main__":
    sys.stderr.write('This file cannot be loaded as a module!')
    sys.exit()
    

# parse command-line options
disc = """
This utility generates files for the CEF C++ to C API translation layer.
"""

parser = OptionParser(description=disc)
parser.add_option('--cpp-header-dir', dest='cppheaderdir', metavar='DIR',
                  help='input directory for C++ header files [required]')
parser.add_option('--cefglue-dir', dest='cefgluedir', metavar='DIR',
                  help='output directory for cefglue interop files')
parser.add_option('--no-backup',
                  action='store_true', dest='nobackup', default=False,
                  help='do not create a backup of modified files')
parser.add_option('-q', '--quiet',
                  action='store_true', dest='quiet', default=False,
                  help='do not output detailed status information')
(options, args) = parser.parse_args()

# required options: cppheader, cef1 or cef3
if options.cppheaderdir is None or options.cefgluedir is None:
    parser.print_help(sys.stdout)
    sys.exit()

# make sure the header exists
if not path_exists(options.cppheaderdir):
    sys.stderr.write('File '+options.cppheaderdir+' does not exist.')
    sys.exit()

# create the header object
if not options.quiet:
    sys.stdout.write('Parsing C++ headers from '+options.cppheaderdir+'...\n')
header = obj_header()
excluded_files = ['cef_application_mac.h', 'cef_version.h']
excluded_files += ['cef_thread.h', 'cef_waitable_event.h']
header.add_directory(options.cppheaderdir, excluded_files)

writect = 0

if not options.cefgluedir is None:
    # output cefglue interop
    if not options.quiet:
        sys.stdout.write('Generating CefGlue interop files...\n')
    writect += write_interop(header, options.cefgluedir, not options.nobackup, 'cef3', options.cppheaderdir)

if not options.quiet:
    sys.stdout.write('Done - Wrote '+str(writect)+' files.\n')


