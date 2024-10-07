import { decode, encode } from "@msgpack/msgpack"
import { Serializer } from "./main";

export class MsgPackSerializer implements Serializer {
   deserialize<T = any>(value: ArrayBuffer): T {
      return decode(value)as T;
   }
   serialize(value: any, _options?: Partial<{ skipReferenceForInitialArrayObject: boolean; }>): ArrayBuffer {
      return encode(value).buffer;
   }
}
