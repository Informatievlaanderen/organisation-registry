<template>
  <div>
  <select :name="name" :id="id" :class="{
        'select': true,
        'select--block': this.modIsBlock,
        'select--error': this.modIsError,
        'select--disabled': this.disabled
      }"
    :disabled="disabled"
    v-validate="validation">
    <option v-if="includeEmptyOption" value=""></option>
    <template v-for="(item, index) in options">
      <template v-if="item.type === 'option'">
        <option :value="item.value" :key="index" :selected="item.selected">{{ item.label }}</option>
      </template>
      <template v-if="item.type === 'optgroup'">
        <optgroup :label="item.label" :key="item.value">
          <option v-for="(option, index) in item.options" :value="option.value" :key="index" :selected="option.selected">{{ option.label }}</option>
        </optgroup>
      </template>
    </template>
  </select>
  </div>
</template>

<script>
export default {
  inject: ['$validator'],
  name: 'dvSelect',
  props: {
    name: {
      default: '',
      type: String
    },
    value: {
      default: '',
      type: String
    },
    id: {
      default: '',
      type: String
    },
    disabled: {
      default: false,
      type: Boolean
    },
    options: {
      type: Array
    },
    modIsBlock: {
      default: true,
      type: Boolean
    },
    modIsError: {
      default: false,
      type: Boolean
    },
    modShowOnSelect: {
      default: false,
      type: Boolean
    },
    validation: {
      default: '',
      type: String,
    },
    includeEmptyOption: {
      default: false,
      type: Boolean
    }
  },
  data () {
    return {
    };
  },
  mounted () {
    vl.select.showOnSelect.dressAll()
  }
}
</script>
