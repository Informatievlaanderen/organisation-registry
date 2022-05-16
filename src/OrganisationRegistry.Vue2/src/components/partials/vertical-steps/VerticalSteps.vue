<template>
  <component :is="elType" class="vertical-steps">
    <slot></slot>
  </component>
</template>

<script>
export default {
  name: 'vertical-steps',
  props: {
    type: {
      default: 'ol',
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
