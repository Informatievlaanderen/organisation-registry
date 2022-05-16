<template>
  <div>
    <bl-region v-if="loading">
      <bl-layout :mod-is-wide="true">
        <bl-grid :mod-is-stacked="true">
          <bl-column>
            <div class="bl-person-banner">
              <div class="bl-person-banner__content">
                <h1 class="bl-person-banner__title">
                  {{ ($store.getters['session/nickname'] || $store.getters['session/firstName']) + ' ' + $store.getters['session/lastName'] }}
                </h1>
              </div>
            </div>
          </bl-column>
          <bl-column class="js-preloading" aria-label="Uw data wordt geladen">
            <bl-description-data :mod-is-bordered="true">
              <bl-description-data-item-wrapper>
                <bl-grid :mod-is-stacked="true">
                  <bl-column :cols="[{nom: 1, den: 4}, {nom: 1, den: 2, mod: 'm'}, {nom: 1, den: 1, mod: 'xs'}]">
                      <bl-description-data-item class-type="data"></bl-description-data-item>
                      <bl-description-data-item class-type="subdata"></bl-description-data-item>
                  </bl-column>
                  <bl-column :cols="[{nom: 1, den: 4}, {nom: 1, den: 2, mod: 'm'}, {nom: 1, den: 1, mod: 'xs'}]">
                      <bl-description-data-item class-type="data"></bl-description-data-item>
                      <bl-description-data-item class-type="subdata"></bl-description-data-item>
                  </bl-column>
                  <bl-column :cols="[{nom: 1, den: 4}, {nom: 1, den: 2, mod: 'm'}, {nom: 1, den: 1, mod: 'xs'}]">
                      <bl-description-data-item class-type="data"></bl-description-data-item>
                      <bl-description-data-item class-type="subdata"></bl-description-data-item>
                  </bl-column>
                  <bl-column :cols="[{nom: 1, den: 4}, {nom: 1, den: 2, mod: 'm'}, {nom: 1, den: 1, mod: 'xs'}]">
                      <bl-description-data-item class-type="data"></bl-description-data-item>
                      <bl-description-data-item class-type="subdata"></bl-description-data-item>
                  </bl-column>
                </bl-grid>
              </bl-description-data-item-wrapper>
            </bl-description-data>
          </bl-column>
        </bl-grid>
      </bl-layout>
    </bl-region>
    <bl-region v-if="!loading && data">
      <bl-layout :mod-is-wide="true">
        <bl-grid class="grid--is-stacked">
          <bl-column>
            <div class="bl-person-banner">
              <div class="bl-person-banner__content">
                <h1 class="bl-person-banner__title">
                  <span v-if="data.preferredFirstName" v-text="data.preferredFirstName + ' '"></span>
                  <span v-for="(firstName, index) in data.firstNames" :key="index" v-text="firstName + ' '"></span>
                  <span v-for="(lastNames, index) in data.lastNames" :key="index" v-text="lastNames + ' '"></span>
                </h1>
              </div>
            </div>
          </bl-column>
          <bl-column>
            <bl-description-data :mod-is-bordered="true" class="u-spacer--small">
              <bl-grid :mod-is-stacked="true">
                <bl-column :cols="[{nom: 1, den: 4}, {nom: 1, den: 2, mod: 'm'}, {nom: 1, den: 1, mod: 'xs'}]">
                  <bl-description-data-item class-type="data" v-if="data.birthDate || data.birthPlace">
                    <span v-if="data.birthDate.value" v-text="'° ' + $date(data.birthDate.value) + ' '"></span>
                    <span v-if="data.birthPlace.country && data.birthPlace.country.code === 'BE'" v-text="'in ' + data.birthPlace.city.name || data.birthPlace.country.name"></span>
                    <span v-if="data.birthPlace.country && data.birthPlace.country.code !== 'BE'" v-text="'in ' + data.birthPlace.city.name + ', ' + data.birthPlace.country.name"></span>
                  </bl-description-data-item>
                  <bl-description-data-item class-type="subdata" v-if="data.gender">
                    {{ data.gender }}
                  </bl-description-data-item>
                  <bl-description-data-item class-type="subdata" v-if="data.nationalities">
                    <span v-for="(nationality, index) in data.nationalities" :key="index">
                      <span v-if="nationality.code <= 700 && index === 0">Uit </span>
                      {{ nationality.name }} <span v-if="nationality.since">(sinds {{ $date(nationality.since.value) }})</span>
                      <span v-if="index !== data.nationalities.length - 1"> - </span>
                    </span>
                  </bl-description-data-item>
                </bl-column>
                <!-- eID -->
                <bl-column :cols="[{nom: 1, den: 4}, {nom: 1, den: 2, mod: 'm'}, {nom: 1, den: 1, mod: 'xs'}]" v-if="data.identification">
                  <bl-alert v-if="data.identification.expiresWarning" type="warning" :has-icon="false" :title="'eID ' + data.identification.id">
                    vervalt op {{ $date(data.identification.expires.value) }}
                  </bl-alert>
                  <div v-else>
                    <bl-description-data-item class-type="data">
                      eID {{ data.identification.id }}
                    </bl-description-data-item>
                    <bl-description-data-item class-type="subdata">
                      <span v-if="data.identification.expires">vervalt op {{ $date(data.identification.expires.value) }}<br></span>
                    </bl-description-data-item>
                  </div>
                </bl-column>
                <!-- END eID -->
                <!-- Register -->
                <bl-column :cols="[{nom: 1, den: 4}, {nom: 1, den: 2, mod: 'm'}, {nom: 1, den: 1, mod: 'xs'}]" v-if="data.registry || data.administration">
                  <bl-description-data-item class-type="data" v-if="data.registry">
                    Ingeschreven in het {{ data.registry }}
                  </bl-description-data-item>
                  <bl-description-data-item class-type="subdata" v-if="data.administration && (data.administration.country || data.administration.city)">
                    beheerd door
                    <span v-if="data.administration.country && data.administration.country.code === 'BE'" v-text="data.administration.city.name || data.administration.country.name"></span>
                    <span v-if="data.administration.country && data.administration.country.code !== 'BE'" v-text="data.administration.country.name"></span>
                  </bl-description-data-item>
                </bl-column>
                <!-- END Register -->
                <!-- Burgerlijke staat -->
                <bl-column :cols="[{nom: 1, den: 4}, {nom: 1, den: 2, mod: 'm'}, {nom: 1, den: 1, mod: 'xs'}]" v-if="data.civilStatus">
                  <template v-for="(status, index) in data.civilStatus">

                    <!-- Gehuwd -->
                    <template v-if="status && status.label && status.label === 'gehuwd'">
                      <bl-description-data-item class-type="data">
                        <span class="u-text-uppercase-first-letter">
                          {{ status.label }}<span v-if="status.partner"> met {{ status.partner.preferredFirstName || status.partner.firstNames[0] || null }}
                          {{ status.partner.lastNames[0] || null }}</span> <br>
                          <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.since">sinds {{ $date(status.since.value) }} </span>
                          <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code === 'BE'" v-text="'in ' + status.location.city.name || status.location.country.name"></span>
                          <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code !== 'BE'" v-text="'in ' + (status.location.city.name ? status.location.city.name + ' ' : '') + status.location.country.name"></span>
                        </span>
                      </bl-description-data-item>
                    </template>
                    <!-- END Gehuwd -->

                    <!-- Ongehuwd -->
                    <template v-if="status && status.label && status.label === 'ongehuwd'">
                      <bl-description-data-item class-type="data">
                        <span class="u-text-uppercase-first-letter">
                          {{ status.label }} <br>
                          <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code === 'BE'" v-text="'in ' + status.location.city.name || status.location.country.name"></span>
                          <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code !== 'BE'" v-text="'in ' + (status.location.city.name ? status.location.city.name + ' ' : '') + status.location.country.name"></span>
                        </span>
                      </bl-description-data-item>
                    </template>
                    <!-- END Ongehuwd -->

                    <!-- Gescheiden -->
                    <template v-if="status && status.label && status.label === 'gescheiden'">
                      <bl-description-data-item class-type="data">
                        <span class="u-text-uppercase-first-letter">
                          {{ status.label }} <br>
                          <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.since">sinds {{ $date(status.since.value) }} </span>
                          <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code === 'BE'" v-text="'in ' + status.location.city.name || status.location.country.name"></span>
                          <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code !== 'BE'" v-text="'in ' + (status.location.city.name ? status.location.city.name + ' ' : '') + status.location.country.name"></span>
                        </span>
                      </bl-description-data-item>
                    </template>
                    <!-- END Gescheiden -->

                    <!-- Wettelijk samenwonend -->
                    <template v-if="status && status.label && status.label === 'wettelijk samenwonend'">
                      <bl-description-data-item class-type="data">
                        <span class="u-text-uppercase-first-letter">
                          {{ status.label }}<span v-if="status.partner"> met {{ status.partner.preferredFirstName || status.partner.firstNames[0] || null }}
                          {{ status.partner.lastNames[0] || null }}</span>
                        </span> <br>
                        <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.since">sinds {{ $date(status.since.value) }} </span>
                        <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code === 'BE'" v-text="'in ' + status.location.city.name || status.location.country.name"></span>
                        <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code !== 'BE'" v-text="'in ' + (status.location.city.name ? status.location.city.name + ' ' : '') + status.location.country.name"></span>
                      </bl-description-data-item>
                    </template>
                    <!-- END Wettelijk samenwonend -->

                    <!-- partnerschap -->
                    <template v-if="status && status.label && status.label === 'partnerschap'">
                      <bl-description-data-item class-type="data">
                        <span class="u-text-uppercase-first-letter">
                          {{ status.label }}<span v-if="status.partner"> met {{ status.partner.preferredFirstName || status.partner.firstNames[0] || null }}
                          {{ status.partner.lastNames[0] || null }}</span>
                        </span> <br>
                        <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.since">sinds {{ $date(status.since.value) }} </span>
                        <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code === 'BE'" v-text="'in ' + status.location.city.name || status.location.country.name"></span>
                        <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code !== 'BE'" v-text="'in ' + (status.location.city.name ? status.location.city.name + ' ' : '') + status.location.country.name"></span>
                      </bl-description-data-item>
                    </template>
                    <!-- END partnerschap -->

                    <!-- weduw(e)naar -->
                    <template v-if="status && status.label && status.label === 'weduwnaar (weduwe)'">
                      <bl-description-data-item class-type="data">
                        <span class="u-text-uppercase-first-letter">
                          {{ status.label }}
                        </span> <br>
                        <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.since">sinds {{ $date(status.since.value) }} </span>
                        <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code === 'BE'" v-text="'in ' + status.location.city.name || status.location.country.name"></span>
                        <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code !== 'BE'" v-text="'in ' + (status.location.city.name ? status.location.city.name + ' ' : '') + status.location.country.name"></span>
                      </bl-description-data-item>
                    </template>
                    <!-- END weduw(e)naar -->

                    <!-- Putatief huwelijk -->
                    <template v-if="status && status.label && status.label === 'putatief huwelijk'">
                      <bl-description-data-item class-type="data">
                        <span class="u-text-uppercase-first-letter">
                          {{ status.label }}<span v-if="status.partner"> met {{ status.partner.preferredFirstName || status.partner.firstNames[0] || null }}
                          {{ status.partner.lastNames[0] || null }}</span> <br>
                          <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.since">sinds {{ $date(status.since.value) }} </span>
                          <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code === 'BE'" v-text="'in ' + status.location.city.name || status.location.country.name"></span>
                          <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code !== 'BE'" v-text="'in ' + (status.location.city.name ? status.location.city.name + ' ' : '') + status.location.country.name"></span>
                        </span>
                      </bl-description-data-item>
                    </template>
                    <!-- END Putatief huwelijk -->

                    <!-- nietigverklaring van het huwelijk -->
                    <template v-if="status && status.label && status.label === 'nietigverklaring van het huwelijk'">
                      <bl-description-data-item class-type="data">
                        <span class="u-text-uppercase-first-letter">
                          {{ status.label }}
                        </span> <br>
                        <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.since">sinds {{ $date(status.since.value) }} </span>
                        <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code === 'BE'" v-text="'in ' + status.location.city.name || status.location.country.name"></span>
                        <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code !== 'BE'" v-text="'in ' + (status.location.city.name ? status.location.city.name + ' ' : '') + status.location.country.name"></span>
                      </bl-description-data-item>
                    </template>
                    <!-- END nietigverklaring van het huwelijk -->

                    <!-- beëindiging partnerschap -->
                    <template v-if="status && status.label && status.label === 'beëindiging partnerschap'">
                      <bl-description-data-item class-type="data">
                        <span class="u-text-uppercase-first-letter">
                          {{ status.label }}
                        </span> <br>
                        <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.since">sinds {{ $date(status.since.value) }} </span>
                        <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code === 'BE'" v-text="'in ' + status.location.city.name || status.location.country.name"></span>
                        <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code !== 'BE'" v-text="'in ' + (status.location.city.name ? status.location.city.name + ' ' : '') + status.location.country.name"></span>
                      </bl-description-data-item>
                    </template>
                    <!-- END beëindiging partnerschap -->

                    <!-- onbepaald -->
                    <template v-if="status && status.label && status.label === 'onbepaald'">
                      <bl-description-data-item class-type="data">
                        <span class="u-text-uppercase-first-letter">
                          {{ status.label }}
                        </span> <br>
                        <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.since">sinds {{ $date(status.since.value) }} </span>
                        <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code === 'BE'" v-text="'in ' + status.location.city.name || status.location.country.name"></span>
                        <span class="u-small-text u-color-gray u-font-weight-regular" v-if="status.location && status.location.country && status.location.country.code !== 'BE'" v-text="'in ' + (status.location.city.name ? status.location.city.name + ' ' : '') + status.location.country.name"></span>
                      </bl-description-data-item>
                    </template>
                    <!-- END onbepaald -->
                  </template>
                </bl-column>
                <!-- END Burgerlijke staat -->
              </bl-grid>
            </bl-description-data>
            <bl-typography v-if="data.urlNationalRegister">
              <a class="link--icon--external" :href="data.urlNationalRegister.url" target="_BLANK">Bekijk meer details over uw dossier bij het {{ data.urlNationalRegister.label }} <span class="link__note">(vereist opnieuw aanmelden)</span></a>
            </bl-typography>
          </bl-column>
        </bl-grid>
      </bl-layout>
    </bl-region>
    <bl-region v-if="!loading && error">
      <bl-layout :mod-is-wide="true">
        <bl-grid class="grid--is-stacked">
          <bl-column>
            <div class="bl-person-banner">
              <div class="bl-person-banner__content">
                <h1 class="bl-person-banner__title">
                  {{ ($store.getters['session/nickname'] || $store.getters['session/firstName']) + ' ' + $store.getters['session/lastName'] }}
                </h1>
              </div>
            </div>
          </bl-column>
          <bl-column>
            <bl-alert type="error" title="Geen data beschikbaar">
              <p>Er liep iets fout met de connectie naar MAGDA. (boodschap nog aan te passen)</p>
            </bl-alert>
          </bl-column>
        </bl-grid>
      </bl-layout>
    </bl-region>
  </div>
</template>

<script>

import { mapGetters } from 'vuex'

import BlDescriptionData from '~components/partials/description-data/DescriptionData.vue'
import BlDescriptionDataItem from '~components/partials/description-data/DescriptionDataItem.vue'
import BlDescriptionDataItemWrapper from '~components/partials/description-data/DescriptionDataItemWrapper.vue'
import BlAlert from '~components/partials/alert/Alert.vue'
import BlPreloadCard from '~components/service-components/preload-card/PreloadCard.vue'

export default {
  name: 'person-particulars',
  middleware: ['authenticated'],
  components: {
    BlDescriptionData,
    BlDescriptionDataItem,
    BlDescriptionDataItemWrapper,
    BlAlert,
    BlPreloadCard
  },
  computed: {
    ...mapGetters({
      data: 'person/particulars',
      error: 'person/particularsFailed'
    }),
    loading () {
      const resolved = this.$store.getters['person/particularsResolved']
      const failed = this.$store.getters['person/particularsFailed']
      return !resolved && !failed
    }
  },
  mounted () {
    // Get a reference to ourself.
    const self = this
    // Resolve person Particulars.
    self.$store.dispatch('person/particulars')
  }
}
</script>
