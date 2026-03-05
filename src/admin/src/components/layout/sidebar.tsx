"use client"

import Link from "next/link"
import { usePathname } from "next/navigation"
import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import { useSidebarStore } from "@/store/sidebar-store"
import { useAuthStore } from "@/store/auth-store"
import {
  LayoutDashboard,
  Music,
  PlusCircle,
  User,
  LogOut,
  ChevronLeft,
  ChevronRight,
  FileText,
  Megaphone,
} from "lucide-react"

const menuItems = [
  {
    label: "Dashboard",
    icon: LayoutDashboard,
    href: "/dashboard",
  },
  {
    label: "Mis Campanias",
    icon: Music,
    href: "/campanias",
  },
  {
    label: "Nueva Campania",
    icon: PlusCircle,
    href: "/campanias/nueva",
  },
  {
    label: "Templates",
    icon: FileText,
    href: "/crowdsourcing/templates",
  },
  {
    label: "Mi Perfil",
    icon: User,
    href: "/perfil",
  },
  {
    label: "Promotor",
    icon: Megaphone,
    href: "/promotor",
  },
]

export function Sidebar() {
  const pathname = usePathname()
  const { logout } = useAuthStore()
  const { isCollapsed, toggle } = useSidebarStore()

  return (
    <aside
      className={cn(
        "fixed left-0 top-0 z-40 h-screen border-r bg-background transition-all duration-300",
        isCollapsed ? "w-[70px]" : "w-[250px]",
        "hidden lg:flex flex-col"
      )}
    >
      {/* Header */}
      <div className="flex h-16 items-center justify-between border-b px-4">
        {!isCollapsed && (
          <Link href="/dashboard" className="flex items-center gap-2">
            <Music className="h-6 w-6 text-primary" />
            <span className="text-lg font-bold">WePlay</span>
          </Link>
        )}
        {isCollapsed && (
          <Link href="/dashboard" className="mx-auto">
            <Music className="h-6 w-6 text-primary" />
          </Link>
        )}
      </div>

      {/* Navigation */}
      <nav className="flex-1 space-y-1 p-2">
        {menuItems.map((item) => {
          const isActive = pathname === item.href
          return (
            <Link key={item.href} href={item.href}>
              <Button
                variant={isActive ? "secondary" : "ghost"}
                className={cn(
                  "w-full justify-start gap-3",
                  isCollapsed && "justify-center px-2"
                )}
              >
                <item.icon className="h-4 w-4 shrink-0" />
                {!isCollapsed && <span>{item.label}</span>}
              </Button>
            </Link>
          )
        })}
      </nav>

      {/* Footer */}
      <div className="border-t p-2">
        <Button
          variant="ghost"
          className={cn("w-full justify-start gap-3", isCollapsed && "justify-center px-2")}
          onClick={logout}
        >
          <LogOut className="h-4 w-4 shrink-0" />
          {!isCollapsed && <span>Cerrar sesion</span>}
        </Button>
      </div>

      {/* Collapse toggle */}
      <Button
        variant="ghost"
        size="icon"
        className="absolute -right-3 top-20 h-6 w-6 rounded-full border bg-background"
        onClick={toggle}
      >
        {isCollapsed ? (
          <ChevronRight className="h-3 w-3" />
        ) : (
          <ChevronLeft className="h-3 w-3" />
        )}
      </Button>
    </aside>
  )
}
