<template>
  <component :is="elType">
    <slot></slot>
  </component>
</template>

<script>
export default {
  name: 'h',
  props: {
    type: {
      default: 'h1',
      type: String
    }
  },
  data () {
    return {
      elType: this.type
    }
  }
}
</script>
