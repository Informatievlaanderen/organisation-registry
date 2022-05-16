<template>
  <div class="bl-card">
    <div class="bl-card__header bl-card__header--alt">
      <div class="bl-card__header__inner">
        <div class="bl-card__header__content">
          <h1 class="bl-card__header__title">{{ title }}</h1>
          <h2 class="bl-card__header__meta" v-if="subtitle">{{subtitle}}</h2>
        </div>
      </div>
    </div>
    <div class="bl-card__content">
      <bl-description-data v-if="data.queries.length">
        <bl-grid :mod-is-stacked="true">
          <bl-column :cols="[{nom: 1, den: 3}, {nom: 1, den: 2, mod: 's'}, {nom: 1, den: 1, mod: 'xs'}]" v-for="(query, index) in data.queries" :key="index">
            <bl-description-data-item-wrapper :mod-is-bordered="true">
              <bl-description-data-item class-type="data">{{ query.title }}</bl-description-data-item>
              <bl-description-data-item class-type="subdata">bij {{ query.source }}</bl-description-data-item>
            </bl-description-data-item-wrapper>
          </bl-column>
        </bl-grid>
      </bl-description-data>
      <!-- END Certificates -->
      <div class="u-spacer" v-if="data.queries.length && data.permissions.length"></div>
      <div class="bl-card__content__accordion js-accordion" v-if="data.permissions.length">
        <a class="bl-card__content__accordion__toggle toggle--arrow-down js-accordion__toggle"><i class="toggle__icon" aria-hidden="true"></i>{{ data.permissions.length }} machtiging<template v-if="data.permissions.length > 1">en</template></a>
        <div class="accordion__content">
          <div class="bl-card__content__accordion__content">
            <ul class="link-list link-list--small">
              <li class="" v-for="(permission, index) in data.permissions" :key="index">
                <a target="_BLANK" :href="permission.url" :class="{'link': true, 'link--icon': (permission.type == 'application/pdf')}">
                  <i class="vi vi-paperclip"></i>
                  {{ permission.label }} <span v-if="permission.type === 'application/pdf'">(pdf)</span>
                </a>
              </li>
            </ul>
          </div>
        </div>
      </div>
      <div class="bl-card__footer">
        <p class="u-small-text u-color-dove">{{ data.queries.length }} opvraging<span v-if="data.queries.length > 1">en</span> | {{date}}</p>
      </div>
    </div>
  </div>
</template>

<script>

import BlDescriptionData from '~components/partials/description-data/DescriptionData.vue'
import BlDescriptionDataItem from '~components/partials/description-data/DescriptionDataItem.vue'
import BlDescriptionDataItemWrapper from '~components/partials/description-data/DescriptionDataItemWrapper.vue'

export default {
  name: 'extraction-card',
  components: {
    BlDescriptionData,
    BlDescriptionDataItem,
    BlDescriptionDataItemWrapper
  },
  props: ['data'],
  computed: {
    title () {
      const organization = this.data.organization
      const service = this.data.service

      if (organization) {
        return organization.name + (organization.shortName ? ` (${organization.shortName})` : '')
      } else if (service) {
        return 'Opgevraagde gegevens voor ' + service
      }

      return ''
    },
    subtitle () {
      if (this.data.organization && this.data.service) {
        return 'Opgevraagde gegevens voor ' + this.data.service
      }

      return ''
    },
    date () {
      return this.$date(this.data.date, 'long')
    }
  },
  mounted () {
    vl.accordion.dressAll()
  }
}
</script>

