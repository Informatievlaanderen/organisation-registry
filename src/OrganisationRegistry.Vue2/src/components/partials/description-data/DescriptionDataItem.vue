<template>
    <component :is="elType" :class="className">
      <slot></slot>
    </component>
</template>

<script>
export default {
  name: 'description-data-item',
  props: {
    type: {
      default: 'p',
      type: String
    },
    classType: {
      default: '',
      type: String
    }
  },
  data () {
    return {
      className: `description-data__${this.classType}`,
      elType: this.type
    }
  }
}
</script>
