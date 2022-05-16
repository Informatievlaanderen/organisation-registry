<template>
  <div data-show-on-select-wrapper>
    <select :name="name" :id="id" :class="classes" :disabled="disabled" data-show-on-select>
      <template v-for="(item, index) in options">
        <template v-if="item.type === 'option'">
          <option :value="item.value" data-show-on-select-option :data-target="item.value">{{ item.label }}</option>
        </template>
        <template v-if="item.type === 'optgroup'">
          <optgroup :label="item.label">
            <option v-for="(option, index) in item.options" :value="option.value" :selected="option.selected" :data-show-on-select-option="modShowOnSelect" :data-target="modShowOnSelect ? option.value : false">{{ option.label }}</option>
          </optgroup>
        </template>
      </template>
    </select>
    <div class="u-spacer"></div>
    <div v-for="option in options" v-if="option.showOnSelect" class="u-bg-block" data-show-on-select-content :data-hook="option.value">
      {{ option.showOnSelect }}
    </div>
  </div>
</template>

<script>
export default {
  name: 'select',
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
    label: {
      default: '',
      type: String
    },
    options: {
      type: Array
    },
    modIsBlock: {
      default: false,
      type: Boolean
    },
    modIsError: {
      default: false,
      type: Boolean
    },
    modShowOnSelect: {
      default: false,
      type: Boolean
    }
  },
  data () {
    return {
      classes: {
        'select': true,
        'select--block': this.modIsBlock,
        'select--error': this.modIsError,
        'select--disabled': this.disabled
      }
    }
  },
  mounted () {
    vl.select.showOnSelect.dressAll()
  }
}
</script>
