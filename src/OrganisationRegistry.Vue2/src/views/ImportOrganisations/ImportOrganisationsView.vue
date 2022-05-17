<template>
  <div class="home">
    <h1>Opladen Organisaties</h1>
    <UploadOrganisations @file-selected="select" />
  </div>
</template>

<script>
import { postImportOrganisations } from "@/api/importOrganisations";
import UploadOrganisations from "@/views/ImportOrganisations/UploadOrganisations";

export default {
  name: "ImportOrganisationsView",
  components: { UploadOrganisations },
  methods: {
    async select(file) {
      await postImportOrganisations(file);
    },
  },
};
</script>
