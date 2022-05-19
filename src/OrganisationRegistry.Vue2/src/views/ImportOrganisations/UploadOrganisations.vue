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
    />
    <vl-button @click="uploadFile">Opladen</vl-button>
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
    },
  },
};
</script>

<style scoped></style>
