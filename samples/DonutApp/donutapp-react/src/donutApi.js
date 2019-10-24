import { authProvider } from './authProvider';

class DonutApiImplementation {
    baseUrl = 'https://datagraph.azurewebsites.net/api/graphs/9898b560-b584-442b-bf72-b0491c1f9a33/3/';

    getDonutsAsync() {
        return this.getJsonAsync('global/donuts');
    }

    addDonutToCartAsync(donutId) {
        return this.postJsonAsync('me/donutsInCart', donutId, true);
    }

    getDonutsInCartAsync() {
        return this.getJsonAsync('me/donutsInCart', true);
    }

    async getJsonAsync(relativeUrl, authenticated) {
        var resp = await fetch(this.baseUrl + relativeUrl, {
            headers: {
                'Authorization': authenticated ? 'Bearer ' + await this.getAccessTokenAsync() : ''
            }
        });
        return await resp.json();
    }

    async postJsonAsync(relativeUrl, data, authenticated) {
        var resp = await fetch(this.baseUrl + relativeUrl, {
            headers: {
                'Content-Type': 'application/json',
                'Authorization': authenticated ? 'Bearer ' + await this.getAccessTokenAsync() : ''
            },
            method: 'POST',
            body: JSON.stringify(data)
        });
        if (resp.ok) {
            return;
        } else {
            throw new Error('Response was ' + resp.statusText);
        }
    }

    async getAccessTokenAsync() {
        var token = await authProvider.getAccessToken();
        var accessToken = token.accessToken;
        console.log('Token: ' + accessToken);
        return accessToken;
    }
}

const DonutApi = new DonutApiImplementation();

export default DonutApi;