import api from './api';
import { Task, TaskListResponse, TaskFilter, CreateTaskRequest, UpdateTaskRequest } from '@/types';

export const taskService = {
  async getAll(filter?: TaskFilter): Promise<TaskListResponse> {
    const params = new URLSearchParams();
    if (filter) {
      Object.entries(filter).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== '') {
          params.append(key, String(value));
        }
      });
    }
    const response = await api.get<TaskListResponse>(`/tasks?${params.toString()}`);
    return response.data;
  },

  async getById(id: string): Promise<Task> {
    const response = await api.get<Task>(`/tasks/${id}`);
    return response.data;
  },

  async getByProject(projectId: string): Promise<Task[]> {
    const response = await api.get<Task[]>(`/tasks/project/${projectId}`);
    return response.data;
  },

  async getMyTasks(): Promise<Task[]> {
    const response = await api.get<Task[]>('/tasks/my-tasks');
    return response.data;
  },

  async search(query: string): Promise<Task[]> {
    const response = await api.get<Task[]>(`/tasks/search?q=${encodeURIComponent(query)}`);
    return response.data;
  },

  async create(data: CreateTaskRequest): Promise<Task> {
    const response = await api.post<Task>('/tasks', data);
    return response.data;
  },

  async update(id: string, data: UpdateTaskRequest): Promise<Task> {
    const response = await api.put<Task>(`/tasks/${id}`, data);
    return response.data;
  },

  async delete(id: string): Promise<void> {
    await api.delete(`/tasks/${id}`);
  },
};
