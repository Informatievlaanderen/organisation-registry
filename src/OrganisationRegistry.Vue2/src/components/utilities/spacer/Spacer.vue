<template>
  <div class="classes"></div>
</template>

<script>
export default {
  name: 'spacer',
  props: {
    modIsTiny: {
      default: false,
      type: Boolean
    },
    modIsSmall: {
      default: false,
      type: Boolean
    },
    modIsLarge: {
      default: false,
      type: Boolean
    },
    modIsNone: {
      default: false,
      type: Boolean
    }
  },
  data () {
    return {
      classes: {
        'u-spacer': true,
        'u-spacer--tiny': this.modIsTiny,
        'u-spacer--small': this.modIsSmall,
        'u-spacer--large': this.modIsLarge,
        'u-spacer--none': this.modIsNone
      }
    }
  }
}
</script>
