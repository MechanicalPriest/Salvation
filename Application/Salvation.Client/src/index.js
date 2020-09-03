import React from 'react';
import ReactDOM from 'react-dom';
import App from "./App.js";
import './index.css';

/*

class App extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      
    };
  }

  render() {
    return (
      <div className="app">
        <Container maxWidth="lg">
          <div className="header">
            <img src="wings_banner_offset_small.png" />
          </div>
          <ModellingPanel />
        </Container>
      </div>
    );
  }
} */

ReactDOM.render(
  <App />,
  document.getElementById('root')
);
