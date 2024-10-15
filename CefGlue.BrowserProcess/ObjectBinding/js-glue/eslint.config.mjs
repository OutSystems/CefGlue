import globals from "globals";
import pluginJs from "@eslint/js";
import tseslint from "typescript-eslint";
import prettier from 'prettier';

export default [
   {
      plugins: {
         prettier
      },
      files: ["**/*.{js,mjs,cjs,ts}"],
      languageOptions: { globals: globals.browser },
      rules: {
         'prettier/prettier': 'error',
      }
   },
   prettier.configs.recommended,
   pluginJs.configs.recommended,
   tseslint.configs.recommended,
];
