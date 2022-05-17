<template>
  <div>
    <vl-upload
      id="bulkimportfile"
      max-files="1"
      name="bulkimportfile"
      allowed-file-types="text/csv"
      url="/"
      @upload-file-added="fileAdded"
    />
    <vl-button @click="select">Process</vl-button>
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
    select() {
      console.log("attempting selection of file");
      if (this.file) {
        console.log("file selected");
        this.$emit("file-selected", this.file);
      }
    },
    fileAdded(e) {
      this.file = e;
    },
  },
};
</script>

<style scoped></style>
