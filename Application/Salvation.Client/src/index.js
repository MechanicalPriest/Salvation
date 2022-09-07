import React from 'react';
import ReactDOM from 'react-dom';
import App from "./App.js";
import './index.css';
import { ThemeProvider, createMuiTheme, CssBaseline } from '@material-ui/core';

import { Provider } from 'react-redux'
import configureStore from './redux/store'

const store = configureStore();

const darkTheme = createMuiTheme({
  palette: {
    type: 'dark',
    primary: {
      main: '#a6d4fa',
    },
    secondary: {
      main: '#f48fb1'
    },
  },
});

ReactDOM.render(
  <Provider store={store}>
    <ThemeProvider theme={darkTheme}>
      <CssBaseline />
      <App />
    </ThemeProvider>
  </Provider>,
  document.getElementById('root')
);
