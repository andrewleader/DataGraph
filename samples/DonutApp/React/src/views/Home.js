import React, { Fragment } from "react";

import Hero from "../components/Hero";
import Content from "../components/Content";
import Donuts from "../components/Donuts";

const Home = () => (
  <Fragment>
    <Hero />
    <hr />
    {/* <Content /> */}
    <Donuts />
  </Fragment>
);

export default Home;
