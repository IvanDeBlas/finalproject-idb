import { Link } from "react-router-dom"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Progress } from "@/components/ui/progress"
import { ROUTES } from "@/lib/constants"
import { useAuthStore } from "@/store/auth-store"
import { useMisCampanias } from "@/features/campanias"
import { useMyArtistProfile } from "../../application"
import { PlusCircle, Music, TrendingUp, Users } from "lucide-react"

export default function DashboardPage() {
  const { user } = useAuthStore()
  const { data: artista } = useMyArtistProfile()
  const { data: campanias, isLoading } = useMisCampanias(artista?.id || "")

  const stats = {
    totalRecaudado: campanias?.reduce((acc, c) => acc + c.importePledgedActual, 0) || 0,
    campaniaActivas: campanias?.filter((c) => c.estado === "activa").length || 0,
    totalCampanias: campanias?.length || 0,
  }

  return (
    <div className="space-y-8">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold">Dashboard</h1>
          <p className="text-muted-foreground">
            Bienvenido, {artista?.nombreArtistico || user?.nombreCompleto}
          </p>
        </div>
        <Link to={ROUTES.CAMPANIA_NEW}>
          <Button>
            <PlusCircle className="mr-2 h-4 w-4" />
            Nueva campania
          </Button>
        </Link>
      </div>

      {/* Stats Cards */}
      <div className="grid gap-4 md:grid-cols-3">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium">Total Recaudado</CardTitle>
            <TrendingUp className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {stats.totalRecaudado.toLocaleString("es-ES")} EUR
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium">Campanias Activas</CardTitle>
            <Music className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.campaniaActivas}</div>
          </CardContent>
        </Card>
        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium">Total Campanias</CardTitle>
            <Users className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats.totalCampanias}</div>
          </CardContent>
        </Card>
      </div>

      {/* Mis Campanias */}
      <Card>
        <CardHeader>
          <CardTitle>Mis Campanias</CardTitle>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <div className="space-y-4">
              {[1, 2].map((i) => (
                <div key={i} className="h-20 animate-pulse rounded bg-muted" />
              ))}
            </div>
          ) : campanias && campanias.length > 0 ? (
            <div className="space-y-4">
              {campanias.map((campania) => {
                const porcentaje = Math.round(
                  (campania.importePledgedActual / campania.importeObjetivo) * 100
                )
                return (
                  <div
                    key={campania.id}
                    className="flex items-center justify-between rounded-lg border p-4"
                  >
                    <div className="flex-1">
                      <h3 className="font-medium">{campania.titulo}</h3>
                      <div className="mt-2 flex items-center gap-4">
                        <Progress value={porcentaje} className="h-2 w-32" />
                        <span className="text-sm text-muted-foreground">
                          {porcentaje}% - {campania.importePledgedActual.toLocaleString()} EUR
                        </span>
                      </div>
                    </div>
                    <div className="flex items-center gap-2">
                      <span
                        className={`rounded-full px-2 py-1 text-xs ${
                          campania.estado === "activa"
                            ? "bg-green-100 text-green-700"
                            : campania.estado === "borrador"
                            ? "bg-yellow-100 text-yellow-700"
                            : "bg-gray-100 text-gray-700"
                        }`}
                      >
                        {campania.estado}
                      </span>
                      <Link to={`/campanias/${campania.id}`}>
                        <Button variant="ghost" size="sm">
                          Ver
                        </Button>
                      </Link>
                    </div>
                  </div>
                )
              })}
            </div>
          ) : (
            <div className="text-center py-8">
              <p className="text-muted-foreground mb-4">
                No tienes campanias aun. Crea tu primera campania!
              </p>
              <Link to={ROUTES.CAMPANIA_NEW}>
                <Button>
                  <PlusCircle className="mr-2 h-4 w-4" />
                  Crear campania
                </Button>
              </Link>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
