<template>
  <nav class="bl-navigation-header__navigation">
    <bl-layout :mod-is-wide="true">
      <bl-grid>
        <bl-column>
          <ul class="bl-navigation-header__navigation__list">
            <li class="bl-navigation-header__navigation__item">
              <nuxt-link to="/uw-gegevens-bij-de-overheid/u-en-uw-gezin" :class="{ 'bl-navigation-header__navigation__item__cta': true, 'bl-navigation-header__navigation__item__cta--active': isActive === 'u-en-uw-gezin' }">
                <svg role="presentation" class="bl-navigation-header__navigation__item__cta__icon" v-svg symbol="icon-people" size="0 0 25 25" aria-hidden="true"></svg>
                <span class="bl-navigation-header__navigation__item__cta__label">U & uw<br> gezin</span>
              </nuxt-link>
            </li>
            <li class="bl-navigation-header__navigation__item">
              <nuxt-link to="/uw-gegevens-bij-de-overheid/woonst-en-vastgoed" :class="{ 'bl-navigation-header__navigation__item__cta': true, 'bl-navigation-header__navigation__item__cta--active': isActive === 'woonst-en-vastgoed' }">
                <svg role="presentation" class="bl-navigation-header__navigation__item__cta__icon" v-svg symbol="icon-house" size="0 0 25 25" aria-hidden="true"></svg>
                <span class="bl-navigation-header__navigation__item__cta__label">Woons & vastgoed</span>
              </nuxt-link>
            </li>
            <li class="bl-navigation-header__navigation__item">
              <a href="" @click.prevent="" :class="{ 'bl-navigation-header__navigation__item__cta': true, 'bl-navigation-header__navigation__item__cta--active': isActive === 'wagen' }">
                <svg role="presentation" class="bl-navigation-header__navigation__item__cta__icon" v-svg symbol="icon-car" size="0 0 25 25" aria-hidden="true"></svg>
                <span class="bl-navigation-header__navigation__item__cta__label">Wagen</span>
              </a>
            </li>
            <li class="bl-navigation-header__navigation__item">
              <a href="" @click.prevent="" :class="{ 'bl-navigation-header__navigation__item__cta': true, 'bl-navigation-header__navigation__item__cta--active': isActive === 'werk' }">
                <svg role="presentation" class="bl-navigation-header__navigation__item__cta__icon" v-svg symbol="icon-suitcase" size="0 0 25 25" aria-hidden="true"></svg>
                <span class="bl-navigation-header__navigation__item__cta__label">Werk</span>
              </a>
            </li>
            <li class="bl-navigation-header__navigation__item" v-if="education">
              <nuxt-link to="/uw-gegevens-bij-de-overheid/onderwijs" :class="{ 'bl-navigation-header__navigation__item__cta': true, 'bl-navigation-header__navigation__item__cta--active': isActive === 'onderwijs' }">
                <svg role="presentation" class="bl-navigation-header__navigation__item__cta__icon" v-svg symbol="icon-hat" size="0 0 25 25" aria-hidden="true"></svg>
                <span class="bl-navigation-header__navigation__item__cta__label">Onderwijs <template v-if="educationHasIntegration">& inburgering</template></span>
              </nuxt-link>
            </li>
            <li class="bl-navigation-header__navigation__item">
              <a href="" @click.prevent="" :class="{ 'bl-navigation-header__navigation__item__cta': true, 'bl-navigation-header__navigation__item__cta--active': isActive === 'assistentie-en-hulp' }">
                <svg role="presentation" class="bl-navigation-header__navigation__item__cta__icon" v-svg symbol="icon-hands" size="0 0 25 25" aria-hidden="true"></svg>
                <span class="bl-navigation-header__navigation__item__cta__label">Assistentie & hulp</span>
              </a>
            </li>
            <li class="bl-navigation-header__navigation__item">
              <nuxt-link to="/uw-gegevens-bij-de-overheid/attesten" :class="{ 'bl-navigation-header__navigation__item__cta': true, 'bl-navigation-header__navigation__item__cta--active': isActive === 'attesten' }">
                <svg role="presentation" class="bl-navigation-header__navigation__item__cta__icon" v-svg symbol="icon-diploma" size="0 0 25 25" aria-hidden="true"></svg>
                <span class="bl-navigation-header__navigation__item__cta__label">Attesten</span>
              </nuxt-link>
            </li>
            <li role="separator" class="bl-navigation-header__navigation__separator" v-if="audit"></li>
            <li class="bl-navigation-header__navigation__item" v-if="audit">
              <nuxt-link to="/uw-gegevens-bij-de-overheid/overzicht-opvragingen" :class="{ 'bl-navigation-header__navigation__item__cta': true, 'bl-navigation-header__navigation__item__cta--active': isActive === 'overzicht-opvragingen' }">
                <svg role="presentation" class="bl-navigation-header__navigation__item__cta__icon" v-svg symbol="icon-eye" size="0 0 25 25" aria-hidden="true"></svg>
                <span class="bl-navigation-header__navigation__item__cta__label">Overzicht opvragingen</span>
              </nuxt-link>
            </li>
          </ul>
        </bl-column>
      </bl-grid>
    </bl-layout>
  </nav>
</template>

<script>

export default {
  name: 'navigation-header-navigation',
  data () {
    return {
      isActive: '',
      education: false,
      educationHasIntegration: false,
      audit: false
    }
  },
  mounted () {
    // Get a reference to ourself.
    const self = this
    // Resolve the Education Events.
    self.$store.dispatch('education/dashboardStatus').then(() => {
      if (self.$store.getters['education/dashboardStatusResolved']) {
        // Set education status.
        self.education = self.$store.getters['education/dashboardStatus'].status || false
        // Check if integration is present.
        self.educationHasIntegration = self.$store.getters['education/dashboardStatus'].meta.integration || false
      }
    })
    self.$store.dispatch('education/events').then(() => {
      // If resolved status is true get status.
      if (self.$store.getters['education/dashboardStatusResolved']) {
        // Set education status.
        self.education = self.$store.getters['education/dashboardStatus'].status || false
        // Check if integration is present.
        self.educationHasIntegration = self.$store.getters['education/dashboardStatus'].meta.integration || false
      }
    })
    self.$store.dispatch('audit/dashboardStatus').then(() => {
      if (self.$store.getters['audit/dashboardStatusResolved']) {
        // Set audit status.
        self.audit = self.$store.getters['audit/dashboardStatus'].status || false
      }
    })
    // Split path and detect if present in array
    const arrPath = self.$route.path.split('/')
    // Set isActive element
    self.isActive = arrPath[arrPath.length - 1]
  }
}
</script>
