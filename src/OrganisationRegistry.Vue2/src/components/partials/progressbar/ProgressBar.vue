<template>
  <div class="progress-bar">
    <template v-for="(step, index) in steps">
      <div :class="{'progress-bar__step': true, 'progress-bar__step--active': step.isActive}">
        <div class="progress-bar__bullet" :class="{'progress-bar__bullet--current': step.isCurrent}">
          <div class="tooltip tooltip--top tooltip--static" style="white-space: nowrap" v-if="step.isActive">
            <div class="tooltip__content"><span class="u-visually-hidden">Stap {{ index+1 }} van {{ steps.length }}: </span>{{ step.title }}</div>
            <div class="tooltip__arrow"></div>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>

<script>
  export default {
    name: 'progress-bar',
    props: {
      steps: {
        default: [],
        type: Array
      }
    }
  }
</script>
