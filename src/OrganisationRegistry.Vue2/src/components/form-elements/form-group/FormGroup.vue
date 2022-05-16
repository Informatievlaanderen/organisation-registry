<template>
  <div class="form__group">
    <slot></slot>
  </div>
</template>

<script>
export default {
  name: 'form-group',
  props: {
  },
  data () {
    return {
    };
  },
};
</script>
<style>
.form__group {
  padding: 0rem 1.5rem;
  background-color: #FFF;
  margin: 2rem 0 0;
}
</style>
