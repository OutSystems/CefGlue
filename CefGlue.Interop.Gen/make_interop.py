#
# Copyright (C) Xilium CefGlue Project
#
from cef_parser import *
import sys
import schema
import file_util
import os
from xml.dom.minidom import getDOMImplementation

#
# settings
#
indent = '    '

#
# Common
#
def get_func_parts(func, slot, is_global = False):
    virtual = isinstance(func, obj_function_virtual)
    capi_parts = func.get_capi_parts()

    csn_name = capi_parts['name']
    if not virtual:
        if is_global:
            if csn_name[0:4] == 'cef_':
                csn_name = csn_name[4:]
        else:
            csn_name = get_capi_name(func.get_name(), False, None)
            prefix = func.parent.get_capi_name()
            if prefix[-2:] == '_t':
                prefix = prefix[:-2]
            if prefix[0:3] == 'cef':
                subprefix = prefix[3:]
                pos = csn_name.find(subprefix)
                if pos >= 0:
                    csn_name = csn_name[0:pos]

    csn_args = []
    for carg in capi_parts['args']:
        # C Function prototype: some_name(void) is function without arguments for us
        if carg != "void":
          type = schema.c2cs_type( carg[:carg.rindex(' ')] )
          name = schema.quote_name( carg[carg.rindex(' ')+1:] )
          csn_args.append({'name' : name, 'type' : type})

    iname = ''
    if virtual:
        iname = schema.get_iname(func.parent)

    result = {
        'basefunc': False,
        'virtual': virtual,
        'obj': func,
        'slot': '%x' % slot,
        'name': func.get_name(),
        'field_name': '_' + func.get_capi_name(),
        'delegate_type': func.get_capi_name() + '_delegate',
        'delegate_slot': '_ds%x' % slot,

        'capi_name': capi_parts['name'],
        'capi_retval': capi_parts['retval'],
        'capi_args': capi_parts['args'],

        'csn_name': csn_name,
        'csn_retval': schema.c2cs_type( capi_parts['retval'] ),
        'csn_args': csn_args,
        'csn_entrypoint': capi_parts['name'],

        'csn_args_proto': ', '.join(map(lambda x: '%s %s' % (x['type'], x['name']), csn_args)),

        'iname': iname,
        }
    return result

def get_base_func(cls, slot, name, cname):
    cretval = ''
    if name == 'AddRef':
        cretval = 'void'
    elif name == 'Release':
        cretval = 'int'
    elif name == 'HasOneRef':
        cretval = 'int'
    elif name == 'HasAtLeastOneRef':
        cretval = 'int'
    elif name == 'Del':
        cretval = 'void'
    return {
            'basefunc': True,
            'virtual': True,
            'obj': None,
            'slot': '%x' % slot,
            'name': name,
            'field_name': '_base._%s' % cname,
            'delegate_type': '%s_delegate' % cname,
            'delegate_slot': '_ds%x' % slot,
            'capi_name': cname,
            'capi_retval': cretval,
            'capi_args': ['%s* self' % cls.get_capi_name()],

            'csn_name': cname,
            'csn_retval': cretval,
            'csn_args': [ { 'type': '%s*' % cls.get_capi_name(), 'name': 'self' } ],

            'csn_args_proto': '%s* self' % cls.get_capi_name(),

            'iname': schema.get_iname(cls),
        }

def get_base_funcs(cls):
    baseClassName = cls.get_parent_capi_name() # FIXME: It should get real base class, not direct parent.
    if baseClassName == "cef_base_t" or baseClassName == "cef_base_ref_counted_t":
        return [
            get_base_func(cls, 0, 'AddRef', 'add_ref'),
            get_base_func(cls, 1, 'Release', 'release'),
            get_base_func(cls, 2, 'HasOneRef', 'has_one_ref'),
            get_base_func(cls, 3, 'HasAtLeastOneRef', 'has_at_least_one_ref'),
            ]
    elif baseClassName == "cef_base_scoped_t":
        return [
            get_base_func(cls, 0, 'Del', 'del'),
            ]
    else:
        raise Exception("Unknown base class: %s." % baseClassName)

def make_file_header():
    return """//
// DO NOT MODIFY! THIS IS AUTOGENERATED FILE!
//
""";

def append_dllimport(result, func):
    result.append('// %(name)s' % func)
    result.append('[DllImport(%(nm_class)s.DllName, EntryPoint = "%(entrypoint)s", CallingConvention = %(nm_class)s.CEF_CALL)]' % { 'nm_class': schema.nm_class, 'entrypoint': func['csn_entrypoint'] })
    result.append('public static extern %(csn_retval)s %(csn_name)s(%(csn_args_proto)s);' % func)
    result.append('')

