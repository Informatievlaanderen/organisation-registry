<template>
  <div :class="classes">
    <i class="bl-description-data__badge__icon" :class="icon" aria-hidden="true"></i>
  </div>
</template>

<script>
export default {
  name: 'description-data-badge',
  props: {
    modIsAlt: {
      default: false,
      type: Boolean
    },
    icon: {
      default: '',
      type: String
    }
  },
  data () {
    return {
      classes: {
        'bl-description-data__badge': true,
        'bl-description-data__badge--alt': this.modIsAlt
      }
    }
  }
}
</script>
