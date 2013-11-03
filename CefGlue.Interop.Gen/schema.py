#
# Copyright (C) Xilium CefGlue Project
#
import sys

#
# consts
#
ROLE_HANDLER = 1
ROLE_PROXY = 2

#
# common settings
#
namespace = "Xilium.CefGlue"
interop_namespace = namespace + ".Interop"
nm_class = "libcef"
CEF_ALIGN = nm_class + ".ALIGN"
CEF_CALL = nm_class + ".CEF_CALL"
CEF_CALLBACK = nm_class + ".CEF_CALLBACK"

struct_path = 'Interop/Classes.g'
libcef_path = 'Interop/'
libcef_filename = 'libcef.g.cs'
libcef_version_filename = 'version.g.cs'
wrapper_g_path = 'Classes.g'
handler_tmpl_path = 'Classes.Handlers.tmpl'
proxy_tmpl_path = 'Classes.Proxies.tmpl'

c2cs_types = {
    'void': 'void',
    'char': 'byte',
    'int': 'int',
    'int32': 'int',
    'uint32': 'uint',
    'int64': 'long',
    'uint64': 'ulong',
    'float': 'float',
    'double': 'double',
    'size_t': 'UIntPtr',

    'cef_string_t': 'cef_string_t',
    'cef_string_userfree_t': 'cef_string_userfree*',

    'cef_string_list_t': 'cef_string_list*',
    'cef_string_map_t': 'cef_string_map*',
    'cef_string_multimap_t': 'cef_string_multimap*',

    'time_t': 'int',

    'cef_window_handle_t': 'IntPtr',
    'cef_event_handle_t': 'IntPtr',
    'cef_cursor_handle_t': 'IntPtr',

    'cef_base_t': 'cef_base_t',

    # structs
    'cef_urlparts_t': 'cef_urlparts_t',
    'cef_proxy_info_t': 'cef_proxy_info_t',
    'cef_popup_features_t': 'cef_popup_features_t',
    'cef_browser_settings_t': 'cef_browser_settings_t',
    'cef_time_t': 'cef_time_t',
    'cef_cookie_t': 'cef_cookie_t',
    'cef_settings_t': 'cef_settings_t',
    'cef_key_event_t': 'cef_key_event_t',
    'cef_geoposition_t': 'cef_geoposition_t',
    'cef_rect_t': 'cef_rect_t',
    'cef_mouse_event_t': 'cef_mouse_event_t',
    'cef_screen_info_t': 'cef_screen_info_t',

    # platform dependend structs
    'cef_main_args_t': 'cef_main_args_t',
    'cef_window_info_t': 'cef_window_info_t',

    'cef_text_input_context_t': 'IntPtr'
    }

c2cs_platform_retval = {
	# generates multiple delegates/methods if return value is platform specific
    # 'cef_time_t': ['_other', '_mac']
    }

c2cs_enumtypes = {
    'cef_errorcode_t': 'CefErrorCode',
    'cef_log_severity_t': 'CefLogSeverity',
    'cef_postdataelement_type_t': 'CefPostDataElementType',
    'cef_process_id_t': 'CefProcessId',
    'cef_proxy_type_t': 'CefProxyType',
    'cef_handler_statustype_t': 'CefStatusMessageType',
    'cef_storage_type_t': 'CefStorageType',
    'cef_thread_id_t': 'CefThreadId',
    'cef_v8_accesscontrol_t': 'CefV8AccessControl',
    'cef_v8_propertyattribute_t': 'CefV8PropertyAttribute',
    'cef_value_type_t': 'CefValueType',
    'cef_xml_encoding_type_t': 'CefXmlEncoding',
    'cef_xml_node_type_t': 'CefXmlNodeType',
    'cef_event_flags_t': 'CefEventFlags',
    'cef_context_menu_type_flags_t': 'CefContextMenuTypeFlags',
    'cef_context_menu_media_type_t': 'CefContextMenuMediaType',
    'cef_context_menu_media_state_flags_t': 'CefContextMenuMediaStateFlags',
    'cef_context_menu_edit_state_flags_t': 'CefContextMenuEditStateFlags',
    'cef_dom_document_type_t': 'CefDomDocumentType',
    'cef_dom_node_type_t': 'CefDomNodeType',
    'cef_dom_event_category_t': 'CefDomEventCategory',
    'cef_dom_event_phase_t': 'CefDomEventPhase',
    'cef_jsdialog_type_t': 'CefJSDialogType',
    'cef_menu_item_type_t': 'CefMenuItemType',
    'cef_event_flags_t': 'CefEventFlags',
    'cef_focus_source_t': 'CefFocusSource',
    'cef_urlrequest_flags_t': 'CefUrlRequestOptions',
    'cef_urlrequest_status_t': 'CefUrlRequestStatus',
    'cef_termination_status_t': 'CefTerminationStatus',
    'cef_path_key_t': 'CefPathKey',
    'cef_file_dialog_mode_t': 'CefFileDialogMode',
    'cef_geoposition_error_code_t': 'CefGeopositionErrorCode',
    'cef_navigation_type_t': 'CefNavigationType',
    'cef_mouse_button_type_t': 'CefMouseButtonType',
    'cef_paint_element_type_t': 'CefPaintElementType',
    'cef_drag_operations_mask_t': 'CefDragOperationsMask',
    'cef_resource_type_t': 'CefResourceType',
    'cef_transition_type_t': 'CefTransitionType'
    }

