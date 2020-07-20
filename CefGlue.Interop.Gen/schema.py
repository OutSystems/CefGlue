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
    'int16': 'short',
    'uint16': 'ushort',
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
    'cef_base_ref_counted_t': 'cef_base_ref_counted_t',
    'cef_base_scoped_t': 'cef_base_scoped_t',

    'cef_color_t': 'uint',

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
    'cef_point_t': 'cef_point_t',
    'cef_size_t': 'cef_size_t',
    'cef_rect_t': 'cef_rect_t',
    'cef_mouse_event_t': 'cef_mouse_event_t',
    'cef_screen_info_t': 'cef_screen_info_t',
    'cef_page_range_t': 'cef_page_range_t',
    'cef_cursor_info_t': 'cef_cursor_info_t',
    'cef_request_context_settings_t': 'cef_request_context_settings_t',
    'cef_draggable_region_t': 'cef_draggable_region_t',
    'cef_pdf_print_settings_t': 'cef_pdf_print_settings_t',
    'cef_composition_underline_t': 'cef_composition_underline_t',
    'cef_touch_event_t': 'cef_touch_event_t',
    'cef_audio_parameters_t': 'cef_audio_parameters_t',
    'cef_media_sink_device_info_t': 'cef_media_sink_device_info_t',
    

    # platform dependend structs
    'cef_main_args_t': 'cef_main_args_t',
    'cef_window_info_t': 'cef_window_info_t',

    'cef_text_input_context_t': 'IntPtr',

    'cef_color_model_t': 'CefColorModel',
    'cef_duplex_mode_t': 'CefDuplexMode',

    'cef_cursor_type_t': 'CefCursorType',

	'cef_range_t': 'cef_range_t',
    'cef_channel_layout_t': 'CefChannelLayout',
    'cef_text_input_mode_t': 'CefTextInputMode',
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
    'cef_transition_type_t': 'CefTransitionType',
    'cef_uri_unescape_rule_t': 'CefUriUnescapeRules',
    'cef_window_open_disposition_t': 'CefWindowOpenDisposition',
    'cef_return_value_t': 'CefReturnValue',
    'cef_json_parser_options_t': 'CefJsonParserOptions',
    'cef_json_writer_options_t': 'CefJsonWriterOptions',
    'cef_json_parser_error_t': 'CefJsonParserError',
    'cef_pdf_print_margin_type_t': 'CefPdfPrintMarginType',
	'cef_scale_factor_t': 'CefScaleFactor',
	'cef_plugin_policy_t': 'CefPluginPolicy',
    'cef_cert_status_t': 'CefCertStatus',
    'cef_response_filter_status_t': 'CefResponseFilterStatus',
    'cef_referrer_policy_t': 'CefReferrerPolicy',
	'cef_color_type_t': 'CefColorType',
	'cef_alpha_type_t': 'CefAlphaType',
    'cef_cdm_registration_error_t': 'CefCdmRegistrationError',
	'cef_ssl_version_t': 'CefSslVersion',
	'cef_ssl_content_status_t': 'CefSslContentStatus',
    'cef_menu_color_type_t': 'CefMenuColorType',
    'cef_state_t': 'CefState',
    'cef_media_route_connection_state_t': 'CefMediaRouteConnectionState',
    'cef_media_route_create_result_t': 'CefMediaRouteCreateResult',
    'cef_media_sink_icon_type_t': 'CefMediaSinkIconType',
    }

c2cs_structtypes = { }

cs_keywords = [ 'object', 'string', 'checked', 'event', 'params', 'delegate' ]

classdef = { }



def load(schema_name, header):
    import schema_cef3
    global classdef
    classdef = schema_cef3.classdef

    # exec("from schema_%s import *" % schema_name) in globals()

    # build C API type name to C# type name map (struct types)
    c2cs_structtypes.clear();
    for cls in header.get_classes():
        c2cs_structtypes[cls.get_capi_name()] = get_iname(cls)
        # sys.stdout.write('struct _%s*' % cls.get_capi_name() + ' -> ' + '%s*' % cls.get_capi_name() + '\n' )

    return


def is_handler(cls):
    cls_name = cls.get_name()
    if cls_name in classdef:
        return classdef[cls_name]['role'] & ROLE_HANDLER == ROLE_HANDLER
    return False

def is_proxy(cls):
    cls_name = cls.get_name()
    if cls_name in classdef:
        return classdef[cls_name]['role'] & ROLE_PROXY == ROLE_PROXY
    return False

def is_reversible(cls):
    name = cls.get_name()
    if name in classdef:
        cdef = classdef[name]
        if 'reversible' in cdef:
            return cdef['reversible']
    return False

def isref(ctype):
    return ctype.endswith('*')

def is_platform_retval(csntype):
    return csntype in c2cs_platform_retval

def get_platform_retval_postfixs(csntype):
    if csntype in c2cs_platform_retval:
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

    if ctype in c2cs_types:
        ret = c2cs_types[ctype]
    elif ctype in c2cs_enumtypes:
        ret = c2cs_enumtypes[ctype]
    elif ctype in c2cs_structtypes:
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
    if cppname in classdef:
        if 'name' in classdef[cppname]:
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

def is_autodispose(cls):
    if cls.get_name() in classdef:
        clsinfo = classdef[cls.get_name()]
        return 'autodispose' in clsinfo and clsinfo['autodispose'] == True
    return False
