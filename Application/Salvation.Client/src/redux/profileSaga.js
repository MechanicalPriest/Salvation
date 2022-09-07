import { put, takeEvery, takeLatest, all, select } from 'redux-saga/effects'
import {
  FETCH_PROFILE_LOADING,
  FETCH_PROFILE_SUCCESS,
  FETCH_PROFILE_ERROR,
  FETCH_RESULTS_LOADING,
  FETCH_RESULTS_SUCCESS,
  FETCH_RESULTS_ERROR,
  FETCH_APPLY_GEAR_LOADING,
  FETCH_APPLY_GEAR_SUCCESS,
  FETCH_APPLY_GEAR_ERROR
} from "./actionTypes";
import { apiGetProfile, apiGetResults, apiApplySimc } from './api';

async function fetchAsync(func) {
  console.log('fetchAsync: Fetching');

  const response = await func();

  console.log('fetchAsync: Response:', response);

  return await response.data;
}

async function postAsync(func, args) {
  console.log('postAsync: Posting');

  const response = await func(args);

  console.log('postAsync: Response:', response);

  return await response.data;
}

function* fetchProfile() {
  console.log('fetchProfile: Fetching profile.');
  try {
    const profileResponse = yield fetchAsync(apiGetProfile);
    yield put({ type: FETCH_PROFILE_SUCCESS, payload: profileResponse.data });
  } catch (e) {
    yield put({ type: FETCH_PROFILE_ERROR, error: e.message });
  }
}

export function* profileSaga() {
  // Allows concurrent fetches of profile
  //yield takeEvery(FETCH_PROFILE_LOADING, fetchProfile);

  // Does not allow concurrent fetches of profile
  yield takeLatest(FETCH_PROFILE_LOADING, fetchProfile);
}

export function* resultsSaga() {
  yield takeLatest(FETCH_RESULTS_LOADING, fetchResults);
}

function* fetchResults(action) {
  console.log('fetchResults: Fetching results.');
  try {
    const resultsResponse = yield postAsync(apiGetResults, action.payload);
    yield put({ type: FETCH_RESULTS_SUCCESS, payload: resultsResponse.data });
  } catch (e) {
    yield put({ type: FETCH_RESULTS_ERROR, error: e.message });
  }
} 

export function* applySimcSaga() {
  yield takeLatest(FETCH_APPLY_GEAR_LOADING, fetchApplySimc);
}

function* fetchApplySimc(action) {
  console.log('fetchApplySimc: Fetching results.');
  try {
    const resultsResponse = yield postAsync(apiApplySimc, action.payload);
    yield put({ type: FETCH_PROFILE_SUCCESS, payload: resultsResponse.data });
    yield put({ type: FETCH_APPLY_GEAR_SUCCESS });
  } catch (e) {
    yield put({ type: FETCH_APPLY_GEAR_ERROR, error: e.message });
  }
} 

// Log all dispatch calls
function* watchAndLog() {
  yield takeEvery('*', function* logger(action) {
    const state = yield select()

    console.log('action', action)
    console.log('state after', state)
  })
}

export default function* rootSaga() {
  console.log('Initialising all sagas');

  yield all([
    profileSaga(),
    resultsSaga(),
    watchAndLog(),
    applySimcSaga(),
  ]);
}