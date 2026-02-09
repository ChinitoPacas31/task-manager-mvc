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

  async exportProductivityReportToCsv(): Promise<void> {
    try {
      const response = await api.get('/reports/productivity/export-csv', {
        responseType: 'blob'
      });
      
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `ProductivityReport_${new Date().toISOString().split('T')[0]}.csv`);
      document.body.appendChild(link);
      link.click();
      link.parentNode?.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error('Error exporting productivity report:', error);
      throw error;
    }
  },

  async exportDashboardStatsToCsv(): Promise<void> {
    try {
      const response = await api.get('/reports/dashboard/export-csv', {
        responseType: 'blob'
      });
      
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `DashboardStats_${new Date().toISOString().split('T')[0]}.csv`);
      document.body.appendChild(link);
      link.click();
      link.parentNode?.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error('Error exporting dashboard stats:', error);
      throw error;
    }
  },

  async exportRecentActivityToCsv(limit: number = 100): Promise<void> {
    try {
      const response = await api.get(`/reports/activity/export-csv?limit=${limit}`, {
        responseType: 'blob'
      });
      
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `RecentActivity_${new Date().toISOString().split('T')[0]}.csv`);
      document.body.appendChild(link);
      link.click();
      link.parentNode?.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error('Error exporting recent activity:', error);
      throw error;
    }
  },
};