#
# Generating Introp/Classes.X/*.g.cs
#
def make_struct_file(cls):
    body = []
    body.append('using System;')
    body.append('using System.Diagnostics.CodeAnalysis;')
    body.append('using System.Runtime.InteropServices;')
    body.append('using System.Security;')
    body.append('');

    body.append('[StructLayout(LayoutKind.Sequential, Pack = %s)]' % schema.CEF_ALIGN)
    body.append('[SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]')
    body.append('internal unsafe struct %s' % schema.get_iname(cls))
    body.append('{')
    body.append( indent + ('\n' + indent + indent).join( make_struct_members(cls) ) )
    body.append('}')

    return make_file_header() + \
"""namespace %(namespace)s
{
%(body)s
}
""" % {
        'namespace': schema.interop_namespace,
        'body': indent + ('\n'+indent).join(body)
      }

def get_funcs(cls, base = True, inherited = True):
    funcs = []

    classes = []
    current_cls = cls
    while True:
        classes.append(current_cls)
        if base and is_base_class(current_cls.get_parent_name()):
            for func in get_base_funcs(current_cls):
                funcs.append( func )
        if not inherited:
            break
        current_cls = current_cls.parent.get_class(current_cls.parent_name)
        if current_cls is None:
            break

    i = len(funcs)
    while len(classes) > 0:
        current_cls = classes.pop()
        if current_cls is None:
            break
        for func in current_cls.get_virtual_funcs():
            funcs.append( get_func_parts(func, i) )
            i += 1

    return funcs

def get_top_base_class_name(cls):
    while cls is not None:
        if is_base_class(cls.parent_name):
            return cls.get_parent_capi_name()
        cls = cls.parent.get_class(cls.parent_name)
    return None

def make_struct_members(cls):
    result = []

    static_funcs = []
    funcs = get_funcs(cls)

    delegate_visibility = "internal"
    if schema.is_proxy(cls):
        delegate_visibility = "private"

    for func in cls.get_static_funcs():
        static_funcs.append( get_func_parts(func, 0) )

    parentClassName = get_top_base_class_name(cls)
    if (parentClassName != "cef_base_ref_counted_t"
        and parentClassName != "cef_base_scoped_t"):
        message = "Error: Generation for base class \"" + cls.get_parent_name() + "\" is not supported."
        raise Exception(message)

    result.append('internal {0} _base;'.format(parentClassName))
    for func in funcs:
        if not func['basefunc']:
            result.append('internal IntPtr %(field_name)s;' % func)
    result.append('')

    for func in static_funcs:
        append_dllimport(result, func)

    for func in funcs:
        postfixs = schema.get_platform_retval_postfixs(func['csn_retval'])
        for px in postfixs:
            func['px'] = px
            result.append('[UnmanagedFunctionPointer(%s)]' % schema.CEF_CALLBACK)
            result.append('#if !DEBUG')
            result.append('[SuppressUnmanagedCodeSecurity]')
            result.append('#endif')
            result.append(delegate_visibility + ' delegate %(csn_retval)s%(px)s %(delegate_type)s%(px)s(%(csn_args_proto)s);' % func)
            result.append('')

    for func in funcs:
        if schema.is_proxy(cls):
            postfixs = schema.get_platform_retval_postfixs(func['csn_retval'])
            for px in postfixs:
                func['px'] = px
                result.append('// %(name)s' % func)
                result.append('private static IntPtr _p%(slot)s%(px)s;' % func)
                result.append('private static %(delegate_type)s%(px)s _d%(slot)s%(px)s;' % func)
                result.append('')
                result.append('public static %(csn_retval)s%(px)s %(csn_name)s%(px)s(%(csn_args_proto)s)' % func)
                result.append('{')
                result.append('    %(delegate_type)s%(px)s d;' % func)
                result.append('    var p = self->%(field_name)s;' % func)
                result.append('    if (p == _p%(slot)s%(px)s) { d = _d%(slot)s%(px)s; }' % func)
                result.append('    else')
                result.append('    {')
                result.append('        d = (%(delegate_type)s%(px)s)Marshal.GetDelegateForFunctionPointer(p, typeof(%(delegate_type)s%(px)s));' % func)
                result.append('        if (_p%(slot)s%(px)s == IntPtr.Zero) { _d%(slot)s%(px)s = d; _p%(slot)s%(px)s = p; }' % func)
                result.append('    }')
                args = ', '.join(map(lambda x: x['name'], func['csn_args']))
                if func['csn_retval'] == 'void':
                    result.append('    d(%s);' % args)
                else:
                    result.append('    return d(%s);' % args)
                result.append('}')
                result.append('')

    if schema.is_handler(cls):
        iname = schema.get_iname(cls)
        result.append('private static int _sizeof;')
        result.append('')
        result.append('static %s()' % iname)
        result.append('{')
        result.append(indent + '_sizeof = Marshal.SizeOf(typeof(%s));' % iname)
        result.append('}')
        result.append('')

        result.append('internal static %s* Alloc()' % iname)
        result.append('{')
        result.append(indent + 'var ptr = (%s*)Marshal.AllocHGlobal(_sizeof);' % iname)
        result.append(indent + '*ptr = new %s();' % iname)
        result.append(indent + 'ptr->_base._size = (UIntPtr)_sizeof;')
        result.append(indent + 'return ptr;')
        result.append('}')
        result.append('')

        result.append('internal static void Free(%s* ptr)' % iname)
        result.append('{')
        result.append(indent + 'Marshal.FreeHGlobal((IntPtr)ptr);')
        result.append('}')
        result.append('')

    return result

