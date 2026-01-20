import type { Metadata } from 'next'
import './globals.css'

export const metadata: Metadata = {
  title: 'School Onboarding Platform',
  description: 'Manage student and staff onboarding/offboarding',
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  )
}
