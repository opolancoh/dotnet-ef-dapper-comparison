import http from 'k6/http';
import { check } from 'k6';
import { API_BASE_URL, BOOKS_ENDPOINT } from '../config.js';

export function setup() {
  const requestUrl = `${API_BASE_URL}/${__ENV.API_VERSION}/${BOOKS_ENDPOINT}/${id}`;
  console.log(`requestUrl: ${requestUrl}`);

  return { requestUrl };
}

export default function ({ requestUrl }) {
  const params = {
    headers: { 'Content-Type': 'application/json' },
  };

  const res = http.get(requestUrl, params);

  check(res, {
    'Status 200': (r) => r.status === 200,
  });
}
