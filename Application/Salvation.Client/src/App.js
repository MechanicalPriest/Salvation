import React from 'react';
import Profile from './Modules/Profile';
import GearImport from './Modules/Gear';
import Results from './Modules/Results';

const App = () => {

  return (
    <div>
      <div>Select Configuration</div>
      <Profile />
      <div>Apply Gear</div>
      <GearImport />
      <div>View Results</div>
      <Results />
    </div>
  );
}

export default App;