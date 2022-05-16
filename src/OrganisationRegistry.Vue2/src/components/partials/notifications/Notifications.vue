<template>
  <div class="bl-notifications" v-if="notificationsLoaded && notifications.length">
    <div id="bl-notifications-popover" class="popover popover--right popover--xl popover--caret popover--has-header popover--has-footer js-popover js-popover--open" v-on-clickaway="closeNotificationsPopover">
      <button type="button" class="bl-notifications__toggle" @click="toggleNotificationsPopover">
        <template v-if="$store.getters['notification/notificationsUnreadCount'] && $store.getters['notification/notificationsUnreadCount'] > 0">
          <span class="bl-notifications__toggle__counter">
            {{ $store.getters['notification/notificationsUnreadCount'] }}
          </span>
          <span class="u-visually-hidden">
            toon {{ $store.getters['notification/notificationsUnreadCount'] }} nieuwe melding
            <template v-if="$store.getters['notification/notificationsUnreadCount'] > 1">en</template>
          </span>
        </template>
        <div class="bl-notifications__toggle__icon-wrapper">
          <svg class="bl-notifications__toggle__icon" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 14 14" style="enable-background:new 0 0 14 14;" xml:space="preserve">
          <g>
            <path class="st0" d="M12.9,1.9l0.4-0.4c0.2-0.2,0.2-0.6,0-0.8c-0.2-0.2-0.6-0.2-0.8,0l-0.4,0.4C11.2,0.4,10.2,0,9.1,0
              c-1.5,0-3,0.7-4.2,1.9c-1.4,1.4-2.3,1.4-3,1.5c-0.5,0-1,0.1-1.4,0.5C0.1,4.2,0,4.7,0,5.1C0,5.6,0.2,6,0.6,6.4l7.1,7.1
              C8,13.8,8.5,14,8.9,14c0.4,0,0.9-0.2,1.2-0.5c0.4-0.4,0.5-1,0.5-1.4c0.1-0.7,0.1-1.6,1.5-3C14.3,6.9,14.6,4,12.9,1.9z M2.7,13.3
              c0.9,0.9,2.4,0.9,3.3,0L2.7,10C1.7,10.9,1.7,12.4,2.7,13.3z"/>
          </g>
          </svg>
        </div>
      </button>
      <div class="popover__content bl-notifications__popover js-bl-notifications" v-if="notificationsOpen">
        <div class="popover__content__inner">
          <transition name="first-fade">
            <div class="bl-notifications__menu" v-show="!showDetail && !showNested">
              <div class="popover__header">
                <div class="popover__header__title">
                  Meldingen
                </div>
              </div>
              <div class="bl-notifications__list-wrapper">
                <!-- <div class="bl-notifications__loading">
                  <div class="loader"></div>
                </div> -->
                <ul class="bl-notifications__list">
                  <!-- Actual data -->
                  <li v-for="(notification, index) in notifications" :key="index" :class="{ 'bl-notifications__item': true, 'bl-notifications__item--new': !notification.isRead, 'bl-notifications__item--external': !notification.body && notification.urlMoreInfo !== null }">
                    <article>
                      <i v-if="notification.body" class="bl-notifications__item__arrow vi vi-arrow" aria-hidden></i>
                      <a :href="( notification.urlMoreInfo !== null ? notification.urlMoreInfo : '#' )" class="bl-notifications__item__link" @click.prevent="onClickNotification(notification)">
                        <template v-if="notification.icon !== ''">
                          <bl-badge class="bl-notifications__item__badge" :icon="notification.icon" :mod-is-accent="true" :mod-is-bordered="true" :mod-is-small="true" :mod-requires-action="notification.requiresAction"></bl-badge>
                        </template>
                        <template v-if="notification.type === 'image'">
                          <img v-if="notification.src" :src="notification.badge.src" :alt="notification.badge.alt || null" class="bl-notifications__item__badge" />
                        </template>
                        <div class="bl-notifications__item__content">
                          <h1 class="bl-notifications__item__title" data-clamp="2" v-if="notification.title">{{ notification.title }}</h1>
                          <h2 class="bl-notifications__item__subtitle" v-if="notification.subtitle">{{ notification.subtitle }}</h2>
                          <time class="bl-notifications__item__time" v-if="notification.date" :datetime="notification.date.value">{{ $date(notification.date, 'relative') }}</time>
                        </div>
                      </a>
                      <a v-if="notification.nested" class="bl-notifications__item__nested" @click.prevent="onClickNestedNotification(notification)"><i class="vi vi-u-badge vi-u-badge--small vi-arrow" aria-hidden="true"></i>{{ notification.nested.length-1 }} oudere melding
                      <template v-if="notification.nested.length > 2">en</template>
                      </a>
                    </article>
                  </li>
                </ul>
              </div>
              <div class="popover__footer">
                <nuxt-link to="/meest-recent" class="popover__footer__cta link--icon link--small link--icon--caret">Bekijk alle meldingen</nuxt-link>
              </div>
            </div>
          </transition>
          <transition name="second-fade">
            <div class="bl-notifications__nested" v-bind:class="{ 'reversed': reverseAnimationSecond == true}" v-if="showNested">
              <div class="popover__header">
                <div class="popover__header__title">
                  <div class="popover__header__cta popover__header__cta--left popover__header__cta--bordered">
                    <button type="button" class="popover__header__cta__button" @click="closeNestedNotifications(notification.nested)">
                      <i class="vi vi-arrow vi-u-180deg vi-u-m"></i>
                    </button>
                  </div>
                  Dossier: alle meldingen
                </div>
              </div>
              <div class="bl-notifications__list-wrapper">
                <ul class="bl-notifications__list">
                  <li v-for="(notification, index) in notification.nested" :key="index" :class="{ 'bl-notifications__item': true, 'bl-notifications__item--new': !notification.isRead }">
                    <article>
                      <i v-if="notification.body" class="bl-notifications__item__arrow vi vi-arrow" aria-hidden></i>
                      <a :href="( notification.urlMoreInfo !== null ? notification.urlMoreInfo : '#' )" class="bl-notifications__item__link" @click.prevent="onClickNotification(notification)">
                        <template v-if="notification.icon !== ''">
                          <bl-badge class="bl-notifications__item__badge" :icon="notification.icon" :mod-is-accent="true" :mod-is-bordered="true" :mod-is-small="true" :mod-requires-action="notification.requiresAction"></bl-badge>
                        </template>
                        <template v-if="notification.type === 'image'">
                          <img v-if="notification.src" :src="notification.badge.src" :alt="notification.badge.alt || null" class="bl-notifications__item__badge" />
                        </template>
                        <div class="bl-notifications__item__content">
                          <h1 class="bl-notifications__item__title" data-clamp="2" v-if="notification.title">{{ notification.title }}</h1>
                          <h2 class="bl-notifications__item__subtitle" v-if="notification.subtitle">{{ notification.subtitle }}</h2>
                          <time class="bl-notifications__item__time" v-if="notification.date" :datetime="notification.date.value">{{ $date(notification.date, 'relative') }}</time>
                          <p v-if="notification.nested" class="bl-notifications__item__nested">{{ notification.nested.length-1 }} oudere meldingen</p>
                        </div>
                      </a>
                    </article>
                  </li>
                </ul>
              </div>
            </div>
          </transition>
          <transition name="third-fade">
            <div class="bl-notifications__detail" v-if="showDetail">
              <article>
                <div class="popover__header popover__header--ml">
                  <div class="popover__header__title">
                    <div class="popover__header__cta popover__header__cta--left popover__header__cta--bordered">
                      <button type="button" class="popover__header__cta__button" @click="closeNotificationDetail(notification)">
                        <i class="vi vi-arrow vi-u-180deg vi-u-m"></i>
                      </button>
                    </div><div data-clamp="2">{{ notification.title }}</div>
                    </div>
                </div>
                <div class="bl-notifications__detail-wrapper" v-html="notification.body"></div>
              </article>
            </div>
          </transition>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { mixin as clickaway } from 'vue-clickaway'

