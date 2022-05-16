<template>
  <div :class="{
        'button-group': true,
        'button-group--right': this.modIsRight
      }">
    <slot></slot>
  </div>
</template>

<script>
export default {
  name: 'button-group',
  props: {
    modIsRight: {
      default: false,
      type: Boolean,
    },
  },
  data () {
    return {
    };
  },
};
</script>
<style>
.button-group--right {
  float: right;
}
</style>
