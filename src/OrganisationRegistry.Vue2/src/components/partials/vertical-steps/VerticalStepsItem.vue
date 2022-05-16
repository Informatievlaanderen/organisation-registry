<template>
  <component :is="elType" :class="{ 'vertical-step': true, 'vertical-step--checked': isChecked, 'vertical-step--current': isCurrent }">
    <h1 v-if="isChecked" class="vertical-step__title">{{ title }}</h1>
    <div v-if="isCurrent" class="vertical-step__subtitle">{{ subtitle }}</div>
    <slot></slot>
  </component>
</template>

<script>
export default {
  name: 'vertical-steps',
  props: {
    type: {
      default: 'li',
      type: String
    },
    isChecked: {
      default: false,
      type: Boolean
    },
    isCurrent: {
      default: false,
      type: Boolean
    },
    title: {
      default: '',
      type: String
    },
    subtitle: {
      default: '',
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
