import { Link } from "react-router-dom"
import { ROUTES, APP_NAME } from "@/lib/constants"
import { useAuthStore } from "@/store/auth-store"
import { Button } from "@/components/ui/button"
import { Music } from "lucide-react"
import { useNoLeidosCount } from "@/features/crowdsourcing/application/hooks/useNoLeidosCount"
import { NavbarMensajesIcon } from "@/features/crowdsourcing/presentation/components/NavbarMensajesIcon"

export function Header() {
  const { isAuthenticated, logout, user } = useAuthStore()
  const { data: noLeidosData } = useNoLeidosCount()
  const totalNoLeidos = noLeidosData?.totalNoLeidos ?? 0

  return (
    <header className="sticky top-0 z-50 w-full border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="container flex h-16 items-center justify-between">
        <Link to={ROUTES.HOME} className="flex items-center gap-2">
          <Music className="h-6 w-6 text-primary" />
          <span className="text-xl font-bold">{APP_NAME}</span>
        </Link>

        <nav className="flex items-center gap-6">
          <Link
            to={ROUTES.EXPLORAR}
            className="text-sm font-medium text-muted-foreground hover:text-foreground transition-colors"
          >
            Explorar
          </Link>
          <Link
            to={ROUTES.CAMPANIAS}
            className="text-sm font-medium text-muted-foreground hover:text-foreground transition-colors"
          >
            Campanias
          </Link>

          {isAuthenticated ? (
            <div className="flex items-center gap-4">
              <NavbarMensajesIcon totalNoLeidos={totalNoLeidos} />
              <Link to={ROUTES.DASHBOARD}>
                <Button variant="ghost" size="sm">
                  Dashboard
                </Button>
              </Link>
              <span className="text-sm text-muted-foreground">
                {user?.email}
              </span>
              <Button variant="outline" size="sm" onClick={logout}>
                Cerrar sesion
              </Button>
            </div>
          ) : (
            <div className="flex items-center gap-2">
              <Link to={ROUTES.LOGIN}>
                <Button variant="ghost" size="sm">
                  Iniciar sesion
                </Button>
              </Link>
              <Link to={ROUTES.REGISTER}>
                <Button size="sm">
                  Registrarse
                </Button>
              </Link>
            </div>
          )}
        </nav>
      </div>
    </header>
  )
}
