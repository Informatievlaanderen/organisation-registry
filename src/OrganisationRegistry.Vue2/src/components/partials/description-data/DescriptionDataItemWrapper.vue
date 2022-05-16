<template>
  <div :class="{ 'bl-description-data__wrapper': true, 'bl-description-data__wrapper--alt': modIsAlt, 'bl-description-data__wrapper--bordered': modIsBordered }">
    <slot></slot>
  </div>
</template>

<script>
export default {
  name: 'description-data-item-wrapper',
  props: {
    modIsBordered: {
      default: false,
      type: Boolean
    },
    modIsAlt: {
      default: false,
      type: Boolean
    }
  }
}
</script>
