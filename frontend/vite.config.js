import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import autoprefixer from 'autoprefixer'

export default defineConfig({
  plugins: [
    vue(),
  ],

  base: './',

  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },

    extensions: [
      '.mjs',
      '.js',
      '.jsx',
      '.json',
      '.vue',
      '.scss',
    ],
  },

  css: {
    postcss: {
      plugins: [
        autoprefixer(),
      ],
    },
  },
})