#
# Generating Introp/libcef.g.cs
#
def make_libcef_file(header):
    result = []

    for func in header.get_funcs():
        append_dllimport(result, get_func_parts(func, 0, True))

    body = []
    body.append('using System;')
    body.append('using System.Runtime.InteropServices;')
    body.append('using System.Diagnostics.CodeAnalysis;')
    body.append('');

    body.append('internal static unsafe partial class %s' % schema.nm_class)
    body.append('{')
    body.append( indent + ('\n' + indent + indent).join( result ) )
    body.append('}')

    return make_file_header() + \
"""namespace %(namespace)s
{
%(body)s
}
""" % {
        'namespace': schema.interop_namespace,
        'body': indent + ('\n'+indent).join(body)
      }

#
# Generating C# wrappers
#
def make_wrapper_g_file(cls):
    body = []

    body.append('using System;')
    body.append('using System.Collections.Generic;')
    body.append('using System.Diagnostics;')
    body.append('using System.Runtime.InteropServices;')
    # body.append('using System.Diagnostics.CodeAnalysis;')
    body.append('using System.Threading;')
    body.append('using %s;' % schema.interop_namespace)
    body.append('')

    for line in schema.get_overview(cls):
        body.append('// %s' % line)

    isRefCountedImpl = cls.get_parent_capi_name() == "cef_base_ref_counted_t"
    isScopedImpl = cls.get_parent_capi_name() == "cef_base_scoped_t"
    maybeSealedModifier = "sealed "

    if schema.is_abstract(cls):
        maybeSealedModifier = ""

    if schema.is_proxy(cls):
        proxyBase = None
        if isRefCountedImpl:
            proxyBase = " : IDisposable"
        elif isScopedImpl:
            proxyBase = ""
        elif cls.get_parent_capi_name() == "cef_preference_manager_t":
            proxyBase = " : CefPreferenceManager"
        else:
            raise Exception("Unknown base class type.")
        body.append(('public ' + maybeSealedModifier + 'unsafe partial class %s' + proxyBase) % schema.cpp2csname(cls.get_name()))
        body.append('{')
        body.append( indent + ('\n' + indent + indent).join( make_proxy_g_body(cls) ) )
        body.append('}')

    if schema.is_handler(cls):
        body.append('public abstract unsafe partial class %s' % schema.cpp2csname(cls.get_name()))
        body.append('{')
        body.append( indent + ('\n' + indent + indent).join( make_handler_g_body(cls) ) )
        body.append('}')

    return make_file_header() + \
"""namespace %(namespace)s
{
%(body)s
}
""" % {
        'namespace': schema.namespace,
        'body': indent + ('\n'+indent).join(body)
      }

