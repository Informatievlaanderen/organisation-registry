<template>
  <div>
    <bl-region class="region--no-space-top" v-if="loading">
      <bl-layout :mod-is-wide="true">
        <bl-grid :mod-is-stacked="true">
          <bl-column>
            <div class="title-sublink">
              <bl-h type="h2" class="h2">Gezinssamenstelling <span class="title-sublink__sub">volgens het rijksregister</span></bl-h>
            </div>
          </bl-column>
          <bl-column :cols="[{nom: 1, den: 3}, {nom: 1, den: 2, mod: 's'}, {nom: 1, den: 1, mod: 'xs'}]">
            <bl-preload-card :mod-has-content="false" />
          </bl-column>
        </bl-grid>
      </bl-layout>
    </bl-region>
    <bl-region class="region--no-space-top" v-if="!loading && data && !data.incompleteData">
      <bl-layout :mod-is-wide="true">
        <bl-grid :mod-is-stacked="true">
          <bl-column>
            <div class="title-sublink">
              <bl-h type="h2" class="h2">Gezinssamenstelling <span class="title-sublink__sub">volgens het rijksregister</span></bl-h>
            </div>
          </bl-column>
          <bl-column>
            <bl-separator v-if="data.address" :mod-is-small="true" :title="'Gezinsleden verblijvend op ' + generateFormattedAddress(data.address)">
              <p v-if="data.accommodation" class="u-small-text u-color-gray u-font-weight-regular">{{ data.accommodation }}</p>
            </bl-separator>
          </bl-column>
          <bl-column>
            <bl-grid class="js-equal-height-container" :mod-is-stacked="true">
              <bl-column :cols="[{nom: 1, den: 3}, {nom: 1, den: 2, mod: 's'}, {nom: 1, den: 1, mod: 'xs'}]" v-if="data.reference">
                <section class="bl-family-tile bl-family-tile--alt js-equal-height">
                  <div class="bl-family-tile__badge">
                    <div class="bl-family-tile__badge__image" :data-initials="generateInitials(data.reference)"></div>
                  </div>
                  <div class="bl-family-tile__content">
                    <h1 class="bl-family-tile__title">{{ data.reference.preferredFirstName || data.reference.firstNames[0]}} {{ data.reference.lastNames[0]}}</h1>
                    <p class="bl-family-tile__meta" v-if="data.reference.nationalId">{{ data.reference.nationalId }}</p>
                    <p class="bl-family-tile__meta" v-if="data.reference.relation">{{ data.reference.relation }}</p>
                    <p class="bl-family-tile__meta" v-if="data.reference.self"><span class="bl-tag">uzelf</span></p>
                  </div>
                </section>
              </bl-column>
              <bl-column :cols="[{nom: 1, den: 3}, {nom: 1, den: 2, mod: 's'}, {nom: 1, den: 1, mod: 'xs'}]" v-if="data.family && data.family[0] && data.family[0].length" v-for="(partner, index) in data.family[0]" :key="index">
                <section class="bl-family-tile js-equal-height">
                  <div class="bl-family-tile__badge">
                    <div class="bl-family-tile__badge__image" :data-initials="generateInitials(partner)"></div>
                  </div>
                  <div class="bl-family-tile__content">
                    <h1 class="bl-family-tile__title">{{ partner.preferredFirstName || partner.firstNames[0]}} {{ partner.lastNames[0]}}</h1>
                    <p class="bl-family-tile__meta" v-if="partner.nationalId">{{ partner.nationalId }}</p>
                    <p class="bl-family-tile__meta">{{ partner.relation }} van {{ data.reference.preferredFirstName || data.reference.firstNames[0]}} {{ data.reference.lastNames[0]}}</p>
                    <p class="bl-family-tile__meta" v-if="partner.self"><span class="bl-tag">uzelf</span></p>
                  </div>
                </section>
              </bl-column>
            </bl-grid>
          </bl-column>
          <bl-column>
            <bl-grid class="js-equal-height-container" :mod-is-stacked="true">
              <bl-column :cols="[{nom: 1, den: 3}, {nom: 1, den: 2, mod: 's'}, {nom: 1, den: 1, mod: 'xs'}]" v-if="data.family && data.family[1] && data.family[1].length" v-for="(child, index) in data.family[1]" :key="index">
                <section class="bl-family-tile js-equal-height">
                  <div class="bl-family-tile__badge">
                    <div class="bl-family-tile__badge__image" :data-initials="generateInitials(child)"></div>
                  </div>
                  <div class="bl-family-tile__content">
                    <h1 class="bl-family-tile__title">{{ child.preferredFirstName || child.firstNames[0]}} {{ child.lastNames[0]}}</h1>
                    <p class="bl-family-tile__meta" v-if="child.nationalId">{{ child.nationalId }}</p>
                    <p class="bl-family-tile__meta">{{ child.relation }} van {{ data.reference.preferredFirstName || data.reference.firstNames[0]}} {{ data.reference.lastNames[0]}}</p>
                    <p class="bl-family-tile__meta" v-if="child.self"><span class="bl-tag">uzelf</span></p>
                  </div>
                </section>
              </bl-column>
            </bl-grid>
          </bl-column>
          <bl-column>
            <bl-grid :mod-is-stacked="true">
              <bl-column :cols="[{nom: 1, den: 3}, {nom: 1, den: 2, mod: 's'}, {nom: 1, den: 1, mod: 'xs'}]" v-if="data.family && data.family[2] && data.family[2].length" v-for="(child, index) in data.family[2]" :key="index">
                <section class="bl-family-tile js-equal-height">
                  <div class="bl-family-tile__badge">
                    <div class="bl-family-tile__badge__image" :data-initials="generateInitials(child)"></div>
                  </div>
                  <div class="bl-family-tile__content">
                    <h1 class="bl-family-tile__title">{{ child.preferredFirstName || child.firstNames[0]}} {{ child.lastNames[0]}}</h1>
                    <p class="bl-family-tile__meta" v-if="child.nationalId">{{ child.nationalId }}</p>
                    <p class="bl-family-tile__meta">niet verwant aan {{ data.reference.preferredFirstName || data.reference.firstNames[0]}} {{ data.reference.lastNames[0]}}</p>
                    <p class="bl-family-tile__meta" v-if="child.self"><span class="bl-tag">uzelf</span></p>
                  </div>
                </section>
              </bl-column>
            </bl-grid>
          </bl-column>
        </bl-grid>
      </bl-layout>
    </bl-region>
    <bl-region class="region--no-space-top" v-if="!loading && data && data.incompleteData">
      <bl-layout :mod-is-wide="true">
        <bl-grid :mod-is-stacked="true">
          <bl-column>
            <div class="title-sublink">
              <bl-h type="h2" class="h2">Gezinssamenstelling <span class="title-sublink__sub">volgens het rijksregister</span></bl-h>
            </div>
          </bl-column>
          <bl-column :cols="[{nom: 1, den: 1}]">
            <bl-alert type="error" title="Geen data beschikbaar">
              <p>{{ data.incompleteData }}</p>
            </bl-alert>
          </bl-column>
        </bl-grid>
      </bl-layout>
    </bl-region>
    <bl-region class="region--no-space-top" v-if="!loading && error">
      <bl-layout :mod-is-wide="true">
        <bl-grid :mod-is-stacked="true">
          <bl-column>
            <div class="title-sublink">
              <bl-h type="h2" class="h2">Gezinssamenstelling <span class="title-sublink__sub">volgens het rijksregister</span></bl-h>
            </div>
          </bl-column>
          <bl-column :cols="[{nom: 1, den: 1}]">
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

