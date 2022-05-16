<template>
  <header class="bl-accent-header js-accent-header" role="banner" v-if="profile">
      <bl-layout :mod-is-wide="true">
        <bl-grid>
          <bl-column :cols="[{nom: 1, den: 2}, {nom: 1, den: 1, mod: 's'}]">
            <div class="bl-accent-header__content">
              <component
                v-if="!avatarLoading"
                :is="avatar ? 'img' : 'div'"
                :data-initials="initials"
                class="bl-accent-header__badge"
                :src="avatar"
                :alt="'Foto van ' + fullName"
              />
              <h1 class="bl-accent-header__title" v-if="showName"><span v-if="fullName">{{ (nickname || firstName) + ' ' + lastName }}</span></h1>
              <nuxt-link to="/profiel" class="vi vi-u-badge vi-u-badge--action vi-u-badge--xxsmall vi-edit"><span class="u-visually-hidden">Profiel</span></nuxt-link>
            </div>
          </bl-column>
          <bl-column :cols="[{nom: 1, den: 2}, {nom: 1, den: 1, mod: 's'}]">
            <ul class="bl-accent-header__actions">
              <li class="bl-accent-header__actions__item" v-if="audit">
                <nuxt-link to="/wie-heeft-uw-gegevens-geraadpleegd" class="bl-accent-header__actions__item__cta link u-small-text">Wie heeft uw gegevens geraadpleegd</nuxt-link>
            </li>
              </li>
              <li class="bl-accent-header__actions__item">
                <bl-notifications />&nbsp;
                <nuxt-link to="/login" class="bl-accent-header__actions__item__cta link u-small-text">INSZ</nuxt-link>&nbsp;
                <a href="#" class="bl-accent-header__actions__item__cta link u-small-text" @click.prevent="$store.dispatch('session/logout', true)">Uitloggen</a>
              </li>
            </ul>
          </bl-column>
        </bl-grid>
      </bl-layout>
    <slot></slot>
  </header>
</template>

<script>
import { mapGetters } from 'vuex'

import BlNotifications from '~components/partials/notifications/Notifications.vue'

export default {
  name: 'accent-header',
  components: {
    BlNotifications
  },
  props: {
    showName: {
      default: true,
      type: Boolean
    },
    showMeta: {
      default: true,
      type: Boolean
    },
    showEditProfile: {
      default: true,
      type: Boolean
    }
  },
  data () {
    return {
      audit: false
    }
  },
  computed: {
    initials () {
      return (this.nickname.charAt(0) || this.firstName.charAt(0)) + this.lastName.charAt(0)
    },
    birthday () {
      return this.$date(this.$store.getters['session/birthday'])
    },
    ...mapGetters({
      profile: 'session/profile',
      nickname: 'session/nickname',
      firstName: 'session/firstName',
      lastName: 'session/lastName',
      fullName: 'session/fullName',
      city: 'session/city',
      avatar: 'photo/profile'
    }),
    avatarLoading () {
      const resolved = this.$store.getters['photo/profileResolved']
      const failed = this.$store.getters['photo/profileFailed']
      return !resolved && !failed
    }
  },
  mounted () {
    // Get a reference to ourselves.
    const self = this

    // Dispatch profile.
    self.$store.dispatch('session/profile')

    // Dispatch fetching profile picture.
    this.$store.dispatch('photo/profile')

    // Dispatch audit dashboardstatus.
    self.$store.dispatch('audit/dashboardStatus').then(() => {
      if (self.$store.getters['audit/dashboardStatusResolved']) {
        // Set audit status.
        self.audit = self.$store.getters['audit/dashboardStatus'].status || false
      }
    })
  }
}
</script>
