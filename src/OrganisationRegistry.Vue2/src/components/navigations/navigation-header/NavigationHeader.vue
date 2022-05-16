<template>
  <header class="bl-navigation-header">
    <div class="bl-navigation-header__main">
      <bl-layout :mod-is-wide="true">
        <bl-grid :mod-is-v-center="true">
          <bl-column>
            <div class="bl-navigation-header__main__heading">
              <nuxt-link :to="backUrl" class="bl-navigation-header__main__heading__back">
                <i class="vi vi-arrow vi-u-180deg" aria-hidden="true"></i>
                <span class="u-visually-hidden">Ga terug</span>
              </nuxt-link>
              <div class="bl-navigation-header__main__heading__content">
                <template v-if="hasBadge && badge.type == 'image'">
                  <component
                    :is="( badge.image.src ? 'img' : 'div')"
                    :src="badge.image.src"
                    :alt="(badge.image.src ? 'Afbeelding' : 'initialen') + ` van ${$store.getters['session/fullName']}`"
                    :data-initials="($store.getters['session/nickname'].charAt(0) || $store.getters['session/firstName'].charAt(0)) + $store.getters['session/lastName'].charAt(0)"
                    class="bl-navigation-header__main__heading__content__badge bl-user-badge bl-user-badge--large"
                  />
                </template>
                <template v-if="hasBadge && badge.type == 'icon'">
                  <div class="bl-navigation-header__main__heading__content__badge">
                    <i class="bl-navigation-header__main__heading__content__badge__icon" :class="badge.icon" aria-hidden="true"></i>
                  </div>
                </template>
                <p class="bl-navigation-header__main__heading__content__name">
                  <span class="name">{{ ($store.getters['session/nickname'] || $store.getters['session/firstName']) + ' ' + $store.getters['session/lastName'] }}</span><span class="caption" v-if="pageTitle"><span class="u-hidden-mobile">: </span>{{ pageTitle }}</span>
                </p>
              </div>
            </div>
          </bl-column>
        </bl-grid>
      </bl-layout>
    </div>
    <slot></slot>
  </header>
</template>

<script>

export default {
  name: 'navigation-header',
  middleware: ['authenticated'],
  props: {
    pageTitle: {
      default: '',
      type: String
    },
    hasBadge: {
      default: true,
      type: Boolean
    }
  },
  data () {
    return {
      badge: {
        type: 'image',
        image: {
          src: this.$store.getters['session/avatar'],
          alt: `foto van ${this.$store.getters['session/fullName']}`
        }
      },
      backUrl: '/meest-recent'
    }
  }
}
</script>
