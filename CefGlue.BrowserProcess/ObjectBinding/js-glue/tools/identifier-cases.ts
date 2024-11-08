function splitIdentifier(id: string): string[] {
   const matches = id.match(/\p{Lu}*[\p{Ll}\p{Nd}]+/gu);
   return matches ? [...matches] : [];
}
function toTitleCase(word: string): string {
   return word.charAt(0).toUpperCase() + word.slice(1).toLowerCase();
}
function toLowerCase(word: string): string {
   return word.toLowerCase();
}
function toUpperCase(word: string): string {
   return word.toUpperCase();
}
export function toPascalCase(id: string): string {
   return splitIdentifier(id).map(toTitleCase).join('');
}
export function toKebabCase(id: string): string {
   return splitIdentifier(id).map(toLowerCase).join('-');
}
export function toUpperSnakeCase(id: string): string {
   return splitIdentifier(id).map(toUpperCase).join('_');
}
export function toLowerSnakeCase(id: string): string {
   return splitIdentifier(id).map(toLowerCase).join('_');
}
export function toSnakeCase(id: string): string {
   return splitIdentifier(id).join('_');
}
export function toCamelCase(id: string) {
   const words = splitIdentifier(id);
   return words.length > 0 ? toLowerCase(words.shift()!) + words.map(toTitleCase).join('') : '';
}
