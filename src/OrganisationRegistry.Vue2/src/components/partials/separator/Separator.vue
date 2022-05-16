<template>
  <div :class="classes">
    <div class="bl-separator__label" v-if="title">{{ title }}<slot></slot></div>
  </div>
</template>

<script>
export default {
  name: 'separator',
  props: {
    title: {
      default: '',
      type: [Number, String]
    },
    modIsSmall: {
      default: false,
      type: Boolean
    }
  },
  data () {
    return {
      classes: {
        'bl-separator': true,
        'bl-separator--small': this.modIsSmall
      }
    }
  }
}
</script>
