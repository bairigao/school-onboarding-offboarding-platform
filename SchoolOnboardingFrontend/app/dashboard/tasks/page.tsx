'use client'

import { useAuth } from '@/lib/auth'
import { useState, useEffect } from 'react'
import { lifecycleTasksAPI } from '@/lib/api'

interface Task {
  id: number
  lifecycleRequestId: number
  taskType: string
  completed: boolean
  completedAt?: string
  required: boolean
}

export default function TasksPage() {
  const auth = useAuth()
  const [tasks, setTasks] = useState<Task[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    if (auth.role !== 'it') {
      return
    }

    const loadTasks = async () => {
      try {
        const response = await lifecycleTasksAPI.getAll({
          role: auth.role || '',
          userId: auth.userId || '',
        })
        setTasks(response.data || [])
      } catch (error) {
        console.error('Error loading tasks:', error)
      } finally {
        setLoading(false)
      }
    }

    loadTasks()
  }, [auth])

  const handleCompleteTask = async (taskId: number) => {
    try {
      await lifecycleTasksAPI.update(taskId, { completed: true }, {
        role: auth.role || '',
        userId: auth.userId || '',
      })
      setTasks(tasks.map(t =>
        t.id === taskId ? { ...t, completed: true, completedAt: new Date().toISOString() } : t
      ))
    } catch (error) {
      console.error('Error completing task:', error)
    }
  }

  if (auth.role !== 'it') {
    return (
      <div className="text-center text-gray-600">
        Only IT staff can view this page.
      </div>
    )
  }

  const pendingTasks = tasks.filter(t => !t.completed)
  const completedTasks = tasks.filter(t => t.completed)

  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold text-gray-900">Tasks</h1>

      {/* Pending Tasks */}
      <div>
        <h2 className="text-xl font-semibold text-gray-900 mb-4">
          Pending Tasks ({pendingTasks.length})
        </h2>
        {loading ? (
          <div className="p-8 text-center text-gray-500">Loading...</div>
        ) : pendingTasks.length === 0 ? (
          <div className="p-8 text-center text-gray-500 bg-white rounded-lg">
            All tasks completed!
          </div>
        ) : (
          <div className="space-y-3">
            {pendingTasks.map((task) => (
              <div
                key={task.id}
                className="bg-white rounded-lg shadow p-4 flex items-center justify-between"
              >
                <div>
                  <p className="font-medium text-gray-900">
                    {task.taskType.replace(/_/g, ' ')}
                  </p>
                  <p className="text-sm text-gray-600">
                    Request #{task.lifecycleRequestId}
                    {task.required && (
                      <span className="ml-2 text-red-600">Required</span>
                    )}
                  </p>
                </div>
                <button
                  onClick={() => handleCompleteTask(task.id)}
                  className="bg-green-600 hover:bg-green-700 text-white font-medium py-2 px-4 rounded-lg transition-colors"
                >
                  Mark Complete
                </button>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Completed Tasks */}
      {completedTasks.length > 0 && (
        <div>
          <h2 className="text-xl font-semibold text-gray-900 mb-4">
            Completed Tasks ({completedTasks.length})
          </h2>
          <div className="space-y-3">
            {completedTasks.map((task) => (
              <div
                key={task.id}
                className="bg-white rounded-lg shadow p-4 flex items-center justify-between opacity-75"
              >
                <div>
                  <p className="font-medium text-gray-600 line-through">
                    {task.taskType.replace(/_/g, ' ')}
                  </p>
                  <p className="text-sm text-gray-600">
                    Completed {task.completedAt ? new Date(task.completedAt).toLocaleDateString() : 'N/A'}
                  </p>
                </div>
                <span className="text-green-600 font-semibold">✓ Complete</span>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  )
}
