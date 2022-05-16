<template>
  <div class="bl-navigation-header__profile">
    <bl-layout :mod-is-wide="true">
      <div class="bl-navigation-header__profile__content">
      <bl-grid :mod-is-v-center="true">
        <bl-column :cols="[{nom: 8, den: 12}, {nom: 1, den: 1, mod: 's'}]">
          <a href="#" class="bl-navigation-header__profile__content__img-wrapper" @click.prevent="editProfileClickHandler">
            <img
              v-if="!avatarLoading && avatar"
              :src="avatar"
              :alt="(avatar ? 'Afbeelding' : 'initialen') + ' van ' + fullName"
              class="bl-navigation-header__profile__content__img" />
            <span class="bl-navigation-header__profile__content__img__label link link--underlined u-small-text">wijzigen</span>
          </a>
          <p class="bl-navigation-header__profile__content__title">
            {{ (nickname || firstName) + ' ' + lastName }}
          </p>
        </bl-column>
        <bl-column :cols="[{nom: 4, den: 12}]" class="u-hidden-mobile">
          <div class="u-align-right">
            <a href="#" class="bl-navigation-header__profile__content__cta button">Wijzigingen opslaan</a>
          </div>
        </bl-column>
      </bl-grid>
      </div>
    </bl-layout>
  </div>
</template>

<script>

import { mapGetters } from 'vuex'

export default {
  name: 'navigation-header-profile',
  methods: {
    editProfileClickHandler () {
      this.$emit('edit-profile-was-clicked')
      vl.equalheight.resize()
    }
  },
  computed: {
    ...mapGetters({
      avatar: 'photo/profile',
      fullName: 'session/fullName',
      nickname: 'session/nickname',
      firstName: 'session/firstName',
      lastName: 'session/lastName'
    }),
    avatarLoading () {
      const resolved = this.$store.getters['photo/profileResolved']
      const failed = this.$store.getters['photo/profileFailed']
      return !resolved && !failed
    },
    initials () {
      const first = this.nickname || this.firstName || ''
      const last = this.lastName || ''
      return first.charAt(0) + last.charAt(0)
    }
  },
  mounted () {
    this.$store.dispatch('photo/profile')
  }
}
</script>
