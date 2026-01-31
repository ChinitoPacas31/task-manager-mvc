import api from './api';
import { Comment } from '@/types';

export const commentService = {
  async getByTask(taskId: string): Promise<Comment[]> {
    const response = await api.get<Comment[]>(`/comments/task/${taskId}`);
    return response.data;
  },

  async create(taskId: string, content: string): Promise<Comment> {
    const response = await api.post<Comment>('/comments', { taskId, content });
    return response.data;
  },

  async update(id: string, content: string): Promise<Comment> {
    const response = await api.put<Comment>(`/comments/${id}`, { content });
    return response.data;
  },

  async delete(id: string): Promise<void> {
    await api.delete(`/comments/${id}`);
  },
};
