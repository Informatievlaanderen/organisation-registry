<template>
  <div class="bl-card">
    <div :class="{'bl-card__header': true, 'bl-card__header--alt': hasCertificates, 'bl-card__header--has-popover': true }">
      <div class="bl-card__header__inner">
        <div class="popover popover--right popover--xxl popover--has-footer bl-card__header__popover js-popover js-popover--open" v-on-clickaway="closeEducationPopover">
          <a href="#" class="js-popover__toggle" @click.prevent="toggleEducationPopover">
            <i class="vi vi-u-badge vi-u-badge--action vi-u-badge--xsmall vi-info" aria-hidden="true"></i>
            <span class="u-visually-hidden">Meer info</span>
          </a>
          <div class="popover__content" v-if="educationPopoverOpen">
            <a href="" class="popover__close link--icon--close" @click.prevent="toggleEducationPopover"><span class="u-visually-hidden">Venster sluiten</span></a>
            <div class="popover__content__inner">
              <bl-description-data>
                <bl-description-data-item-wrapper>
                  <bl-grid :mod-is-stacked="true">
                    <bl-column :cols="[{nom: 1, den: 1}]">
                        <bl-description-data-item class-type="data">Uitreikingsdatum:</bl-description-data-item>
                        <bl-description-data-item class-type="subdata">De datum waarop de school u het diploma heeft uitgereikt</bl-description-data-item>
                        <bl-description-data-item class-type="source">Bron: Departement Onderwijs</bl-description-data-item>

                    </bl-column>
                    <bl-column :cols="[{nom: 1, den: 1}]">

                        <bl-description-data-item class-type="data">Deelcertificaat:</bl-description-data-item>
                        <bl-description-data-item class-type="subdata">Een studiebewijs in het volwassenenonderwijs; voor elke module waarvoor u slaagt, ontvangt u een deelcertificaat</bl-description-data-item>
                        <bl-description-data-item class-type="source">Bron: Departement Onderwijs</bl-description-data-item>

                    </bl-column>
                  </bl-grid>
                </bl-description-data-item-wrapper>
              </bl-description-data>
            </div>
            <footer class="popover__content__footer">
              <bl-grid :mod-is-stacked="true">
                <bl-column :cols="[{nom: 1, den: 2}]">
                  <bl-description-data>
                    <bl-description-data-item class-type="data">Zijn deze gegevens niet correct?</bl-description-data-item>
                    <bl-description-data-item class-type="subdata">
                      <nuxt-link to="/onderwijs/edit">Meld het ons</nuxt-link> zodat we het kunnen aanpassen.
                    </bl-description-data-item>
                  </bl-description-data>
                </bl-column>
                <bl-column :cols="[{nom: 1, den: 2}]">
                  <bl-description-data>
                    <bl-description-data-item class-type="data">Vragen of hulp nodig?</bl-description-data-item>
                    <bl-description-data-item class-type="subdata">
                      Check onze <a href="#">veelgestelde vragen</a> of <a href="#">contacteer ons</a>
                    </bl-description-data-item>
                  </bl-description-data>
                </bl-column>
              </bl-grid>
            </footer>
          </div>
        </div>
        <div class="bl-card__badges" aria-hidden="true">
          <bl-badge :icon="data.icon" :mod-is-alt="!hasCertificates"></bl-badge>
          <bl-badge v-if="hasCertificates > 0" icon="icon-document-certificate" :mod-is-success="hasCertificates" :mod-is-overlap="true"></bl-badge>
        </div>
        <div class="bl-card__header__content">
          <h1 class="bl-card__header__title" v-if="data.education">{{ data.education }}</h1>
          <h2 class="bl-card__header__meta" v-if="data.institute || data.category"><template v-if="data.institute && data.institute.name">{{ data.institute.name }}</template><template v-if="data.institute && data.institute.name && data.category"> | </template>{{ data.category }}<template v-if="data.meta.grantsCertificateSecundary"> | {{ lbl.grantsCertificateSecundary }}</template></h2>
        </div>
      </div>
    </div>
    <div class="bl-card__content">
      <template v-if="data.institute.address && data.institute.address.street && data.institute.address.city">
        <p class="bl-icon-text"><svg role="presentation" class="bl-icon-text__icon" v-svg symbol="icon-pin-alt" size="0 0 24 24" aria-hidden="true"></svg><span class="bl-icon-text__label">{{ data.institute.address.street.name }} {{ data.institute.address.street.number }}, <template v-if="data.institute.address.city && data.institute.address.city.code">{{ data.institute.address.city.code }}</template> <template v-if="data.institute.address.city && data.institute.address.city.name">{{ data.institute.address.city.name }}</template></span></p>
      </template>
      <div v-if="data.institute.address" class="u-spacer--small"></div>
      <bl-description-data v-if="hasCertificates">
        <bl-grid :mod-is-stacked="true">
          <bl-column v-for="(certificate, index) in certificates" :key="index" :index="index" :cols="[{nom: 1, den: 3}, {nom: 1, den: 2, mod: 's'}, {nom: 1, den: 1, mod: 'xs'}]">
              <bl-description-data-item-wrapper :mod-is-bordered="true">
                <bl-description-data-item class-type="subdata" v-if="certificate.issueDate"><time :datetime="certificate.issueDate.value">{{ $date(certificate.issueDate.value) }}</time></bl-description-data-item>
                <bl-description-data-item class-type="data" v-if="certificate.label">{{ certificate.label }}</bl-description-data-item>
                <bl-description-data-item class-type="subdata">
                  {{ lbl.certificate }}
                  <template v-if="certificate.meta && certificate.meta.country && certificate.meta.country.code !== 'BE'">- {{ lbl.countryBefore }} {{ certificate.meta.country.naam }}</template>
                </bl-description-data-item>
              </bl-description-data-item-wrapper>
          </bl-column>
        </bl-grid>
      </bl-description-data>
      <div v-if="hasRegistrations && hasCertificates" class="u-spacer--small"></div>
      <div v-if="hasRegistrations" :class="{'bl-card__content__accordion js-accordion': true, 'js-accordion--open': !hasCertificates}">
        <a class="bl-card__content__accordion__toggle toggle--arrow-down js-accordion__toggle"><i class="toggle__icon" aria-hidden="true"></i>{{ lbl.subscribed }}<span v-if="firstRegistration && firstRegistration !== 0"> {{ lbl.from }} {{ firstRegistration }}</span></a>
        <div class="accordion__content">
          <div class="bl-card__content__accordion__content">
            <bl-description-data>
              <bl-grid :mod-is-stacked="true">
                <bl-column v-for="(registration, index) in registrations" :key="index" :index="index" :cols="[{nom: 1, den: 4}, {nom: 1, den: 3, mod: 'm'}, {nom: 1, den: 2, mod: 's'}, {nom: 1, den: 1, mod: 'xs'}]">
                  <!-- Higher (hogeschool) -->
                  <template v-if="registration.type === 'higher-education'">
                    <bl-description-data-item class-type="subdata" v-if="registration.year">
                      <time :datetime="registration.year">{{ registration.year || null }} <template v-if="registration.course && registration.course.einde">- {{ registration.course.einde }}</template></time>
                    </bl-description-data-item>
                    <bl-description-data-item class-type="data" v-if="registration.meta.education">
                      {{ registration.meta.education.name || null }}
                    </bl-description-data-item>
                    <bl-description-data-item class-type="subdata" v-if="registration.meta.contract">
                      <template v-if="registration.meta.contract.name.toLowerCase() !== 'voltijds'">{{ registration.meta.contract.name || null }}</template>
                    </bl-description-data-item>
                    <bl-description-data-item class-type="subdata" v-if="registration.status.code === 'UIT'">
                      {{ registration.status.name }}
                    </bl-description-data-item>
                  </template>
                  <!-- END -->

                  <!-- Adult (volwassenen) -->
                  <template v-if="registration.type === 'adult-education'">
                    <bl-description-data-item class-type="subdata" v-if="registration.year">
                      <time :datetime="registration.year">{{ registration.year }}</time>
                    </bl-description-data-item>
                    <bl-description-data-item class-type="data" v-if="registration.meta.module">
                      <template v-if="registration.meta.module.name.toLowerCase() !== 'voltijds'">{{ registration.meta.module.name }}</template>
                    </bl-description-data-item>
                    <bl-description-data-item class-type="subdata" v-if="registration.status.code === 'UIT'">
                      {{ registration.status.name }}
                    </bl-description-data-item>
                    <bl-description-data-item class-type="subdata" v-if="registration.meta && registration.meta.result">
                      {{ registration.meta.result.name }}
                    </bl-description-data-item>
                  </template>
                  <!-- END -->

                  <!-- Compulsory (leerplicht) -->
                  <template v-if="registration.type === 'compulsory-education'">
                    <bl-description-data-item class-type="subdata" v-if="registration.year">
                      <time :datetime="registration.year">{{ registration.year || null }} <template v-if="registration.course && registration.course.einde">- {{ registration.course.einde }}</template></time>
                    </bl-description-data-item>
                    <bl-description-data-item class-type="data" v-if="registration.meta.exceptionalEducation">
                      <template v-if="registration.meta.exceptionalEducation.code">{{ registration.meta.exceptionalEducation.code }} -</template>
                      {{ registration.meta.exceptionalEducation.name || null }}
                    </bl-description-data-item>
                    <bl-description-data-item class-type="subdata" v-if="registration.meta.programType">
                      <template v-if="registration.meta.programType.name.toLowerCase() !== 'voltijds'">{{ registration.meta.programType.name }}</template>
                    </bl-description-data-item>
                    <bl-description-data-item class-type="subdata" v-if="registration.status.code === 'UIT'">
                      {{ registration.status.name }}
                    </bl-description-data-item>
                  </template>
                  <!-- END -->

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

