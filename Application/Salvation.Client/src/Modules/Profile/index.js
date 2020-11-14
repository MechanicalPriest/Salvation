import React, { useContext, useMemo, useEffect } from 'react';
import { GlobalContext } from '../GlobalState';

// This component is responsible handling the top level ProfileConfiguration (profile)
const Profile = () => {

  const {
    currentProfile,
    updateSpellValue,
  } = useContext(GlobalContext);

  const profile = useMemo(() => currentProfile, [currentProfile]);

  console.log(currentProfile);
  console.log(profile);

  useEffect(() => {
    console.log('effectProfile:', currentProfile);
  }, [currentProfile]);

  function printState() {
    console.log('printState:', profile);
  }

  return (
    <div>
      <p>{currentProfile.Name}</p>
      {currentProfile.spells.map((spell) => {
        return (
          <div key={spell.id}>
            <p>{spell.Name} ({spell.id})</p>
            <input type='text' value={spell.value} onChange={(e) => { updateSpellValue(spell, e.target.value); }} />
          </div>
        );
      })}
      <p onClick={printState}>Show State</p>
    </div>
  );
};

export default Profile;