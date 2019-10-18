import React, { Fragment } from "react";

import Hero from "../components/Hero";
// import Content from "../components/Content";
import Donuts from "../components/Donuts";
import Cart from "../components/Cart";

const Home = () => (
  <Fragment>
    <Hero />
    <hr />
    {/* <Content /> */}
    <Donuts />
    <Cart />
  </Fragment>
);

export default Home;
