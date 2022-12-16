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
            return byteArray;
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
                    switch (value.substring(0, $DataMarkerLength$)) {
                        case "$StringMarker$":
                            return value.substring($DataMarkerLength$);
                        case "$DateTimeMarker$":
                            return new Date(value.substring($DataMarkerLength$));
                        case "$BinaryMarker$":
                            return convertBase64ToBinary(value.substring($DataMarkerLength$));
                    }
                }
            }
            return value;
        }
        function parseResult(result) {
            const refs = new Map();
            const pendingRefs = new Map();
            return isString(result) ? JSON.parse(result, (name, value) => revive(name, value, refs, pendingRefs)) : result;
        }
        function argumentsStringifier() {
            const refs = new Map();
            const marker = Symbol("marker");
            return function (key, value) {
                if (value === null) {
                    return null;
                }
                const valueType = typeof value;
                // case the property is of type Date, the argument 'value' will be of type 'string',
                // in this case, we need to return the date as a string, but prefixed with a marker,
                // so it can be properly deserialized
                if (valueType === "string" && Reflect.get(this, key) instanceof Date) {
                    return "$DateTimeMarker$" + value;
                }
                if (valueType !== "object" || Reflect.has(value, marker) || (key === valuesPropertyName && Reflect.has(this, marker))) {
                    return value;
                }
                const ref = refs.get(value);
                if (ref) {
                    return ref;
                }
                const id = refs.size.toString();
                refs.set(value, { [refPropertyName]: id, [marker]: undefined });
                if (Array.isArray(value)) {
                    // If it is an array, replicate the array.
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
                            const argsJson = args.length === 0 ? "" : JSON.stringify(args, argumentsStringifier());
                            return targetValue.apply(target, [argsJson]);
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
                return JSON.stringify(fn(), argumentsStringifier());
            }
        };
    })();
}