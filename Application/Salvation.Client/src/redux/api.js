import axios from 'axios';

const getProfileEndpoint = process.env.REACT_APP_API_GET_PROFILE;
const getProcessModelEndpoint = process.env.REACT_APP_API_POST_RESULTS;

export function apiGetProfile() {
  console.log('apiGetProfile', getProfileEndpoint);
  return axios.get(getProfileEndpoint, { params: { specid: 257 } });
}

export function apiGetResults(data) {
  console.log('apiGetResults', getProcessModelEndpoint);
  return axios.post(getProcessModelEndpoint, data);
}