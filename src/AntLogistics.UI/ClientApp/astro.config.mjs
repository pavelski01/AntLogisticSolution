// @ts-check
import { defineConfig } from "astro/config";

import react from "@astrojs/react";
import sitemap from "@astrojs/sitemap";
import tailwindcss from "@tailwindcss/vite";
import node from "@astrojs/node";

// https://astro.build/config
export default defineConfig({
  output: "server",
  integrations: [react(), sitemap()],
  server: {
    port: parseInt(process.env.PORT || "4321"),
    host: true,
  },
  vite: {
    plugins: [tailwindcss()],
    server: {
      proxy: {
        "/api": {
          target: process.env.services__core__http__0 || process.env.services__core__https__0 || "http://localhost:5000",
          changeOrigin: true,
          secure: false,
        },
      },
    },
  },
  adapter: node({
    mode: "standalone",
  }),
});
