<template>
  <div class="bl-card">
    <div :class="{ 'bl-card__header': true, 'bl-card__header--alt': hasCertificates }">
      <div class="bl-card__header__inner">
        <div class="bl-card__badges" aria-hidden="true">
          <bl-badge :mod-is-alt="true" icon="icon-inburgeringstraject"></bl-badge>
          <bl-badge v-if="hasCertificates" icon="icon-document-certificate" :mod-is-success="hasCertificates" :mod-is-overlap="true"></bl-badge>
        </div>
        <div class="bl-card__header__content">
          <h1 class="bl-card__header__title">{{ data.education }}</h1>
          <h2 class="bl-card__header__meta"><template v-if="data.institute">{{ data.institute.name }}</template></h2>
        </div>
      </div>
    </div>
    <div class="bl-card__content">
      <template v-if="data.institute.address">
        <p class="bl-icon-text"><svg role="presentation" class="bl-icon-text__icon" v-svg symbol="icon-pin-alt" size="0 0 24 24" aria-hidden="true"></svg><span class="bl-icon-text__label">{{ data.institute.address.street.name }} {{ data.institute.address.street.number }}, {{ data.institute.address.city.code }} {{ data.institute.address.city.name }}</span></p>
      </template>
      <div v-if="data.institute.address" class="u-spacer--small"></div>
      <!-- Certificates -->
      <bl-description-data v-if="hasCertificates">
        <bl-grid :mod-is-stacked="true">
          <bl-column :cols="[{nom: 1, den: 3}, {nom: 1, den: 2, mod: 's'}, {nom: 1, den: 1, mod: 'xs'}]" v-for="(certificate, index) in certificates" :key="index">
            <bl-description-data-item-wrapper :mod-is-bordered="true">
              <bl-description-data-item class-type="subdata"><time :pubtime="$date(certificate.issueDate.value)">{{ $date(certificate.issueDate.value) }}</time></bl-description-data-item>
              <bl-description-data-item class-type="data">{{ certificate.label.charAt(0).toUpperCase() + certificate.label.slice(1) }}</bl-description-data-item>
              <bl-description-data-item class-type="meta" v-if="certificate.meta && certificate.meta.issuer">{{ lbl.issuer }}: {{ certificate.meta.issuer.naam }}</bl-description-data-item>
            </bl-description-data-item-wrapper>
          </bl-column>
        </bl-grid>
      </bl-description-data>
      <!-- END Certificates -->
      <div class="u-spacer" v-if="hasCertificates"></div>
      <div :class="{ 'bl-card__content__accordion js-accordion': true, 'js-accordion--open': !hasCertificates }">
        <a class="bl-card__content__accordion__toggle toggle--arrow-down js-accordion__toggle"><i class="toggle__icon" aria-hidden="true"></i>{{ lbl.road }}</a>
        <div class="accordion__content">
          <div class="bl-card__content__accordion__content">
            <bl-description-data>
              <bl-grid :mod-is-stacked="true">
                <bl-column :cols="[{nom: 1, den: 2}, {nom: 1, den: 1, mod: 's'}]" v-if="meta.target.reason.notMandatory">
                  <bl-description-data-item class-type="data">Vrijgestelde inburgeraar</bl-description-data-item>
                  <bl-description-data-item class-type="subdata">{{ meta.target.reason.notMandatory }}</bl-description-data-item>
                </bl-column>
                <bl-column :cols="[{nom: 1, den: 2}, {nom: 1, den: 1, mod: 's'}]" v-if="meta.status">
                  <bl-description-data-item class-type="data">{{ meta.target.type.naam }}</bl-description-data-item>
                  <bl-description-data-item class-type="subdata">{{ ((meta.target.type.code === 'NEE') ? meta.target.reason.noTarget : meta.status.name) }}</bl-description-data-item>
                </bl-column>
                <bl-column :cols="[{nom: 1, den: 2}, {nom: 1, den: 1, mod: 's'}]" v-if="meta.careerOrientationAdvice">
                  <bl-description-data-item class-type="subdata">{{ lbl.adviseOrientation }}</bl-description-data-item>
                  <bl-description-data-item class-type="data">{{ meta.careerOrientationAdvice.name }}</bl-description-data-item>
                </bl-column>
                <bl-column :cols="[{nom: 1, den: 2}, {nom: 1, den: 1, mod: 's'}]" v-if="meta.decision">
                  <bl-description-data-item class-type="subdata" v-if="meta.decision.date">{{ lbl.decisionFrom }} <time :datetime="$date(meta.decision.date.value)">{{ $date(meta.decision.date.value) }}</time></bl-description-data-item>
                  <bl-description-data-item class-type="data">{{ meta.decision.name }}</bl-description-data-item>
                  <bl-description-data-item class-type="subdata">{{ meta.decision.reason }}</bl-description-data-item>
                </bl-column>
              </bl-grid>
            </bl-description-data>
          </div>
        </div>
      </div>
      <div class="bl-card__content__accordion bl-card__content__accordion--up js-accordion js-accordion--open" v-if="hasViolations">
        <a class="bl-card__content__accordion__toggle toggle--arrow-down js-accordion__toggle"><i class="toggle__icon" aria-hidden></i>{{ lbl.violations }} ({{ violations.length }})</a>
        <div class="accordion__content">
          <div class="bl-card__content__accordion__content">
            <bl-description-data>
              <bl-grid :mod-is-stacked="true">
                <bl-column :cols="[{nom: 1, den: 3}, {nom: 1, den: 2, mod: 'm'}, {nom: 1, den: 1, mod: 's'}]" v-for="(violation, index) in violations" :key="index">
                  <section class="bl-violation">
                    <h3 class="bl-violation__title" v-if="violation.type">{{ violation.type.name }}</h3>
                    <div class="bl-violation__content">
                      <p>{{ lbl.seenOn }} <time :datetime="$date(violation.recordDate.value)" v-if="violation.recordDate">{{ $date(violation.recordDate.value) }}</time></p>
                      <p>{{ lbl.sanctionedOn }} <time :datetime="$date(violation.noticeSendDate.value)" v-if="violation.noticeSendDate">{{ $date(violation.noticeSendDate.value) }}</time></p>
                    </div>
                    <p class="bl-violation__meta" v-if="violation.sactioningOrganisation">{{ lbl.fileBy }} {{ violation.sactioningOrganisation.name }}</p>
                  </section>
                </bl-column>
              </bl-grid>
            </bl-description-data>
          </div>
        </div>
      </div>
      <div class="bl-card__footer" v-if="data.reference">
        <a :href="data.reference.url.toLowerCase()" class="link--icon--external" target="_BLANK">{{ lbl.moreDetailsOn }} {{ data.reference.label }}</a>
      </div>
    </div>
  </div>
