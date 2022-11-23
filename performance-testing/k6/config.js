const API_BASE_URL = 'https://localhost:7030/api';
const BOOKS_ENDPOINT = 'books';
const REVIEWS_ENDPOINT = 'books';

export { API_BASE_URL, BOOKS_ENDPOINT, REVIEWS_ENDPOINT };

export const smokeTestOptions = {
  vus: 1,
  duration: '30s',
  thresholds: {
    http_req_duration: ['p(95)<100'],
  },
};
