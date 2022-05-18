<template>
  <div class="home">
    <h1>Opladen Organisaties</h1>
    <UploadOrganisations @file-selected="select" />
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
    async select(file) {
      await postImportOrganisations(file);
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
