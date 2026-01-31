'use client';

import { useState, useEffect } from 'react';
import { Task, Comment } from '@/types';
import { taskService } from '@/services/task.service';
import { commentService } from '@/services/comment.service';
import { MessageSquare, Send, Edit2, Trash2, ChevronDown, ChevronRight, User } from 'lucide-react';
import { format } from 'date-fns';
import { es } from 'date-fns/locale';
import toast from 'react-hot-toast';
import { clsx } from 'clsx';

export default function CommentsPage() {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [selectedTask, setSelectedTask] = useState<Task | null>(null);
  const [comments, setComments] = useState<Comment[]>([]);
  const [loading, setLoading] = useState(true);
  const [commentsLoading, setCommentsLoading] = useState(false);
  const [newComment, setNewComment] = useState('');
  const [editingComment, setEditingComment] = useState<string | null>(null);
  const [editContent, setEditContent] = useState('');

  useEffect(() => {
    const fetchTasks = async () => {
      try {
        const response = await taskService.getAll({ pageSize: 100 });
        setTasks(response.tasks);
      } catch (error) {
        console.error('Error fetching tasks:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchTasks();
  }, []);

  const handleSelectTask = async (task: Task) => {
    if (selectedTask?.id === task.id) {
      setSelectedTask(null);
      setComments([]);
      return;
    }

    setSelectedTask(task);
    setCommentsLoading(true);
    try {
      const data = await commentService.getByTask(task.id);
      setComments(data);
    } catch (error) {
      console.error('Error fetching comments:', error);
    } finally {
      setCommentsLoading(false);
    }
  };

  const handleAddComment = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedTask || !newComment.trim()) return;

    try {
      const comment = await commentService.create(selectedTask.id, newComment);
      setComments([comment, ...comments]);
      setNewComment('');
      toast.success('Comentario agregado');
    } catch (error) {
      toast.error('Error al agregar comentario');
    }
  };

  const handleUpdateComment = async (id: string) => {
    if (!editContent.trim()) return;

    try {
      const updated = await commentService.update(id, editContent);
      setComments(comments.map(c => c.id === id ? updated : c));
      setEditingComment(null);
      toast.success('Comentario actualizado');
    } catch (error) {
      toast.error('Error al actualizar');
    }
  };

  const handleDeleteComment = async (id: string) => {
    if (!confirm('¿Eliminar este comentario?')) return;

    try {
      await commentService.delete(id);
      setComments(comments.filter(c => c.id !== id));
      toast.success('Comentario eliminado');
    } catch (error) {
      toast.error('Error al eliminar');
    }
  };

  return (
    <div className="space-y-6 animate-fade-in">
      <div>
        <h2 className="text-2xl font-bold text-gray-900">Comentarios</h2>
        <p className="text-gray-500">Gestiona los comentarios de las tareas</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Task List */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <div className="p-4 border-b bg-gray-50">
            <h3 className="font-semibold text-gray-900">Tareas</h3>
          </div>
          <div className="max-h-[600px] overflow-y-auto">
            {loading ? (
              <div className="flex items-center justify-center h-32">
                <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-primary-600"></div>
              </div>
            ) : tasks.length === 0 ? (
              <div className="p-8 text-center text-gray-500">No hay tareas</div>
            ) : (
              <div className="divide-y">
                {tasks.map((task) => (
                  <button
                    key={task.id}
                    onClick={() => handleSelectTask(task)}
                    className={clsx(
                      'w-full p-4 text-left hover:bg-gray-50 transition-colors flex items-center gap-3',
                      selectedTask?.id === task.id && 'bg-primary-50'
                    )}
                  >
                    {selectedTask?.id === task.id ? (
                      <ChevronDown size={18} className="text-primary-600" />
                    ) : (
                      <ChevronRight size={18} className="text-gray-400" />
                    )}
                    <div className="flex-1 min-w-0">
                      <p className="font-medium text-gray-900 truncate">{task.title}</p>
                      <p className="text-sm text-gray-500 truncate">{task.project?.name || 'Sin proyecto'}</p>
                    </div>
                    {task.commentCount > 0 && (
                      <span className="flex items-center gap-1 text-sm text-gray-500">
                        <MessageSquare size={14} />
                        {task.commentCount}
                      </span>
                    )}
                  </button>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Comments Panel */}
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <div className="p-4 border-b bg-gray-50">
            <h3 className="font-semibold text-gray-900">
              {selectedTask ? `Comentarios: ${selectedTask.title}` : 'Selecciona una tarea'}
            </h3>
          </div>

          {selectedTask ? (
            <div className="flex flex-col h-[536px]">
              {/* Comments List */}
              <div className="flex-1 overflow-y-auto p-4 space-y-4">
                {commentsLoading ? (
                  <div className="flex items-center justify-center h-32">
                    <div className="animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 border-primary-600"></div>
                  </div>
                ) : comments.length === 0 ? (
                  <div className="text-center text-gray-500 py-8">
                    <MessageSquare size={32} className="mx-auto mb-2 text-gray-300" />
                    <p>No hay comentarios</p>
                  </div>
                ) : (
                  comments.map((comment) => (
                    <div key={comment.id} className="bg-gray-50 rounded-lg p-4">
                      <div className="flex items-start gap-3">
                        <div className="w-8 h-8 bg-primary-100 rounded-full flex items-center justify-center flex-shrink-0">
                          <User size={16} className="text-primary-600" />
                        </div>
                        <div className="flex-1 min-w-0">
                          <div className="flex items-center justify-between">
                            <span className="font-medium text-gray-900">{comment.user.fullName}</span>
                            <div className="flex items-center gap-1">
                              <button
                                onClick={() => {
                                  setEditingComment(comment.id);
                                  setEditContent(comment.content);
                                }}
                                className="p-1 text-gray-400 hover:text-primary-600 rounded"
                              >
                                <Edit2 size={14} />
                              </button>
                              <button
                                onClick={() => handleDeleteComment(comment.id)}
                                className="p-1 text-gray-400 hover:text-red-600 rounded"
                              >
                                <Trash2 size={14} />
                              </button>
                            </div>
                          </div>
                          
                          {editingComment === comment.id ? (
                            <div className="mt-2">
                              <textarea
                                value={editContent}
                                onChange={(e) => setEditContent(e.target.value)}
                                className="w-full px-3 py-2 border border-gray-300 rounded-lg resize-none"
                                rows={2}
                              />
                              <div className="flex gap-2 mt-2">
                                <button
                                  onClick={() => handleUpdateComment(comment.id)}
                                  className="px-3 py-1 bg-primary-600 text-white text-sm rounded-lg"
                                >
                                  Guardar
                                </button>
                                <button
                                  onClick={() => setEditingComment(null)}
                                  className="px-3 py-1 text-gray-600 text-sm rounded-lg hover:bg-gray-200"
                                >
                                  Cancelar
                                </button>
                              </div>
                            </div>
                          ) : (
                            <p className="text-gray-700 mt-1">{comment.content}</p>
                          )}
                          
                          <p className="text-xs text-gray-400 mt-2">
                            {format(new Date(comment.createdAt), "dd MMM yyyy 'a las' HH:mm", { locale: es })}
                            {comment.isEdited && ' (editado)'}
                          </p>
                        </div>
                      </div>
                    </div>
                  ))
                )}
              </div>

              {/* Add Comment */}
              <form onSubmit={handleAddComment} className="p-4 border-t bg-gray-50">
                <div className="flex gap-2">
                  <input
                    type="text"
                    value={newComment}
                    onChange={(e) => setNewComment(e.target.value)}
                    placeholder="Escribe un comentario..."
                    className="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500"
                  />
                  <button
                    type="submit"
                    disabled={!newComment.trim()}
                    className="px-4 py-2 bg-primary-600 hover:bg-primary-700 text-white rounded-lg disabled:opacity-50 transition-colors"
                  >
                    <Send size={18} />
                  </button>
                </div>
              </form>
            </div>
          ) : (
            <div className="flex items-center justify-center h-[536px] text-gray-500">
              <div className="text-center">
                <MessageSquare size={48} className="mx-auto mb-4 text-gray-300" />
                <p>Selecciona una tarea para ver sus comentarios</p>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
