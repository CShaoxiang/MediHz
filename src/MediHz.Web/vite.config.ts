import { readFileSync } from 'node:fs'
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

const useHttps =
  process.env.npm_lifecycle_event && process.env.npm_lifecycle_event.includes('https')
const apiTarget = useHttps ? 'https://localhost:7001' : 'http://localhost:5001'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    https: useHttps
      ? { cert: readFileSync('certs/localhost.pem'), key: readFileSync('certs/localhost.key') }
      : undefined,
    proxy: {
      '/api': {
        target: apiTarget,
        changeOrigin: true,
        secure: false,
      },
      '/hubs': {
        target: apiTarget,
        ws: true,
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
