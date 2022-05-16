<template>
  <div :class="classes">
    <input
      :class="{
        'input-field': true,
        'input-required': this.pristineAndInvalid,
      }"
      type="text"
      :placeholder="placeholder"
      :name="name"
      data-datepicker
      :data-datepicker-min="min"
      :data-datepicker-max="max"
      @input="onInput"
      :value="inputValue"
      v-validate="validation" />
  </div>
</template>

<script>
export default {
  inject: ['$validator'],
  name: 'date-picker',
  props: {
    name: {
      default: '',
      type: String,
    },
    value: {
      default: '',
      type: String,
    },
    placeholder: {
      default: '',
      type: String,
    },
    min: {
      default: '',
      type: String,
    },
    max: {
      default: '',
      type: String,
    },
    validation: {
      default: '',
      type: String,
    },
  },
  computed: {
    pristineAndInvalid() {
      return this.fieldsBag[this.name] &&
        this.fieldsBag[this.name].pristine &&
        !this.fieldsBag[this.name].valid;
    },
  },
  data() {
    return {
      inputValue: '',
      classes: {
        datepicker: true,
      },
    };
  },
  methods: {
    onInput(event) {
      const { target = {} } = event;
      this.inputValue = target.value;
      console.log('on input');
    },
  },
  mounted(){
    vl.datepicker.dressAll();
    this.inputValue = this.value;
    console.log('mounted');
  },
  watch: {
    value(updatedValue, previousValue) {
      this.inputValue = updatedValue;
      console.log('watch');
    },
  },
};
</script>

<style scoped>
  :not(.input-field--error).input-required {
    background-color: rgba(255,230,21,.3);
  }
</style>