import { mixin as clickaway } from 'vue-clickaway'

import BlDescriptionData from '~components/partials/description-data/DescriptionData.vue'
import BlDescriptionDataItem from '~components/partials/description-data/DescriptionDataItem.vue'
import BlDescriptionDataItemWrapper from '~components/partials/description-data/DescriptionDataItemWrapper.vue'
import BlGrid from '~components/frame/grid/Grid.vue'
import BlColumn from '~components/frame/column/Column.vue'
import BlBadge from '~components/partials/badge/Badge.vue'

export default {
  name: 'education-card',
  mixins: [ clickaway ],
  components: {
    BlDescriptionData,
    BlDescriptionDataItem,
    BlDescriptionDataItemWrapper,
    BlGrid,
    BlColumn,
    BlBadge
  },
  props: ['data'],
  data () {
    return {
      // Meta information
      meta: this.data.meta || null,
      // Certificates boolean
      hasCertificates: this.data.certificates.items.length > 0,
      // Certificates
      certificates: this.data.certificates.items || null,
      // Registrations boolean
      hasRegistrations: this.data.registrations.items.length > 0,
      // First element of registrations
      firstRegistration: this.data.registrations.meta.firstRegistration || null,
      // Registrations
      registrations: this.data.registrations.items,
      // Labels (to change with i18n)
      lbl: {
        certificate: 'Diploma',
        countryBefore: 'uitgereikt in',
        from: 'vanaf',
        moreDetailsOn: 'Bekijk meer details op',
        subscribed: 'Ingeschreven',
        grantsCertificateSecundary: 'Recht op diploma secundair onderwijs'
      },
      // Popover
      educationPopoverOpen: false
    }
  },
  // @todo to remove
  methods: {
    toggleEducationPopover () {
      this.educationPopoverOpen = !this.educationPopoverOpen
    },
    closeEducationPopover () {
      if (this.educationPopoverOpen) {
        this.educationPopoverOpen = false
      }
    }
  }
}
</script>

