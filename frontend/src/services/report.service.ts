import api from './api';
import { DashboardStats, RecentActivity } from '@/types';

export const reportService = {
  async getDashboardStats(): Promise<DashboardStats> {
    const response = await api.get<DashboardStats>('/reports/dashboard');
    return response.data;
  },

  async getProductivityReport(): Promise<any[]> {
    const response = await api.get('/reports/productivity');
    return response.data;
  },

  async getRecentActivity(limit: number = 20): Promise<RecentActivity[]> {
    const response = await api.get<RecentActivity[]>(`/reports/activity?limit=${limit}`);
    return response.data;
  },
};
