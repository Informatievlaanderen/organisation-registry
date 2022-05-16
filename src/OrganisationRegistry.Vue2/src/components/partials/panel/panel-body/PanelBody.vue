<template>
  <div :class="classes">
    <slot name="content"></slot>
    <div class="bl-panel__body__accordions" v-if="$slots.footer">
      <slot name="accordions"></slot>
    </div>
    <footer class="bl-panel__body__footer" v-if="$slots.accordions">
      <slot name="footer"></slot>
    </footer>
  </div>
</template>

<script>
export default {
  name: 'panel-body',
  props: {
    modIsAlt: {
      default: false,
      type: Boolean
    },
    modIsAccordion: {
      default: false,
      type: Boolean
    }
  },
  data () {
    return {
      classes: {
        'bl-panel__body': true,
        'accordion__content': this.modIsAccordion
      }
    }
  }
}
</script>
