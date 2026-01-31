'use client';

import { Task } from '@/types';
import { Calendar, User, MessageSquare, Clock } from 'lucide-react';
import { clsx } from 'clsx';
import { format } from 'date-fns';
import { es } from 'date-fns/locale';

interface TaskCardProps {
  task: Task;
  onClick: () => void;
}

const statusColors: Record<string, string> = {
  Pending: 'bg-yellow-100 text-yellow-800',
  InProgress: 'bg-blue-100 text-blue-800',
  Review: 'bg-purple-100 text-purple-800',
  Completed: 'bg-green-100 text-green-800',
  Cancelled: 'bg-gray-100 text-gray-800',
};

const priorityColors: Record<string, string> = {
  Low: 'bg-gray-100 text-gray-600',
  Medium: 'bg-blue-100 text-blue-600',
  High: 'bg-orange-100 text-orange-600',
  Critical: 'bg-red-100 text-red-600',
};

const statusLabels: Record<string, string> = {
  Pending: 'Pendiente',
  InProgress: 'En Progreso',
  Review: 'En Revisión',
  Completed: 'Completada',
  Cancelled: 'Cancelada',
};

const priorityLabels: Record<string, string> = {
  Low: 'Baja',
  Medium: 'Media',
  High: 'Alta',
  Critical: 'Crítica',
};

export default function TaskCard({ task, onClick }: TaskCardProps) {
  const isOverdue = task.dueDate && new Date(task.dueDate) < new Date() && task.status !== 'Completed';

  return (
    <div
      onClick={onClick}
      className="bg-white rounded-xl p-5 shadow-sm border border-gray-100 hover:shadow-md hover:border-primary-200 transition-all cursor-pointer group"
    >
      <div className="flex items-start justify-between gap-4">
        <div className="flex-1 min-w-0">
          <h3 className="font-semibold text-gray-900 group-hover:text-primary-600 transition-colors truncate">
            {task.title}
          </h3>
          {task.description && (
            <p className="text-sm text-gray-500 mt-1 line-clamp-2">{task.description}</p>
          )}
        </div>
        {task.project && (
          <div
            className="w-3 h-3 rounded-full flex-shrink-0"
            style={{ backgroundColor: task.project.color }}
            title={task.project.name}
          />
        )}
      </div>

      <div className="flex flex-wrap items-center gap-2 mt-4">
        <span className={clsx('px-2 py-1 text-xs font-medium rounded-full', statusColors[task.status])}>
          {statusLabels[task.status]}
        </span>
        <span className={clsx('px-2 py-1 text-xs font-medium rounded-full', priorityColors[task.priority])}>
          {priorityLabels[task.priority]}
        </span>
      </div>

      <div className="flex items-center gap-4 mt-4 text-sm text-gray-500">
        {task.dueDate && (
          <div className={clsx('flex items-center gap-1', isOverdue && 'text-red-500')}>
            <Calendar size={14} />
            <span>{format(new Date(task.dueDate), 'dd MMM yyyy', { locale: es })}</span>
          </div>
        )}
        {task.assignedTo && (
          <div className="flex items-center gap-1">
            <User size={14} />
            <span className="truncate max-w-[100px]">{task.assignedTo.fullName}</span>
          </div>
        )}
        {task.commentCount > 0 && (
          <div className="flex items-center gap-1">
            <MessageSquare size={14} />
            <span>{task.commentCount}</span>
          </div>
        )}
        {task.estimatedHours && (
          <div className="flex items-center gap-1">
            <Clock size={14} />
            <span>{task.estimatedHours}h</span>
          </div>
        )}
      </div>
    </div>
  );
}
