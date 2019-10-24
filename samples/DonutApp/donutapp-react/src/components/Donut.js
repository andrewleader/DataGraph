import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Card from '@material-ui/core/Card';
import CardActionArea from '@material-ui/core/CardActionArea';
import CardActions from '@material-ui/core/CardActions';
import CardContent from '@material-ui/core/CardContent';
import CardMedia from '@material-ui/core/CardMedia';
import Button from '@material-ui/core/Button';
import Typography from '@material-ui/core/Typography';

import DonutApi from '../donutApi';

const useStyles = makeStyles({
    card: {
      maxWidth: 345,
    },
    media: {
      height: 140,
    },
  });

  export default function Donut(props) {
    const classes = useStyles();
    const donut = props.donut;

    const addToCart = async () => {
        await DonutApi.addDonutToCartAsync(donut.Id);
        alert("Added!");
    }
  
    return (
      <Card className={classes.card}>
        <CardActionArea>
          <CardMedia
            className={classes.media}
            image={donut.Picture}
            title={donut.Name}
          />
          <CardContent>
            <Typography gutterBottom variant="h5" component="h2">
                {donut.Name}
            </Typography>
            <Typography variant="body2" color="textSecondary" component="p">
              {donut.Price}
            </Typography>
          </CardContent>
        </CardActionArea>
        <CardActions>
          <Button size="small" color="primary" onClick={addToCart}>
            Add to cart
          </Button>
        </CardActions>
      </Card>
    );
  }