#
# make proxy body
#
def make_proxy_g_body(cls):
    csname = schema.cpp2csname(cls.get_name())
    iname = schema.get_iname(cls)

    isRefCountedImpl = cls.get_parent_capi_name() == "cef_base_ref_counted_t"
    isScopedImpl = cls.get_parent_capi_name() == "cef_base_scoped_t"
    isImpl = isRefCountedImpl or isScopedImpl
    isRefCounted = get_top_base_class_name(cls) == "cef_base_ref_counted_t"

    selfAccessor = "_self"
    if not isImpl:
        selfAccessor = "(%s*)_self" % iname

    maybeNewKeyword = ""
    if not isImpl:
        maybeNewKeyword = "new "

    privateOrProtected = "private"
    if schema.is_abstract(cls):
        privateOrProtected = "private protected"

    result = []

    # result.append('#if DEBUG')
    # result.append('private static int _objCt;')
    # result.append('internal static int ObjCt { get { return _objCt; } }')
    # result.append('#endif')
    # result.append('')

    # static methods
    result.append('internal static %(csname)s FromNative(%(iname)s* ptr)' % { 'csname' : csname, 'iname' : iname })
    result.append('{')
    result.append(indent + 'return new %s(ptr);' % csname)
    result.append('}')
    result.append('')

    result.append('internal static %(csname)s FromNativeOrNull(%(iname)s* ptr)' % { 'csname' : csname, 'iname' : iname })
    result.append('{')
    result.append(indent + 'if (ptr == null) return null;')
    result.append(indent + 'return new %s(ptr);' % csname)
    result.append('}')
    result.append('')

    # private fields
    if isImpl:
      result.append(privateOrProtected + ' %s* _self;' % iname)
      result.append(privateOrProtected + ' int _disposed = 0;')
      result.append('')

    isRefCounted = cls.get_parent_capi_name() == "cef_base_ref_counted_t"
    isScoped = cls.get_parent_capi_name() == "cef_base_scoped_t"

    # ctor
    result.append(privateOrProtected + ' %(csname)s(%(iname)s* ptr)' % { 'csname' : csname, 'iname' : iname })
    if isImpl:
        result.append('{')
        result.append(indent + 'if (ptr == null) throw new ArgumentNullException("ptr");')
        result.append(indent + '_self = ptr;')
        if isRefCountedImpl:
            result.append(indent + 'CefObjectTracker.Track(this);')
        #
        # todo: diagnostics code: Interlocked.Increment(ref _objCt);
        #
        result.append('}')
    else:
        base_cls = cls.parent.get_class(cls.parent_name)
        base_ptr_type = schema.get_iname(base_cls)
        result.append('    : base((%s*)ptr) {}' % base_ptr_type)
    result.append('')

    if cls.get_parent_capi_name() == "cef_preference_manager_t":
        # no-op
        pass
    elif isRefCountedImpl:
        # disposable
        result.append('~%s()' % csname)
        result.append('{')
        result.append(indent + 'if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)')
        result.append(indent + '{')
        result.append(indent + indent + 'Release();')
        result.append(indent + indent + '_self = null;')
        result.append(indent + '}')
        result.append('}')
        result.append('')

        result.append('public void Dispose()')
        result.append('{')
        result.append(indent + 'if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)')
        result.append(indent + '{')
        result.append(indent + indent + 'Release();')
        result.append(indent + indent + '_self = null;')
        result.append(indent + '}')
        result.append(indent + 'CefObjectTracker.Untrack(this);')
        result.append(indent + 'GC.SuppressFinalize(this);')
        result.append('}')
        result.append('')

        result.append('internal void AddRef()')
        result.append('{')
        result.append(indent + '%(iname)s.add_ref(_self);' % { 'iname': iname })
        result.append('}')
        result.append('')

        result.append('internal bool Release()')
        result.append('{')
        result.append(indent + 'return %(iname)s.release(_self) != 0;' % { 'iname': iname })
        result.append('}')
        result.append('')

        result.append('internal bool HasOneRef')
        result.append('{')
        result.append(indent + 'get { return %(iname)s.has_one_ref(_self) != 0; }' % { 'iname': iname })
        result.append('}')
        result.append('')

        result.append('internal bool HasAtLeastOneRef')
        result.append('{')
        result.append(indent + 'get { return %(iname)s.has_at_least_one_ref(_self) != 0; }' % { 'iname': iname })
        result.append('}')
        result.append('')
    elif isScopedImpl:
        result.append("// FIXME: code for CefBaseScoped is not generated")
        result.append("")
    else:
        raise Exception("Unsupported base class name.")

    # TODO: use it only if it is really necessary!
    # result.append('internal %(iname)s* Pointer' % { 'iname' : iname })
    # result.append('{')
    # result.append(indent + 'get { return _self; }')
    # result.append('}')
    # result.append('')

    result.append('internal %(maybeNewKeyword)s%(iname)s* ToNative()' % { 'iname' : iname, 'maybeNewKeyword': maybeNewKeyword })
    result.append('{')
    if isRefCounted:
        result.append(indent + 'AddRef();')
    result.append(indent + 'return ' + selfAccessor + ';')
    result.append('}')

    return result

