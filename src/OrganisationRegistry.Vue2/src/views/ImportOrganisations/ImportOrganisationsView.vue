<template>
  <div class="home">
    <div class="cta-title">
      <h1 class="h2 cta-title__title">Opladen organisaties</h1>
    </div>
    <UploadOrganisations @file-selected="uploadFile" @file-added="fileAdded" />
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
import { useAlertStore } from "@/stores/alert";
import Alerts from "@/alerts/alerts";

export default {
  name: "ImportOrganisationsView",
  components: { ImportStatusList, UploadOrganisations },
  methods: {
    async uploadFile(file, clearUploadOrganisations) {
      await postImportOrganisations({
        file,
        onSuccess: async () => {
          const alertStore = useAlertStore();
          alertStore.$reset();
          clearUploadOrganisations();
          this.importStatuses = await getImportStatuses();
        },
        onError: this.showError,
      });
    },
    async fileAdded() {
      const alertStore = useAlertStore();
      alertStore.$reset();
    },
    async refresh() {
      try {
        this.isLoading = true;
        this.importStatuses = await getImportStatuses();
      } finally {
        this.isLoading = false;
      }
    },
    async showError(response) {
      const error = await response.json();
      if (!error.isValid) {
        const message = error.validationIssues
          .map((issue) => issue.description)
          .reduce((aggregated, issue) => `${aggregated}\n${issue}`);

        const alertStore = useAlertStore();
        alertStore.setAlert(Alerts.createDomainError(message));
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
