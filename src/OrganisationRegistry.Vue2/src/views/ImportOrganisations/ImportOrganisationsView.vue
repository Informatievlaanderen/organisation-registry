<template>
  <div class="home">
    <div class="cta-title">
      <h1 class="h2 cta-title__title">Opladen organisaties</h1>
    </div>
    <UploadOrganisations @file-selected="uploadFile" />
    <ImportStatusList :import-statuses="importStatuses.imports" />
  </div>
</template>

<script>
import {
  getImportStatus,
  postImportOrganisations,
} from "@/api/importOrganisations";
import UploadOrganisations from "@/views/ImportOrganisations/UploadOrganisations";
import ImportStatusList from "@/views/ImportOrganisations/ImportStatusList";

export default {
  name: "ImportOrganisationsView",
  components: { ImportStatusList, UploadOrganisations },
  methods: {
    async uploadFile(file, onSuccess) {
      await postImportOrganisations(file, onSuccess);
      this.importStatuses = await getImportStatus();
    },
  },
  async mounted() {
    this.importStatuses = await getImportStatus();
  },
  data() {
    return {
      importStatuses: {},
    };
  },
};
</script>
