import { Serializer } from "./main";
import { isString } from "./type-check";

const idPropertyName = "$id";
const refPropertyName = "$ref";
const valuesPropertyName = "$values";

// special data markers used to distinguish the several string types
enum TypeMarker {
   dateTime = "D",
   string = "S",
   binary = "B",
}

function convertBase64ToBinary(value: string) {
   const byteCharacters = atob(value);
   const byteArray = new Array(byteCharacters.length);
   for (let i = 0; i < byteCharacters.length; i++) {
      byteArray[i] = byteCharacters.charCodeAt(i);
   }
   return new Uint8Array(byteArray);;
}
function convertBinaryToBase64(byteArray: Uint8Array) {
   return btoa(String.fromCharCode(...byteArray));
}

export class JsonSerializer implements Serializer {
   static Instance = new JsonSerializer();

   static convertStringToJsType(value: string) {
      switch (value.substring(0, 1)) {
         case TypeMarker.string:
            return value.substring(1);
         case TypeMarker.dateTime:
            return new Date(value.substring(1));
         case TypeMarker.binary:
            return convertBase64ToBinary(value.substring(1));
      }
      return value;
   }

   deserialize<T = any>(value: ArrayBuffer): T {
      const refs = new Map();
      const pendingRefs = new Map();
      const textDecoder = new TextDecoder();
      return JSON.parse(textDecoder.decode(value), function (name, value) {
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
               return JsonSerializer.convertStringToJsType(value);
            }
         }
         return value;
      });
   }

   serialize(value: any, options: Partial<{ skipReferenceForInitialArrayObject: boolean; }> = {}): ArrayBuffer {
      const refs = new Map();
      const marker = Symbol("marker");
      const textEncoder = new TextEncoder();
      return textEncoder.encode(JSON.stringify(value, function (this: object, key: string, value: object) {
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
                  return TypeMarker.dateTime + value;
               }
               return TypeMarker.string + value;
            case "object":
               if (value instanceof Uint8Array) {
                  return TypeMarker.binary + convertBinaryToBase64(value);
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
            if (options.skipReferenceForInitialArrayObject && key === "") {
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
      })).buffer;
   }
}
