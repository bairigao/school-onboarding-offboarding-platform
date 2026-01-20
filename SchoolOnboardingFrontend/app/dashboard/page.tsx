'use client'

import { useAuth } from '@/lib/auth'
import { useEffect, useState } from 'react'

interface Stats {
  totalPeople: number
  activeRequests: number
  completedTasks: number
  availableDevices: number
}

export default function DashboardPage() {
  const auth = useAuth()
  const [stats, setStats] = useState<Stats>({
    totalPeople: 0,
    activeRequests: 0,
    completedTasks: 0,
    availableDevices: 0,
  })
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    // Simulate loading stats
    setTimeout(() => {
      setStats({
        totalPeople: 142,
        activeRequests: 8,
        completedTasks: 23,
        availableDevices: 15,
      })
      setLoading(false)
    }, 500)
  }, [])

  const getRoleGreeting = () => {
    const greetings: Record<string, string> = {
      eo: 'Welcome, Enrollment Officer! Manage student onboarding requests below.',
      hr: 'Welcome, HR Team! Manage staff onboarding requests below.',
      it: 'Welcome, IT Team! Complete technical tasks and manage devices.',
    }
    return greetings[auth.role || ''] || 'Welcome!'
  }

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-600 mt-2">{getRoleGreeting()}</p>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div className="bg-white rounded-lg shadow p-6 border-l-4 border-blue-500">
          <p className="text-gray-600 text-sm font-medium">Total People</p>
          <p className="text-3xl font-bold text-gray-900 mt-2">
            {loading ? '-' : stats.totalPeople}
          </p>
        </div>
        <div className="bg-white rounded-lg shadow p-6 border-l-4 border-green-500">
          <p className="text-gray-600 text-sm font-medium">Active Requests</p>
          <p className="text-3xl font-bold text-gray-900 mt-2">
            {loading ? '-' : stats.activeRequests}
          </p>
        </div>
        <div className="bg-white rounded-lg shadow p-6 border-l-4 border-purple-500">
          <p className="text-gray-600 text-sm font-medium">Completed Tasks</p>
          <p className="text-3xl font-bold text-gray-900 mt-2">
            {loading ? '-' : stats.completedTasks}
          </p>
        </div>
        <div className="bg-white rounded-lg shadow p-6 border-l-4 border-orange-500">
          <p className="text-gray-600 text-sm font-medium">Available Devices</p>
          <p className="text-3xl font-bold text-gray-900 mt-2">
            {loading ? '-' : stats.availableDevices}
          </p>
        </div>
      </div>

      {/* Quick Actions */}
      <div className="bg-white rounded-lg shadow p-6">
        <h2 className="text-lg font-semibold text-gray-900 mb-4">
          Quick Actions
        </h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <button className="bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded-lg transition-colors">
            Create New Request
          </button>
          <button className="bg-gray-200 hover:bg-gray-300 text-gray-900 font-medium py-2 px-4 rounded-lg transition-colors">
            View My Requests
          </button>
          <button className="bg-gray-200 hover:bg-gray-300 text-gray-900 font-medium py-2 px-4 rounded-lg transition-colors">
            Browse Available Devices
          </button>
        </div>
      </div>

      {/* Recent Activity */}
      <div className="bg-white rounded-lg shadow overflow-hidden">
        <div className="p-6 border-b border-gray-200">
          <h2 className="text-lg font-semibold text-gray-900">Recent Activity</h2>
        </div>
        <div className="divide-y divide-gray-200">
          <div className="p-4 flex items-center justify-between hover:bg-gray-50">
            <div>
              <p className="font-medium text-gray-900">Sarah Johnson - Onboarding</p>
              <p className="text-sm text-gray-600">Student enrollment started</p>
            </div>
            <span className="text-xs bg-green-100 text-green-800 px-2 py-1 rounded">
              Active
            </span>
          </div>
          <div className="p-4 flex items-center justify-between hover:bg-gray-50">
            <div>
              <p className="font-medium text-gray-900">Mike Davis - Offboarding</p>
              <p className="text-sm text-gray-600">Waiting for device return</p>
            </div>
            <span className="text-xs bg-yellow-100 text-yellow-800 px-2 py-1 rounded">
              In Progress
            </span>
          </div>
          <div className="p-4 flex items-center justify-between hover:bg-gray-50">
            <div>
              <p className="font-medium text-gray-900">Lisa Chen - Onboarding</p>
              <p className="text-sm text-gray-600">All tasks completed</p>
            </div>
            <span className="text-xs bg-blue-100 text-blue-800 px-2 py-1 rounded">
              Complete
            </span>
          </div>
        </div>
      </div>
    </div>
  )
}
