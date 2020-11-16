import { FETCH_RESULTS_SUCCESS } from "../actionTypes";

const initialState = {
  results: '',
  loading: false,
  error: '',
};

export default function resultsReducer(state = initialState, action) {

  switch (action.type) {
    // Process the response from successfully grabbing a profile
    case FETCH_RESULTS_SUCCESS: {
      console.log('resultsReducer: ', FETCH_RESULTS_SUCCESS, action.payload, state);
      return {
        ...state,
        results: action.payload,
      };
    }

    default:
      return state;
  }
}