import BlDescriptionData from '~components/partials/description-data/DescriptionData.vue'
import BlDescriptionDataItem from '~components/partials/description-data/DescriptionDataItem.vue'
import BlDescriptionDataItemWrapper from '~components/partials/description-data/DescriptionDataItemWrapper.vue'
import BlBadge from '~components/partials/badge/Badge.vue'

export default {
  name: 'notifications',
  components: {
    BlDescriptionData,
    BlDescriptionDataItem,
    BlDescriptionDataItemWrapper,
    BlBadge
  },
  mixins: [ clickaway ],
  data () {
    return {
      unreadNotifications: null,
      notifications: [],
      notificationsLoaded: false,
      notificationsOpen: false,
      notificationsMarkAsReadTreshold: 3000,
      showDetail: false,
      showNested: false,
      reverseAnimationFirst: false,
      reverseAnimationSecond: false,
      notification: ''
    }
  },
  methods: {
    onClickNotification (notification) {
      // open the notification body
      if (notification.body) {
        this.openNotificationDetail(notification)
        return false
      }
      // open the notification urlMoreInfo
      if (notification.urlMoreInfo !== null) {
        window.open(notification.urlMoreInfo, '_blank')
        return false
      }
      // open the notification detail view based on detailId
      if (notification.detailId) {
        this.$router.push('meest-recent/' + notification.detailId)
        return false
      }
    },
    onClickNestedNotification (notification) {
      // open a notification with child notifications (nested)
      if (notification.nested) {
        this.openNotificationNested(notification)
        return false
      }
    },
    toggleNotificationsPopover () {
      if (this.notificationsOpen) {
        this.closeNotificationsPopover()
      } else {
        this.openNotificationsPopover()
      }
    },
    closeNotificationsPopover () {
      // close notifications popover and stop mark as read timer
      if (this.notificationsOpen) {
        // return to starting point when popover closes
        this.showDetail = false
        this.showNested = false
        // close the popover
        this.notificationsOpen = false
        // stop mark as read timers
        clearTimeout(this.timer)
        clearTimeout(this.nestedTimer)
      }
    },
    openNotificationsPopover () {
      // open notifications popover
      if (!this.notificationsOpen) {
        this.notificationsOpen = true
        // equal height popover panels
        vl.equalheight.resize()
        // mark notifications as read
        this.timer = setTimeout(() => {
          this.notifications.filter(notification => !notification.isRead).forEach(notification => {
            this.$store.dispatch('notification/markNotificationAsRead', notification)
          })
        }, this.notificationsMarkAsReadTreshold)
        // limit text lines
        setTimeout(() => { vl.textclamp.dressAll() }, 10)
      }
    },
    openNotificationDetail (notification) {
      this.notification = notification
      // cancel reverse animation (transform direction) on the second panel (just in case it was reversed before)
      this.reverseAnimationSecond = false
      if (this.notification.parentId) {
        // reverse animation (transform direction) on the second panel (back button)
        this.reverseAnimationSecond = true
      } else {
        // clear nested notification history if this is not a nested notification
        this.goBackTo = false
      }
      // close the second panel (nested notifications)
      this.showNested = false
      // show notification detail
      this.showDetail = true
      // equal height popover panels
      vl.equalheight.resize()
      // limit text lines
      setTimeout(() => { vl.textclamp.dressAll() }, 10)
    },
    closeNotificationDetail (notification) {
      this.notification = notification
      // close notification detail
      this.showDetail = false
      // if this is a nested notification
      if (this.goBackTo) {
        // reverse animation (transform direction) on the second panel (back button)
        this.reverseAnimationSecond = true
        // open the second panel (nested notifications)
        this.openNotificationNested(this.goBackTo)
        // show the second panel (nested notifications)
        this.showNested = true
      }
      // equal height popover panels
      setTimeout(() => { vl.equalheight.resize() }, 250)
    },
    closeNestedNotifications (notification) {
      // stop the nestedTimer when the second panel (nested notifications) closes
      clearTimeout(this.nestedTimer)
      // reverse animation (transform direction) on the second panel (back button)
      this.reverseAnimationSecond = true
      // close the second panel (nested notifications) and go back to the list => setTimeout 50 because we need the reverseAnimationSecond to be true first
      setTimeout(() => { this.showNested = false }, 50)
      // cancel reverse animation (transform direction) on the second panel for future use
      setTimeout(() => { this.reverseAnimationSecond = false }, 250)
      // equal height popover panels
      vl.equalheight.resize()
    },
    openNotificationNested (notification) {
      this.notification = notification
      // cancel reverse animation (transform direction) on the second panel
      setTimeout(() => { this.reverseAnimationSecond = false }, 250)
      // set history for the back button to work as expected in the next step
      this.goBackTo = notification
      // open the second panel (nested notifications)
      this.showNested = true
      // close the notification detail
      this.showDetail = false
      // mark nested notifications as read
      this.nestedTimer = setTimeout(() => {
        this.notification.nested.filter(notification => !notification.isRead).forEach(notification => {
          this.$store.dispatch('notification/markNotificationAsRead', notification)
        })
      }, this.notificationsMarkAsReadTreshold)
      // equal height popover panels
      vl.equalheight.resize()
      // limit text lines
      setTimeout(() => { vl.textclamp.dressAll() }, 10)
      // equal height popover panels
      setTimeout(() => { vl.equalheight.resize() }, 250)
    },
    // mock a mocked notification (remove this)
    openNotificationMockDetail () {
      this.notification = {
        isRead: true,
        title: 'Statuswijziging (gemockte data)',
        status: 'werken gepland',
        subtitle: 'De studietoelage voor Broos Deprez werd toegekend. Bijgevoegd vindt u de beslissingsbrief met de details van de berekening en de uitbetalingsdatum.',
        date: '02 mei 2017'
      }
      this.showDetail = true
      vl.equalheight.resize()
    }
  },
  mounted () {
    // Get a reference to ourself.
    const self = this
    // Resolve the Education Events.
    self.$store.dispatch('notification/notifications').then(() => {
      // Get the education events timeline.
      self.notifications = self.$store.getters['notification/notifications']
      // Mark the educatione events timeline as loaded.
      self.notificationsLoaded = true
      // Dress the components using Flanders UI.
      setTimeout(() => { vl.popover.dressAll() }, 10)
    })
  }
}
</script>
