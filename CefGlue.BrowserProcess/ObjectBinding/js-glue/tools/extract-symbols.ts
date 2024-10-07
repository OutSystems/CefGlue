import { readFileSync, writeFileSync } from 'node:fs';
import { format, parse } from 'node:path';
import { argv } from 'node:process';
import { FunctionDeclaration, Node, ScriptTarget, SyntaxKind, createSourceFile, isFunctionDeclaration } from 'typescript';
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

function* getFunctionDeclarations(node: Node): Iterable<FunctionDeclaration> {
   for (const childNode of node.getChildren()) {
      if (isFunctionDeclaration(childNode) && hasExportModifier(childNode)) {
         yield childNode;
      }
      yield* getFunctionDeclarations(childNode)
   }
}

const declarations = [...getFunctionDeclarations(sourceFile)]
const formatSymbol = (decl: FunctionDeclaration) => {
   const name = decl.name?.getText() ?? ''
   return `   public const string ${toPascalCase(name)} = "${name}";`
}
const sourceCode = `
namespace ${namespace};

public static class ${toPascalCase(module)}
{
${declarations.map(formatSymbol).join('\n')}
}
`
writeFileSync(target, sourceCode)