#
# make handler body
#
def make_handler_g_body(cls):
    csname = schema.cpp2csname(cls.get_name())
    iname = schema.get_iname(cls)

    funcs = get_funcs(cls)

    result = []

    # this dictionary used to keep object alive even when we doesn't reference object directly, but it can be referenced only from native side
    result.append('private static Dictionary<IntPtr, %(csname)s> _roots = new Dictionary<IntPtr, %(csname)s>();' % { 'csname' : csname })
    result.append('')

    result.append('private int _refct;')
    result.append('private %s* _self;' % iname)
    # result.append('private bool _disposed;')
    result.append('')

    result.append('protected object SyncRoot { get { return this; } }')
    result.append('')

    if schema.is_reversible(cls):
        result.append('internal static %s FromNativeOrNull(%s* ptr)' % (csname, iname))
        result.append('{')
        result.append(indent + '%s value = null;' % csname)
        result.append(indent + 'bool found;')
        result.append(indent + 'lock (_roots)')
        result.append(indent + '{')
        result.append(indent + indent + 'found = _roots.TryGetValue((IntPtr)ptr, out value);')
        result.append(indent + indent + '// as we\'re getting the ref from the outside, it\'s our responsibility to decrement it')
        result.append(indent + indent + 'value.release(ptr);')
        result.append(indent + '}')
        result.append(indent + 'return found ? value : null;')
        result.append('}')
        result.append('')

        result.append('internal static %s FromNative(%s* ptr)' % (csname, iname))
        result.append('{')
        result.append(indent + 'var value = FromNativeOrNull(ptr);')
        result.append(indent + 'if (value == null) throw ExceptionBuilder.ObjectNotFound();')
        result.append(indent + 'return value;')
        result.append('}')
        result.append('')

    for func in funcs:
        result.append('private %(iname)s.%(delegate_type)s %(delegate_slot)s;' % func)
    result.append('')

    # ctor
    result.append('protected %s()' % csname)
    result.append('{')
    result.append(indent + '_self = %s.Alloc();' % iname)
    result.append('')
    for func in funcs:
        result.append(indent + '%(delegate_slot)s = new %(iname)s.%(delegate_type)s(%(csn_name)s);' % func)
        result.append(indent + '_self->%(field_name)s = Marshal.GetFunctionPointerForDelegate(%(delegate_slot)s);' % func)
    result.append('}')
    result.append('')

    # finalizer & dispose
    result.append('~%s()' % csname)
    result.append('{')
    result.append(indent + 'Dispose(false);')
    result.append('}')
    result.append('')

    if schema.is_autodispose(cls):
        result.append('private void Dispose()')
        result.append('{')
        result.append(indent + 'Dispose(true);')
        result.append(indent + 'GC.SuppressFinalize(this);')
        result.append('}')
        result.append('')

    result.append('protected virtual void Dispose(bool disposing)')
    result.append('{')
    # result.append(indent + '_disposed = true;')
    result.append(indent + 'if (_self != null)')
    result.append(indent + '{')
    result.append(indent + indent + '%s.Free(_self);' % iname)
    result.append(indent + indent + '_self = null;')
    result.append(indent + '}')
    result.append('}')
    result.append('')

    # todo: this methods must throw exception if object already disposed
    # todo: verify self pointer in debug
    result.append('private void add_ref(%s* self)' % iname)
    result.append('{')
    result.append(indent + 'lock (SyncRoot)')
    result.append(indent + '{')
    result.append(indent + indent + 'var result = ++_refct;')
    result.append(indent + indent + 'if (result == 1)')
    result.append(indent + indent + '{')
    result.append(indent + indent + indent + 'lock (_roots) { _roots.Add((IntPtr)_self, this); }')
    result.append(indent + indent + '}')
    result.append(indent + '}')
    result.append('}')
    result.append('')

    result.append('private int release(%s* self)' % iname)
    result.append('{')
    result.append(indent + 'lock (SyncRoot)')
    result.append(indent + '{')
    result.append(indent + indent + 'var result = --_refct;')
    result.append(indent + indent + 'if (result == 0)')
    result.append(indent + indent + '{')
    result.append(indent + indent + indent + 'lock (_roots) { _roots.Remove((IntPtr)_self); }')
    if schema.is_autodispose(cls):
        result.append(indent + indent + indent + 'Dispose();')
    result.append(indent + indent + indent + 'return 1;')
    result.append(indent + indent + '}')
    result.append(indent + indent + 'return 0;')
    result.append(indent + '}')
    result.append('}')
    result.append('')

    result.append('private int has_one_ref(%s* self)' % iname)
    result.append('{')
    result.append(indent + 'lock (SyncRoot) { return _refct == 1 ? 1 : 0; }')
    result.append('}')
    result.append('')

    result.append('private int has_at_least_one_ref(%s* self)' % iname)
    result.append('{')
    result.append(indent + 'lock (SyncRoot) { return _refct != 0 ? 1 : 0; }')
    result.append('}')
    result.append('')

    result.append('internal %s* ToNative()' % iname)
    result.append('{')
    result.append(indent + 'add_ref(_self);')
    result.append(indent + 'return _self;')
    result.append('}')
    result.append('')

    result.append('[Conditional("DEBUG")]')
    result.append('private void CheckSelf(%s* self)' % iname)
    result.append('{')
    result.append(indent + 'if (_self != self) throw ExceptionBuilder.InvalidSelfReference();')
    result.append('}')
    result.append('')

    return result

