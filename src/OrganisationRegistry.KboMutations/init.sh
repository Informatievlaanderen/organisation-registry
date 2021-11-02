#!/bin/sh

CONTAINERID=$(curl -s http://169.254.170.2/v2/metadata | jq -r ".Containers[] | select(.Labels[\"com.amazonaws.ecs.container-name\"] | startswith(\"basisregisters-\") and endswith(\"-kbomutations\")) | .DockerId")

sed -i "s/REPLACE_CONTAINERID/$CONTAINERID/g" appsettings.json

echo $CERT > ./cert.crt
echo $CACERT >  ./cacert.crt
echo $KEY > ./key.key

./OrganisationRegistry.KboMutations
