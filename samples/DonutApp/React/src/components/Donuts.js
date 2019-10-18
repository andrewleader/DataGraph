import React, { Component, useState } from "react";

import { Row, Col, Button } from "reactstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

import { useAuth0 } from "../react-auth0-spa";

const Donuts = () => {

    const [donuts, setDonuts] = useState(null);
    const [error, setError] = useState(null);
    const { getTokenSilently } = useAuth0();
    React.useEffect(() => {
        fetch('https://localhost:44397/api/graphs/windowslive|7d1cac342168ec7f/2/global')
        .then(results => {
            return results.json();
        }).then(data => {
            setDonuts(data.Donuts);
        }).catch(error => {
            console.log(error);
            setError('Failed to load');
        });
    });

    // componentDidMount() {
    //     fetch('https://localhost:44397/api/graphs/windowslive|7d1cac342168ec7f/2/global')
    //     .then(results => {
    //         return results.json();
    //     }).then(data => {
    //         this.setState({
    //             donuts: data.Donuts
    //         });
    //     }).catch(error => {
    //         console.log(error);
    //         this.setState({
    //             error: 'Failed to load'
    //         });
    //     });
    // }

    const addToCart = async (donutId) => {
        try {
            var token = await getTokenSilently();
            console.log("Token: " + token);
            // return;

            const response = await fetch('https://localhost:44397/api/graphs/windowslive|7d1cac342168ec7f/2/me/donutsInCart', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                },
                body: donutId
            });

            alert("Added!");
        } catch (err) {
            alert(JSON.stringify(err));
        }
    }

//   render() {
    return (
      <div className="donuts">
        <h2 className="my-5 text-center">Donuts</h2>

        {(donuts == null) ? 
            error == null ? (
                <p>Loading...</p>
            ) : (
                <p>Error: {error}</p>
            )
         : (
            <Row className="d-flex justify-content-between">
            {donuts.map((donut, i) => (
                <Col key={i} md={5} className="mb-4">
                <h6 className="mb-3">
                    <a href={donut.Id}>
                    <FontAwesomeIcon icon="link" className="mr-2" />
                    {donut.Name}
                    </a>
                </h6>
                <p>{donut.Price}</p>
                <Button
                    onClick={() => addToCart(donut.Id)}>
                    Add to cart
                </Button>
                </Col>
            ))}
            </Row>
        )}
        
      </div>
    );
//   }
}

export default Donuts;
