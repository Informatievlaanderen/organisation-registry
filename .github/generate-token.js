const axios = require('axios');
const jwt = require('jsonwebtoken');
const fs = require('fs');

const APP_ID = process.env.APP_ID;
const PRIVATE_KEY = process.env.PRIVATE_KEY.replace(/\\n/g, '\n');

// Create JWT
const jwtToken = jwt.sign({}, PRIVATE_KEY, {
  algorithm: 'RS256',
  expiresIn: '10m',  // 10 minutes
  issuer: APP_ID
});

// Fetch the installation ID dynamically
axios.get(`https://api.github.com/app/installations`, {
  headers: {
    Authorization: `Bearer ${jwtToken}`,
    Accept: 'application/vnd.github.v3+json',
  }
}).then(response => {
  const installationId = response.data[0].id;  // Assuming only one installation. Adjust if needed.

  // Now, fetch installation token
  return axios.post(`https://api.github.com/app/installations/${installationId}/access_tokens`, {}, {
    headers: {
      Authorization: `Bearer ${jwtToken}`,
      Accept: 'application/vnd.github.v3+json',
    }
  });

}).then(response => {
  const installationToken = response.data.token;
  console.log(`::set-output name=installationToken::${installationToken}`);
}).catch(error => {
  console.error('Error fetching installation token:', error);
});
