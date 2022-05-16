<template>
  <nav class="bl-icon-navigation">
    <bl-layout :mod-is-wide="true">
      <bl-grid>
        <bl-column>
          <ul class="bl-icon-navigation__list">
            <li class="bl-icon-navigation__item">
              <nuxt-link to="/meest-recent" :class="{ 'bl-icon-navigation__item__cta': true, 'bl-icon-navigation__item__cta--active': isActive === 'meest-recent' }">
                <div class="bl-icon-navigation__item__cta__icon-wrapper">
                  <svg role="presentation" class="bl-icon-navigation__item__cta__icon" v-svg symbol="icon-bell" size="0 0 19 19" aria-hidden="true"></svg>
                </div>
                <span class="bl-icon-navigation__item__cta__label">Meest recent</span>
              </nuxt-link>
            </li>
            <li role="separator" aria-hidden="true" class="bl-icon-navigation__separator"></li>
            <li class="bl-icon-navigation__item">
              <nuxt-link to="/u-en-uw-gezin" :class="{ 'bl-icon-navigation__item__cta': true, 'bl-icon-navigation__item__cta--active': isActive === 'u-en-uw-gezin' }">
                <div class="bl-icon-navigation__item__cta__icon-wrapper">
                  <svg role="presentation" class="bl-icon-navigation__item__cta__icon" v-svg symbol="icon-people" size="0 0 25 25" aria-hidden="true"></svg>
                </div>
                <span class="bl-icon-navigation__item__cta__label">U & uw gezin</span>
              </nuxt-link>
            </li>
            <li class="bl-icon-navigation__item">
              <nuxt-link to="/woonst-en-vastgoed" :class="{ 'bl-icon-navigation__item__cta': true, 'bl-icon-navigation__item__cta--active': isActive === 'woonst-en-vastgoed' }">
                <div class="bl-icon-navigation__item__cta__icon-wrapper">
                  <svg role="presentation" class="bl-icon-navigation__item__cta__icon" v-svg symbol="icon-house" size="0 0 25 25" aria-hidden="true"></svg>
                </div>
                <span class="bl-icon-navigation__item__cta__label">Woonst &<br>vastgoed</span>
              </nuxt-link>
            </li>
            <li class="bl-icon-navigation__item">
              <a href="" @click.prevent="" :class="{ 'bl-icon-navigation__item__cta': true, 'bl-icon-navigation__item__cta--active': isActive === 'mobiliteit' }">
                <div class="bl-icon-navigation__item__cta__icon-wrapper">
                  <svg role="presentation" class="bl-icon-navigation__item__cta__icon" v-svg symbol="icon-car" size="0 0 25 25" aria-hidden="true"></svg>
                </div>
                <span class="bl-icon-navigation__item__cta__label">Mobiliteit</span>
              </a>
            </li>
            <li class="bl-icon-navigation__item">
              <a href="" @click.prevent="" :class="{ 'bl-icon-navigation__item__cta': true, 'bl-icon-navigation__item__cta--active': isActive === 'werk-en-pensioen' }">
                <div class="bl-icon-navigation__item__cta__icon-wrapper">
                  <svg role="presentation" class="bl-icon-navigation__item__cta__icon" v-svg symbol="icon-suitcase" size="0 0 25 25" aria-hidden="true"></svg>
                </div>
                <span class="bl-icon-navigation__item__cta__label">Werk &<br>pensioen</span>
              </a>
            </li>
            <li class="bl-icon-navigation__item" v-if="education">
              <nuxt-link to="/onderwijs" :class="{ 'bl-icon-navigation__item__cta': true, 'bl-icon-navigation__item__cta--active': isActive === 'onderwijs' }">
                <div class="bl-icon-navigation__item__cta__icon-wrapper">
                  <svg role="presentation" class="bl-icon-navigation__item__cta__icon" v-svg symbol="icon-hat" size="0 0 25 25" aria-hidden="true"></svg>
                </div>
                <span class="bl-icon-navigation__item__cta__label">Onderwijs <template v-if="educationHasIntegration">& inburgering</template></span>
              </nuxt-link>
            </li>
            <li class="bl-icon-navigation__item">
              <nuxt-link to="/belastingen-en-voordelen" :class="{ 'bl-icon-navigation__item__cta': true, 'bl-icon-navigation__item__cta--active': isActive === 'belastingen-en-voordelen' }">
                <div class="bl-icon-navigation__item__cta__icon-wrapper">
                  <svg role="presentation" class="bl-icon-navigation__item__cta__icon" v-svg symbol="icon-cash" size="0 0 25 25" aria-hidden="true"></svg>
                </div>
                <span class="bl-icon-navigation__item__cta__label">Belastingen &<br>voordelen</span>
              </nuxt-link>
            </li>
            <li class="bl-icon-navigation__item">
              <nuxt-link to="/attesten" :class="{ 'bl-icon-navigation__item__cta': true, 'bl-icon-navigation__item__cta--active': isActive === 'attesten' }">
                <div class="bl-icon-navigation__item__cta__icon-wrapper">
                  <svg role="presentation" class="bl-icon-navigation__item__cta__icon" v-svg symbol="icon-diploma" size="0 0 25 25" aria-hidden="true"></svg>
                </div>
                <span class="bl-icon-navigation__item__cta__label">Attesten</span>
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
  name: 'icon-navigation',
  data () {
    return {
      isActive: '',
      education: false,
      educationHasIntegration: false
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
    // Split path and detect if present in array
    const arrPath = self.$route.path.split('/')
    // Set isActive element
    self.isActive = arrPath[arrPath.length - 1]

    setTimeout(() => { self.dressNavigation() }, 50)
  },
  methods: {
    dressNavigation () {
      // Menu animation
      const navigation = document.getElementsByClassName('bl-icon-navigation')[0]
      const accentHeader = document.getElementsByClassName('js-accent-header')[0]

      let latestKnownScrollY = 0
      let ticking = false

      // only on medium or up
      if (vl.breakpoint.value === 'small' || vl.breakpoint.value === 'xsmall') {
        return
      }

      // Detect scrolling
      window.addEventListener('scroll', onScroll, false)
      onScroll()
      function onScroll () {
        latestKnownScrollY = window.pageYOffset
        requestTick()
      }
      function requestTick () {
        if (!ticking) {
          requestAnimationFrame(update)
        }
        ticking = true
      }
      function update () {
        ticking = false
        // Get your scroll
        let currentScrollY = latestKnownScrollY
        // Dress the stickybar
        dressStickyBar(navigation, currentScrollY)
      }
      function dressStickyBar (navigation, currentScrollY) {
        // Get the base - ether head, gh2 or gh3
        let base = document.querySelector('.js-iwgh3-bc') || document.querySelector('.iwgh2-bar') || document.querySelector('head')
        let baseTop = base.getBoundingClientRect().bottom
        let accentHeaderHeight = accentHeader.getBoundingClientRect().height
        let navigationHeight = navigation.getBoundingClientRect().height || 0
        let top = baseTop + accentHeaderHeight

        if (currentScrollY > (top - 5)) {
          addClass(navigation, 'bl-icon-navigation--fixed')
          navigation.style.top = baseTop + 'px'
          accentHeader.style.marginBottom = navigationHeight + 'px'
        }
        if (((baseTop - 5) + accentHeaderHeight) > (accentHeaderHeight + 39)) {
          if (currentScrollY < (accentHeaderHeight)) {
            removeClass(navigation, 'bl-icon-navigation--fixed')
            accentHeader.style.marginBottom = '0px'
          }
        } else {
          if (currentScrollY < (top - 5)) {
            removeClass(navigation, 'bl-icon-navigation--fixed')
            accentHeader.style.marginBottom = '0px'
          }
        }
      }
    }
  }
}
</script>
