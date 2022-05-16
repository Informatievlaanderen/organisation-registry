<template>
  <div :class="classes" :data-initials="initials">
    <svg v-if="icon" role="presentation" class="badge__icon" v-svg :symbol="icon" aria-hidden="true"></svg>
    <svg v-if="modRequiresAction" role="presentation" v-svg symbol="icon-warning-fill-bordered" class="badge__warning icon icon--color-warning" size="0 0 20 20" aria-hidden="true"></svg>
  </div>
</template>

<script>

export default {
  name: 'badge',
  props: {
    icon: {
      type: String,
      default: ''
    },
    initials: {
      type: String,
      default: ''
    },
    modIsAlt: {
      default: false,
      type: Boolean
    },
    modIsAccent: {
      default: false,
      type: Boolean
    },
    modIsSuccess: {
      default: false,
      type: Boolean
    },
    modIsPlaceholder: {
      default: false,
      type: Boolean
    },
    modIsBordered: {
      default: false,
      type: Boolean
    },
    modIsOverlap: {
      default: false,
      type: Boolean
    },
    modIsSmall: {
      default: false,
      type: Boolean
    },
    modIsLarge: {
      default: false,
      type: Boolean
    },
    modIsIconStretched: {
      default: false,
      type: Boolean
    },
    modRequiresAction: {
      default: false,
      type: Boolean
    }
  },
  data () {
    return {
      classes: {
        'badge': true,
        'badge--alt': this.modIsAlt,
        'badge--accent': this.modIsAccent,
        'badge--success': this.modIsSuccess,
        'badge--placeholder': this.modIsPlaceholder,
        'badge--bordered': this.modIsBordered,
        'badge--overlap': this.modIsOverlap,
        'badge--small': this.modIsSmall,
        'badge--large': this.modIsLarge,
        'badge--icon-stretched': this.modIsIconStretched
      }
    }
  }
}
</script>
