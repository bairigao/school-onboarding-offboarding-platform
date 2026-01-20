'use client'

import { useAuth } from '@/lib/auth'
import { useState, useEffect } from 'react'
import { peopleAPI } from '@/lib/api'

interface Person {
  id: number
  firstName: string
  lastName: string
  personType: string
  identifier: string
  status: string
}

export default function PeoplePage() {
  const auth = useAuth()
  const [people, setPeople] = useState<Person[]>([])
  const [loading, setLoading] = useState(true)
  const [showForm, setShowForm] = useState(false)

  useEffect(() => {
    const loadPeople = async () => {
      try {
        const response = await peopleAPI.getAll({
          role: auth.role || '',
          userId: auth.userId || '',
        })
        setPeople(response.data || [])
      } catch (error) {
        console.error('Error loading people:', error)
      } finally {
        setLoading(false)
      }
    }

    loadPeople()
  }, [auth])

  const getStatusColor = (status: string) => {
    const colors: Record<string, string> = {
      onboarding: 'bg-blue-100 text-blue-800',
      active: 'bg-green-100 text-green-800',
      offboarding: 'bg-yellow-100 text-yellow-800',
      offboarded: 'bg-gray-100 text-gray-800',
    }
    return colors[status] || 'bg-gray-100 text-gray-800'
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold text-gray-900">People</h1>
        {(auth.role === 'eo' || auth.role === 'hr') && (
          <button
            onClick={() => setShowForm(!showForm)}
            className="bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded-lg transition-colors"
          >
            Add Person
          </button>
        )}
      </div>

      {/* Add Person Form */}
      {showForm && (
        <div className="bg-white rounded-lg shadow p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">
            Add New Person
          </h2>
          <form className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <input
                type="text"
                placeholder="First Name"
                className="px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-600"
              />
              <input
                type="text"
                placeholder="Last Name"
                className="px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-600"
              />
              <select className="px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-600">
                <option value="">Select Type</option>
                <option value="student">Student</option>
                <option value="staff">Staff</option>
              </select>
            </div>
            <div className="flex gap-4">
              <button
                type="submit"
                className="bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded-lg transition-colors"
              >
                Create
              </button>
              <button
                type="button"
                onClick={() => setShowForm(false)}
                className="bg-gray-200 hover:bg-gray-300 text-gray-900 font-medium py-2 px-4 rounded-lg transition-colors"
              >
                Cancel
              </button>
            </div>
          </form>
        </div>
      )}

      {/* People Table */}
      <div className="bg-white rounded-lg shadow overflow-hidden">
        {loading ? (
          <div className="p-8 text-center text-gray-500">Loading...</div>
        ) : people.length === 0 ? (
          <div className="p-8 text-center text-gray-500">No people found</div>
        ) : (
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">
                  Name
                </th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">
                  Type
                </th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">
                  Identifier
                </th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">
                  Status
                </th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {people.map((person) => (
                <tr key={person.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 text-sm font-medium text-gray-900">
                    {person.firstName} {person.lastName}
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-600">
                    {person.personType}
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-600">
                    {person.identifier}
                  </td>
                  <td className="px-6 py-4 text-sm">
                    <span className={`px-2 py-1 rounded text-xs font-medium ${getStatusColor(person.status)}`}>
                      {person.status}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-sm">
                    <button className="text-blue-600 hover:text-blue-700 font-medium">
                      View
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  )
}
