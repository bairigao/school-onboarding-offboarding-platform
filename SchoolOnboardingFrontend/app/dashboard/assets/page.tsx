'use client'

import { useAuth } from '@/lib/auth'
import { useState, useEffect } from 'react'
import { assetsAPI } from '@/lib/api'

interface Asset {
  id: number
  name: string
  assetTag: string
  model: string
  manufacturer: string
  status: string
  assignedTo?: string
  available: boolean
}

export default function AssetsPage() {
  const auth = useAuth()
  const [assets, setAssets] = useState<Asset[]>([])
  const [loading, setLoading] = useState(true)
  const [page, setPage] = useState(1)
  const pageSize = 20

  useEffect(() => {
    const loadAssets = async () => {
      try {
        const response = await assetsAPI.getAll(page, pageSize, {
          role: auth.role || '',
          userId: auth.userId || '',
        })
        setAssets(response.data?.assets || [])
      } catch (error) {
        console.error('Error loading assets:', error)
      } finally {
        setLoading(false)
      }
    }

    loadAssets()
  }, [auth, page])

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold text-gray-900">Devices (Snipe-IT)</h1>
        <button
          onClick={() => {
            setLoading(true)
            assetsAPI.sync({
              role: auth.role || '',
              userId: auth.userId || '',
            }).finally(() => setLoading(false))
          }}
          className="bg-blue-600 hover:bg-blue-700 text-white font-medium py-2 px-4 rounded-lg transition-colors"
        >
          Sync from Snipe-IT
        </button>
      </div>

      {/* Assets Table */}
      <div className="bg-white rounded-lg shadow overflow-hidden">
        {loading ? (
          <div className="p-8 text-center text-gray-500">Loading...</div>
        ) : assets.length === 0 ? (
          <div className="p-8 text-center text-gray-500">No devices found</div>
        ) : (
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">
                  Asset Tag
                </th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">
                  Name
                </th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">
                  Model
                </th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">
                  Status
                </th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">
                  Assigned To
                </th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {assets.map((asset) => (
                <tr key={asset.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 text-sm font-mono text-gray-900">
                    {asset.assetTag}
                  </td>
                  <td className="px-6 py-4 text-sm font-medium text-gray-900">
                    {asset.name}
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-600">
                    {asset.model}
                  </td>
                  <td className="px-6 py-4 text-sm">
                    <span className={`px-2 py-1 rounded text-xs font-medium ${
                      asset.available
                        ? 'bg-green-100 text-green-800'
                        : 'bg-yellow-100 text-yellow-800'
                    }`}>
                      {asset.status}
                    </span>
                  </td>
                  <td className="px-6 py-4 text-sm text-gray-600">
                    {asset.assignedTo || '-'}
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

      {/* Pagination */}
      <div className="flex justify-between items-center">
        <button
          onClick={() => setPage(Math.max(1, page - 1))}
          disabled={page === 1}
          className="bg-gray-200 hover:bg-gray-300 disabled:opacity-50 text-gray-900 font-medium py-2 px-4 rounded-lg transition-colors"
        >
          Previous
        </button>
        <span className="text-gray-600">Page {page}</span>
        <button
          onClick={() => setPage(page + 1)}
          className="bg-gray-200 hover:bg-gray-300 text-gray-900 font-medium py-2 px-4 rounded-lg transition-colors"
        >
          Next
        </button>
      </div>
    </div>
  )
}
