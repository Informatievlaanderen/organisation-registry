#!/bin/sh
export NVM_DIR=$HOME/.nvm;
source $NVM_DIR/nvm.sh;
nvm use v9.3.0 && npm run start:hmr