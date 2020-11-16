import axios from 'axios';

const apiRoot = process.env.REACT_APP_API_ROOT + '/api';
const getProfileEndpoint = apiRoot + '/DefaultProfile';
const getProcessModelEndpoint = apiRoot + '/ProcessModel';

export function apiGetProfile() {
  console.log('apiGetProfile', getProfileEndpoint);
  return axios.get(getProfileEndpoint, { params: { specid: 257 } });
}

export function apiGetResults(data) {
  console.log('apiGetResults', getProcessModelEndpoint);
  return axios.post(getProcessModelEndpoint, data);
}