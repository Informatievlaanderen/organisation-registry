#!/bin/sh

CONTAINERID=$(curl -s http://169.254.170.2/v2/metadata | jq -r ".Containers[] | select(.Labels[\"com.amazonaws.ecs.container-name\"] | startswith(\"basisregisters-\") and endswith(\"-kbomutations\")) | .DockerId")

sed -i "s/REPLACE_CONTAINERID/$CONTAINERID/g" appsettings.json

echo $CERT | base64 -d > ./cert.crt
echo $CACERT | base64 -d >  ./cacert.crt
echo $KEY | base64 -d > ./key.key

./OrganisationRegistry.KboMutations
