<template>
  <div class="home">
    <div class="cta-title">
      <h1 class="h2 cta-title__title">Opladen organisaties</h1>
    </div>
    <UploadOrganisations @file-selected="uploadFile" />
    <ImportStatusList
      :import-statuses="importStatuses.imports"
      :isLoading="isLoading"
      @refresh="refresh"
    />
  </div>
</template>

<script>
import {
  getImportStatuses,
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
      this.importStatuses = await getImportStatuses();
    },
    async refresh() {
      try {
        this.isLoading = true;
        this.importStatuses = await getImportStatuses();
      } finally {
        this.isLoading = false;
      }
    },
  },
  async mounted() {
    await this.refresh();
  },
  data() {
    return {
      importStatuses: {},
      isLoading: false,
    };
  },
};
</script>
