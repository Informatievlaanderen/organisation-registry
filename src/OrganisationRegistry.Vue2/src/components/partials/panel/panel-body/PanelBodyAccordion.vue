<template>
  <div class="bl-panel__body__accordion" :class="{'bl-panel__body__accordion--open': data.open}">
    <a href="#" class="bl-panel__body__accordion__toggle toggle toggle--arrow-down" :class="{'bl-panel__body__accordion__toggle--reverse': data.open}" @click.prevent="emitClickEvent" :aria-expanded="String(data.open)" :aria-controls="'dropdown-' + data.id">
      <i class="toggle__icon" aria-hidden="true"></i>
      {{ data.toggle }}
    </a>
    <transition name="bl-panel-accordion">
      <div :id="'dropdown-' + data.id" class="bl-panel__body__accordion__content" v-if="data.open" tabindex="0">
        <slot></slot>
      </div>
    </transition>
  </div>
</template>

<script>
export default {
  name: 'panel-body-accordion',
  props: {
    data: {
      default: null,
      type: Object
    }
  },
  methods: {
    emitClickEvent (id) {
      this.$emit('accordion-was-clicked', id)
    }
  }
}
</script>

