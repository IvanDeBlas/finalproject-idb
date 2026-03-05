import { Outlet, Navigate } from "react-router-dom"
import { Sidebar } from "./Sidebar"
import { useAuthStore } from "@/store/auth-store"
import { ROUTES } from "@/lib/constants"

export function DashboardLayout() {
  const { isAuthenticated } = useAuthStore()

  if (!isAuthenticated) {
    return <Navigate to={ROUTES.LOGIN} replace />
  }

  return (
    <div className="flex h-screen">
      <Sidebar />
      <main className="flex-1 overflow-auto bg-muted/30 p-6">
        <Outlet />
      </main>
    </div>
  )
}
