<template>
  <router-link class="tab" active-class="tab--active" :exact="exact" :to="to">
    <li>
      <a link :id="id || null" class="tab__link">{{ title }}</a>
    </li>
  </router-link>
</template>

<script>
export default {
  name: "or-tab",
  props: {
    id: {
      default: "",
      type: String,
    },
    title: {
      required: true,
      type: String,
    },
    to: {
      required: true,
      type: Object,
    },
    exact: {
      default: false,
      type: Boolean,
    },
  },
  data() {
    return {
      classes: {
        tab: true,
        "tab--active": this.isActive,
      },
    };
  },
};
</script>
