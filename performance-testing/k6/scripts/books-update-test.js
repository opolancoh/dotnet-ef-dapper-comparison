import http from 'k6/http';
import { check } from 'k6';
import { API_BASE_URL, BOOKS_ENDPOINT } from '../config.js';

export function setup() {
  // Get an ID to execute the test
  /* const url = `${API_BASE_URL}/${__ENV.API_VERSION}/${BOOKS_ENDPOINT}`;
  const res = http.get(url);
  const id = res.json()[0].id; */
  const id = '02a24d88-0c02-41fc-8fe7-a9ff1f86fbd0';

  const requestUrl = `${API_BASE_URL}/${__ENV.API_VERSION}/${BOOKS_ENDPOINT}/${id}`;
  console.log(`requestUrl: ${requestUrl}`);

  return { requestUrl, itemId: id };
}

export default function ({ requestUrl, itemId }) {
  const params = {
    headers: { 'Content-Type': 'application/json' },
  };

  const now = new Date();

  const payload = {
    title: 'Don Quijote de la Mancha (updated)',
    publishedOn: now.toISOString(),
  };

  const res = http.put(requestUrl, JSON.stringify(payload), params);

  check(res, {
    'Status 204': (r) => r.status === 204,
  });
}
