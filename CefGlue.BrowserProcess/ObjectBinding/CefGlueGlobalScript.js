var cefglue = (function (exports) {
    'use strict';

    function isFunction(value) {
        return typeof value === "function";
    }
    function isArrayBuffer(value) {
        return value instanceof ArrayBuffer;
    }

    // special data markers used to distinguish the several string types
    var TypeMarker;
    (function (TypeMarker) {
        TypeMarker["dateTime"] = "D";
        TypeMarker["string"] = "S";
        TypeMarker["binary"] = "B";
    })(TypeMarker || (TypeMarker = {}));

    function utf8Count(str) {
        const strLength = str.length;
        let byteLength = 0;
        let pos = 0;
        while (pos < strLength) {
            let value = str.charCodeAt(pos++);
            if ((value & 0xffffff80) === 0) {
                // 1-byte
                byteLength++;
                continue;
            }
            else if ((value & 0xfffff800) === 0) {
                // 2-bytes
                byteLength += 2;
            }
            else {
                // handle surrogate pair
                if (value >= 0xd800 && value <= 0xdbff) {
                    // high surrogate
                    if (pos < strLength) {
                        const extra = str.charCodeAt(pos);
                        if ((extra & 0xfc00) === 0xdc00) {
                            ++pos;
                            value = ((value & 0x3ff) << 10) + (extra & 0x3ff) + 0x10000;
                        }
                    }
                }
                if ((value & 0xffff0000) === 0) {
                    // 3-byte
                    byteLength += 3;
                }
                else {
                    // 4-byte
                    byteLength += 4;
                }
            }
        }
        return byteLength;
    }
    function utf8EncodeJs(str, output, outputOffset) {
        const strLength = str.length;
        let offset = outputOffset;
        let pos = 0;
        while (pos < strLength) {
            let value = str.charCodeAt(pos++);
            if ((value & 0xffffff80) === 0) {
                // 1-byte
                output[offset++] = value;
                continue;
            }
            else if ((value & 0xfffff800) === 0) {
                // 2-bytes
                output[offset++] = ((value >> 6) & 0x1f) | 0xc0;
            }
            else {
                // handle surrogate pair
                if (value >= 0xd800 && value <= 0xdbff) {
                    // high surrogate
                    if (pos < strLength) {
                        const extra = str.charCodeAt(pos);
                        if ((extra & 0xfc00) === 0xdc00) {
                            ++pos;
                            value = ((value & 0x3ff) << 10) + (extra & 0x3ff) + 0x10000;
                        }
                    }
                }
                if ((value & 0xffff0000) === 0) {
                    // 3-byte
                    output[offset++] = ((value >> 12) & 0x0f) | 0xe0;
                    output[offset++] = ((value >> 6) & 0x3f) | 0x80;
                }
                else {
                    // 4-byte
                    output[offset++] = ((value >> 18) & 0x07) | 0xf0;
                    output[offset++] = ((value >> 12) & 0x3f) | 0x80;
                    output[offset++] = ((value >> 6) & 0x3f) | 0x80;
                }
            }
            output[offset++] = (value & 0x3f) | 0x80;
        }
    }
    // TextEncoder and TextDecoder are standardized in whatwg encoding:
    // https://encoding.spec.whatwg.org/
    // and available in all the modern browsers:
    // https://caniuse.com/textencoder
    // They are available in Node.js since v12 LTS as well:
    // https://nodejs.org/api/globals.html#textencoder
    let sharedTextEncoder;
    // This threshold should be determined by benchmarking, which might vary in engines and input data.
    // Run `npx ts-node benchmark/encode-string.ts` for details.
    const TEXT_ENCODER_THRESHOLD = 50;
    function utf8EncodeTE(str, output, outputOffset) {
        if (!sharedTextEncoder) {
            sharedTextEncoder = new TextEncoder();
        }
        sharedTextEncoder.encodeInto(str, output.subarray(outputOffset));
    }
    function utf8Encode(str, output, outputOffset) {
        if (str.length > TEXT_ENCODER_THRESHOLD) {
            utf8EncodeTE(str, output, outputOffset);
        }
        else {
            utf8EncodeJs(str, output, outputOffset);
        }
    }
    const CHUNK_SIZE = 0x1_000;
    function utf8DecodeJs(bytes, inputOffset, byteLength) {
        let offset = inputOffset;
        const end = offset + byteLength;
        const units = [];
        let result = "";
        while (offset < end) {
            const byte1 = bytes[offset++];
            if ((byte1 & 0x80) === 0) {
                // 1 byte
                units.push(byte1);
            }
            else if ((byte1 & 0xe0) === 0xc0) {
                // 2 bytes
                const byte2 = bytes[offset++] & 0x3f;
                units.push(((byte1 & 0x1f) << 6) | byte2);
            }
            else if ((byte1 & 0xf0) === 0xe0) {
                // 3 bytes
                const byte2 = bytes[offset++] & 0x3f;
                const byte3 = bytes[offset++] & 0x3f;
                units.push(((byte1 & 0x1f) << 12) | (byte2 << 6) | byte3);
            }
            else if ((byte1 & 0xf8) === 0xf0) {
                // 4 bytes
                const byte2 = bytes[offset++] & 0x3f;
                const byte3 = bytes[offset++] & 0x3f;
                const byte4 = bytes[offset++] & 0x3f;
                let unit = ((byte1 & 0x07) << 0x12) | (byte2 << 0x0c) | (byte3 << 0x06) | byte4;
                if (unit > 0xffff) {
                    unit -= 0x10000;
                    units.push(((unit >>> 10) & 0x3ff) | 0xd800);
                    unit = 0xdc00 | (unit & 0x3ff);
                }
                units.push(unit);
            }
            else {
                units.push(byte1);
            }
            if (units.length >= CHUNK_SIZE) {
                result += String.fromCharCode(...units);
                units.length = 0;
            }
        }
        if (units.length > 0) {
            result += String.fromCharCode(...units);
        }
        return result;
    }
    let sharedTextDecoder;
    // This threshold should be determined by benchmarking, which might vary in engines and input data.
    // Run `npx ts-node benchmark/decode-string.ts` for details.
    const TEXT_DECODER_THRESHOLD = 200;
    function utf8DecodeTD(bytes, inputOffset, byteLength) {
        if (!sharedTextDecoder) {
            sharedTextDecoder = new TextDecoder();
        }
        const stringBytes = bytes.subarray(inputOffset, inputOffset + byteLength);
        return sharedTextDecoder.decode(stringBytes);
    }
    function utf8Decode(bytes, inputOffset, byteLength) {
        if (byteLength > TEXT_DECODER_THRESHOLD) {
            return utf8DecodeTD(bytes, inputOffset, byteLength);
        }
        else {
            return utf8DecodeJs(bytes, inputOffset, byteLength);
        }
    }

    /**
     * ExtData is used to handle Extension Types that are not registered to ExtensionCodec.
     */
    class ExtData {
        type;
        data;
        constructor(type, data) {
            this.type = type;
            this.data = data;
        }
    }

    class DecodeError extends Error {
        constructor(message) {
            super(message);
            // fix the prototype chain in a cross-platform way
            const proto = Object.create(DecodeError.prototype);
            Object.setPrototypeOf(this, proto);
            Object.defineProperty(this, "name", {
                configurable: true,
                enumerable: false,
                value: DecodeError.name,
            });
        }
    }

    // Integer Utility
    const UINT32_MAX = 0xffff_ffff;
    // DataView extension to handle int64 / uint64,
    // where the actual range is 53-bits integer (a.k.a. safe integer)
    function setUint64(view, offset, value) {
        const high = value / 0x1_0000_0000;
        const low = value; // high bits are truncated by DataView
        view.setUint32(offset, high);
        view.setUint32(offset + 4, low);
    }
    function setInt64(view, offset, value) {
        const high = Math.floor(value / 0x1_0000_0000);
        const low = value; // high bits are truncated by DataView
        view.setUint32(offset, high);
        view.setUint32(offset + 4, low);
    }
    function getInt64(view, offset) {
        const high = view.getInt32(offset);
        const low = view.getUint32(offset + 4);
        return high * 0x1_0000_0000 + low;
    }
    function getUint64(view, offset) {
        const high = view.getUint32(offset);
        const low = view.getUint32(offset + 4);
        return high * 0x1_0000_0000 + low;
    }

    // https://github.com/msgpack/msgpack/blob/master/spec.md#timestamp-extension-type
    const EXT_TIMESTAMP = -1;
    const TIMESTAMP32_MAX_SEC = 0x100000000 - 1; // 32-bit unsigned int
    const TIMESTAMP64_MAX_SEC = 0x400000000 - 1; // 34-bit unsigned int
    function encodeTimeSpecToTimestamp({ sec, nsec }) {
        if (sec >= 0 && nsec >= 0 && sec <= TIMESTAMP64_MAX_SEC) {
            // Here sec >= 0 && nsec >= 0
            if (nsec === 0 && sec <= TIMESTAMP32_MAX_SEC) {
                // timestamp 32 = { sec32 (unsigned) }
                const rv = new Uint8Array(4);
                const view = new DataView(rv.buffer);
                view.setUint32(0, sec);
                return rv;
            }
            else {
                // timestamp 64 = { nsec30 (unsigned), sec34 (unsigned) }
                const secHigh = sec / 0x100000000;
                const secLow = sec & 0xffffffff;
                const rv = new Uint8Array(8);
                const view = new DataView(rv.buffer);
                // nsec30 | secHigh2
                view.setUint32(0, (nsec << 2) | (secHigh & 0x3));
                // secLow32
                view.setUint32(4, secLow);
                return rv;
            }
        }
        else {
            // timestamp 96 = { nsec32 (unsigned), sec64 (signed) }
            const rv = new Uint8Array(12);
            const view = new DataView(rv.buffer);
            view.setUint32(0, nsec);
            setInt64(view, 4, sec);
            return rv;
        }
    }
    function encodeDateToTimeSpec(date) {
        const msec = date.getTime();
        const sec = Math.floor(msec / 1e3);
        const nsec = (msec - sec * 1e3) * 1e6;
        // Normalizes { sec, nsec } to ensure nsec is unsigned.
        const nsecInSec = Math.floor(nsec / 1e9);
        return {
            sec: sec + nsecInSec,
            nsec: nsec - nsecInSec * 1e9,
        };
    }
    function encodeTimestampExtension(object) {
        if (object instanceof Date) {
            const timeSpec = encodeDateToTimeSpec(object);
            return encodeTimeSpecToTimestamp(timeSpec);
        }
        else {
            return null;
        }
    }
    function decodeTimestampToTimeSpec(data) {
        const view = new DataView(data.buffer, data.byteOffset, data.byteLength);
        // data may be 32, 64, or 96 bits
        switch (data.byteLength) {
            case 4: {
                // timestamp 32 = { sec32 }
                const sec = view.getUint32(0);
                const nsec = 0;
                return { sec, nsec };
            }
            case 8: {
                // timestamp 64 = { nsec30, sec34 }
                const nsec30AndSecHigh2 = view.getUint32(0);
                const secLow32 = view.getUint32(4);
                const sec = (nsec30AndSecHigh2 & 0x3) * 0x100000000 + secLow32;
                const nsec = nsec30AndSecHigh2 >>> 2;
                return { sec, nsec };
            }
            case 12: {
                // timestamp 96 = { nsec32 (unsigned), sec64 (signed) }
                const sec = getInt64(view, 4);
                const nsec = view.getUint32(0);
                return { sec, nsec };
            }
            default:
                throw new DecodeError(`Unrecognized data size for timestamp (expected 4, 8, or 12): ${data.length}`);
        }
    }
    function decodeTimestampExtension(data) {
        const timeSpec = decodeTimestampToTimeSpec(data);
        return new Date(timeSpec.sec * 1e3 + timeSpec.nsec / 1e6);
    }
    const timestampExtension = {
        type: EXT_TIMESTAMP,
        encode: encodeTimestampExtension,
        decode: decodeTimestampExtension,
    };

    // ExtensionCodec to handle MessagePack extensions
    class ExtensionCodec {
        static defaultCodec = new ExtensionCodec();
        // ensures ExtensionCodecType<X> matches ExtensionCodec<X>
        // this will make type errors a lot more clear
        // eslint-disable-next-line @typescript-eslint/naming-convention
        __brand;
        // built-in extensions
        builtInEncoders = [];
        builtInDecoders = [];
        // custom extensions
        encoders = [];
        decoders = [];
        constructor() {
            this.register(timestampExtension);
        }
        register({ type, encode, decode, }) {
            if (type >= 0) {
                // custom extensions
                this.encoders[type] = encode;
                this.decoders[type] = decode;
            }
            else {
                // built-in extensions
                const index = 1 + type;
                this.builtInEncoders[index] = encode;
                this.builtInDecoders[index] = decode;
            }
        }
        tryToEncode(object, context) {
            // built-in extensions
            for (let i = 0; i < this.builtInEncoders.length; i++) {
                const encodeExt = this.builtInEncoders[i];
                if (encodeExt != null) {
                    const data = encodeExt(object, context);
                    if (data != null) {
                        const type = -1 - i;
                        return new ExtData(type, data);
                    }
                }
            }
            // custom extensions
            for (let i = 0; i < this.encoders.length; i++) {
                const encodeExt = this.encoders[i];
                if (encodeExt != null) {
                    const data = encodeExt(object, context);
                    if (data != null) {
                        const type = i;
                        return new ExtData(type, data);
                    }
                }
            }
            if (object instanceof ExtData) {
                // to keep ExtData as is
                return object;
            }
            return null;
        }
        decode(data, type, context) {
            const decodeExt = type < 0 ? this.builtInDecoders[-1 - type] : this.decoders[type];
            if (decodeExt) {
                return decodeExt(data, type, context);
            }
            else {
                // decode() does not fail, returns ExtData instead.
                return new ExtData(type, data);
            }
        }
    }

    function ensureUint8Array(buffer) {
        if (buffer instanceof Uint8Array) {
            return buffer;
        }
        else if (ArrayBuffer.isView(buffer)) {
            return new Uint8Array(buffer.buffer, buffer.byteOffset, buffer.byteLength);
        }
        else if (buffer instanceof ArrayBuffer) {
            return new Uint8Array(buffer);
        }
        else {
            // ArrayLike<number>
            return Uint8Array.from(buffer);
        }
    }
    function createDataView(buffer) {
        if (buffer instanceof ArrayBuffer) {
            return new DataView(buffer);
        }
        const bufferView = ensureUint8Array(buffer);
        return new DataView(bufferView.buffer, bufferView.byteOffset, bufferView.byteLength);
    }

    const DEFAULT_MAX_DEPTH = 100;
    const DEFAULT_INITIAL_BUFFER_SIZE = 2048;
    class Encoder {
        extensionCodec;
        context;
        useBigInt64;
        maxDepth;
        initialBufferSize;
        sortKeys;
        forceFloat32;
        ignoreUndefined;
        forceIntegerToFloat;
        pos;
        view;
        bytes;
        constructor(options) {
            this.extensionCodec = options?.extensionCodec ?? ExtensionCodec.defaultCodec;
            this.context = options?.context; // needs a type assertion because EncoderOptions has no context property when ContextType is undefined
            this.useBigInt64 = options?.useBigInt64 ?? false;
            this.maxDepth = options?.maxDepth ?? DEFAULT_MAX_DEPTH;
            this.initialBufferSize = options?.initialBufferSize ?? DEFAULT_INITIAL_BUFFER_SIZE;
            this.sortKeys = options?.sortKeys ?? false;
            this.forceFloat32 = options?.forceFloat32 ?? false;
            this.ignoreUndefined = options?.ignoreUndefined ?? false;
            this.forceIntegerToFloat = options?.forceIntegerToFloat ?? false;
            this.pos = 0;
            this.view = new DataView(new ArrayBuffer(this.initialBufferSize));
            this.bytes = new Uint8Array(this.view.buffer);
        }
        reinitializeState() {
            this.pos = 0;
        }
        /**
         * This is almost equivalent to {@link Encoder#encode}, but it returns an reference of the encoder's internal buffer and thus much faster than {@link Encoder#encode}.
         *
         * @returns Encodes the object and returns a shared reference the encoder's internal buffer.
         */
        encodeSharedRef(object) {
            this.reinitializeState();
            this.doEncode(object, 1);
            return this.bytes.subarray(0, this.pos);
        }
        /**
         * @returns Encodes the object and returns a copy of the encoder's internal buffer.
         */
        encode(object) {
            this.reinitializeState();
            this.doEncode(object, 1);
            return this.bytes.slice(0, this.pos);
        }
        doEncode(object, depth) {
            if (depth > this.maxDepth) {
                throw new Error(`Too deep objects in depth ${depth}`);
            }
            if (object == null) {
                this.encodeNil();
            }
            else if (typeof object === "boolean") {
                this.encodeBoolean(object);
            }
            else if (typeof object === "number") {
                if (!this.forceIntegerToFloat) {
                    this.encodeNumber(object);
                }
                else {
                    this.encodeNumberAsFloat(object);
                }
            }
            else if (typeof object === "string") {
                this.encodeString(object);
            }
            else if (this.useBigInt64 && typeof object === "bigint") {
                this.encodeBigInt64(object);
            }
            else {
                this.encodeObject(object, depth);
            }
        }
        ensureBufferSizeToWrite(sizeToWrite) {
            const requiredSize = this.pos + sizeToWrite;
            if (this.view.byteLength < requiredSize) {
                this.resizeBuffer(requiredSize * 2);
            }
        }
        resizeBuffer(newSize) {
            const newBuffer = new ArrayBuffer(newSize);
            const newBytes = new Uint8Array(newBuffer);
            const newView = new DataView(newBuffer);
            newBytes.set(this.bytes);
            this.view = newView;
            this.bytes = newBytes;
        }
        encodeNil() {
            this.writeU8(0xc0);
        }
        encodeBoolean(object) {
            if (object === false) {
                this.writeU8(0xc2);
            }
            else {
                this.writeU8(0xc3);
            }
        }
        encodeNumber(object) {
            if (!this.forceIntegerToFloat && Number.isSafeInteger(object)) {
                if (object >= 0) {
                    if (object < 0x80) {
                        // positive fixint
                        this.writeU8(object);
                    }
                    else if (object < 0x100) {
                        // uint 8
                        this.writeU8(0xcc);
                        this.writeU8(object);
                    }
                    else if (object < 0x10000) {
                        // uint 16
                        this.writeU8(0xcd);
                        this.writeU16(object);
                    }
                    else if (object < 0x100000000) {
                        // uint 32
                        this.writeU8(0xce);
                        this.writeU32(object);
                    }
                    else if (!this.useBigInt64) {
                        // uint 64
                        this.writeU8(0xcf);
                        this.writeU64(object);
                    }
                    else {
                        this.encodeNumberAsFloat(object);
                    }
                }
                else {
                    if (object >= -0x20) {
                        // negative fixint
                        this.writeU8(0xe0 | (object + 0x20));
                    }
                    else if (object >= -0x80) {
                        // int 8
                        this.writeU8(0xd0);
                        this.writeI8(object);
                    }
                    else if (object >= -0x8000) {
                        // int 16
                        this.writeU8(0xd1);
                        this.writeI16(object);
                    }
                    else if (object >= -0x80000000) {
                        // int 32
                        this.writeU8(0xd2);
                        this.writeI32(object);
                    }
                    else if (!this.useBigInt64) {
                        // int 64
                        this.writeU8(0xd3);
                        this.writeI64(object);
                    }
                    else {
                        this.encodeNumberAsFloat(object);
                    }
                }
            }
            else {
                this.encodeNumberAsFloat(object);
            }
        }
        encodeNumberAsFloat(object) {
            if (this.forceFloat32) {
                // float 32
                this.writeU8(0xca);
                this.writeF32(object);
            }
            else {
                // float 64
                this.writeU8(0xcb);
                this.writeF64(object);
            }
        }
        encodeBigInt64(object) {
            if (object >= BigInt(0)) {
                // uint 64
                this.writeU8(0xcf);
                this.writeBigUint64(object);
            }
            else {
                // int 64
                this.writeU8(0xd3);
                this.writeBigInt64(object);
            }
        }
        writeStringHeader(byteLength) {
            if (byteLength < 32) {
                // fixstr
                this.writeU8(0xa0 + byteLength);
            }
            else if (byteLength < 0x100) {
                // str 8
                this.writeU8(0xd9);
                this.writeU8(byteLength);
            }
            else if (byteLength < 0x10000) {
                // str 16
                this.writeU8(0xda);
                this.writeU16(byteLength);
            }
            else if (byteLength < 0x100000000) {
                // str 32
                this.writeU8(0xdb);
                this.writeU32(byteLength);
            }
            else {
                throw new Error(`Too long string: ${byteLength} bytes in UTF-8`);
            }
        }
        encodeString(object) {
            const maxHeaderSize = 1 + 4;
            const byteLength = utf8Count(object);
            this.ensureBufferSizeToWrite(maxHeaderSize + byteLength);
            this.writeStringHeader(byteLength);
            utf8Encode(object, this.bytes, this.pos);
            this.pos += byteLength;
        }
        encodeObject(object, depth) {
            // try to encode objects with custom codec first of non-primitives
            const ext = this.extensionCodec.tryToEncode(object, this.context);
            if (ext != null) {
                this.encodeExtension(ext);
            }
            else if (Array.isArray(object)) {
                this.encodeArray(object, depth);
            }
            else if (ArrayBuffer.isView(object)) {
                this.encodeBinary(object);
            }
            else if (typeof object === "object") {
                this.encodeMap(object, depth);
            }
            else {
                // symbol, function and other special object come here unless extensionCodec handles them.
                throw new Error(`Unrecognized object: ${Object.prototype.toString.apply(object)}`);
            }
        }
        encodeBinary(object) {
            const size = object.byteLength;
            if (size < 0x100) {
                // bin 8
                this.writeU8(0xc4);
                this.writeU8(size);
            }
            else if (size < 0x10000) {
                // bin 16
                this.writeU8(0xc5);
                this.writeU16(size);
            }
            else if (size < 0x100000000) {
                // bin 32
                this.writeU8(0xc6);
                this.writeU32(size);
            }
            else {
                throw new Error(`Too large binary: ${size}`);
            }
            const bytes = ensureUint8Array(object);
            this.writeU8a(bytes);
        }
        encodeArray(object, depth) {
            const size = object.length;
            if (size < 16) {
                // fixarray
                this.writeU8(0x90 + size);
            }
            else if (size < 0x10000) {
                // array 16
                this.writeU8(0xdc);
                this.writeU16(size);
            }
            else if (size < 0x100000000) {
                // array 32
                this.writeU8(0xdd);
                this.writeU32(size);
            }
            else {
                throw new Error(`Too large array: ${size}`);
            }
            for (const item of object) {
                this.doEncode(item, depth + 1);
            }
        }
        countWithoutUndefined(object, keys) {
            let count = 0;
            for (const key of keys) {
                if (object[key] !== undefined) {
                    count++;
                }
            }
            return count;
        }
        encodeMap(object, depth) {
            const keys = Object.keys(object);
            if (this.sortKeys) {
                keys.sort();
            }
            const size = this.ignoreUndefined ? this.countWithoutUndefined(object, keys) : keys.length;
            if (size < 16) {
                // fixmap
                this.writeU8(0x80 + size);
            }
            else if (size < 0x10000) {
                // map 16
                this.writeU8(0xde);
                this.writeU16(size);
            }
            else if (size < 0x100000000) {
                // map 32
                this.writeU8(0xdf);
                this.writeU32(size);
            }
            else {
                throw new Error(`Too large map object: ${size}`);
            }
            for (const key of keys) {
                const value = object[key];
                if (!(this.ignoreUndefined && value === undefined)) {
                    this.encodeString(key);
                    this.doEncode(value, depth + 1);
                }
            }
        }
        encodeExtension(ext) {
            const size = ext.data.length;
            if (size === 1) {
                // fixext 1
                this.writeU8(0xd4);
            }
            else if (size === 2) {
                // fixext 2
                this.writeU8(0xd5);
            }
            else if (size === 4) {
                // fixext 4
                this.writeU8(0xd6);
            }
            else if (size === 8) {
                // fixext 8
                this.writeU8(0xd7);
            }
            else if (size === 16) {
                // fixext 16
                this.writeU8(0xd8);
            }
            else if (size < 0x100) {
                // ext 8
                this.writeU8(0xc7);
                this.writeU8(size);
            }
            else if (size < 0x10000) {
                // ext 16
                this.writeU8(0xc8);
                this.writeU16(size);
            }
            else if (size < 0x100000000) {
                // ext 32
                this.writeU8(0xc9);
                this.writeU32(size);
            }
            else {
                throw new Error(`Too large extension object: ${size}`);
            }
            this.writeI8(ext.type);
            this.writeU8a(ext.data);
        }
        writeU8(value) {
            this.ensureBufferSizeToWrite(1);
            this.view.setUint8(this.pos, value);
            this.pos++;
        }
        writeU8a(values) {
            const size = values.length;
            this.ensureBufferSizeToWrite(size);
            this.bytes.set(values, this.pos);
            this.pos += size;
        }
        writeI8(value) {
            this.ensureBufferSizeToWrite(1);
            this.view.setInt8(this.pos, value);
            this.pos++;
        }
        writeU16(value) {
            this.ensureBufferSizeToWrite(2);
            this.view.setUint16(this.pos, value);
            this.pos += 2;
        }
        writeI16(value) {
            this.ensureBufferSizeToWrite(2);
            this.view.setInt16(this.pos, value);
            this.pos += 2;
        }
        writeU32(value) {
            this.ensureBufferSizeToWrite(4);
            this.view.setUint32(this.pos, value);
            this.pos += 4;
        }
        writeI32(value) {
            this.ensureBufferSizeToWrite(4);
            this.view.setInt32(this.pos, value);
            this.pos += 4;
        }
        writeF32(value) {
            this.ensureBufferSizeToWrite(4);
            this.view.setFloat32(this.pos, value);
            this.pos += 4;
        }
        writeF64(value) {
            this.ensureBufferSizeToWrite(8);
            this.view.setFloat64(this.pos, value);
            this.pos += 8;
        }
        writeU64(value) {
            this.ensureBufferSizeToWrite(8);
            setUint64(this.view, this.pos, value);
            this.pos += 8;
        }
        writeI64(value) {
            this.ensureBufferSizeToWrite(8);
            setInt64(this.view, this.pos, value);
            this.pos += 8;
        }
        writeBigUint64(value) {
            this.ensureBufferSizeToWrite(8);
            this.view.setBigUint64(this.pos, value);
            this.pos += 8;
        }
        writeBigInt64(value) {
            this.ensureBufferSizeToWrite(8);
            this.view.setBigInt64(this.pos, value);
            this.pos += 8;
        }
    }

    /**
     * It encodes `value` in the MessagePack format and
     * returns a byte buffer.
     *
     * The returned buffer is a slice of a larger `ArrayBuffer`, so you have to use its `#byteOffset` and `#byteLength` in order to convert it to another typed arrays including NodeJS `Buffer`.
     */
    function encode(value, options) {
        const encoder = new Encoder(options);
        return encoder.encodeSharedRef(value);
    }

    function prettyByte(byte) {
        return `${byte < 0 ? "-" : ""}0x${Math.abs(byte).toString(16).padStart(2, "0")}`;
    }

    const DEFAULT_MAX_KEY_LENGTH = 16;
    const DEFAULT_MAX_LENGTH_PER_KEY = 16;
    class CachedKeyDecoder {
        maxKeyLength;
        maxLengthPerKey;
        hit = 0;
        miss = 0;
        caches;
        constructor(maxKeyLength = DEFAULT_MAX_KEY_LENGTH, maxLengthPerKey = DEFAULT_MAX_LENGTH_PER_KEY) {
            this.maxKeyLength = maxKeyLength;
            this.maxLengthPerKey = maxLengthPerKey;
            // avoid `new Array(N)`, which makes a sparse array,
            // because a sparse array is typically slower than a non-sparse array.
            this.caches = [];
            for (let i = 0; i < this.maxKeyLength; i++) {
                this.caches.push([]);
            }
        }
        canBeCached(byteLength) {
            return byteLength > 0 && byteLength <= this.maxKeyLength;
        }
        find(bytes, inputOffset, byteLength) {
            const records = this.caches[byteLength - 1];
            FIND_CHUNK: for (const record of records) {
                const recordBytes = record.bytes;
                for (let j = 0; j < byteLength; j++) {
                    if (recordBytes[j] !== bytes[inputOffset + j]) {
                        continue FIND_CHUNK;
                    }
                }
                return record.str;
            }
            return null;
        }
        store(bytes, value) {
            const records = this.caches[bytes.length - 1];
            const record = { bytes, str: value };
            if (records.length >= this.maxLengthPerKey) {
                // `records` are full!
                // Set `record` to an arbitrary position.
                records[(Math.random() * records.length) | 0] = record;
            }
            else {
                records.push(record);
            }
        }
        decode(bytes, inputOffset, byteLength) {
            const cachedValue = this.find(bytes, inputOffset, byteLength);
            if (cachedValue != null) {
                this.hit++;
                return cachedValue;
            }
            this.miss++;
            const str = utf8DecodeJs(bytes, inputOffset, byteLength);
            // Ensure to copy a slice of bytes because the byte may be NodeJS Buffer and Buffer#slice() returns a reference to its internal ArrayBuffer.
            const slicedCopyOfBytes = Uint8Array.prototype.slice.call(bytes, inputOffset, inputOffset + byteLength);
            this.store(slicedCopyOfBytes, str);
            return str;
        }
    }

    const STATE_ARRAY = "array";
    const STATE_MAP_KEY = "map_key";
    const STATE_MAP_VALUE = "map_value";
    const isValidMapKeyType = (key) => {
        return typeof key === "string" || typeof key === "number";
    };
    class StackPool {
        stack = [];
        stackHeadPosition = -1;
        get length() {
            return this.stackHeadPosition + 1;
        }
        top() {
            return this.stack[this.stackHeadPosition];
        }
        pushArrayState(size) {
            const state = this.getUninitializedStateFromPool();
            state.type = STATE_ARRAY;
            state.position = 0;
            state.size = size;
            state.array = new Array(size);
        }
        pushMapState(size) {
            const state = this.getUninitializedStateFromPool();
            state.type = STATE_MAP_KEY;
            state.readCount = 0;
            state.size = size;
            state.map = {};
        }
        getUninitializedStateFromPool() {
            this.stackHeadPosition++;
            if (this.stackHeadPosition === this.stack.length) {
                const partialState = {
                    type: undefined,
                    size: 0,
                    array: undefined,
                    position: 0,
                    readCount: 0,
                    map: undefined,
                    key: null,
                };
                this.stack.push(partialState);
            }
            return this.stack[this.stackHeadPosition];
        }
        release(state) {
            const topStackState = this.stack[this.stackHeadPosition];
            if (topStackState !== state) {
                throw new Error("Invalid stack state. Released state is not on top of the stack.");
            }
            if (state.type === STATE_ARRAY) {
                const partialState = state;
                partialState.size = 0;
                partialState.array = undefined;
                partialState.position = 0;
                partialState.type = undefined;
            }
            if (state.type === STATE_MAP_KEY || state.type === STATE_MAP_VALUE) {
                const partialState = state;
                partialState.size = 0;
                partialState.map = undefined;
                partialState.readCount = 0;
                partialState.type = undefined;
            }
            this.stackHeadPosition--;
        }
        reset() {
            this.stack.length = 0;
            this.stackHeadPosition = -1;
        }
    }
    const HEAD_BYTE_REQUIRED = -1;
    const EMPTY_VIEW = new DataView(new ArrayBuffer(0));
    const EMPTY_BYTES = new Uint8Array(EMPTY_VIEW.buffer);
    try {
        // IE11: The spec says it should throw RangeError,
        // IE11: but in IE11 it throws TypeError.
        EMPTY_VIEW.getInt8(0);
    }
    catch (e) {
        if (!(e instanceof RangeError)) {
            throw new Error("This module is not supported in the current JavaScript engine because DataView does not throw RangeError on out-of-bounds access");
        }
    }
    const DataViewIndexOutOfBoundsError = RangeError;
    const MORE_DATA = new DataViewIndexOutOfBoundsError("Insufficient data");
    const sharedCachedKeyDecoder = new CachedKeyDecoder();
    class Decoder {
        extensionCodec;
        context;
        useBigInt64;
        maxStrLength;
        maxBinLength;
        maxArrayLength;
        maxMapLength;
        maxExtLength;
        keyDecoder;
        totalPos = 0;
        pos = 0;
        view = EMPTY_VIEW;
        bytes = EMPTY_BYTES;
        headByte = HEAD_BYTE_REQUIRED;
        stack = new StackPool();
        constructor(options) {
            this.extensionCodec = options?.extensionCodec ?? ExtensionCodec.defaultCodec;
            this.context = options?.context; // needs a type assertion because EncoderOptions has no context property when ContextType is undefined
            this.useBigInt64 = options?.useBigInt64 ?? false;
            this.maxStrLength = options?.maxStrLength ?? UINT32_MAX;
            this.maxBinLength = options?.maxBinLength ?? UINT32_MAX;
            this.maxArrayLength = options?.maxArrayLength ?? UINT32_MAX;
            this.maxMapLength = options?.maxMapLength ?? UINT32_MAX;
            this.maxExtLength = options?.maxExtLength ?? UINT32_MAX;
            this.keyDecoder = options?.keyDecoder !== undefined ? options.keyDecoder : sharedCachedKeyDecoder;
        }
        reinitializeState() {
            this.totalPos = 0;
            this.headByte = HEAD_BYTE_REQUIRED;
            this.stack.reset();
            // view, bytes, and pos will be re-initialized in setBuffer()
        }
        setBuffer(buffer) {
            this.bytes = ensureUint8Array(buffer);
            this.view = createDataView(this.bytes);
            this.pos = 0;
        }
        appendBuffer(buffer) {
            if (this.headByte === HEAD_BYTE_REQUIRED && !this.hasRemaining(1)) {
                this.setBuffer(buffer);
            }
            else {
                const remainingData = this.bytes.subarray(this.pos);
                const newData = ensureUint8Array(buffer);
                // concat remainingData + newData
                const newBuffer = new Uint8Array(remainingData.length + newData.length);
                newBuffer.set(remainingData);
                newBuffer.set(newData, remainingData.length);
                this.setBuffer(newBuffer);
            }
        }
        hasRemaining(size) {
            return this.view.byteLength - this.pos >= size;
        }
        createExtraByteError(posToShow) {
            const { view, pos } = this;
            return new RangeError(`Extra ${view.byteLength - pos} of ${view.byteLength} byte(s) found at buffer[${posToShow}]`);
        }
        /**
         * @throws {@link DecodeError}
         * @throws {@link RangeError}
         */
        decode(buffer) {
            this.reinitializeState();
            this.setBuffer(buffer);
            const object = this.doDecodeSync();
            if (this.hasRemaining(1)) {
                throw this.createExtraByteError(this.pos);
            }
            return object;
        }
        *decodeMulti(buffer) {
            this.reinitializeState();
            this.setBuffer(buffer);
            while (this.hasRemaining(1)) {
                yield this.doDecodeSync();
            }
        }
        async decodeAsync(stream) {
            let decoded = false;
            let object;
            for await (const buffer of stream) {
                if (decoded) {
                    throw this.createExtraByteError(this.totalPos);
                }
                this.appendBuffer(buffer);
                try {
                    object = this.doDecodeSync();
                    decoded = true;
                }
                catch (e) {
                    if (!(e instanceof DataViewIndexOutOfBoundsError)) {
                        throw e; // rethrow
                    }
                    // fallthrough
                }
                this.totalPos += this.pos;
            }
            if (decoded) {
                if (this.hasRemaining(1)) {
                    throw this.createExtraByteError(this.totalPos);
                }
                return object;
            }
            const { headByte, pos, totalPos } = this;
            throw new RangeError(`Insufficient data in parsing ${prettyByte(headByte)} at ${totalPos} (${pos} in the current buffer)`);
        }
        decodeArrayStream(stream) {
            return this.decodeMultiAsync(stream, true);
        }
        decodeStream(stream) {
            return this.decodeMultiAsync(stream, false);
        }
        async *decodeMultiAsync(stream, isArray) {
            let isArrayHeaderRequired = isArray;
            let arrayItemsLeft = -1;
            for await (const buffer of stream) {
                if (isArray && arrayItemsLeft === 0) {
                    throw this.createExtraByteError(this.totalPos);
                }
                this.appendBuffer(buffer);
                if (isArrayHeaderRequired) {
                    arrayItemsLeft = this.readArraySize();
                    isArrayHeaderRequired = false;
                    this.complete();
                }
                try {
                    while (true) {
                        yield this.doDecodeSync();
                        if (--arrayItemsLeft === 0) {
                            break;
                        }
                    }
                }
                catch (e) {
                    if (!(e instanceof DataViewIndexOutOfBoundsError)) {
                        throw e; // rethrow
                    }
                    // fallthrough
                }
                this.totalPos += this.pos;
            }
        }
        doDecodeSync() {
            DECODE: while (true) {
                const headByte = this.readHeadByte();
                let object;
                if (headByte >= 0xe0) {
                    // negative fixint (111x xxxx) 0xe0 - 0xff
                    object = headByte - 0x100;
                }
                else if (headByte < 0xc0) {
                    if (headByte < 0x80) {
                        // positive fixint (0xxx xxxx) 0x00 - 0x7f
                        object = headByte;
                    }
                    else if (headByte < 0x90) {
                        // fixmap (1000 xxxx) 0x80 - 0x8f
                        const size = headByte - 0x80;
                        if (size !== 0) {
                            this.pushMapState(size);
                            this.complete();
                            continue DECODE;
                        }
                        else {
                            object = {};
                        }
                    }
                    else if (headByte < 0xa0) {
                        // fixarray (1001 xxxx) 0x90 - 0x9f
                        const size = headByte - 0x90;
                        if (size !== 0) {
                            this.pushArrayState(size);
                            this.complete();
                            continue DECODE;
                        }
                        else {
                            object = [];
                        }
                    }
                    else {
                        // fixstr (101x xxxx) 0xa0 - 0xbf
                        const byteLength = headByte - 0xa0;
                        object = this.decodeUtf8String(byteLength, 0);
                    }
                }
                else if (headByte === 0xc0) {
                    // nil
                    object = null;
                }
                else if (headByte === 0xc2) {
                    // false
                    object = false;
                }
                else if (headByte === 0xc3) {
                    // true
                    object = true;
                }
                else if (headByte === 0xca) {
                    // float 32
                    object = this.readF32();
                }
                else if (headByte === 0xcb) {
                    // float 64
                    object = this.readF64();
                }
                else if (headByte === 0xcc) {
                    // uint 8
                    object = this.readU8();
                }
                else if (headByte === 0xcd) {
                    // uint 16
                    object = this.readU16();
                }
                else if (headByte === 0xce) {
                    // uint 32
                    object = this.readU32();
                }
                else if (headByte === 0xcf) {
                    // uint 64
                    if (this.useBigInt64) {
                        object = this.readU64AsBigInt();
                    }
                    else {
                        object = this.readU64();
                    }
                }
                else if (headByte === 0xd0) {
                    // int 8
                    object = this.readI8();
                }
                else if (headByte === 0xd1) {
                    // int 16
                    object = this.readI16();
                }
                else if (headByte === 0xd2) {
                    // int 32
                    object = this.readI32();
                }
                else if (headByte === 0xd3) {
                    // int 64
                    if (this.useBigInt64) {
                        object = this.readI64AsBigInt();
                    }
                    else {
                        object = this.readI64();
                    }
                }
                else if (headByte === 0xd9) {
                    // str 8
                    const byteLength = this.lookU8();
                    object = this.decodeUtf8String(byteLength, 1);
                }
                else if (headByte === 0xda) {
                    // str 16
                    const byteLength = this.lookU16();
                    object = this.decodeUtf8String(byteLength, 2);
                }
                else if (headByte === 0xdb) {
                    // str 32
                    const byteLength = this.lookU32();
                    object = this.decodeUtf8String(byteLength, 4);
                }
                else if (headByte === 0xdc) {
                    // array 16
                    const size = this.readU16();
                    if (size !== 0) {
                        this.pushArrayState(size);
                        this.complete();
                        continue DECODE;
                    }
                    else {
                        object = [];
                    }
                }
                else if (headByte === 0xdd) {
                    // array 32
                    const size = this.readU32();
                    if (size !== 0) {
                        this.pushArrayState(size);
                        this.complete();
                        continue DECODE;
                    }
                    else {
                        object = [];
                    }
                }
                else if (headByte === 0xde) {
                    // map 16
                    const size = this.readU16();
                    if (size !== 0) {
                        this.pushMapState(size);
                        this.complete();
                        continue DECODE;
                    }
                    else {
                        object = {};
                    }
                }
                else if (headByte === 0xdf) {
                    // map 32
                    const size = this.readU32();
                    if (size !== 0) {
                        this.pushMapState(size);
                        this.complete();
                        continue DECODE;
                    }
                    else {
                        object = {};
                    }
                }
                else if (headByte === 0xc4) {
                    // bin 8
                    const size = this.lookU8();
                    object = this.decodeBinary(size, 1);
                }
                else if (headByte === 0xc5) {
                    // bin 16
                    const size = this.lookU16();
                    object = this.decodeBinary(size, 2);
                }
                else if (headByte === 0xc6) {
                    // bin 32
                    const size = this.lookU32();
                    object = this.decodeBinary(size, 4);
                }
                else if (headByte === 0xd4) {
                    // fixext 1
                    object = this.decodeExtension(1, 0);
                }
                else if (headByte === 0xd5) {
                    // fixext 2
                    object = this.decodeExtension(2, 0);
                }
                else if (headByte === 0xd6) {
                    // fixext 4
                    object = this.decodeExtension(4, 0);
                }
                else if (headByte === 0xd7) {
                    // fixext 8
                    object = this.decodeExtension(8, 0);
                }
                else if (headByte === 0xd8) {
                    // fixext 16
                    object = this.decodeExtension(16, 0);
                }
                else if (headByte === 0xc7) {
                    // ext 8
                    const size = this.lookU8();
                    object = this.decodeExtension(size, 1);
                }
                else if (headByte === 0xc8) {
                    // ext 16
                    const size = this.lookU16();
                    object = this.decodeExtension(size, 2);
                }
                else if (headByte === 0xc9) {
                    // ext 32
                    const size = this.lookU32();
                    object = this.decodeExtension(size, 4);
                }
                else {
                    throw new DecodeError(`Unrecognized type byte: ${prettyByte(headByte)}`);
                }
                this.complete();
                const stack = this.stack;
                while (stack.length > 0) {
                    // arrays and maps
                    const state = stack.top();
                    if (state.type === STATE_ARRAY) {
                        state.array[state.position] = object;
                        state.position++;
                        if (state.position === state.size) {
                            object = state.array;
                            stack.release(state);
                        }
                        else {
                            continue DECODE;
                        }
                    }
                    else if (state.type === STATE_MAP_KEY) {
                        if (!isValidMapKeyType(object)) {
                            throw new DecodeError("The type of key must be string or number but " + typeof object);
                        }
                        if (object === "__proto__") {
                            throw new DecodeError("The key __proto__ is not allowed");
                        }
                        state.key = object;
                        state.type = STATE_MAP_VALUE;
                        continue DECODE;
                    }
                    else {
                        // it must be `state.type === State.MAP_VALUE` here
                        state.map[state.key] = object;
                        state.readCount++;
                        if (state.readCount === state.size) {
                            object = state.map;
                            stack.release(state);
                        }
                        else {
                            state.key = null;
                            state.type = STATE_MAP_KEY;
                            continue DECODE;
                        }
                    }
                }
                return object;
            }
        }
        readHeadByte() {
            if (this.headByte === HEAD_BYTE_REQUIRED) {
                this.headByte = this.readU8();
                // console.log("headByte", prettyByte(this.headByte));
            }
            return this.headByte;
        }
        complete() {
            this.headByte = HEAD_BYTE_REQUIRED;
        }
        readArraySize() {
            const headByte = this.readHeadByte();
            switch (headByte) {
                case 0xdc:
                    return this.readU16();
                case 0xdd:
                    return this.readU32();
                default: {
                    if (headByte < 0xa0) {
                        return headByte - 0x90;
                    }
                    else {
                        throw new DecodeError(`Unrecognized array type byte: ${prettyByte(headByte)}`);
                    }
                }
            }
        }
        pushMapState(size) {
            if (size > this.maxMapLength) {
                throw new DecodeError(`Max length exceeded: map length (${size}) > maxMapLengthLength (${this.maxMapLength})`);
            }
            this.stack.pushMapState(size);
        }
        pushArrayState(size) {
            if (size > this.maxArrayLength) {
                throw new DecodeError(`Max length exceeded: array length (${size}) > maxArrayLength (${this.maxArrayLength})`);
            }
            this.stack.pushArrayState(size);
        }
        decodeUtf8String(byteLength, headerOffset) {
            if (byteLength > this.maxStrLength) {
                throw new DecodeError(`Max length exceeded: UTF-8 byte length (${byteLength}) > maxStrLength (${this.maxStrLength})`);
            }
            if (this.bytes.byteLength < this.pos + headerOffset + byteLength) {
                throw MORE_DATA;
            }
            const offset = this.pos + headerOffset;
            let object;
            if (this.stateIsMapKey() && this.keyDecoder?.canBeCached(byteLength)) {
                object = this.keyDecoder.decode(this.bytes, offset, byteLength);
            }
            else {
                object = utf8Decode(this.bytes, offset, byteLength);
            }
            this.pos += headerOffset + byteLength;
            return object;
        }
        stateIsMapKey() {
            if (this.stack.length > 0) {
                const state = this.stack.top();
                return state.type === STATE_MAP_KEY;
            }
            return false;
        }
        decodeBinary(byteLength, headOffset) {
            if (byteLength > this.maxBinLength) {
                throw new DecodeError(`Max length exceeded: bin length (${byteLength}) > maxBinLength (${this.maxBinLength})`);
            }
            if (!this.hasRemaining(byteLength + headOffset)) {
                throw MORE_DATA;
            }
            const offset = this.pos + headOffset;
            const object = this.bytes.subarray(offset, offset + byteLength);
            this.pos += headOffset + byteLength;
            return object;
        }
        decodeExtension(size, headOffset) {
            if (size > this.maxExtLength) {
                throw new DecodeError(`Max length exceeded: ext length (${size}) > maxExtLength (${this.maxExtLength})`);
            }
            const extType = this.view.getInt8(this.pos + headOffset);
            const data = this.decodeBinary(size, headOffset + 1 /* extType */);
            return this.extensionCodec.decode(data, extType, this.context);
        }
        lookU8() {
            return this.view.getUint8(this.pos);
        }
        lookU16() {
            return this.view.getUint16(this.pos);
        }
        lookU32() {
            return this.view.getUint32(this.pos);
        }
        readU8() {
            const value = this.view.getUint8(this.pos);
            this.pos++;
            return value;
        }
        readI8() {
            const value = this.view.getInt8(this.pos);
            this.pos++;
            return value;
        }
        readU16() {
            const value = this.view.getUint16(this.pos);
            this.pos += 2;
            return value;
        }
        readI16() {
            const value = this.view.getInt16(this.pos);
            this.pos += 2;
            return value;
        }
        readU32() {
            const value = this.view.getUint32(this.pos);
            this.pos += 4;
            return value;
        }
        readI32() {
            const value = this.view.getInt32(this.pos);
            this.pos += 4;
            return value;
        }
        readU64() {
            const value = getUint64(this.view, this.pos);
            this.pos += 8;
            return value;
        }
        readI64() {
            const value = getInt64(this.view, this.pos);
            this.pos += 8;
            return value;
        }
        readU64AsBigInt() {
            const value = this.view.getBigUint64(this.pos);
            this.pos += 8;
            return value;
        }
        readI64AsBigInt() {
            const value = this.view.getBigInt64(this.pos);
            this.pos += 8;
            return value;
        }
        readF32() {
            const value = this.view.getFloat32(this.pos);
            this.pos += 4;
            return value;
        }
        readF64() {
            const value = this.view.getFloat64(this.pos);
            this.pos += 8;
            return value;
        }
    }

    /**
     * It decodes a single MessagePack object in a buffer.
     *
     * This is a synchronous decoding function.
     * See other variants for asynchronous decoding: {@link decodeAsync}, {@link decodeStream}, or {@link decodeArrayStream}.
     *
     * @throws {@link RangeError} if the buffer is incomplete, including the case where the buffer is empty.
     * @throws {@link DecodeError} if the buffer contains invalid data.
     */
    function decode(buffer, options) {
        const decoder = new Decoder(options);
        return decoder.decode(buffer);
    }

    class MsgPackSerializer {
        deserialize(value) {
            return decode(value);
        }
        serialize(value, _options) {
            return encode(value).buffer;
        }
    }

    const serializer = new MsgPackSerializer();
    function createPromise() {
        const result = {};
        const promise = new Promise(function (resolve, reject) {
            result.resolve = function (result) {
                resolve(isArrayBuffer(result)
                    ? serializer.deserialize(result)
                    : result);
            };
            result.reject = reject;
        });
        result.promise = promise;
        return result;
    }
    function createInterceptor(targetObj) {
        const functionsMap = new Map();
        const handler = {
            get(target, propKey, receiver) {
                const func = functionsMap.get(propKey);
                if (func) {
                    return func;
                }
                const targetValue = Reflect.get(target, propKey);
                if (!isFunction(targetValue)) {
                    return targetValue;
                }
                const interceptor = function (...args) {
                    let parameters = args;
                    if (args.length > targetValue.length) {
                        parameters = args.slice(0, targetValue.length);
                        parameters.push(args.slice(targetValue.length));
                    }
                    const convertedArgs = parameters.length === 0
                        ? parameters
                        : [serializer.serialize(parameters, { skipReferenceForInitialArrayObject: true })];
                    return targetValue.apply(target, convertedArgs);
                };
                functionsMap.set(propKey, interceptor);
                return interceptor;
            }
        };
        return new Proxy(targetObj, handler);
    }
    function checkObjectBound(objName) {
        //native function Bind()
        if (window.hasOwnProperty(objName)) {
            // quick check
            return Promise.resolve(true);
        }
        return Bind(objName);
    }
    function deleteObjectBound(objName) {
        //native function Unbind()
        Unbind(objName);
    }
    function evaluateScript(fn) {
        return serializer.serialize(fn());
    }

    exports.checkObjectBound = checkObjectBound;
    exports.createInterceptor = createInterceptor;
    exports.createPromise = createPromise;
    exports.deleteObjectBound = deleteObjectBound;
    exports.evaluateScript = evaluateScript;

    return exports;

})({});
