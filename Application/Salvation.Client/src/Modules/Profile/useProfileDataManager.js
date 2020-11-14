import { useEffect, useReducer } from 'react';
import profileReducer from './profileReducer';

// Acts as a proxy to provide methods to retreive/update state data
// Pulsl data from API, stores state in the reducer
function useProfileDataManager() {
  // Use the reducer to keep track of the state
  const [{ currentProfile, }, profileDispatch, ] = useReducer(profileReducer, {
    currentProfile: {
      name: "Blank Profile",
      spells: [],
    }
  });

  function updateSpellValue(spell, newValue) {
    console.log('updateSpellValue:', newValue);
    profileDispatch({ type: 'updateSpellValue', data: { spell: spell, newValue: newValue } })
  }

  // When the hook loads, load the data into it
  useEffect(() => {
    // Create a function to call to get the data
    const fetchData = async function () {
      let result = {
        data: {
          name: "test playstyle",
          spells: [
            {
              name: "test spell",
              id: 123,
              value: 0.1,
            },
            {
              name: "test spell 2",
              id: 456,
              value: 500.0,
            }
          ]
        }
      };

      profileDispatch({ type: 'setProfile', data: result.data });
    }

    // Workaround for calling an async function inside an effect
    fetchData();

    // This fires when the hook is unloaded
    return console.log('cleanup run for useProfileDataManager');
  }, []);

  return {
    currentProfile,
    updateSpellValue,
  };
}

export default useProfileDataManager;