import BlHSublink from '~components/typography/HSublink.vue'
import BlSeparator from '~components/partials/separator/Separator.vue'
import BlAlert from '~components/partials/alert/Alert.vue'
import BlPreloadCard from '~components/service-components/preload-card/PreloadCard.vue'

export default {
  name: 'person-family',
  middleware: ['authenticated'],
  components: {
    BlHSublink,
    BlSeparator,
    BlAlert,
    BlPreloadCard
  },
  computed: {
    ...mapGetters({
      data: 'person/family',
      error: 'person/familyFailed'
    }),
    loading () {
      const resolved = this.$store.getters['person/familyResolved']
      const failed = this.$store.getters['person/familyFailed']
      return !resolved && !failed
    },
    computedChildren () {
      // Transform children array into array of arrays with max $rowSize elements each
      return this.data.children.reduce((result, child, i) => {
        if (i % this.rowSize === 0) {
          result.push([])
        }
        const row = result.slice(-1)[0]
        row.push(child)
        return result
      }, [])
    }
  },
  mounted () {
    // Get a reference to ourself.
    const self = this
    // Resolve person family.
    self.$store.dispatch('person/family')
  },
  methods: {
    generateInitials (data) {
      if (data.preferredFirstName !== null) {
        return data.preferredFirstName.charAt(0) + data.lastNames[0].charAt(0)
      } else {
        return data.firstNames[0].charAt(0) + data.lastNames[0].charAt(0)
      }
    },
    generateFormattedAddress (data) {
      if (data !== null) {
        let str = `${data.street.name || ''} ${data.street.number || ''}`
        str += (data.street.box ? ` (Bus ${data.street.box})` : '')
        str += `, ${data.city.code || ''} ${data.city.name || ''}`
        return str
      } else {
        return ''
      }
    }
  }
}
</script>
