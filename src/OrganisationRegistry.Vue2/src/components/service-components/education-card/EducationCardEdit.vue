<template>
  <div class="bl-card">
    <div class="bl-card__header">
      <div class="bl-card__header__inner">
        <div class="bl-card__badges" aria-hidden="true">
          <bl-badge icon="icon-education" :mod-is-alt="true"></bl-badge>
        </div>
        <div class="bl-card__header__content">
          <h1 class="bl-card__header__title" v-if="data.education">{{ data.education }}</h1>
          <h2 class="bl-card__header__meta" v-if="data.institute || data.category"><template v-if="data.institute && data.institute.name">{{ data.institute.name }}</template><template v-if="data.institute && data.institute.name && data.category"> | </template>{{ data.category }}<template v-if="data.meta.grantsCertificateSecundary"> | {{ lbl.grantsCertificateSecundary }}</template></h2>
          <button type="button" :class="{ 'button': true, 'bl-card__header__button': true, 'button--secondary' : educationEditOpen}" @click="toggleEditEducation">{{ educationEditOpen ? 'Annuleren' : 'Wijziging melden' }}</button> 
        </div>
      </div>
    </div>
    <div class="bl-card__content" v-show="educationEditOpen">
      <div class="bl-card__content__accordion js-accordion js-accordion--open">
        <div class="accordion__content">
          <div class="bl-card__content__accordion__content">
            <form class="form" action="">
              <bl-grid :mod-is-stacked="true">
                <bl-column :cols="[{nom: 1, den: 2}, {nom: 2, den: 3, mod: 'm'}, {nom: 1, den: 1, mod: 's'}]">
                  <div class="u-spacer--small">
                    <h4 class="h4">Welke velden zijn niet correct? (gemockte data)</h4>
                  </div>

                  <div class="u-spacer--small">
                    <label class="checkbox checkbox--block u-spacer">
                      <input checked type="checkbox" :name="'institute-2-'+data.id" class="checkbox__toggle" data-show-checked="true" :data-show-checked-target="'institute-2-'+data.id" value="1"> <span></span>{{ data.institute.name }}
                    </label>
                    <div class="js-show-checked js-show-checked--open" :data-show-checked-trigger="'institute-2-'+data.id">
                      <bl-input-field name="institute-2" :value="data.institute.name" label="" :mod-show-label="false" :mod-is-block="true" />
                    </div>

                    <label class="checkbox checkbox--block u-spacer">
                      <input type="checkbox" :name="'category-'+data.id" class="checkbox__toggle" data-show-checked="true" :data-show-checked-target="'category-'+data.id" value="1"> <span></span>{{ data.category }}
                    </label>
                    <div class="js-show-checked" :data-show-checked-trigger="'category-'+data.id">
                      <bl-input-field name="category" :value="data.category" label="" :mod-show-label="false" :mod-is-block="true" />
                    </div>

                    <label class="checkbox checkbox--block u-spacer">
                      <input type="checkbox" :name="'year-'+data.id" class="checkbox__toggle" data-show-checked="true" :data-show-checked-target="'year-'+data.id" value="1"> <span></span>Uitreikingsdatum: {{ data.year }}
                    </label>
                    <div class="js-show-checked" :data-show-checked-trigger="'year-'+data.id">
                      <bl-input-field name="category" :value="data.year" label="" :mod-show-label="false" :mod-is-block="true" />
                    </div>
                    
                    <label class="checkbox checkbox--block u-spacer">
                      <input type="checkbox" :name="'institute-'+data.id" class="checkbox__toggle" data-show-checked="true" :data-show-checked-target="'institute-'+data.id" value="1"> <span></span>Uitgereikt door {{ data.institute.name }}
                    </label>
                    <div class="js-show-checked" :data-show-checked-trigger="'institute-'+data.id">
                      <bl-input-field name="category" :value="data.institute.name" label="" :mod-show-label="false" :mod-is-block="true" />
                    </div>
                  </div>

                  <div class="u-spacer--small">
                    <bl-textarea name="extra-opmerkingen" label="Extra opmerkingen" :mod-show-label="false" :mod-is-block="true" />
                    <bl-file-upload />
                  </div>
                  
                  <div class="u-spacer--small">
                    <label class="checkbox checkbox--block u-spacer">
                      <input checked type="checkbox" :name="'notify-me-'+data.id" class="checkbox__toggle" data-show-checked="true" :data-show-checked-target="'notify-me-'+data.id" value="1"> <span></span>Houd me op de hoogte van de status van mijn melding
                    </label>
                    <div class="js-show-checked js-show-checked--open" :data-show-checked-trigger="'notify-me-'+data.id">
                      <bl-input-field name="notify-me-email" label="E-mailadres" :mod-show-label="false" :mod-is-block="true" />
                    </div>
                  </div>
                  
                  <div class="button-group">
                    <button type="submit" class="button">Wijzigingen verzenden</button>
                    <button type="button" class="button button--secondary" @click="educationEditOpen = false">Annuleren</button>
                  </div>
                  
                </bl-column>
              </bl-grid>
              
            </form> 
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>

import BlDescriptionData from '~components/partials/description-data/DescriptionData.vue'
import BlDescriptionDataItem from '~components/partials/description-data/DescriptionDataItem.vue'
import BlDescriptionDataItemWrapper from '~components/partials/description-data/DescriptionDataItemWrapper.vue'
import BlGrid from '~components/frame/grid/Grid.vue'
import BlColumn from '~components/frame/column/Column.vue'
import BlBadge from '~components/partials/badge/Badge.vue'

import BlInputField from '~components/form-elements/input-field/InputField.vue'
import BlTextarea from '~components/form-elements/textarea/Textarea.vue'
import BlFileUpload from '~components/form-elements/file-upload/FileUpload.vue'

export default {
  name: 'education-card',
  components: {
    BlDescriptionData,
    BlDescriptionDataItem,
    BlDescriptionDataItemWrapper,
    BlGrid,
    BlColumn,
    BlBadge,
    BlInputField,
    BlTextarea,
    BlFileUpload
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
      educationPopoverOpen: false,
      educationEditOpen: false
    }
  },
  // @todo to remove
  methods: {
    toggleEducationPopover () {
      this.educationPopoverOpen = !this.educationPopoverOpen
    },
    toggleEditEducation () {
      this.educationEditOpen = !this.educationEditOpen
    }
  }
}
</script>

