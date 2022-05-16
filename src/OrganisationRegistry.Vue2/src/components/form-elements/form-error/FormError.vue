<template>
    <div v-show="errors.has(id)" :class="classes" :data-error-id="id">
        <slot></slot>{{errors.first(id)}}
    </div>
</template>

<script>
export default {
  inject: ['$validator'],
  name: 'form-error',
  props: {
    id: {
      default: '',
      type: String,
    },
  },
  data() {
    return {
      classes: {
        form__error: true,
      },
    };
  },
};
</script>
