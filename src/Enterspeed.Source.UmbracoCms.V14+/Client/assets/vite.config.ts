import { defineConfig } from "vite";

export default defineConfig({
    build: {
        lib: {
            entry: "src/index.ts", // your web component source file
            formats: ["es"],
        },
        outDir: "../../../../../enterspeed-test-sites/U14/wwwroot/App_Plugins/Enterspeed.Source.UmbracoCms", 
        emptyOutDir: true,
        sourcemap: true,
        rollupOptions: {
            external: [/^@umbraco/], 
        },
    },
});