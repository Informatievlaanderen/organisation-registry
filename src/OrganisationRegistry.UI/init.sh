#!/bin/sh

echo "window.organisationRegistryVersion=\"v2-$API_VERSION\";" >> /usr/share/nginx/html/config.js
echo "window.organisationRegistryApiEndpoint=\"$API_ENDPOINT\";" >> /usr/share/nginx/html/config.js
echo "window.organisationRegistryUiEndpoint=\"$UI_ENDPOINT\";" >> /usr/share/nginx/html/config.js
echo "window.organisationRegistryInformatieVlaanderenLink=\"$AIV_URI\";" >> /usr/share/nginx/html/config.js

sed -i 's/__MATOMO_SITE_ID__/'"$MATOMO_SITE_ID"'/' /usr/share/nginx/html/index.html
sed -i 's/__MATOMO_URL__/'"$MATOMO_URL"'/' /usr/share/nginx/html/index.html
sed -i 's/__MATOMO_CDN__/'"$MATOMO_CDN"'/' /usr/share/nginx/html/index.html

nginx -g 'daemon off;'
