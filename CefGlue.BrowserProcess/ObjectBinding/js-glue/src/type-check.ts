
export function isString(value: unknown): value is string {
    return typeof value === "string";
}

export function isFunction(value: unknown): value is Function {
    return typeof value === "function";
}

export function isArrayBuffer(value: unknown): value is ArrayBuffer {
   return value instanceof ArrayBuffer;
}