#
# Generating impl templates
#
def make_impl_tmpl_file(cls):
    body = []

    body.append('using System;')
    body.append('using System.Collections.Generic;')
    body.append('using System.Diagnostics;')
    body.append('using System.Runtime.InteropServices;')
    # body.append('using System.Diagnostics.CodeAnalysis;')
    body.append('using %s;' % schema.interop_namespace)
    body.append('')

    append_xmldoc(body, cls.get_comment())

    if schema.is_proxy(cls):
        body.append('public sealed unsafe partial class %s' % schema.cpp2csname(cls.get_name()))
        body.append('{')
        body.append( indent + ('\n' + indent + indent).join( make_proxy_impl_tmpl_body(cls) ) )
        body.append('}')

    if schema.is_handler(cls):
        body.append('public abstract unsafe partial class %s' % schema.cpp2csname(cls.get_name()))
        body.append('{')
        body.append( indent + ('\n' + indent + indent).join( make_handler_impl_tmpl_body(cls) ) )
        body.append('}')

    return \
"""namespace %(namespace)s
{
%(body)s
}
""" % {
        'namespace': schema.namespace,
        'body': indent + ('\n'+indent).join(body)
      }



#
# make proxy impl tmpl body
#
def make_proxy_impl_tmpl_body(cls):
    csname = schema.cpp2csname(cls.get_name())
    iname = schema.get_iname(cls)
    funcs = get_funcs(cls, False, False)
    static_funcs = []
    for func in cls.get_static_funcs():
        static_funcs.append( get_func_parts(func, 0) )

    result = []

    for func in static_funcs:
        append_xmldoc(result, func['obj'].get_comment())
        result.append('public static %(csn_retval)s %(name)s(%(csn_args_proto)s)' % func)
        result.append('{')
        result.append('    throw new NotImplementedException(); // TODO: %(csname)s.%(name)s' % { 'csname': csname, 'name' : func['name'] })
        result.append('}')
        result.append('')

    for func in funcs:
        name = func['name']
        retval = func['csn_retval']
        args = func['csn_args_proto']
        if func['csn_args'][0]['name'] == 'self':
            args = ', '.join(map(lambda x: '%s %s' % (x['type'], x['name']), func['csn_args'][1:]))

        append_xmldoc(result, func['obj'].get_comment())

        result.append('public %s %s(%s)' % (retval, name, args))
        result.append('{')
        result.append('    throw new NotImplementedException(); // TODO: %(csname)s.%(name)s' % { 'csname': csname, 'name' : func['name'] })
        result.append('}')
        result.append('')

    return result

#
# make handler impl tmpl body
#
def make_handler_impl_tmpl_body(cls):
    csname = schema.cpp2csname(cls.get_name())
    iname = schema.get_iname(cls)
    funcs = get_funcs(cls, False)
    static_funcs = []
    for func in cls.get_static_funcs():
        static_funcs.append( get_func_parts(func, 0) )

    result = []

    for func in static_funcs:
        append_xmldoc(result, func['obj'].get_comment())
        result.append('public static %(csn_retval)s %(name)s(%(csn_args_proto)s)' % func)
        result.append('{')
        result.append('    throw new NotImplementedException(); // TODO: %(csname)s.%(name)s' % { 'csname': csname, 'name' : func['name'] })
        result.append('}')
        result.append('')

    for func in funcs:
        result.append('private %s %s(%s)' % (func['csn_retval'], func['csn_name'], func['csn_args_proto']))
        result.append('{')
        result.append(indent + 'CheckSelf(self);')
        result.append(indent + 'throw new NotImplementedException(); // TODO: %(csname)s.%(name)s' % { 'csname': csname, 'name' : func['name'] })
        result.append('}')
        result.append('')

        name = func['name']
        retval = func['csn_retval']
        args = func['csn_args_proto']
        if func['csn_args'][0]['name'] == 'self':
            args = ', '.join(map(lambda x: '%s %s' % (x['type'], x['name']), func['csn_args'][1:]))

        append_xmldoc(result, func['obj'].get_comment())

        result.append('// protected abstract %s %s(%s);' % (retval, name, args))
        result.append('')

    return result


