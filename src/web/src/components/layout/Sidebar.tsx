import { Link, useLocation } from "react-router-dom"
import { cn } from "@/lib/utils"
import { ROUTES, APP_NAME } from "@/lib/constants"
import {
  LayoutDashboard,
  Music,
  PlusCircle,
  User,
  LogOut,
  Megaphone,
  UserCheck,
} from "lucide-react"
import { useAuthStore } from "@/store/auth-store"
import { Button } from "@/components/ui/button"

const menuItems = [
  {
    label: "Dashboard",
    icon: LayoutDashboard,
    href: ROUTES.DASHBOARD,
  },
  {
    label: "Nueva Campania",
    icon: PlusCircle,
    href: ROUTES.CAMPANIA_NEW,
  },
  {
    label: "Mi Perfil",
    icon: User,
    href: ROUTES.ARTISTA_PERFIL,
  },
  {
    label: "Promotor",
    icon: Megaphone,
    href: "/promotor/dashboard",
  },
  {
    label: "Perfil Promotor",
    icon: UserCheck,
    href: "/promotor/perfil",
  },
]

export function Sidebar() {
  const location = useLocation()
  const { logout } = useAuthStore()

  return (
    <aside className="flex h-full w-64 flex-col border-r bg-background">
      <div className="flex h-16 items-center border-b px-6">
        <Link to={ROUTES.HOME} className="flex items-center gap-2">
          <Music className="h-6 w-6 text-primary" />
          <span className="text-lg font-bold">{APP_NAME}</span>
        </Link>
      </div>

      <nav className="flex-1 space-y-1 p-4">
        {menuItems.map((item) => {
          const isActive = location.pathname === item.href
          return (
            <Link
              key={item.href}
              to={item.href}
              className={cn(
                "flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors",
                isActive
                  ? "bg-primary text-primary-foreground"
                  : "text-muted-foreground hover:bg-muted hover:text-foreground"
              )}
            >
              <item.icon className="h-4 w-4" />
              {item.label}
            </Link>
          )
        })}
      </nav>

      <div className="border-t p-4">
        <Button
          variant="ghost"
          className="w-full justify-start gap-3"
          onClick={logout}
        >
          <LogOut className="h-4 w-4" />
          Cerrar sesion
        </Button>
      </div>
    </aside>
  )
}
