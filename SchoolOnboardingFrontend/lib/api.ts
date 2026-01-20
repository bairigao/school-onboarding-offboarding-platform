import axios from 'axios'

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5007/api'

interface RequestConfig {
  role?: string
  userId?: string
}

const createApiClient = (config?: RequestConfig) => {
  const headers: Record<string, string> = {}

  if (config?.role) {
    headers['X-User-Role'] = config.role
  }
  if (config?.userId) {
    headers['X-User-Id'] = config.userId
  }

  return axios.create({
    baseURL: API_BASE_URL,
    headers,
  })
}

// People endpoints
export const peopleAPI = {
  getAll: (config?: RequestConfig) =>
    createApiClient(config).get('/people'),
  create: (data: any, config?: RequestConfig) =>
    createApiClient(config).post('/people', data),
  getById: (id: number, config?: RequestConfig) =>
    createApiClient(config).get(`/people/${id}`),
  update: (id: number, data: any, config?: RequestConfig) =>
    createApiClient(config).put(`/people/${id}`, data),
}

// Assets endpoints
export const assetsAPI = {
  getAll: (page = 1, pageSize = 50, config?: RequestConfig) =>
    createApiClient(config).get(`/assets?page=${page}&pageSize=${pageSize}`),
  getById: (id: number, config?: RequestConfig) =>
    createApiClient(config).get(`/assets/${id}`),
  sync: (config?: RequestConfig) =>
    createApiClient(config).post('/assets/sync'),
}

// Lifecycle endpoints
export const lifecycleAPI = {
  getRequests: (config?: RequestConfig) =>
    createApiClient(config).get('/lifecycle'),
  create: (data: any, config?: RequestConfig) =>
    createApiClient(config).post('/lifecycle', data),
  getById: (id: number, config?: RequestConfig) =>
    createApiClient(config).get(`/lifecycle/${id}`),
  update: (id: number, data: any, config?: RequestConfig) =>
    createApiClient(config).put(`/lifecycle/${id}`, data),
}

// Lifecycle Tasks endpoints
export const lifecycleTasksAPI = {
  getAll: (config?: RequestConfig) =>
    createApiClient(config).get('/lifecycle-tasks'),
  getById: (id: number, config?: RequestConfig) =>
    createApiClient(config).get(`/lifecycle-tasks/${id}`),
  update: (id: number, data: any, config?: RequestConfig) =>
    createApiClient(config).put(`/lifecycle-tasks/${id}`, data),
}
