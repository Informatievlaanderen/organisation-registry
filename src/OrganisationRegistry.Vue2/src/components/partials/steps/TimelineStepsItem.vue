<template>
  <li class="step">
    <div class="step__icon">{{day}}<span class="step__icon__sub">{{month}}</span></div>
    <div class="step__wrapper">
      <div class="step__header">
        <div class="step__header__titles">
          <h3 class="step__title bl-step__title"><svg v-if="titleHasIcon" role="presentation" v-svg :symbol="titleIcon" :class="titleIconClass" size="0 0 20 20" aria-hidden="true"></svg>{{ title }}</h3>
        </div>
      </div>
      <div class="step__content-wrapper" v-if="hasContent">
        <div class="step__content">
          <slot></slot>
        </div>
      </div>
    </div>
  </li>
</template>

<script>
export default {
  name: 'timeline-steps-item',
  props: {
    date: {
      default: '',
      type: String
    },
    title: {
      default: '',
      type: String
    },
    titleHasIcon: {
      default: false,
      type: Boolean
    },
    titleIcon: {
      default: '',
      type: String
    },
    titleIconClass: {
      default: '',
      type: String
    },
    hasContent: {
      default: false,
      type: Boolean
    }
  },
  computed: {
    dateParts () {
      return this.$date(this.date, 'short').split(' ')
    },
    day () {
      return this.dateParts[0] || ''
    },
    month () {
      return this.dateParts[1] || ''
    }
  }
}
</script>
