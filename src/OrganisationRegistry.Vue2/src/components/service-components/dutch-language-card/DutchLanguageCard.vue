<template>
  <div class="bl-card">
    <div :class="{'bl-card__header': true, 'bl-card__header--alt': hasCertificates }">
      <div class="bl-card__header__inner">
        <div class="bl-card__badges" aria-hidden="true">
          <bl-badge icon="icon-nederlandse-taal" :mod-is-alt="true"></bl-badge>
          <bl-badge v-if="hasCertificates" icon="icon-document-certificate" :mod-is-success="hasCertificates" :mod-is-overlap="true"></bl-badge>
        </div>
        <div class="bl-card__header__content">
          <h1 class="bl-card__header__title">{{ data.education }}</h1>
          <h2 class="bl-card__header__meta"><template v-if="data.institute">{{ data.institute.name }}</template> <template v-if="meta.education">{{ '| ' + meta.education }}</template></h2>
        </div>
      </div>
    </div>
    <div class="bl-card__content">
      <template v-if="data.institute.address && data.institute.address.street && data.institute.address.city">
        <p class="bl-icon-text"><svg role="presentation" class="bl-icon-text__icon" v-svg symbol="icon-pin-alt" size="0 0 24 24" aria-hidden="true"></svg><span class="bl-icon-text__label">{{ data.institute.address.street.name }} {{ data.institute.address.street.number }}, <template v-if="data.institute.address.city && data.institute.address.city.code">{{ data.institute.address.city.code }}</template>&nbsp;<template v-if="data.institute.address.city && data.institute.address.city.name">{{ data.institute.address.city.name }}</template></span></p>
      </template>
      <div v-if="data.institute.address" class="u-spacer--small"></div>
      <div v-if="certificates.length > 0" class="description-data">
        <bl-grid :mod-is-stacked="true">
          <bl-column :cols="[{nom: 1, den: 3}, {nom: 1, den: 2, mod: 's'}, {nom: 1, den: 1, mod: 'xs'}]" v-for="(certificate, index) in certificates" :key="index" :index="index">
            <div class="bl-description-data__item bl-description-data__item--bordered">
              <p class="description-data__subdata" v-if="certificate.date"><time :datetime="certificate.date.value">{{ $date(certificate.date.value) }}</time></p>
              <p class="description-data__data">{{ certificate.label }}</p>
              <p class="description-data__subdata">Diploma</p>
            </div>
          </bl-column>
        </bl-grid>
      </div>
      <div v-if="meta !== null && certificates.length > 0 && Object.keys(meta).length !== 0 && meta.constructor === Object" class="u-spacer--small"></div>
      <!-- @todo Object.keys(meta).length > 1 is not a clean way to check, but the meta always contains "grantsCertificateSecundary. To check with Adriaan how to do properly" -->
      <div v-if="meta && Object.keys(meta).length > 1 && meta.constructor === Object" :class="{'bl-card__content__accordion js-accordion': true, 'js-accordion--open': !hasCertificates}">
        <a class="bl-card__content__accordion__toggle toggle--arrow-down js-accordion__toggle"><i class="toggle__icon" aria-hidden="true"></i>{{ lbl.advise }}</a>
        <div class="accordion__content">
          <div class="bl-card__content__accordion__content">
            <div class="description-data">
              <bl-grid :mod-is-stacked="true">
                <bl-column :cols="[{nom: 1, den: 2}, {nom: 1, den: 1, mod: 's'}]" v-if="meta.shortname || meta.longName">
                  <bl-description-data-item class-type="data">
                    {{ meta.shortName }}
                  </bl-description-data-item>
                  <bl-description-data-item class-type="subdata">
                    {{ meta.longName }}
                  </bl-description-data-item>
                </bl-column>
                <bl-column :cols="[{nom: 1, den: 2}, {nom: 1, den: 1, mod: 's'}]">
                  <bl-description-data-item class-type="data" v-if="meta.level">{{ lbl.level }}</bl-description-data-item>
                  <bl-description-data-item class-type="subdata" v-if="meta.level">
                    <p v-if="meta.level.mostRecent"><strong>{{ meta.level.mostRecent.shortName }}</strong> ({{ lbl.mostRecent }})</p>
                    <p v-if="meta.level.highest"><strong>{{ meta.level.highest.shortName }}</strong> ({{ lbl.highest }})</p>
                  </bl-description-data-item>
                </bl-column>
              </bl-grid>
            </div>
          </div>
          <slot></slot>
        </div>
      </div>
      <div class="bl-card__footer" v-if="reference">
        <a :href="reference.url.toLowerCase()" class="link--icon--external" target="_BLANK">{{ lbl.moreDetailsOn }} {{ stripMetaFromUrl(reference.url.toLowerCase()) }}</a>
      </div>
    </div>
  </div>
</template>

<script>

import BlDescriptionData from '~components/partials/description-data/DescriptionData.vue'
import BlDescriptionDataItem from '~components/partials/description-data/DescriptionDataItem.vue'
import BlBadge from '~components/partials/badge/Badge.vue'

export default {
  name: 'dutch-language-card',
  components: {
    BlDescriptionData,
    BlDescriptionDataItem,
    BlBadge
  },
  props: {
    data: {
      default: null,
      type: Object
    }
  },
  data () {
    return {
      // Meta information.
      meta: this.data.meta || null,
      // Certificates count check, used in template.
      hasCertificates: this.data.certificates.items.length > 0,
      // Certificates array
      certificates: this.data.certificates.items || null,
      // References
      reference: this.data.reference || null,
      // Labels (to change with i18n)
      lbl: {
        advise: 'Advies',
        moreDetailsOn: 'Bekijk meer details op',
        mostRecent: 'Laatst behaald',
        highest: 'Hoogst behaald',
        level: 'Niveau'
      }
    }
  },
  // @todo to remove
  methods: {
    stripMetaFromUrl: (url) => {
      if (!url) return ''
      return url.replace(/(^\w+:|^)\/\//, '')
    }
  }
}
</script>

