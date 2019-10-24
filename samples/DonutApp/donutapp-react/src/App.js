import React from 'react';
import logo from './logo.svg';
import './App.css';
import { authProvider } from './authProvider';
import { makeStyles } from '@material-ui/core/styles';
import Drawer from '@material-ui/core/Drawer';
import AppBar from '@material-ui/core/AppBar';
import CssBaseline from '@material-ui/core/CssBaseline';
import Toolbar from '@material-ui/core/Toolbar';
import List from '@material-ui/core/List';
import Typography from '@material-ui/core/Typography';
import Divider from '@material-ui/core/Divider';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import InboxIcon from '@material-ui/icons/MoveToInbox';
import MailIcon from '@material-ui/icons/Mail';

import Donuts from './components/Donuts';
import Cart from './components/Cart';

const drawerWidth = 240;

const useStyles = makeStyles(theme => ({
  root: {
    display: 'flex',
  },
  appBar: {
    zIndex: theme.zIndex.drawer + 1,
  },
  drawer: {
    width: drawerWidth,
    flexShrink: 0,
  },
  drawerPaper: {
    width: drawerWidth,
  },
  content: {
    flexGrow: 1,
    padding: theme.spacing(3),
  },
  toolbar: theme.mixins.toolbar,
}));

function App() {

  const classes = useStyles();

  var showAccessToken = async () => {
    var token = await authProvider.getAccessToken();
    alert('Token: ' + token.accessToken);
  }

  const [ selectedMenuItem, setSelectedMenuItem ] = React.useState("donuts");

  const renderContent = () => {
    switch (selectedMenuItem) {
      case 'donuts':
        return <Donuts />

      case 'orders':
        return (<p>Coming soon</p>);

      case 'cart':
        return <Cart />
    }
  }

  return (
    <div className={classes.root}>
      <CssBaseline />
      <AppBar position="fixed" className={classes.appBar}>
        <Toolbar>
          <Typography variant="h6" noWrap>
            Donut shop
          </Typography>
        </Toolbar>
      </AppBar>
      <Drawer
        className={classes.drawer}
        variant="permanent"
        classes={{
          paper: classes.drawerPaper,
        }}
      >
        <div className={classes.toolbar} />
        <List>
          <ListItem
            button
            key="donuts"
            selected={selectedMenuItem === 'donuts'}
            onClick={event => setSelectedMenuItem('donuts')}>
            <ListItemIcon>
              <InboxIcon/>
            </ListItemIcon>
            <ListItemText primary="Donuts"/>
          </ListItem>
          <ListItem
            button
            key="orders"
            selected={selectedMenuItem === 'orders'}
            onClick={event => setSelectedMenuItem('orders')}>
            <ListItemIcon>
              <MailIcon/>
            </ListItemIcon>
            <ListItemText primary="Orders"/>
          </ListItem>
        </List>
        <Divider />
        <List>
          <ListItem
            button
            key="cart"
            selected={selectedMenuItem === 'cart'}
            onClick={event => setSelectedMenuItem('cart')}>
            <ListItemIcon>
              <MailIcon/>
            </ListItemIcon>
            <ListItemText primary="Cart"/>
          </ListItem>
        </List>
      </Drawer>
      <main className={classes.content}>
        <div className={classes.toolbar} />
        {renderContent()}
      </main>
    </div>
  );
}

export default App;
