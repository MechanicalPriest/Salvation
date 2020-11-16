import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux'
import { FETCH_PROFILE_LOADING, UPDATE_PLAYSTYLE_VALUE, UPDATE_CAST_EFFICIENCY, UPDATE_CAST_OVERHEAL, FETCH_RESULTS_LOADING } from '../../redux/actionTypes';
import { Link } from '@material-ui/core';

// This component is responsible handling the top level ProfileConfiguration (profile)
const Profile = () => {


  const dispatch = useDispatch();

  const newProfile = useSelector(state => state.profileReducer.profile);

  function updatePlaystyleValue(playstyle, value) {
    dispatch({ type: UPDATE_PLAYSTYLE_VALUE, payload: { playstyle: playstyle, newValue: value } });
  }

  function updateEfficiencyValue(cast, value) {
    dispatch({ type: UPDATE_CAST_EFFICIENCY, payload: { cast: cast, newValue: value } });
  }

  function updateOverhealValue(cast, value) {
    dispatch({ type: UPDATE_CAST_OVERHEAL, payload: { cast: cast, newValue: value } });
  }

  function submitProfile() {
    dispatch({ type: FETCH_RESULTS_LOADING, payload: newProfile });
  }

  // Request to load the default profile
  useEffect(() => {
    dispatch({ type: FETCH_PROFILE_LOADING });
  }, [dispatch]);

  // Update the wowhead tooltips each time the profile changes
  useEffect(() => {
    window.$WowheadPower.refreshLinks();
  }, [newProfile]);

  return (
    <div>
      <p>Cast Profile Entries</p>
      {newProfile.casts?.map((cast) => {
        return (
          <div key={cast.spellId}>
            <label>
              <Link target='_blank' rel='noreferrer' href={'//wowhead.com/spell=' + cast.spellId} data-wowhead={'spell=' + cast.spellId}>{cast.spellId}</Link> Efficiency
            </label>
            <input type='text' value={cast.efficiency} onChange={(e) => { updateEfficiencyValue(cast, e.target.value); }} />
            <label>Overheal</label>
            <input type='text' value={cast.overhealPercent} onChange={(e) => { updateOverhealValue(cast, e.target.value); }} />
          </div>
        );
      })}
      <p>Playstyle Entries</p>
      {newProfile.playstyleEntries?.map((playstyle) => {
        return (
          <div key={playstyle.name}>
            <label>{playstyle.name} ({playstyle.spellId})</label>
            <input type='text' value={playstyle.value} onChange={(e) => { updatePlaystyleValue(playstyle, e.target.value); }} />
          </div>
        );
      })}
      <p>Items</p>
      {newProfile.items?.map((item) => {
        if (item.equipped) {
          return (
            <div key={item.itemId + item.itemLevel}>
              <Link target='_blank' rel='noreferrer' href={'//wowhead.com/item=' + item.itemId + '?ilvl=' + item.itemLevel} data-wowhead={'item=' + item.itemId}>{item.name}</Link>
            </div>
          );
        }
        return null;
      })}
      <p>State</p>
      <pre>{JSON.stringify(newProfile)}</pre>
      <button onClick={submitProfile}>Generate Results</button>
    </div>
  );
};

export default Profile;