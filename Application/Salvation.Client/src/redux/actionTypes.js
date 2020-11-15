// Fired to update part of the state with a new playstyle value
export const UPDATE_PLAYSTYLE_VALUE = 'UPDATE_PLAYSTYLE_VALUE';

// Fired to update part of the state with a new cast value
export const UPDATE_CAST_EFFICIENCY = 'UPDATE_CAST_EFFICIENCY';
export const UPDATE_CAST_OVERHEAL = 'UPDATE_CAST_OVERHEAL';

// Fired to request the profile be loaded
export const FETCH_PROFILE_LOADING = 'FETCH_PROFILE_LOADING';
// Fired to notify the profile state has been updated
export const FETCH_PROFILE_SUCCESS = 'FETCH_PROFILE_SUCCESS';
// Fired to notify of a failure updating the profile state
export const FETCH_PROFILE_ERROR = 'FETCH_PROFILE_ERROR';

// Request to apply gear to the current profile
export const UPDATE_GEARSTRING = 'UPDATE_GEARSTRING';
export const FETCH_APPLY_GEAR_LOADING = 'FETCH_APPLY_GEAR_LOADING';
export const FETCH_APPLY_GEAR_SUCCESS = 'FETCH_APPLY_GEAR_SUCCESS';
export const FETCH_APPLY_GEAR_ERROR = 'FETCH_APPLY_GEAR_ERROR';