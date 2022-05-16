<template>
  <component :is="type" :class="classes">
    <slot></slot>
  </component>
</template>
<script>
export default {
  name: 'panel',
  props: {
    type: {
      type: String,
      default: 'section'
    },
    modIsAlt: {
      type: Boolean,
      default: false
    },
    modIsAccordion: {
      default: false,
      type: Boolean
    },
    modIsAccordionOpen: {
      default: false,
      type: Boolean
    }
  },
  data () {
    return {
      classes: {
        'bl-panel': true,
        'bl-panel--alt': this.modIsAlt,
        'js-accordion': this.modIsAccordion,
        'js-accordion--open': this.modIsAccordionOpen
      }
    }
  }
}
</script>


<!--     <br>
  <section class="bl-panel">
    <div class="bl-panel__header">
      <div class="bl-panel__header__badges" aria-hidden="true">
        <div class="bl-panel__header__badges__item bl-panel__header__badges__item--alt">
          <span class="bl-panel__header__badges__item__icon vibl vibl-education"></span>
        </div>
        <div class="bl-panel__header__badges__item bl-panel__header__badges__item--success bl-panel__header__badges__item--overlap">
          <span class="bl-panel__header__badges__item__icon vibl vibl-document-certificate"></span>
        </div>
      </div>
      <div class="bl-panel__header__content bl-panel__header__content--flex">
        <div>
          <h1 class="bl-panel__header__title">
            Met badges
          </h1>
          <h2 class="bl-panel__header__subtitle">
            Schoolabonnementen
          </h2>
        </div>
        <ul class="bl-card__header__content__extra">
          <li class="bl-card__header__content__extra__item">familiedeprez@deprez.be</li>
          <li class="bl-card__header__content__extra__item">0497 632 332</li>
        </ul>
      </div>
    </div>
  </section>
  <br>
  <section class="bl-panel">
    <div class="bl-panel__header">
      <div class="bl-panel__header__badges" aria-hidden="true">
        <div class="bl-panel__header__badges__item bl-panel__header__badges__item--placeholder"></div>
      </div>
      <div class="bl-panel__header__content">
        <h1 class="bl-panel__header__title">
          Met placeholder
        </h1>
        <h2 class="bl-panel__header__subtitle">
          Schoolabonnementen
        </h2>
      </div>
      <div class="bl-panel__header__info popover popover--right popover--xxl popover--has-footer js-popover">
        <button type="button" class="bl-panel__header__info__toggle js-popover__toggle">
          <i class="vi vi-u-badge vi-u-badge--action vi-u-badge--xsmall vi-info" aria-hidden="true"></i>
          <span class="u-visually-hidden">Meer info</span>
        </button>
        <div class="popover__content">
          <p>popover</p>
        </div>
      </div>
    </div>
  </section>
  <br>
  <section class="bl-panel bl-panel--alt">
    <div class="bl-panel__header">
      <div class="bl-panel__header__badges" aria-hidden="true">
        <div class="bl-panel__header__badges__item">
          <span class="bl-panel__header__badges__item__icon vibl vibl-education"></span>
        </div>
        <div class="bl-panel__header__badges__item bl-panel__header__badges__item--success bl-panel__header__badges__item--overlap">
          <span class="bl-panel__header__badges__item__icon vibl vibl-document-certificate"></span>
        </div>
      </div>
      <div class="bl-panel__header__content">
        <h1 class="bl-panel__header__title">
          Met badges
        </h1>
        <h2 class="bl-panel__header__subtitle">
          Schoolabonnementen
        </h2>
      </div>
    </div>
  </section>
  <br>
  <section class="bl-panel bl-panel--alt">
    <header class="bl-panel__header">
      <div class="bl-panel__header__content">
        <h1 class="bl-panel__header__title">
          Alt
        </h1>
        <h2 class="bl-panel__header__subtitle">
          Schoolabonnementen
        </h2>
      </div>
    </header>
  </section>
  <br>
  <section class="bl-panel">
    <header>
      <button type="button" class="bl-panel__header bl-panel__header--has-action" @click.prevent="isOpened1 = !isOpened1" :aria-expanded="isOpened1" aria-controls="dropdown-1">
        <span aria-hidden="true" class="bl-panel__header__toggle vi vi-u-badge vi-u-badge--small vi-arrow vi-u-90deg" :class="{'bl-panel__header__toggle--reverse': !isOpened1}"></span>
        <div class="bl-panel__header__content">
          <h1 class="bl-panel__header__title">
            Call to action met dropdown
          </h1>
          <h2 class="bl-panel__header__subtitle">
            Subtitel hier
          </h2>
        </div>
      </button>
    </header>
    <transition name="bl-panel">
      <div id="dropdown-1" class="bl-panel__body bl-panel__body--alt" tabindex="0" v-if="isOpened1">
        <p>Body data</p>
        <p>Body data</p>
        <p>Body data</p>
        <p>Body data</p>
        <p>Body data</p>
        <p>Body data</p>
        <p>Body data</p>
        <p>Body data</p>
      </div>
    </transition>
  </section>
  <br>
  <section class="bl-panel bl-panel--alt">
    <a href="" class="bl-panel__header bl-panel__header--has-action">
      <div class="bl-panel__header__content">
        <h1 class="bl-panel__header__title">
          Call to action
        </h1>
        <h2 class="bl-panel__header__subtitle">
          Schoolabonnementen
        </h2>
      </div>
    </a>
  </section>
  <br>
  <section class="bl-panel bl-panel--alt">
    <header class="bl-panel__header">
      <div class="bl-panel__header__content">
        <h1 class="bl-panel__header__title">
          Met open dropdown
        </h1>
        <h2 class="bl-panel__header__subtitle">
          Schoolabonnementen
        </h2>
      </div>
    </header>
    <div class="bl-panel__body">
      <bl-description-data>
        <bl-grid :mod-is-stacked="true">
          <bl-column :cols="[{nom: 1, den: 3}, {nom: 1, den: 2, mod: 's'}, {nom: 1, den: 1, mod: 'xs'}]">
              <bl-description-data-item-wrapper :mod-is-bordered="true">
                <bl-description-data-item class-type="subdata">Lorem</bl-description-data-item>
                <bl-description-data-item class-type="data">Ipsum</bl-description-data-item>
              </bl-description-data-item-wrapper>
          </bl-column>
        </bl-grid>
      </bl-description-data>
      <div class="u-spacer--small"></div>
      <div class="bl-panel__body__accordion" :class="{'bl-panel__body__accordion--open': isOpened2}">
        <a href="#" class="bl-panel__body__accordion__toggle toggle toggle--arrow-down js-accordion__toggle" :class="{'bl-panel__body__accordion__toggle--reverse': isOpened2}" @click.prevent="isOpened2 = !isOpened2" :aria-expanded="String(isOpened2)" aria-controls="dropdown-2">
          <i class="toggle__icon" aria-hidden="true"></i>
          Traject
        </a>
        <transition name="bl-panel-accordion">
          <div id="dropdown-2" class="bl-panel__body__accordion__content" v-if="isOpened2" tabindex="0">
            <bl-description-data>
              <bl-grid :mod-is-stacked="true">
                <bl-column :cols="[{nom: 1, den: 2}, {nom: 1, den: 1, mod: 's'}]">
                  <bl-description-data-item class-type="subdata">Lorem</bl-description-data-item>
                  <bl-description-data-item class-type="data">Ipsum</bl-description-data-item>
                </bl-column>
              </bl-grid>
            </bl-description-data>
          </div>
        </transition>
      </div>
      <div class="bl-panel__body__footer">
        <a href="#" class="link--icon--external" target="_BLANK">Externe link</a>
        <p class="bl-panel__body__footer__meta">3 opvragingen | <time>08/11/1991</time></p>
      </div>
    </div>
  </section> -->
