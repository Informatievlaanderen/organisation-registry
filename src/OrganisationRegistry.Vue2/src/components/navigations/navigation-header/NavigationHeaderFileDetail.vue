<template>
  <div class="bl-navigation-header__file-detail">
    <bl-layout :mod-is-wide="true">
      <bl-grid>
        <bl-column>
          <div class="bl-navigation-header__file-detail__content">
            <bl-grid :mod-is-stacked="true">
              <bl-column :cols="[{nom: 1, den: 4}, {nom: 1, den: 2, mod: 's'}, {nom: 1, den: 1, mod: 'xs'}]" v-if="file.id">
                <bl-description-data-item class-type="label">Dossiernummer</bl-description-data-item>
                <bl-description-data-item class-type="value">{{ file.id }}</bl-description-data-item>
              </bl-column>
              <bl-column :cols="[{nom: 1, den: 4}, {nom: 1, den: 2, mod: 's'}, {nom: 1, den: 1, mod: 'xs'}]" v-if="file.type">
                <bl-description-data-item class-type="label">Type</bl-description-data-item>
                <bl-description-data-item class-type="value">{{ file.type }}</bl-description-data-item>
              </bl-column>
              <bl-column :cols="[{nom: 1, den: 4}, {nom: 1, den: 2, mod: 's'}, {nom: 1, den: 1, mod: 'xs'}]" v-if="file.admin">
                <bl-description-data-item class-type="label">Dossierbeheerder</bl-description-data-item>
                <bl-description-data-item type="div" class-type="value">
                  <p v-if="file.admin.name">{{ file.admin.name }}</p>
                  <p v-if="file.admin.service">{{ file.admin.service }}</p>
                  <p v-if="file.admin.phone">{{ file.admin.phone }}</p>
                  <p v-if="file.admin.email"><a :href="'mailto:' + file.admin.email">{{ file.admin.email }}</a></p>
                  <p v-if="file.admin.website"><a :href="file.admin.website" target="_BLANK">Website</a></p>
                </bl-description-data-item>
              </bl-column>
              <bl-column :cols="[{nom: 1, den: 4}, {nom: 1, den: 2, mod: 's'}, {nom: 1, den: 1, mod: 'xs'}]" v-if="file.reference">
                <div class="u-align-right">
                  <a :href="file.reference" class="button" target="_BLANK">Naar het dossier</a>
                </div>
              </bl-column>
            </bl-grid>
          </div>
        </bl-column>
      </bl-grid>
    </bl-layout>
  </div>
</template>

<script>

import BlDescriptionData from '~components/partials/description-data/DescriptionData.vue'
import BlDescriptionDataItem from '~components/partials/description-data/DescriptionDataItem.vue'

export default {
  name: 'navigation-header-file-detail',
  props: {
    file: {
      default: {},
      type: Object
    }
  },
  components: {
    BlDescriptionData,
    BlDescriptionDataItem
  }
}
</script>
