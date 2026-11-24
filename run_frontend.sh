#!/bin/sh
export NVM_DIR=~/.nvm;
source $NVM_DIR/nvm.sh;
cd src/OrganisationRegistry.UI && nvm use && npm run start:hmr
