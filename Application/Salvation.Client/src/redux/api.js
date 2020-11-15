import axios from 'axios';

const apiRoot = process.env.REACT_APP_API_ROOT + '/api';
const getProfileEndpoint = apiRoot + '/DefaultProfile'

export function apiGetProfile() {
  console.log('apiGetProfile', getProfileEndpoint);
  return axios.get(getProfileEndpoint, { params: { specid: 257 } });
}