c2cs_structtypes = { }

cs_keywords = [ 'object', 'string', 'checked', 'event', 'params' ]

classdef = { }



def load(schema_name, header):
    exec("from schema_%s import *" % schema_name) in globals()

    # build C API type name to C# type name map (struct types)
    c2cs_structtypes.clear();
    for cls in header.get_classes():
        c2cs_structtypes[cls.get_capi_name()] = get_iname(cls)
        # sys.stdout.write('struct _%s*' % cls.get_capi_name() + ' -> ' + '%s*' % cls.get_capi_name() + '\n' )

    return


def is_handler(cls):
    if classdef.has_key(cls.get_name()):
        return classdef[cls.get_name()]['role'] & ROLE_HANDLER == ROLE_HANDLER
    return False

def is_proxy(cls):
    if classdef.has_key(cls.get_name()):
        return classdef[cls.get_name()]['role'] & ROLE_PROXY == ROLE_PROXY
    return False

def is_reversible(cls):
    name = cls.get_name()
    if classdef.has_key(name):
        cdef = classdef[name]
        if cdef.has_key('reversible'):
            return cdef['reversible']
    return False

def isref(ctype):
    return ctype.endswith('*')

def is_platform_retval(csntype):
    return c2cs_platform_retval.has_key(csntype)

def get_platform_retval_postfixs(csntype):
    if c2cs_platform_retval.has_key(csntype):
        return c2cs_platform_retval[csntype]
    return ['']

def quote_name(name):
    if name in cs_keywords:
        return '@' + name
    return name

def c2cs_type(ctype):
    warn_ctype = ctype
    ptrs = 0
    ret = ""
    ctype = ctype.strip()

    while isref(ctype):
        ptrs += 1
        ctype = ctype[:-1].strip()
        if ctype.endswith('const'):
            ctype = ctype[:-5].strip()

    if ctype.startswith('const'):
        ctype = ctype[5:].strip()

    if ctype.startswith('struct'):
        ctype = ctype[6:].strip()

    if ctype.startswith('enum'):
        ctype = ctype[4:].strip()

    if ctype.startswith('_'):
        ctype = ctype[1:].strip()

    if c2cs_types.has_key(ctype):
        ret = c2cs_types[ctype]
    elif c2cs_enumtypes.has_key(ctype):
        ret = c2cs_enumtypes[ctype]
    elif c2cs_structtypes.has_key(ctype):
        ret = c2cs_structtypes[ctype]
    else:
        sys.stdout.write('Warning! C type "%s" is not mapped to C# type (processed to "%s").\n' % (warn_ctype, ctype))
        ret = ctype

    while ptrs > 0:
        ret += "*"
        ptrs -= 1

    #if ctype.endswith('*') and c2cs_types.has_key(ctype[:-1]):
    #    return c2cs_types[ctype[:-1]] + '*'

    #if c2cs_enumtypes.has_key(ctype):
    #    return c2cs_enumtypes[ctype]

    #if ctype.startswith('enum '):
    #    if c2cs_enumtypes.has_key(ctype[5:]):
    #        return c2cs_enumtypes[ctype[5:]]

    #if c2cs_types.has_key(ctype):
    #    return c2cs_types[ctype]

    #if c2cs_structtypes.has_key(ctype):
    #    return c2cs_structtypes[ctype]

    # sys.stdout.write('%s -> %s.\n' % (warn_ctype, ret))
    return ret

def get_iname(cls):
    return cls.get_capi_name()

def cpp2csname(cppname):
    if classdef.has_key(cppname):
        if classdef[cppname].has_key('name'):
            return classdef[cppname]['name']
    return cppname

def get_overview(cls):
    result = []

    # result.append('CEF name (C++): %s' % cls.get_name())

    role = "Role: "
    if is_proxy(cls):
        role += "PROXY"
    if is_proxy(cls) and is_handler(cls):
        role += "+"
    if is_handler(cls):
        role += "HANDLER"
    result.append(role)

    return result