def append_xmldoc(result, lines):
    result.append('/// <summary>')
    for line in lines:
        if line != '/' and not (line is None):
            line = line.strip()
            if line != '':
                temp = line.strip()
                temp = temp.replace("<", "&lt;")
                temp = temp.replace(">", "&gt;")
                result.append('/// %s' % temp)
    result.append('/// </summary>')
    return


def make_version_cs(content, api_hash_content, api_versions_content):
    result = []

    result.append('public const string CEF_VERSION = %s;' % __get_version_constant(content, "CEF_VERSION"))
    result.append('public const int CEF_VERSION_MAJOR = %s;' % __get_version_constant(content, "CEF_VERSION_MAJOR"))
    result.append('public const int CEF_COMMIT_NUMBER = %s;' % __get_version_constant(content, "CEF_COMMIT_NUMBER"))
    result.append('public const string CEF_COMMIT_HASH = %s;' % __get_version_constant(content, "CEF_COMMIT_HASH"))
    result.append("");

    result.append('public const int CHROME_VERSION_MAJOR = %s;' % __get_version_constant(content, "CHROME_VERSION_MAJOR"))
    result.append('public const int CHROME_VERSION_MINOR = %s;' % __get_version_constant(content, "CHROME_VERSION_MINOR"))
    result.append('public const int CHROME_VERSION_BUILD = %s;' % __get_version_constant(content, "CHROME_VERSION_BUILD"))
    result.append('public const int CHROME_VERSION_PATCH = %s;' % __get_version_constant(content, "CHROME_VERSION_PATCH"))
    result.append("");

    api_details = get_cef_api_details(api_versions_content)
    result.append('public const int CEF_API_VERSION = %s;' % api_details["API_VERSION"])
    result.append('public const string CEF_API_HASH_PLATFORM_WIN = "%s";' % api_details["OS_WIN"])
    result.append('public const string CEF_API_HASH_PLATFORM_MACOS = "%s";' % api_details["OS_MAC"])
    result.append('public const string CEF_API_HASH_PLATFORM_LINUX = "%s";' % api_details["OS_LINUX"])

    body = []
    body.append('using System;')
    body.append('using System.Runtime.InteropServices;')
    body.append('using System.Diagnostics.CodeAnalysis;')
    body.append('');

    body.append('internal static unsafe partial class %s' % schema.nm_class)
    body.append('{')
    body.append( indent + ('\n' + indent + indent).join( result ) )
    body.append('}')

    return make_file_header() + \
"""namespace %(namespace)s
{
%(body)s
}
""" % {
        'namespace': schema.interop_namespace,
        'body': indent + ('\n'+indent).join(body)
      }

def get_cef_api_details(content):
    lines = content.splitlines()
    
    # Find the last API version
    match_version = re.search(r"#define\s+CEF_API_VERSION_LAST\s+(CEF_API_VERSION_\d+)", content)
    if not match_version:
        raise ValueError("CEF_API_VERSION_LAST not found in the file")
    
    last_version = match_version.group(1)
    last_version_number = last_version.split('_')[-1]
    
    # Extract OS-specific hashes
    details = { "API_VERSION": last_version_number }
    current_os = None
    
    # Iterate through lines and capture hashes for each OS
    for line in lines:
        line = line.strip()
        
        # Detect OS
        if "#if defined(OS_WIN)" in line:
            current_os = "OS_WIN"
        elif "#elif defined(OS_MAC)" in line:
            current_os = "OS_MAC"
        elif "#elif defined(OS_LINUX)" in line:
            current_os = "OS_LINUX"
        
        # Capture hash value for the detected OS
        match = re.match(rf"#define\s+CEF_API_HASH_{last_version_number}\s+\"(\w+)\"", line)
        if match and current_os:
            details[current_os] = match.group(1)
            current_os = None  # Reset for the next block
    
    return details

