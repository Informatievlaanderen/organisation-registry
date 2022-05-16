<template>
  <component :is="type || 'div'" :class="classes">
    <slot></slot>
  </component>
</template>

<script>
export default {
  // inject: ["$validator"],
  name: "or-column",
  props: {
    cols: {
      default: null,
      type: Array,
    },
    type: {
      default: "div",
      type: String,
    },
  },
  data() {
    return {
      classes: [],
    };
  },
  created() {
    // Possible nominators.
    const arrNoms = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
    // Possible denominators.
    const arrDens = [1, 2, 3, 4, 6, 12];
    // Possible modifiers.
    const arrMods = ["l", "m", "s", "xs"];

    // Check if cols attribute is present.
    if (this.cols) {
      // Loop through all col elements
      this.cols.forEach((col) => {
        // Check if nominator is present.
        if (!col.nom) return "";
        // Check if denominator is present.
        if (!col.den) return "";
        // Check if nominator is present inside the possible options (arrNoms).
        if (!arrNoms.includes(col.nom)) return "";
        // Check if denominator is present inside the possible options (arrDens).
        if (!arrDens.includes(col.den)) return "";
        // Check if modifier is present inside the possible options (arrMods).
        if (col.mod && !arrMods.includes(col.mod)) return "";
        // Create classname based on nom, den and mod.
        const mod = col.mod ? `--${col.mod}` : "";
        const cls = `col--${col.nom}-${col.den}${mod}`;
        this.classes.push(cls);

        return true;
      });
    } else {
      // If cols is not present set to default full-width column.
      this.classes = ["col--1-1"];
    }
  },
};
</script>
