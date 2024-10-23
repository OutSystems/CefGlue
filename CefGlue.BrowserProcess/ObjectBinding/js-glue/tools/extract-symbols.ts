import { readFileSync, writeFileSync } from 'node:fs';
import { format, parse } from 'node:path';
import { argv } from 'node:process';
import { Declaration, DeclarationStatement, FunctionDeclaration, NamedDeclaration, Node, ScriptTarget, SyntaxKind, createSourceFile, isFunctionDeclaration, isVariableDeclaration } from 'typescript';
import { toPascalCase } from './identifier-cases';

if (argv.length < 5) {
   console.error('Usage: extract-symbols <source-file> <target-file> <namespace>')
   process.exit(1)
}  

const source = removeQuotes(argv[2])
const target = removeQuotes(argv[3])
const namespace = removeQuotes(argv[4])
const module = parse(target).name
const sourceFile = createSourceFile(
   source,
   readFileSync(source, 'utf8'),
   ScriptTarget.Latest,
   true
)

function removeQuotes(argument: string) {
   if ((argument.startsWith('"') && argument.endsWith('"')) || (argument.startsWith("'") && argument.endsWith("'"))) {
      return argument.slice(1, -1);
   }
   return argument;
}

function hasExportModifier(declaration: FunctionDeclaration): boolean {
   return declaration.modifiers?.some(modifier => modifier.kind === SyntaxKind.ExportKeyword) ?? false;
}

function* getFunctionDeclarations(node: Node): Iterable<Declaration> {
   for (const childNode of node.getChildren()) {
      if (isFunctionDeclaration(childNode) && hasExportModifier(childNode)) {
         yield childNode;
      }
      yield* getFunctionDeclarations(childNode)
   }
}

function* getVariableDeclarations(node: Node): Iterable<Declaration> {
   for (const childNode of node.getChildren()) {
      if (isVariableDeclaration(childNode)) {
         yield childNode;
      }
      yield* getVariableDeclarations(childNode)
   }
}

const functionDeclarations = [...getFunctionDeclarations(sourceFile)]
const variableDeclarations = [...getVariableDeclarations(sourceFile)]
function formatNamedDeclaration(decl: NamedDeclaration) {
   const name = decl.name?.getText() ?? ''
   return `   public const string ${toPascalCase(name)} = "${name}";`
}
writeFileSync(target+"Function.cs", `
namespace ${namespace};

public static class ${toPascalCase(module)+"Function"}
{
${functionDeclarations.map(formatNamedDeclaration).join('\n')}
}
`)

writeFileSync(target+"Constant.cs", `
namespace ${namespace};

public static class ${toPascalCase(module)+"Constant"}
{
${variableDeclarations.map(formatNamedDeclaration).join('\n')}
}
`)
