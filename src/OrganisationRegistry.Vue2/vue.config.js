const { defineConfig } = require("@vue/cli-service");

module.exports = defineConfig({
  publicPath: "/v2/",
  chainWebpack: (config) => {
    // vue-loader 15 on Vue 2.7 emits `import style0 from "...?vue&type=style"` glue code that
    // webpack flags with "export 'default' was not found". Only the .vue glue is affected, so the
    // check is switched off for that rule only (real missing exports in your JS are still reported).
    config.module.rule("vue").parser({ exportsPresence: false });
  },
});
