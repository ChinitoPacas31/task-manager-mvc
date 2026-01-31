export interface User {
  id: string;
  username: string;
  email: string;
  fullName: string;
  role: string;
}

export interface UserBasic {
  id: string;
  username: string;
  fullName: string;
}

export interface ProjectBasic {
  id: string;
  name: string;
  color: string;
}

export interface Task {
  id: string;
  title: string;
  description: string;
  status: TaskStatus;
  priority: TaskPriority;
  project: ProjectBasic | null;
  assignedTo: UserBasic | null;
  createdBy: UserBasic;
  dueDate: string | null;
  estimatedHours: number | null;
  actualHours: number | null;
  tags: string[];
  createdAt: string;
  updatedAt: string;
  completedAt: string | null;
  commentCount: number;
}

export type TaskStatus = 'Pending' | 'InProgress' | 'Review' | 'Completed' | 'Cancelled';
export type TaskPriority = 'Low' | 'Medium' | 'High' | 'Critical';

export interface CreateTaskRequest {
  title: string;
  description: string;
  status?: string;
  priority?: string;
  projectId?: string;
  assignedToId?: string;
  dueDate?: string;
  estimatedHours?: number;
  tags?: string[];
}

export interface UpdateTaskRequest {
  title?: string;
  description?: string;
  status?: string;
  priority?: string;
  projectId?: string;
  assignedToId?: string;
  dueDate?: string;
  estimatedHours?: number;
  actualHours?: number;
  tags?: string[];
}

export interface TaskListResponse {
  tasks: Task[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface TaskFilter {
  status?: string;
  priority?: string;
  projectId?: string;
  assignedToId?: string;
  searchTerm?: string;
  dueDateFrom?: string;
  dueDateTo?: string;
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface Project {
  id: string;
  name: string;
  description: string;
  status: ProjectStatus;
  owner: UserBasic;
  members: UserBasic[];
  startDate: string | null;
  endDate: string | null;
  color: string;
  createdAt: string;
  updatedAt: string;
  stats: ProjectStats;
}

export type ProjectStatus = 'Active' | 'OnHold' | 'Completed' | 'Archived';

export interface ProjectStats {
  totalTasks: number;
  completedTasks: number;
  pendingTasks: number;
  inProgressTasks: number;
  overdueTasks: number;
  completionPercentage: number;
}

export interface CreateProjectRequest {
  name: string;
  description: string;
  startDate?: string;
  endDate?: string;
  color?: string;
  memberIds?: string[];
}

export interface Comment {
  id: string;
  taskId: string;
  user: UserBasic;
  content: string;
  createdAt: string;
  updatedAt: string;
  isEdited: boolean;
}

export interface Notification {
  id: string;
  title: string;
  message: string;
  type: string;
  relatedTaskId: string | null;
  relatedProjectId: string | null;
  isRead: boolean;
  createdAt: string;
}

export interface NotificationCount {
  total: number;
  unread: number;
}

export interface DashboardStats {
  totalTasks: number;
  completedTasks: number;
  pendingTasks: number;
  inProgressTasks: number;
  highPriorityTasks: number;
  overdueTasks: number;
  totalProjects: number;
  activeProjects: number;
  tasksByStatus: { status: string; count: number }[];
  tasksByPriority: { priority: string; count: number }[];
  tasksByProject: { projectId: string; projectName: string; totalTasks: number; completedTasks: number }[];
  recentActivity: RecentActivity[];
}

export interface RecentActivity {
  id: string;
  taskId: string;
  taskTitle: string;
  action: string;
  description: string;
  user: UserBasic;
  createdAt: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  fullName: string;
}
