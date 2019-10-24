import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';
import * as serviceWorker from './serviceWorker';
// import authentication from 'react-azure-adb2c';
import { AzureAD } from 'react-aad-msal';
import { authProvider } from './authProvider';

// authentication.initialize({
//     instance: 'https://datagraphsamples.b2clogin.com/tfp/',
//     tenant: 'datagraphsamples.onmicrosoft.com',
//     signInPolicy: 'B2C_1_SignInUp',
//     applicationId: 'a9548dbf-3773-4468-bd18-6d585e927f76',
//     cacheLocation: 'sessionStorage', // 'localStorage' or 'sessionStorage'
//     scopes: ['https://datagraphsamples.onmicrosoft.com/donutapp/graph_api'],
//     postLogoutRedirectUri: 'http://localhost:3000/' // Optional, URI to redirect to after logout
// });

// authentication.run(() => {
//     ReactDOM.render(<App />, document.getElementById('root'));
// });

ReactDOM.render(
    <AzureAD provider={authProvider} forceLogin={true}>
        <App />
    </AzureAD>,
    document.getElementById('root')
);


// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