</template>

<script>

import BlDescriptionData from '~components/partials/description-data/DescriptionData.vue'
import BlDescriptionDataItem from '~components/partials/description-data/DescriptionDataItem.vue'
import BlDescriptionDataItemWrapper from '~components/partials/description-data/DescriptionDataItemWrapper.vue'
import BlBadge from '~components/partials/badge/Badge.vue'

export default {
  name: 'integration-card',
  components: {
    BlDescriptionData,
    BlDescriptionDataItem,
    BlDescriptionDataItemWrapper,
    BlBadge
  },
  props: ['data'],
  data () {
    return {
      hasCertificates: this.data.certificates.items.length > 0,
      certificates: this.data.certificates.items,
      meta: this.data.meta || null,
      hasViolations: this.data.meta.violations.items.length > 0,
      violations: this.data.meta.violations.items,
      lbl: {
        adviseOrientation: 'Advies Professionele loopbaanoriëntatie',
        fileBy: 'Dossier bij',
        issuer: 'Uitgereikt door',
        moreDetailsOn: 'Bekijk meer details op',
        requiredIntegration: 'Verplichte inburgeraar',
        road: 'Traject',
        sanctionedOn: 'ingebrekestelling op',
        seenOn: 'vastgesteld op',
        violations: 'Inbreuken',
        decisionFrom: 'Besluit van'
      }
    }
  }
}
</script>

