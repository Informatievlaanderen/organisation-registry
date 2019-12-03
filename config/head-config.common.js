/**
 * Configuration for head elements added during the creation of index.html.
 *
 * All href attributes are added the publicPath (if exists) by default.
 * You can explicitly hint to prefix a publicPath by setting a boolean value to a key that has
 * the same name as the attribute you want to operate on, but prefix with =
 *
 * Example:
 * { name: 'msapplication-TileImage', content: '/assets/icon/ms-icon-144x144.png', '=content': true },
 * Will prefix the publicPath to content.
 *
 * { rel: 'apple-touch-icon', sizes: '57x57', href: '/assets/icon/apple-icon-57x57.png', '=href': false },
 * Will not prefix the publicPath on href (href attributes are added by default
 *
 *
<link rel="apple-touch-icon" sizes="180x180" href="/icons/apple-touch-icon.png?v=gAElMxQYYl">
<link rel="icon" type="image/png" href="/icons/favicon-32x32.png?v=gAElMxQYYl" sizes="32x32">
<link rel="icon" type="image/png" href="/icons/favicon-194x194.png?v=gAElMxQYYl" sizes="194x194">
<link rel="icon" type="image/png" href="/icons/android-chrome-192x192.png?v=gAElMxQYYl" sizes="192x192">
<link rel="icon" type="image/png" href="/icons/favicon-16x16.png?v=gAElMxQYYl" sizes="16x16">
<link rel="manifest" href="/icons/manifest.json?v=gAElMxQYYl">
<link rel="mask-icon" href="/icons/safari-pinned-tab.svg?v=gAElMxQYYl" color="#ffffff">
<link rel="shortcut icon" href="/icons/favicon.ico?v=gAElMxQYYl">
<meta name="apple-mobile-web-app-title" content="OrganisationRegistry">
<meta name="application-name" content="OrganisationRegistry">
<meta name="msapplication-TileColor" content="#ffe615">
<meta name="msapplication-TileImage" content="/icons/mstile-144x144.png?v=gAElMxQYYl">
<meta name="msapplication-config" content="/icons/browserconfig.xml?v=gAElMxQYYl">
<meta name="theme-color" content="#ffe615">
 *
 */
module.exports = {
  link: [
    /** <link> tags for 'apple-touch-icon' (AKA Web Clips). **/
    { rel: 'apple-touch-icon', sizes: '180x180', href: '/assets/icon/apple-touch-icon.png?v=gAElMxQYY' },

    /** <link> tags for android web app icons **/
    { rel: 'icon', type: 'image/png', sizes: '194x194', href: '/assets/icon/favicon-194x194.png?v=gAElMxQYY' },

    /** <link> tags for favicons **/
    { rel: 'icon', type: 'image/png', sizes: '192x192', href: '/assets/icon/android-chrome-192x192.png?v=gAElMxQYY' },
    { rel: 'icon', type: 'image/png', sizes: '32x32', href: '/assets/icon/favicon-32x32.png?v=gAElMxQYY' },
    { rel: 'icon', type: 'image/png', sizes: '16x16', href: '/assets/icon/favicon-16x16.png?v=gAElMxQYY' },

    /** <link> tags for safari */
    { rel: 'mask-icon', color: '#ffffff', href: '/assets/icon/safari-pinned-tab.svg?v=gAElMxQYY' },

    /** <link> for regular favicon */
    { rel: 'shortcut icon', href: '/assets/icon/favicon.ico?v=gAElMxQYY' },

    /** <link> tags for a Web App Manifest **/
    { rel: 'manifest', href: '/assets/manifest.json?v=gAElMxQYY' }
  ],
  meta: [
    { name: 'apple-mobile-web-app-title', content: 'Wegwijs' },
    { name: 'application-name', content: 'Wegwijs' },
    { name: 'msapplication-TileColor', content: '#ffe615' },
    { name: 'msapplication-TileImage', content: '/assets/icon/mstile-144x144.png?v=gAElMxQYY', '=content': true },
    { name: 'msapplication-config', content: '/assets/browserconfig.xml?v=gAElMxQYY' },
    { name: 'theme-color', content: '#ffe615' }
  ]
};
