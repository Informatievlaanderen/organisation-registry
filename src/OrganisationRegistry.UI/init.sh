#!/bin/sh

echo "window.organisationRegistryVersion=\"v2-$API_VERSION\";" >> /usr/share/nginx/html/config.js
echo "window.organisationRegistryApiEndpoint=\"$API_ENDPOINT\";" >> /usr/share/nginx/html/config.js
echo "window.organisationRegistryUiEndpoint=\"$UI_ENDPOINT\";" >> /usr/share/nginx/html/config.js
echo "window.organisationRegistryInformatieVlaanderenLink=\"$AIV_URI\";" >> /usr/share/nginx/html/config.js

nginx -g 'daemon off;'
