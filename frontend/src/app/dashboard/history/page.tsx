'use client';

import { useState, useEffect } from 'react';
import { RecentActivity } from '@/types';
import { reportService } from '@/services/report.service';
import { History, User, CheckSquare, Edit, Trash, UserPlus, MessageSquare } from 'lucide-react';
import { format } from 'date-fns';
import { es } from 'date-fns/locale';
import { clsx } from 'clsx';

const actionIcons: Record<string, any> = {
  Created: CheckSquare,
  Updated: Edit,
  StatusChanged: History,
  Assigned: UserPlus,
  Commented: MessageSquare,
  Deleted: Trash,
  Restored: History,
};

const actionColors: Record<string, string> = {
  Created: 'bg-green-100 text-green-600',
  Updated: 'bg-blue-100 text-blue-600',
  StatusChanged: 'bg-purple-100 text-purple-600',
  Assigned: 'bg-orange-100 text-orange-600',
  Commented: 'bg-cyan-100 text-cyan-600',
  Deleted: 'bg-red-100 text-red-600',
  Restored: 'bg-yellow-100 text-yellow-600',
};

export default function HistoryPage() {
  const [activities, setActivities] = useState<RecentActivity[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchActivities = async () => {
      try {
        const data = await reportService.getRecentActivity(50);
        setActivities(data);
      } catch (error) {
        console.error('Error fetching activities:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchActivities();
  }, []);

  // Group activities by date
  const groupedActivities = activities.reduce((groups, activity) => {
    const date = format(new Date(activity.createdAt), 'yyyy-MM-dd');
    if (!groups[date]) {
      groups[date] = [];
    }
    groups[date].push(activity);
    return groups;
  }, {} as Record<string, RecentActivity[]>);

  return (
    <div className="space-y-6 animate-fade-in">
      <div>
        <h2 className="text-2xl font-bold text-gray-900">Historial</h2>
        <p className="text-gray-500">Registro de actividades recientes</p>
      </div>

      {loading ? (
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-primary-600"></div>
        </div>
      ) : activities.length === 0 ? (
        <div className="bg-white rounded-xl p-12 text-center">
          <History size={48} className="mx-auto text-gray-300 mb-4" />
          <h3 className="text-lg font-medium text-gray-900 mb-2">Sin actividad</h3>
          <p className="text-gray-500">No hay registros de actividad aún</p>
        </div>
      ) : (
        <div className="space-y-8">
          {Object.entries(groupedActivities).map(([date, dayActivities]) => (
            <div key={date}>
              <h3 className="text-sm font-medium text-gray-500 mb-4 sticky top-0 bg-gray-50 py-2">
                {format(new Date(date), "EEEE, d 'de' MMMM 'de' yyyy", { locale: es })}
              </h3>
              <div className="space-y-3">
                {dayActivities.map((activity) => {
                  const Icon = actionIcons[activity.action] || History;
                  return (
                    <div
                      key={activity.id}
                      className="bg-white rounded-xl p-5 shadow-sm border border-gray-100"
                    >
                      <div className="flex items-start gap-4">
                        <div className={clsx('p-3 rounded-lg', actionColors[activity.action] || 'bg-gray-100')}>
                          <Icon size={20} />
                        </div>
                        <div className="flex-1">
                          <div className="flex items-start justify-between">
                            <div>
                              <h4 className="font-medium text-gray-900">{activity.taskTitle}</h4>
                              <p className="text-sm text-gray-500 mt-1">{activity.description}</p>
                            </div>
                            <span className="text-xs text-gray-400">
                              {format(new Date(activity.createdAt), 'HH:mm')}
                            </span>
                          </div>
                          <div className="flex items-center gap-2 mt-3">
                            <div className="w-6 h-6 bg-primary-100 rounded-full flex items-center justify-center">
                              <User size={14} className="text-primary-600" />
                            </div>
                            <span className="text-sm text-gray-600">{activity.user.fullName}</span>
                          </div>
                        </div>
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
