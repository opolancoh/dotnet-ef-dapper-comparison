import http from 'k6/http';
import { check } from 'k6';
import { API_BASE_URL, BOOKS_ENDPOINT } from '../config.js';

let counter = 0;

export function setup() {
  const baseRequestUrl = `${API_BASE_URL}/${__ENV.API_VERSION}/${BOOKS_ENDPOINT}`;
  const url = `${API_BASE_URL}/${__ENV.API_VERSION}/${BOOKS_ENDPOINT}`;
  const res = http.get(url);
  const items = res.json();

  console.log(`baseRequestUrl: ${baseRequestUrl}`);

  return { baseRequestUrl, items };
}

export default function ({ baseRequestUrl, items }) {
  const itemId = items[counter++].id;

  const params = {
    headers: { 'Content-Type': 'application/json' },
  };

  const payload = {};

  const res = http.del(
    `${baseRequestUrl}/${itemId}`,
    JSON.stringify(payload),
    params
  );

  check(res, {
    'Status 204': (r) => r.status === 204,
  });
}
