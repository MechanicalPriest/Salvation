import { put, takeEvery, takeLatest, all, select } from 'redux-saga/effects'
import { FETCH_PROFILE_LOADING, FETCH_PROFILE_SUCCESS, FETCH_PROFILE_ERROR } from "./actionTypes";
import { apiGetProfile } from './api';

async function fetchAsync(func) {
  console.log('fetchAsync: Fetching');

  const response = await func();

  console.log('fetchAsync: Response:', response);

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

// Log all dispatch calls
function* watchAndLog() {
  yield takeEvery('*', function* logger(action) {
    const state = yield select()

    console.log('action', action)
    console.log('state after', state)
  })
}

// TODO: NIU
function* logSuccess() {
  yield console.log('Success!');
}

// TODO: NIU
export function* watchProfileSuccessAsync() {
  console.log('Configuring watcher for', FETCH_PROFILE_SUCCESS);
  yield takeEvery(FETCH_PROFILE_SUCCESS, logSuccess)
}

export default function* rootSaga() {
  console.log('Initialising all sagas');

  yield all([
    profileSaga(),
    watchProfileSuccessAsync(),
    watchAndLog(),
  ]);
}