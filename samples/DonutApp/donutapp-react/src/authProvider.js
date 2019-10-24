import { MsalAuthProvider, LoginType } from 'react-aad-msal';
 
const config = {
  auth: {
    authority: 'https://datagraphsamples.b2clogin.com/tfp/datagraphsamples.onmicrosoft.com/B2C_1_SignInUp/',
    clientId: 'a9548dbf-3773-4468-bd18-6d585e927f76',
    redirectUri: 'http://localhost:3000',
    validateAuthority: false
  },
  cache: {
    cacheLocation: "localStorage",
    storeAuthStateInCookie: true
  }
};
 
const authenticationParameters = {
  scopes: [
    'https://datagraphsamples.onmicrosoft.com/donutapp/graph_api'
  ]
}
 
export const authProvider = new MsalAuthProvider(config, authenticationParameters, LoginType.Popup)