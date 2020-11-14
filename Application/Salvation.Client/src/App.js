import React from 'react';
import { GlobalProvider } from 'Modules/GlobalState';
import Profile from 'Modules/Profile';

const App = () => {
  return (
    <GlobalProvider>
      <div>Select Configuration</div>
      <Profile />
      <div>Apply Gear</div>
      <div>View Results</div>
    </GlobalProvider>
  );
}

export default App;