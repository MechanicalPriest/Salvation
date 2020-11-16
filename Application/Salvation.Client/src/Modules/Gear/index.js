import React from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { UPDATE_GEARSTRING, FETCH_APPLY_GEAR_LOADING } from '../../redux/actionTypes';

// This component is responsible for handling the /simc gear application box

const GearImport = () => {

  const dispatch = useDispatch();

  const newProfile = useSelector(state => state.profileReducer.profile);
  const rawGearString = useSelector(state => state.gearReducer.gearString);

  function updateGearString(value) {
    dispatch({ type: UPDATE_GEARSTRING, payload: value });
  }

  function applyGearString() {
    dispatch({
      type: FETCH_APPLY_GEAR_LOADING, payload:
        { simcProfileString: rawGearString, profile: newProfile }
    });
  }

  return (
    <div>
      <p>Simc Profile</p>
      <textarea valie={rawGearString} onChange={(e) => { updateGearString(e.target.value); }} />
      <button onClick={applyGearString}>Apply Gear</button>
    </div>
  );
};

export default GearImport;