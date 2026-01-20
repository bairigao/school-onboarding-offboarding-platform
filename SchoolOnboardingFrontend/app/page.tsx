'use client'

import { useRouter } from 'next/navigation'
import { useAuth, type UserRole } from '@/lib/auth'
import { useState } from 'react'

export default function LoginPage() {
  const router = useRouter()
  const auth = useAuth()
  const [selectedRole, setSelectedRole] = useState<UserRole>('eo')
  const [userId, setUserId] = useState('user123')

  const handleLogin = () => {
    auth.setAuth(selectedRole, userId)
    router.push('/dashboard')
  }

  const roles: { value: UserRole; label: string; description: string }[] = [
    {
      value: 'eo',
      label: 'Enrollment Officer',
      description: 'Manage student onboarding/offboarding requests',
    },
    {
      value: 'hr',
      label: 'HR Staff',
      description: 'Manage staff onboarding/offboarding requests',
    },
    {
      value: 'it',
      label: 'IT Staff',
      description: 'Complete technical tasks and manage devices',
    },
  ]

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-600 to-blue-800 flex items-center justify-center p-4">
      <div className="bg-white rounded-lg shadow-xl p-8 w-full max-w-md">
        <h1 className="text-3xl font-bold text-gray-900 mb-2">
          School Onboarding
        </h1>
        <p className="text-gray-600 mb-8">
          Platform for managing student and staff onboarding/offboarding
        </p>

        <div className="space-y-4">
          <label className="block text-sm font-medium text-gray-700">
            Select Your Role
          </label>

          {roles.map((role) => (
            <label
              key={role.value}
              className="flex items-center p-4 border border-gray-200 rounded-lg cursor-pointer hover:bg-gray-50"
            >
              <input
                type="radio"
                name="role"
                value={role.value}
                checked={selectedRole === role.value}
                onChange={(e) => setSelectedRole(e.target.value as UserRole)}
                className="w-4 h-4 text-blue-600"
              />
              <div className="ml-3">
                <p className="font-medium text-gray-900">{role.label}</p>
                <p className="text-sm text-gray-600">{role.description}</p>
              </div>
            </label>
          ))}

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              User ID (for testing)
            </label>
            <input
              type="text"
              value={userId}
              onChange={(e) => setUserId(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-600"
              placeholder="Enter user ID"
            />
          </div>

          <button
            onClick={handleLogin}
            className="w-full bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded-lg transition-colors"
          >
            Sign In
          </button>
        </div>

        <p className="text-xs text-gray-500 mt-6 text-center">
          This is a development interface. In production, integrate with school authentication system.
        </p>
      </div>
    </div>
  )
}
