import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux'
import { FETCH_PROFILE_LOADING, UPDATE_PLAYSTYLE_VALUE, UPDATE_CAST_EFFICIENCY, UPDATE_CAST_OVERHEAL } from '../../redux/actionTypes';

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

  // Request to load the default profile
  useEffect(() => {
    dispatch({ type: FETCH_PROFILE_LOADING });
  }, [dispatch]);

  useEffect(() => {
    window.$WowheadPower.refreshLinks();
  }, [newProfile]);

  return (
    <div>
      <p>Cast Profile Entries</p>
      {newProfile.casts?.map((cast) => {
        return (
          <div key={cast.spellId}>
            <label><a target='_blank' rel='noreferrer' href={'//wowhead.com/spell=' + cast.spellId} data-wowhead={'spell=' + cast.spellId}>{cast.spellId}</a> Efficiency</label>
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
      <p>State</p>
      <pre>{JSON.stringify(newProfile)}</pre>
    </div>
  );
};

export default Profile;