def __get_version_constant(content, name, platform = None):
    if platform is None:
        m = re.search(r'^#define\s+' + name + r'\s+(.*?)\n', content, re.MULTILINE)
        if m is None:
            raise Exception('Could not find ' + name + ' constant.');
        value = m.group(1)
    else:
        m = re.search(r'\n#e?l?if defined\(OS_' + platform + r'\)\n+#define\s+' + name + r'\s+(.*?)\n', content, re.DOTALL)
        if m is None:
            raise Exception('Could not find ' + name + ' constant.');
        value = m.group(1)
    return value

#
# Main
#
def write_interop(header, filepath, backup, schema_name, cppheaderdir):
    writect = 0

    project_props_filename = "CefGlue.g.props"
    project_props_compile_items = []

    schema.load(schema_name, header)

    # validate: class role must be defined for all header classes
    for cls in header.get_classes():
        if not schema.is_handler(cls) and not schema.is_proxy(cls):
            msg = 'Class role must be defined. Class name %s.' % cls.get_name()
            sys.stdout.write('ERROR! %s\n' % msg)
            raise Exception(msg)

    # structs
    for cls in header.get_classes():
        content = make_struct_file(cls)
        writect += update_file(project_props_compile_items, filepath + '/' + schema.struct_path, cls.get_capi_name() + ".g.cs", content, backup)

    # libcef.g.cs
    writect += update_file(project_props_compile_items, filepath + '/' + schema.libcef_path, schema.libcef_filename, make_libcef_file(header), backup)

    # wrapper
    for cls in header.get_classes():
        content = make_wrapper_g_file(cls)
        writect += update_file(project_props_compile_items, filepath + '/' + schema.wrapper_g_path, schema.cpp2csname(cls.get_name()) + ".g.cs", content, backup)

    # userdata
    userdatacls = obj_class(header, 'CefUserData', '', 'CefUserData', 'CefBaseRefCounted', '', '', '', [])
    content = make_struct_file(userdatacls)
    writect += update_file(project_props_compile_items, filepath + '/' + schema.struct_path, userdatacls.get_capi_name() + ".g.cs", content, backup)
    content = make_wrapper_g_file(userdatacls)
    writect += update_file(project_props_compile_items, filepath + '/' + schema.wrapper_g_path, schema.cpp2csname(userdatacls.get_name()) + ".g.cs", content, backup)

    # impl template
    for cls in header.get_classes():
        content = make_impl_tmpl_file(cls)
        tmplpath = schema.handler_tmpl_path
        if schema.is_proxy(cls):
            tmplpath = schema.proxy_tmpl_path
        writect += update_file(None, './' + tmplpath, schema.cpp2csname(cls.get_name()) + ".tmpl.g.cs", content, backup)

    # process cef_version.h and cef_api_hash.h
    content = make_version_cs(read_file(cppheaderdir + '/' + 'cef_version.h'), read_file(cppheaderdir + '/' + 'cef_api_hash.h'), read_file(cppheaderdir + '/' + 'cef_api_versions.h'))
    writect += update_file(project_props_compile_items, filepath + '/' + schema.libcef_path, schema.libcef_version_filename, content, backup)

    # make project props file
    content = make_props_file(project_props_compile_items, filepath)
    writect += update_file(None, filepath, project_props_filename, content, backup)

    return writect

#
# Generate "CefGlue.Generated.props" msbuild file.
#
def make_props_file(filelist, basedir):
    document = getDOMImplementation().createDocument(None, None, None)
    documentElement = document.createElementNS(None, "Project")
    document.appendChild(documentElement)

    itemGroupElement = document.createElementNS(None, "ItemGroup")
    documentElement.appendChild(itemGroupElement)
    for x in sorted([os.path.relpath(x, basedir) for x in filelist]):
        compileElement = document.createElementNS(None, "Compile")
        compileElement.setAttribute("Include", x)
        itemGroupElement.appendChild(compileElement)

    return documentElement.toprettyxml(indent = "    ", newl = "\n", encoding = None)

    # return "\n".join(list1)
    # return "\n".join([Path(x).relative_to(basedir).str() for x in filelist])

#
# Utils
#
def update_file(filelist, dir, filename, content, backup):
    if not os.path.isdir(dir):
        os.makedirs(dir)

    sys.stdout.write(filename + "... ")
    filename = dir + "/" + filename

    if filelist is not None:
        filelist.append(filename)

    if path_exists(filename):
        oldcontent = read_file(filename)
    else:
        oldcontent = ''

    if content != oldcontent:
        if backup and oldcontent != '':
            backup_file(filename)
        write_file(filename, content)
        sys.stdout.write("updated.\n")
        return 1

    sys.stdout.write("up-to-date.\n")
    return 0
