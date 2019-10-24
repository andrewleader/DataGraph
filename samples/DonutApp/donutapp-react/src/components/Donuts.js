import React, { useEffect } from 'react';
import Typography from '@material-ui/core/Typography';

import Donut from './Donut';
import DonutApi from '../donutApi';

function Donuts() {

    const [ donuts, setDonuts ] = React.useState(null);

    useEffect(() => {
        async function fetchData() {
            setDonuts(await DonutApi.getDonutsAsync());
        }

        fetchData();
    }, [])

    var renderLoading = () => {
        return <Typography paragraph>Loading donuts...</Typography>
    }

    var renderDonuts = () => {
        return (
            <div>
                {donuts.map((donut, index) => (
                    <Donut donut={donut}/>
                ))}
            </div>
        );
    }

    if (donuts == null) {
        return renderLoading();
    } else {
        return renderDonuts();
    }
}

export default Donuts;