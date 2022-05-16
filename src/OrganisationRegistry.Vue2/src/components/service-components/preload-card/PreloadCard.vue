<template>
  <div class="js-preloading">
    <div class="bl-card">
      <div class="bl-card__header" v-if="modHasHeader">
        <div class="bl-card__header__inner">
          <div class="bl-card__badges" aria-hidden="true" v-if="modHasBadge">
            <bl-badge :mod-is-alt="true"></bl-badge>
          </div>
          <div class="bl-card__header__content">
            <h1 class="bl-card__header__title"></h1>
            <h2 class="bl-card__header__meta"></h2>
          </div>
        </div>
      </div>
      <div class="bl-card__content" v-if="modHasContent"></div>
    </div>
  </div>
</template>

<script>
import BlBadge from '~components/partials/badge/Badge.vue'

export default {
  name: 'preload-card',
  props: {
    modHasBadge: {
      default: true,
      type: Boolean
    },
    modHasHeader: {
      default: true,
      type: Boolean
    },
    modHasContent: {
      default: true,
      type: Boolean
    }
  },
  components: {
    BlBadge
  }
}
</script>
