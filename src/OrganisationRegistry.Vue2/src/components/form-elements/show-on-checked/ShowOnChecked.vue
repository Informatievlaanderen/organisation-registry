<template>
  <div class="js-show-checked-wrap">
    <label :class="classes">
      <input :type="type" :name="name" :id="id" :checked="checked" :value="value" :class="{'checkbox__toggle': type==='checkbox', 'radio__toggle': type==='radio'}" data-show-checked="true" :data-show-checked-target="name"> <span></span>{{ label }}
    </label>
    <div :class="{'js-show-checked': true, 'js-show-checked--open': checked}" :data-show-checked-trigger="name">
      <slot></slot>
    </div>
  </div>
</template>

<script>

export default {
  name: 'show-on-checked',
  props: {
    name: {
      default: '',
      type: String
    },
    type: {
      default: 'checkbox',
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
    label: {
      default: '',
      type: String
    },
    checked: {
      default: false,
      type: Boolean
    },
    modIsBlock: {
      default: false,
      type: Boolean
    }
  },
  data () {
    return {
      classes: {
        'checkbox': true,
        'checkbox--block': this.modIsBlock
      }
    }
  },
  mounted () {
    vl.input.showOnChecked.dressAll()
  }
}
</script>
