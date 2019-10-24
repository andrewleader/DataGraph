import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Table from '@material-ui/core/Table';
import TableBody from '@material-ui/core/TableBody';
import TableCell from '@material-ui/core/TableCell';
import TableHead from '@material-ui/core/TableHead';
import TableRow from '@material-ui/core/TableRow';
import Paper from '@material-ui/core/Paper';
import { Typography } from '@material-ui/core';

import DonutApi from '../donutApi';

const useStyles = makeStyles({
  root: {
    width: '100%',
    overflowX: 'auto',
  },
  table: {
    minWidth: 650,
  },
});

function createData(name, calories, fat, carbs, protein) {
  return { name, calories, fat, carbs, protein };
}

const rows = [
  createData('Frozen yoghurt', 159, 6.0, 24, 4.0),
  createData('Ice cream sandwich', 237, 9.0, 37, 4.3),
  createData('Eclair', 262, 16.0, 24, 6.0),
  createData('Cupcake', 305, 3.7, 67, 4.3),
  createData('Gingerbread', 356, 16.0, 49, 3.9),
];

export default function Cart() {
  const classes = useStyles();

  const [donuts, setDonuts] = React.useState(null);

  React.useEffect(() => {
    async function fetchData() {
        setDonuts(await DonutApi.getDonutsInCartAsync());
    }

    fetchData();
}, []);

    const renderLoading = () => {
        return <Typography paragraph>Loading cart...</Typography>
    }

    const renderCart = () => {
        return (
        <Paper className={classes.root}>
          <Table className={classes.table} aria-label="simple table">
            <TableHead>
              <TableRow>
                <TableCell>Donut</TableCell>
                <TableCell align="right">Price</TableCell>
                <TableCell align="right">Remove</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {donuts.map((donut, index) => (
                <TableRow key={index}>
                  <TableCell component="th" scope="row">
                    {donut.Name}
                  </TableCell>
                  <TableCell align="right">{donut.Price}</TableCell>
                  <TableCell align="right">Remove</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </Paper>
      );
    }

    if (donuts == null) {
        return renderLoading();
    } else {
        return renderCart();
    }
  
}