import http from 'k6/http';
import { check } from 'k6';
import { API_BASE_URL, BOOKS_ENDPOINT } from '../config.js';

export function setup() {
  // Get an ID to execute the test
  /* const url = `${API_BASE_URL}/${__ENV.API_VERSION}/${BOOKS_ENDPOINT}`;
  const res = http.get(url);
  const id = res.json()[0].id; */
  const id = 'c92e95cd-bea8-485c-b550-b8e0632b60bd';

  const requestUrl = `${API_BASE_URL}/${__ENV.API_VERSION}/${BOOKS_ENDPOINT}/${id}`;
  console.log(`requestUrl: ${requestUrl}`);

  return { requestUrl, itemId: id };
}

export default function ({ requestUrl, itemId }) {
  const params = {
    headers: { 'Content-Type': 'application/json' },
  };

  const res = http.get(requestUrl, params);

  check(res, {
    'Status 200': (r) => r.status === 200,
    ID: (r) => r.json().id === itemId,
  });
}
