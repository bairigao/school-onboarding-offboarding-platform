'use client'

import { useAuth } from '@/lib/auth'
import { useRouter } from 'next/navigation'
import Link from 'next/link'
import { useEffect } from 'react'

export default function DashboardLayout({
  children,
}: {
  children: React.ReactNode
}) {
  const auth = useAuth()
  const router = useRouter()

  useEffect(() => {
    if (!auth.role) {
      router.push('/')
    }
  }, [auth.role, router])

  if (!auth.role) {
    return null
  }

  const getRoleName = (role: string) => {
    const names: Record<string, string> = {
      eo: 'Enrollment Officer',
      hr: 'HR Staff',
      it: 'IT Staff',
    }
    return names[role] || role
  }

  const getMenuItems = () => {
    const commonItems = [
      { href: '/dashboard', label: 'Dashboard', icon: '📊' },
      { href: '/people', label: 'People', icon: '👥' },
      { href: '/assets', label: 'Devices', icon: '💻' },
    ]

    const roleItems: Record<string, typeof commonItems> = {
      eo: [
        ...commonItems,
        { href: '/requests', label: 'My Requests', icon: '📋' },
        { href: '/requests/new', label: 'New Request', icon: '➕' },
      ],
      hr: [
        ...commonItems,
        { href: '/requests', label: 'My Requests', icon: '📋' },
        { href: '/requests/new', label: 'New Request', icon: '➕' },
      ],
      it: [
        ...commonItems,
        { href: '/tasks', label: 'Tasks', icon: '✓' },
        { href: '/requests', label: 'All Requests', icon: '📋' },
      ],
    }

    return roleItems[auth.role] || commonItems
  }

  const menuItems = getMenuItems()

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Top Navigation */}
      <nav className="bg-white shadow-sm border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16 items-center">
            <div className="flex items-center">
              <h1 className="text-xl font-bold text-gray-900">
                School Onboarding Platform
              </h1>
            </div>
            <div className="flex items-center gap-4">
              <span className="text-sm text-gray-600">
                {getRoleName(auth.role)} ({auth.userId})
              </span>
              <button
                onClick={() => {
                  auth.logout()
                  router.push('/')
                }}
                className="text-sm text-red-600 hover:text-red-700 font-medium"
              >
                Logout
              </button>
            </div>
          </div>
        </div>
      </nav>

      <div className="flex">
        {/* Sidebar */}
        <aside className="w-64 bg-white shadow-sm min-h-screen border-r border-gray-200">
          <nav className="p-4 space-y-2">
            {menuItems.map((item) => (
              <Link
                key={item.href}
                href={item.href}
                className="flex items-center gap-3 px-4 py-2 text-gray-700 hover:bg-gray-100 rounded-lg transition-colors"
              >
                <span className="text-xl">{item.icon}</span>
                <span className="text-sm font-medium">{item.label}</span>
              </Link>
            ))}
          </nav>
        </aside>

        {/* Main Content */}
        <main className="flex-1 p-8">{children}</main>
      </div>
    </div>
  )
}
