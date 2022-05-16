<template>
  <component :is="elType" :class="classes">
    <slot></slot>
  </component>
</template>

<script>
export default {
  name: 'description-data',
  props: {
    type: {
      default: 'div',
      type: String
    },
    modIsBordered: {
      default: false,
      type: Boolean
    },
    modHasBadge: {
      default: false,
      type: Boolean
    },
    modIsInline: {
      default: false,
      type: Boolean
    }
  },
  data () {
    return {
      elType: this.type,
      classes: {
        'description-data': true,
        'bl-description-data': true,
        'description-data--bordered': this.modIsBordered,
        'bl-description-data--has-badge': this.modHasBadge,
        'bl-description-data--inline': this.modIsInline
      }
    }
  }
}
</script>
