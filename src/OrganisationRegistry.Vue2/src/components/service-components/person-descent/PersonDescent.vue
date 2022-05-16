<template>
  <div>
    <bl-region :mod-is-alt="true" v-if="!loading && data && showDescent(data) === true">
      <bl-layout :mod-is-wide="true">
        <bl-grid :mod-is-stacked="true">
          <bl-column>
            <bl-h type="h2" class="h2">Afstamming</bl-h>
          </bl-column>
          <bl-column>
            <div class="tree">
              <!-- Ouders -->
              <div class="tree__parents" :data-items="data.parents.length" v-if="data.parents && data.parents.length">
                <div class="tree__row grid grid--align-center">
                  <template v-for="(parent, index) in data.parents">
                    <div class="col--1-4 col--1-2--s tree__col" :key="index">
                      <section :class="{ 'tree__item': true, 'tree__item--deceased': parent.deathDate !== null }">
                        <span class="tree__item__ribbon" v-if="parent.deathDate !== null"></span>
                        <div class="tree__item__image tree__item__image--has-initials">
                          <div class="tree__item__image__initials">{{ generateInitials(parent) }}</div>
                        </div>
                        <div class="tree__item__content">
                          <h1 class="tree__item__name">{{ parent.preferredFirstName || parent.firstNames[0] }} {{ parent.lastNames[0] }}</h1>
                          <p class="tree__item__meta" v-if="parent.nationalId">{{ parent.nationalId }}</p>
                          <p class="tree__item__meta" v-if="parent.birthDate || parent.deathDate"><span v-if="parent.birthDate">° {{ $date(parent.birthDate.value) }}</span><span v-if="parent.deathDate"> † {{ $date(parent.deathDate.value) }}</span></p>
                          <p class="tree__item__meta u-color-gray" v-if="parent.relation">uw <span v-if="parent.relation === 'mother'">moeder</span><span v-if="parent.relation === 'father'">vader</span><span v-if="parent.relation === 'parent'">ouder</span></p>
                        </div>
                      </section>
                    </div>
                  </template>
                </div>
              </div>
              <!-- Ouders no data -->
              <div class="tree__parents" v-else>
                <div class="tree__row grid grid--align-center">
                  <div class="col--1-4 col--1-2--s tree__col">
                    <section class="tree__item">
                      <div class="tree__item__image tree__item__image--has-icon">
                        <svg role="presentation" class="tree__item__image__icon" v-svg symbol="icon-question" aria-hidden="true"></svg>
                      </div>
                      <div class="tree__item__content">
                        <p class="tree__item__meta" v-if="data.incompleteData">{{ data.incompleteData }}</p>
                      </div>
                    </section>
                  </div>
                </div>
              </div>
              <!-- END Ouders -->

              <!-- Uzelf -->
              <div :class="{'tree__you': true, 'tree__you--no-children': !data.children.length}" v-if="data.self">
                <div class="grid grid--align-center">
                  <div class="col--1-4 col--1-2--s tree__col">
                    <section :class="{ 'tree__item': true, 'tree__item--deceased': data.self.deathDate !== null }">
                      <span class="tree__item__ribbon" v-if="data.self.deathDate !== null"></span>
                      <div class="tree__item__image">
                        <img class="tree__item__image__figure" v-if="!photoLoading && passportPhoto" :src="passportPhoto">
                      </div>
                      <div class="tree__item__content">
                        <h1 class="tree__item__name">{{ data.self.preferredFirstName || data.self.firstNames[0] }} {{ data.self.lastNames[0] }}</h1>
                        <p class="tree__item__meta" v-if="data.self.nationalId">{{ data.self.nationalId }}</p>
                        <p class="tree__item__meta" v-if="data.self.birthDate || data.self.deathDate"><span v-if="data.self.birthDate">° {{ $date(data.self.birthDate.value) }}</span><span v-if="data.self.deathDate"> † {{ $date(data.self.deathDate.value) }}</span></p>
                        <p class="tree__item__meta"><span class="bl-tag">uzelf</span></p>
                      </div>
                    </section>
                  </div>
                </div>
              </div>
              <!-- END Uzelf -->

              <!-- Kinderen -->
              <div class="tree__children" v-if="data.children.length">
                <div class="tree__row grid grid--is-stacked grid--align-center" v-for="(row, index) in computedChildren" :data-items="row.length" :data-uneven="String(isOdd(row.length))">
                  <div class="col--1-4 col--1-2--s tree__col" v-for="(child, index) in row" :key="index">
                    <section :class="{ 'tree__item': true, 'tree__item--deceased': child.deathDate !== null }">
                      <span class="tree__item__ribbon" v-if="child.deathDate !== null"></span>
                      <div class="tree__item__image tree__item__image--has-initials">
                        <div class="tree__item__image__initials">{{ generateInitials(child) }}</div>
                      </div>
                      <div class="tree__item__content">
                        <h1 class="tree__item__name">{{ child.preferredFirstName || child.firstNames[0] }} {{ child.lastNames[0] }}</h1>
                        <p class="tree__item__meta" v-if="child.nationalId">{{ child.nationalId }}</p>
                        <p class="tree__item__meta" v-if="child.relation">uw <span v-if="child.relation === 'son'">zoon</span><span v-if="child.relation === 'daughter'">dochter</span><span v-if="child.relation === 'child'">kind</span></p>
                        <p class="tree__item__meta" v-if="child.birthDate || child.deathDate"><span v-if="child.birthDate">° {{ $date(child.birthDate.value) }}</span><span v-if="child.deathDate"> † {{ $date(child.deathDate.value) }}</span></p>
                      </div>
                    </section>
                  </div>
                </div>
              </div>
              <!-- END Kinderen -->
            </div>
          </bl-column>
        </bl-grid>
      </bl-layout>
    </bl-region>
  </div>
</template>

<script>

import { mapGetters } from 'vuex'

export default {
  name: 'person-descent',
  middleware: ['authenticated'],
  data () {
    return {
      rowSize: 4
    }
  },
  computed: {
    ...mapGetters({
      data: 'person/descent',
      error: 'person/descentFailed',
      passportPhoto: 'photo/passport'
    }),
    loading () {
      const resolved = this.$store.getters['person/descentResolved']
      const failed = this.$store.getters['person/descentFailed']
      return !resolved && !failed
    },
    photoLoading () {
      const resolved = this.$store.getters['photo/passportResolved']
      const failed = this.$store.getters['photo/passportFailed']
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
    // Resolve person Descent.
    self.$store.dispatch('person/descent')
    self.$store.dispatch('photo/passport')
  },
  methods: {
    showDescent (data) {
      if (data) {
        if (data.parents.length === 0 && data.children.length === 0 && data.incompleteData === undefined) {
          return false
        }
        return true
      } else {
        return false
      }
    },
    generateInitials (data) {
      if (data.preferredFirstName !== null) {
        return data.preferredFirstName.charAt(0) + data.lastNames[0].charAt(0)
      } else {
        return data.firstNames[0].charAt(0) + data.lastNames[0].charAt(0)
      }
    },
    isOdd (n) {
      return Math.abs(n % 2) === 1
    }
  }
}
</script>
