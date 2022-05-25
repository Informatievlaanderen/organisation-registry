<template>
  <div>
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
        this.$emit("file-selected", this.file, this.clearUploads);
      }
    },
    fileAdded(e) {
      this.file = e;
      this.$emit("file-added");
    },
    fileRemoved() {
      this.file = undefined;
    },
  },
};
</script>

<style scoped></style>
