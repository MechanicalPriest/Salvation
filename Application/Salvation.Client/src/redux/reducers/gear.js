import { FETCH_APPLY_GEAR_SUCCESS, UPDATE_GEARSTRING } from "../actionTypes";

const initialState = {
  gearString: '',
  loading: false,
  error: '',
};

export default function gearReducer(state = initialState, action) {

  switch (action.type) {
    // Process the response from successfully grabbing a profile
    case FETCH_APPLY_GEAR_SUCCESS: {
      console.log('gearReducer: ', FETCH_APPLY_GEAR_SUCCESS, action.payload, state);
      return {
        ...state,
        profile: action.payload,
      };
    }
    // Process updating the gear string when it changes in the UI
    case UPDATE_GEARSTRING: {
      console.log('gearReducer: ', UPDATE_GEARSTRING, action.payload, state);
      return {
        ...state,
        gearString: action.payload,
      };
    }

    default:
      return state;
  }
}
