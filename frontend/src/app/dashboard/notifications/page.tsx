'use client';

import { useState, useEffect } from 'react';
import { Notification } from '@/types';
import { notificationService } from '@/services/notification.service';
import { Bell, Check, CheckCheck, Trash2, Info, AlertTriangle, CheckCircle, XCircle } from 'lucide-react';
import { format } from 'date-fns';
import { es } from 'date-fns/locale';
import { clsx } from 'clsx';
import toast from 'react-hot-toast';

const typeIcons: Record<string, any> = {
  Info: Info,
  Warning: AlertTriangle,
  Success: CheckCircle,
  Error: XCircle,
  TaskAssigned: Bell,
  TaskCompleted: CheckCircle,
  TaskDueSoon: AlertTriangle,
  CommentAdded: Bell,
  ProjectUpdate: Info,
};

const typeColors: Record<string, string> = {
  Info: 'bg-blue-100 text-blue-600',
  Warning: 'bg-yellow-100 text-yellow-600',
  Success: 'bg-green-100 text-green-600',
  Error: 'bg-red-100 text-red-600',
  TaskAssigned: 'bg-purple-100 text-purple-600',
  TaskCompleted: 'bg-green-100 text-green-600',
  TaskDueSoon: 'bg-orange-100 text-orange-600',
  CommentAdded: 'bg-blue-100 text-blue-600',
  ProjectUpdate: 'bg-indigo-100 text-indigo-600',
};

export default function NotificationsPage() {
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchNotifications = async () => {
    setLoading(true);
    try {
      const data = await notificationService.getAll();
      setNotifications(data);
    } catch (error) {
      console.error('Error fetching notifications:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchNotifications();
  }, []);

  const handleMarkAsRead = async (id: string) => {
    try {
      await notificationService.markAsRead(id);
      setNotifications(notifications.map(n => 
        n.id === id ? { ...n, isRead: true } : n
      ));
    } catch (error) {
      toast.error('Error al marcar como leída');
    }
  };

  const handleMarkAllAsRead = async () => {
    try {
      await notificationService.markAllAsRead();
      setNotifications(notifications.map(n => ({ ...n, isRead: true })));
      toast.success('Todas las notificaciones marcadas como leídas');
    } catch (error) {
      toast.error('Error al marcar notificaciones');
    }
  };

  const handleDelete = async (id: string) => {
    try {
      await notificationService.delete(id);
      setNotifications(notifications.filter(n => n.id !== id));
      toast.success('Notificación eliminada');
    } catch (error) {
      toast.error('Error al eliminar');
    }
  };

  const unreadCount = notifications.filter(n => !n.isRead).length;

  return (
    <div className="space-y-6 animate-fade-in">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">Notificaciones</h2>
          <p className="text-gray-500">
            {unreadCount > 0 ? `${unreadCount} sin leer` : 'Todas leídas'}
          </p>
        </div>
        {unreadCount > 0 && (
          <button
            onClick={handleMarkAllAsRead}
            className="px-4 py-2 text-primary-600 hover:bg-primary-50 rounded-lg transition-colors flex items-center gap-2"
          >
            <CheckCheck size={18} />
            Marcar todas como leídas
          </button>
        )}
      </div>

      {loading ? (
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-primary-600"></div>
        </div>
      ) : notifications.length === 0 ? (
        <div className="bg-white rounded-xl p-12 text-center">
          <Bell size={48} className="mx-auto text-gray-300 mb-4" />
          <h3 className="text-lg font-medium text-gray-900 mb-2">No hay notificaciones</h3>
          <p className="text-gray-500">Te notificaremos cuando haya algo nuevo</p>
        </div>
      ) : (
        <div className="space-y-3">
          {notifications.map((notification) => {
            const Icon = typeIcons[notification.type] || Bell;
            return (
              <div
                key={notification.id}
                className={clsx(
                  'bg-white rounded-xl p-5 shadow-sm border transition-all',
                  notification.isRead ? 'border-gray-100' : 'border-primary-200 bg-primary-50/30'
                )}
              >
                <div className="flex items-start gap-4">
                  <div className={clsx('p-3 rounded-lg', typeColors[notification.type] || 'bg-gray-100')}>
                    <Icon size={20} />
                  </div>
                  <div className="flex-1 min-w-0">
                    <div className="flex items-start justify-between gap-4">
                      <div>
                        <h4 className="font-medium text-gray-900">{notification.title}</h4>
                        <p className="text-sm text-gray-500 mt-1">{notification.message}</p>
                        <p className="text-xs text-gray-400 mt-2">
                          {format(new Date(notification.createdAt), "dd MMM yyyy 'a las' HH:mm", { locale: es })}
                        </p>
                      </div>
                      <div className="flex items-center gap-2">
                        {!notification.isRead && (
                          <button
                            onClick={() => handleMarkAsRead(notification.id)}
                            className="p-2 text-gray-400 hover:text-primary-600 hover:bg-primary-50 rounded-lg transition-colors"
                            title="Marcar como leída"
                          >
                            <Check size={18} />
                          </button>
                        )}
                        <button
                          onClick={() => handleDelete(notification.id)}
                          className="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                          title="Eliminar"
                        >
                          <Trash2 size={18} />
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
