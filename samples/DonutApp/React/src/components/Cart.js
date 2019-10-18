import React, { Component, useState } from "react";

import { Row, Col, Button } from "reactstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

import { useAuth0 } from "../react-auth0-spa";

const Cart = () => {

    const [donuts, setDonuts] = useState(null);
    const [error, setError] = useState(null);
    const { getTokenSilently } = useAuth0();

    React.useEffect(() => {

        async function fetchData() {
            try {
                var token = await getTokenSilently();
                const response = await fetch('https://localhost:44397/api/graphs/windowslive|7d1cac342168ec7f/2/me/donutsInCart', {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                });

                setDonuts(await response.json());
            } catch (err) {
                console.log(err);
                setError(JSON.stringify(err));
            }
        }

        fetchData();
        
    });

    const removeFromCart = async (donutId) => {
    }

    return (
      <div className="cart">
        <h2 className="my-5 text-center">Cart</h2>

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
                    onClick={() => removeFromCart(donut.Id)}>
                    Remove from cart
                </Button>
                </Col>
            ))}
            </Row>
        )}
        
      </div>
    );
//   }
}

export default Cart;
