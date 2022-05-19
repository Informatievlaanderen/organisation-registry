<template>
  <div class="vl-u-table-overflow">
    <vl-data-table>
      <caption>
        Import status
      </caption>
      <thead>
        <tr>
          <th>Bestandsnaam</th>
          <th>Status</th>
          <th>Resultaat</th>
          <th>Geïmporteerd op</th>
        </tr>
      </thead>
      <tbody>
        <tr :key="importStatus.id" v-for="importStatus in importStatuses">
          <td>{{ importStatus.fileName }}</td>
          <td>{{ importStatus.status }}</td>
          <td><a @click="downloadFile(importStatus)">Resultaat</a></td>
          <td
            :title="
              moment(importStatus.uploadedAt).format('yyyy-MM-DD hh:mm:ss')
            "
          >
            <span>{{ moment(importStatus.uploadedAt).fromNow() }}</span>
          </td>
        </tr>
      </tbody>
      <tfoot>
        <tr>
          <td><vl-annotation></vl-annotation></td>
        </tr>
      </tfoot>
    </vl-data-table>
  </div>
</template>

<script>
import { getImportFile } from "@/api/importOrganisations";

export default {
  name: "ImportStatusList",
  props: {
    importStatuses: Array,
  },
  methods: {
    async downloadFile({ id, fileName }) {
      const blob = await getImportFile(id);
      const link = document.createElement("a");
      link.href = URL.createObjectURL(blob);
      link.download = fileName;
      link.click();
    },
  },
};
</script>

<style scoped></style>
