<template>
  <component :is="type" :class="classes">

    <span aria-hidden="true" class="bl-panel__header__toggle vi vi-u-badge vi-u-badge--small vi-arrow vi-u-90deg" v-if="modIsAccordion"></span>

    <div class="bl-panel__header__badges" aria-hidden="true" v-if="badges">
      <bl-badge v-for="(badge, index) in badges" :key="index" :icon="badge.icon" :initials="badge.initials" :mod-is-alt="badge.modIsAlt" :mod-is-success="badge.modIsSuccess" :mod-is-accent="badge.modIsAccent" :mod-is-overlap="badge.modIsOverlap" :mod-is-placeholder="badge.modIsPlaceholder" :mod-is-bordered="badge.modIsBordered" :mod-is-large="badge.modIsLarge"></bl-badge>
    </div>

    <div class="bl-panel__header__content" :class="{'bl-panel__header__content--flex': extra && extra.length}">
      <div>
        <h1 class="bl-panel__header__title" v-if="title">
          {{ title }}
        </h1>
        <h2 class="bl-panel__header__subtitle" v-if="subtitle">
          {{ subtitle }}
        </h2>
      </div>
    </div>

    <div class="bl-panel__header__content__extra" v-if="extra">
      <p class="bl-panel__header__content__extra__item" v-for="(item, index) in extra" :key="index">{{ item.label }}</p>
    </div>

    <div class="bl-panel__header__info-toggle popover popover--right popover--xxl popover--has-footer js-popover" v-if="modHasInfoToggle">
      <button type="button" class="bl-panel__header__info-toggle__toggle js-popover__toggle">
        <i class="vi vi-u-badge vi-u-badge--action vi-u-badge--xsmall vi-info" aria-hidden="true"></i>
        <span class="u-visually-hidden">Open de accordion</span>
      </button>
      <div class="popover__content">
        <p>popover</p>
      </div>
    </div>

    <slot></slot>

  </component>
</template>

<script>
import BlBadge from '~components/partials/badge/Badge.vue'

export default {
  name: 'panel-header',
  props: {
    type: {
      type: String,
      default: 'header'
    },
    modHasAction: {
      type: Boolean,
      default: false
    },
    modIsAccordion: {
      type: Boolean,
      default: false
    },
    modHasInfoToggle: {
      type: Boolean,
      default: false
    },
    title: {
      type: String,
      default: ''
    },
    subtitle: {
      type: String,
      default: ''
    },
    badges: { // Badges shown on the left side of the panel header component
      type: Array,
      default: null
    },
    extra: { // Extra paragraphs on the right side of the panel header component
      type: Array,
      default: null
    }
  },
  components: {
    BlBadge
  },
  data () {
    return {
      classes: {
        'bl-panel__header': true,
        'bl-panel__header--has-action': this.modHasAction || this.modIsAccordion,
        'bl-panel__header--has-info-toggle': this.modHasInfoToggle,
        'js-accordion__toggle': this.modIsAccordion
      }
    }
  }
}
</script>
