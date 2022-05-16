<template>
  <div :id="drawerId" class="drawer" :data-drawer="drawerId">
    <button type="button" class="drawer__close link link--icon--close js-drawer__close" :title="buttonTitle">{{ buttonTitle }}</button>
    <slot></slot>
  </div>
</template>

<script>

export default {
  name: 'drawer',
  props: {
    buttonTitle: {
      type: String,
      default: 'Sluiten'
    },
    drawerId: {
      type: String,
      required: true
    }
  },
  mounted () {
    vl.drawer.dress(document.getElementById(this.drawerId))
  }
}
</script>
