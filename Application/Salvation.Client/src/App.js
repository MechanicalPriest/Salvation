import React from 'react';
import Profile from './Modules/Profile';
import GearImport from './Modules/Gear';

const App = () => {
  return (
    <div>
      <div>Select Configuration</div>
      <Profile />
      <div>Apply Gear</div>
      <GearImport />
      <div>View Results</div>
    </div>
  );
}

export default App;