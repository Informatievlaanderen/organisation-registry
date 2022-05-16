<template>
  <header class="content-header" role="banner">
    <div class="content-header__wrapper">
      <picture>
        <!--[if IE 9]>
        <video style="display: none;"><![endif]-->
        <source srcset="//www.vlaanderen.be/sites/default/files/ip_acm/page_banner_wide_alt/header.jpg 1x" media="(min-width: 1024px)">
        <source srcset="//www.vlaanderen.be/sites/default/files/ip_acm/page_banner_mobile_alt/header.jpg 1x" media="(max-device-width: 767px)">
        <source srcset="//www.vlaanderen.be/sites/default/files/ip_acm/page_banner_narrow/header.jpg 1x" media="(max-width: 1023px)">
        <!--[if IE 9]></video><![endif]-->
        <!--[if lt IE 9]>
        <img itemprop="image" src=//www.vlaanderen.be/sites/default/files/ip_acm/page_banner_wide_alt/header.jpg" alt="" title=""/>
        <![endif]-->
        <!--[if !lt IE 9]><!-->
        <img src="//www.vlaanderen.be/sites/default/files/ip_acm/page_banner_wide_alt/header.jpg" srcset="//www.vlaanderen.be/sites/default/files/ip_acm/page_banner_mobile_alt/header.jpg 320w, //www.vlaanderen.be/sites/default/files/ip_acm/page_banner_narrow/header.jpg 1024w, //www.vlaanderen.be/sites/default/files/ip_acm/page_banner_wide_alt/header.jpg 1600w" alt="" title="">
        <!-- <![endif]-->
      </picture>
      <bl-layout :mod-is-wide="true">
        <div class="content-header__content">
          <div class="content-header__context content-header__context--has-link">
            <nuxt-link v-if="data && data.parent" class="content-header__context__link" :to="data.parent.to">{{ data.parent.title }}</nuxt-link></div>
            <h1 class="content-header__title" v-if="data"><span class="content-header__title__content">{{ data.title }}</span></h1>
        </div>
      </bl-layout>
    </div>
  </header>
</template>

<script>

export default {
  name: 'content-header',
  props: {
    data: {
      default: null,
      type: Object
    }
  }
}
</script>
