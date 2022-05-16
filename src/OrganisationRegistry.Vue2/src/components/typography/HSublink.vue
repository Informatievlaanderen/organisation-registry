<template>
  <div :class="type + '-sublink'">
    <slot></slot>
  </div>
</template>

<script>
export default {
  name: 'h-sublink',
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
