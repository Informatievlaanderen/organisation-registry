<template>
  <div
    :class="{
          'input-container': true,
          'input-required': this.inputRequired,
          'input-valid': this.inputValid,
        }">
    <input
      type="text"
      :name="name"
      :id="id"
      :class="{
          'input-field': true,
          'input-field--block': this.modIsBlock,
          'input-field--small': this.modIsSmall,
          'input-field--disabled': this.disabled,
        }"
      :value="inputValue"
      :placeholder="placeholder"
      v-bind="attributes"
      :readonly="disabled"
      v-validate="validation"
      @input="onInput" />
  </div>
</template>

<script>

export default {
  inject: ['$validator'],
  name: 'input-field',
  props: {
    name: {
      default: '',
      type: String,
    },
    value: {
      default: '',
      type: String,
    },
    id: {
      default: '',
      type: String,
    },
    placeholder: {
      default: '',
      type: String,
    },
    disabled: {
      default: false,
      type: Boolean,
    },
    modIsBlock: {
      default: true,
      type: Boolean,
    },
    modIsSmall: {
      default: false,
      type: Boolean,
    },
    modIsError: {
      default: false,
      type: Boolean,
    },
    validation: {
      default: '',
      type: String,
    },
  },
  computed: {
    inputRequired() {
      return this.fieldsBag[this.name] && !this.fieldsBag[this.name].valid;
    },
    inputValid() {
      return this.validation && this.fieldsBag[this.name] && this.fieldsBag[this.name].valid;
    },
    isRequired() {
      return (this.validation || '').toLowerCase().includes('required');
    },
  },
  data() {
    return {
      attributes: {},
      inputValue: '',
      requiredFieldPushedToStore: false,
    };
  },
  methods: {
    onInput(event) {
      const { target = {} } = event;
      this.inputValue = target.value;
    },
  },
  mounted(){
    this.inputValue = this.value;
  },
  watch: {
    value(updatedValue, previousValue) {
      this.inputValue = updatedValue;
    },
  },
};
</script>

<style scoped>
  .input-field {
    padding-right:30px;
    display: inline-block;
  }

  .input-container {
    box-sizing: border-box;
    display: block;
    position: relative;
  }

  .input-container::before {
    position: absolute;
    -webkit-box-sizing: border-box;
    box-sizing: border-box;
    right: 11px;
    top: 7px;
    z-index: 1;
    opacity: 1;
    font-size: 15px;
    font-weight: 900;
    z-index: 1;
  }

  .input-valid::before {
    content: "✔";
    color: #0ac05c;
  }

  .input-required::before {
    content: "!";
    color: #db3434;
  }
</style>
