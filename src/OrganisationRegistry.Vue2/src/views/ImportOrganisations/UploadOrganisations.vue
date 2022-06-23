<template>
  <div>
    <div>
      <label for="selectUploadType" class="form__label">Soort import: </label>
      <select
        name="selectUploadType"
        class="select"
        id="select-demo"
        ref="selectUploadType"
      >
        <option value="Create">{{ translateFileType("create") }}</option>
        <option value="Terminate">{{ translateFileType("stop") }}</option>
      </select>
    </div>
    <vl-upload
      id="bulkimportfile"
      max-files="1"
      name="bulkimportfile"
      allowed-file-types="text/csv"
      url="/"
      ref="uploadControl"
      @upload-file-added="fileAdded"
      @upload-removed-file="fileRemoved"
    />
    <vl-button @click="uploadFile" :mod-disabled="this.file === undefined">
      Opladen
    </vl-button>
  </div>
</template>

<script>
document.title = "Wegwijs - Importeren";

import { translateImportFileType } from "./Translations";

export default {
  name: "UploadOrganisations",
  data: () => {
    return {
      file: undefined,
    };
  },
  methods: {
    clearUploads() {
      this.$refs.uploadControl.removeFiles();
    },
    uploadFile() {
      if (this.file) {
        this.$emit(
          "file-selected",
          this.file,
          this.$refs.selectUploadType.value,
          this.clearUploads
        );
      }
    },
    fileAdded(e) {
      this.file = e;
      this.$emit("file-added");
    },
    fileRemoved() {
      this.file = undefined;
    },
    translateFileType(key) {
      return translateImportFileType(key);
    },
  },
};
</script>

<style scoped></style>
