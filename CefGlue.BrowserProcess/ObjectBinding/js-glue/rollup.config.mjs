import typescript from '@rollup/plugin-typescript'
import terser from '@rollup/plugin-terser'
import nodeResolve from '@rollup/plugin-node-resolve'

export default {
   input: {
      'main': 'src/main.ts',
   },
   output: {
      format: 'iife',
      dir: 'dist',
      name: 'cefglue',
      // compact: true,
   },
   plugins: [
      typescript({ rootDir: 'src', declarationDir: 'dist', declaration: true }),
      // terser(),
      nodeResolve(),
   ]
}
