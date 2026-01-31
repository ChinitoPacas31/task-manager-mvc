import { create } from 'zustand';
import { User } from '@/types';
import { authService } from '@/services/auth.service';

interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (username: string, password: string) => Promise<void>;
  register: (username: string, email: string, password: string, fullName: string) => Promise<void>;
  logout: () => void;
  checkAuth: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  token: null,
  isAuthenticated: false,
  isLoading: true,

  login: async (username: string, password: string) => {
    const response = await authService.login({ username, password });
    authService.setAuth(response.token, response.user);
    set({
      user: response.user,
      token: response.token,
      isAuthenticated: true,
    });
  },

  register: async (username: string, email: string, password: string, fullName: string) => {
    const response = await authService.register({ username, email, password, fullName });
    authService.setAuth(response.token, response.user);
    set({
      user: response.user,
      token: response.token,
      isAuthenticated: true,
    });
  },

  logout: () => {
    authService.logout();
    set({
      user: null,
      token: null,
      isAuthenticated: false,
    });
  },

  checkAuth: () => {
    const token = authService.getToken();
    const user = authService.getUser();
    set({
      user,
      token,
      isAuthenticated: !!token,
      isLoading: false,
    });
  },
}));
