import { UPDATE_PLAYSTYLE_VALUE, FETCH_PROFILE_SUCCESS, UPDATE_CAST_EFFICIENCY, UPDATE_CAST_OVERHEAL } from "../actionTypes";

const initialState = {
  profile: { name: 'No profile' },
  loading: false,
  error: '',
};

export default function profileReducer(state = initialState, action) {

  switch (action.type) {
    // Process the response from successfully grabbing a profile
    case FETCH_PROFILE_SUCCESS: {
      console.log('profileReducer: ', FETCH_PROFILE_SUCCESS, action.payload, state);
      return {
        ...state,
          profile: action.payload
      };
    }
    // Process updating a playstyle value when it changes in the UI
    case UPDATE_PLAYSTYLE_VALUE: {
      console.log('profileReducer: ', UPDATE_PLAYSTYLE_VALUE, action.payload, state);
      return {
        ...state,
        profile: {
          ...state.profile,
          playstyleEntries: state.profile.playstyleEntries.map((playstyle, index) => {
            if (playstyle.name === action.payload.playstyle.name) {
              return { ...playstyle, value: action.payload.newValue };
            }
            return playstyle;
          })
        }
      };
    }
    // Process updating a cast value when it changes in the UI
    case UPDATE_CAST_EFFICIENCY: {
      console.log('profileReducer: ', UPDATE_CAST_EFFICIENCY, action.payload, state);
      return {
        ...state,
        profile: {
          ...state.profile,
          casts: state.profile.casts.map((cast, index) => {
            if (cast.spellId === action.payload.cast.spellId) {
              return { ...cast, efficiency: action.payload.newValue };
            }
            return cast;
          })
        }
      };
    }
    // Process updating a cast value when it changes in the UI
    case UPDATE_CAST_OVERHEAL: {
      console.log('profileReducer: ', UPDATE_CAST_OVERHEAL, action.payload, state);
      return {
        ...state,
        profile: {
          ...state.profile,
          casts: state.profile.casts.map((cast, index) => {
            if (cast.spellId === action.payload.cast.spellId) {
              return { ...cast, overhealPercent: action.payload.newValue };
            }
            return cast;
          })
        }
      };
    }

    default:
      return state;
  }
}
