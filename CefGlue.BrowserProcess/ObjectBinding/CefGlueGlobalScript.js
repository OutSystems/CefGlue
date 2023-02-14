var $GlobalObjectName$;
if (!$GlobalObjectName$) {
    $GlobalObjectName$ = (function () {
        const idPropertyName = "$JsonIdAttribute$";
        const refPropertyName = "$JsonRefAttribute$";
        const valuesPropertyName = "$JsonValuesAttribute$";
        function isString(value) {
            return typeof value === "string";
        }
        function convertBase64ToBinary(value) {
            const byteCharacters = atob(value);
            const byteArray = new Array(byteCharacters.length);
            for (let i = 0; i < byteCharacters.length; i++) {
                byteArray[i] = byteCharacters.charCodeAt(i);
            }
            return new Uint8Array(byteArray);;
        }
        function convertBinaryToBase64(byteArray) {
            return btoa(String.fromCharCode(...byteArray));
        }
        function convertStringToJsType(value) {
            switch (value.substring(0, $DataMarkerLength$)) {
                case "$StringMarker$":
                    return value.substring($DataMarkerLength$);
                case "$DateTimeMarker$":
                    return new Date(value.substring($DataMarkerLength$));
                case "$BinaryMarker$":
                    return convertBase64ToBinary(value.substring($DataMarkerLength$));
            }
            return value;
        }
        function revive(name, value, refs, pendingRefs) {
            if (value) {
                const id = value[idPropertyName];
                if (id !== undefined) {
                    delete value[idPropertyName];
                    const pendingRef = pendingRefs.get(id);
                    if (pendingRef) {
                        Object.assign(pendingRef, value);
                        value = pendingRef;
                    }
                    refs.set(id, value);
                    return value;
                }
                const refId = value[refPropertyName];
                if (refId !== undefined) {
                    const ref = refs.get(refId);
                    if (ref) {
                        return ref;
                    }
                    const pendingRef = pendingRefs.get(refId);
                    if (pendingRef) {
                        value = pendingRef;
                    } else {
                        value = {};
                        pendingRefs.set(refId, value);
                    }
                    return value;
                }
                if (isString(value)) {
                    return convertStringToJsType(value);
                }
            }
            return value;
        }
        function parseResult(result) {
            const refs = new Map();
            const pendingRefs = new Map();
            return isString(result) ? JSON.parse(result, (name, value) => revive(name, value, refs, pendingRefs)) : result;
        }
        function objectsStringifier(skipReferenceForInitialArrayObject) {
            const refs = new Map();
            const marker = Symbol("marker");
            return function (key, value) {
                if (value === null || value === undefined) {
                    return value;
                }

                if (key === idPropertyName || key === refPropertyName || (key === valuesPropertyName && Reflect.has(this, marker))) {
                    // its the id, ref or special values property
                    return value;
                }
                switch (typeof value) {
                    case "string":
                        // case the property is of type Date, the argument 'value' is of type 'string',
                        // in this case, we need to return the date as a string, but prefixed with the DateTimeMarker
                        // (there can be a remote problem that the property is accessed two times, which can cause undesirable side effects)
                        if (Reflect.get(this, key) instanceof Date) {
                            return "$DateTimeMarker$" + value;
                        }
                        return "$StringMarker$" + value;
                    case "object":
                        if (value instanceof Uint8Array) {
                            return "$BinaryMarker$" + convertBinaryToBase64(value);
                        }
                        if (Reflect.has(value, marker)) {
                            return value;
                        }
                        break;
                    default:
                        return value;
                }
                const ref = refs.get(value);
                if (ref) {
                    // value has been seen, return that seen value
                    return ref;
                }
                const id = refs.size.toString();
                refs.set(value, { [refPropertyName]: id, [marker]: undefined });
                if (Array.isArray(value)) {
                    if (skipReferenceForInitialArrayObject && key === "") {
                        // For arrays, when the flag is on, the resulting string is not wrapped in "{$id:"",$values:[..]}"
                        // instead, it returns the plain array "[...]"
                        return value;
                    }

                    // If it is an array, wrap the array and add an id and a marker
                    return { [idPropertyName]: id, [valuesPropertyName]: value, [marker]: undefined };
                }
                const tmpObj = { [idPropertyName]: id, [marker]: undefined };
                if (Reflect.has(value, idPropertyName)) {
                    console.warn(`Object contains ${idPropertyName} property which will be ignored.`);
                    // Ensure that the $id property is not overwritten by the value of the property with the same name that already exists inside of the 'value' object
                    return Object.assign(tmpObj, value, tmpObj);
                }
                return Object.assign(tmpObj, value);
            }
        }
        return {
            $PromiseFactoryFunctionName$: function () {
                const result = {};
                const promise = new Promise(function (resolve, reject) {
                    result.resolve = function (result) {
                        resolve(parseResult(result));
                    };
                    result.reject = reject;
                });
                result.promise = promise;
                return result;
            },
            $InterceptorFactoryFunctionName$: function (targetObj) {
                const functionsMap = new Map();
                const handler = {
                    set() { },
                    get(target, propKey, receiver) {
                        const func = functionsMap.get(propKey);
                        if (func) {
                            return func;
                        }
                        const targetValue = Reflect.get(target, propKey);
                        if (typeof targetValue !== "function") {
                            return targetValue;
                        }
                        const interceptor = function (...args) {
                            const convertedArgs = args.length === 0 ? args : [JSON.stringify(args, objectsStringifier(/*skipReferenceForInitialArrayObject*/true))];
                            return targetValue.apply(target, convertedArgs);
                        };
                        functionsMap.set(propKey, interceptor);
                        return interceptor;
                    }
                };
                return new Proxy(targetObj, handler);
            },
            checkObjectBound: function (objName) {
                native function $BindNativeFunctionName$();
                if (window.hasOwnProperty(objName)) {
                    // quick check
                    return Promise.resolve(true);
                }
                return $BindNativeFunctionName$(objName);
            },
            deleteObjectBound: function (objName) {
                native function $UnbindNativeFunctionName$();
                $UnbindNativeFunctionName$(objName);
            },
            $EvaluateScriptFunctionName$: function (fn) {
                return JSON.stringify(fn(), objectsStringifier());
            }
        };
    })();
}