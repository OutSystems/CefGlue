import { isFunction, isArrayBuffer } from "./type-check"
import { JsonSerializer } from "./json-serializer"
import { MsgPackSerializer } from "./msgpack-serializer";

export interface Serializer {
   deserialize<T = any>(value: ArrayBuffer): T;
   serialize(value: any, options?: Partial<{ skipReferenceForInitialArrayObject: boolean }>): ArrayBuffer;
}

const serializer2: Serializer = new JsonSerializer();
const serializer: Serializer = new MsgPackSerializer();

export function createPromise(): any {
   const result: any = {};
   const promise = new Promise(function (resolve, reject) {
      result.resolve = function (result: ArrayBuffer) {
         resolve(
            isArrayBuffer(result)
               ? serializer.deserialize(result)
               : result
         );
      };
      result.reject = reject;
   });
   result.promise = promise;
   return result;
}

export function createInterceptor(targetObj: any): object {
   const functionsMap = new Map();
   const handler: ProxyHandler<string[]> = {
      get(target, propKey, receiver) {
         const func = functionsMap.get(propKey);
         if (func) {
            return func;
         }
         const targetValue = Reflect.get(target, propKey);
         if (!isFunction(targetValue)) {
            return targetValue;
         }
         const interceptor = function (...args: any[]) {
            let parameters = args
            if (args.length > targetValue.length) {
               parameters = args.slice(0, targetValue.length)
               parameters.push(args.slice(targetValue.length))
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
declare function Unbind(objName: string): void;
declare function Bind(objName: string): any;

export function checkObjectBound(objName: string) {
   //native function Bind()
   if (window.hasOwnProperty(objName)) {
      // quick check
      return Promise.resolve(true);
   }
   return Bind(objName);
}

export function deleteObjectBound(objName: string) {
   //native function Unbind()
   Unbind(objName);
}

export function evaluateScript(fn: () => any) {
   return serializer.serialize(fn());
}
