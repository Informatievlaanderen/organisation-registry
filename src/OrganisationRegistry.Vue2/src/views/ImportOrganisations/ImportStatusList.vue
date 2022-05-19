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
          <td>Resultaat</td>
          <td>{{ importStatus.uploadedAt }}</td>
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
export default {
  name: "ImportStatusList",
  props: {
    importStatuses: Array,
  },
};
</script>

<style scoped></style>
