<template>
  <div class="bl-card">
    <a href="#" class="bl-card__wrap">
      <div class="bl-card__content">
        <div class="document-miniature document-miniature--alt document-miniature--flex js-equal-height">
          <div class="document-miniature__type" v-if="modIsFile">
            <span class="document-miniature__type__text">{{ data.fileType }}</span>
          </div>
          <div class="document-miniature__content">
            <span class="document-miniature__title link" href="#">{{ data.title }}</span>
            <div class="document-miniature__metadata" v-if="data.subtitle">{{ data.subtitle }}</div>
          </div>
        </div>
      </div>
    </a>
  </div>
</template>

<script>

import BlDescriptionData from '~components/partials/description-data/DescriptionData.vue'
import BlDescriptionDataItem from '~components/partials/description-data/DescriptionDataItem.vue'
import BlDescriptionDataItemWrapper from '~components/partials/description-data/DescriptionDataItemWrapper.vue'

export default {
  name: 'temporary-card',
  props: {
    data: {
      default: {},
      type: Object
    },
    modIsFile: {
      default: false,
      type: Boolean
    },
    modIsLink: {
      default: false,
      type: Boolean
    }
  },
  components: {
    BlDescriptionData,
    BlDescriptionDataItem,
    BlDescriptionDataItemWrapper
  }
}
</script>

