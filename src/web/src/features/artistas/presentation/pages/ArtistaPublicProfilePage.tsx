import { useParams, Link } from "react-router-dom"
import { useArtista } from "../../application/hooks/useArtista"
import { ArtistaHero } from "../components/ArtistaHero"
import { ArtistaBio } from "../components/ArtistaBio"
import { Card, CardContent } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"
import { Button } from "@/components/ui/button"
import { AlertCircle, ArrowLeft } from "lucide-react"
import { ROUTES } from "@/lib/constants"

function ArtistaProfileSkeleton() {
  return (
    <div className="min-h-screen bg-[#1a1a2e]">
      {/* Hero Skeleton */}
      <div className="relative">
        <Skeleton className="h-64 w-full rounded-none bg-[#0f1729]" />
        <div className="absolute bottom-0 left-8 transform translate-y-1/2">
          <Skeleton className="h-32 w-32 rounded-full bg-[#0f1729]" />
        </div>
        <div className="mt-20 px-8 pb-6">
          <Skeleton className="h-10 w-64 mb-2 bg-[#0f1729]" />
          <Skeleton className="h-6 w-32 bg-[#0f1729]" />
        </div>
      </div>

      {/* Content Skeleton */}
      <div className="container mx-auto px-8 py-8">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <div className="lg:col-span-2">
            <Skeleton className="h-64 w-full bg-[#0f1729]" />
          </div>
          <div>
            <Skeleton className="h-48 w-full bg-[#0f1729]" />
          </div>
        </div>
      </div>
    </div>
  )
}

function ArtistaNotFound() {
  return (
    <div className="min-h-screen bg-[#1a1a2e] flex items-center justify-center px-4">
      <Card className="bg-[#0f1729] border-[#334155] max-w-md w-full">
        <CardContent className="pt-6 text-center space-y-4">
          <AlertCircle className="h-12 w-12 text-red-500 mx-auto" />
          <h2 className="text-2xl font-bold text-white">Artista no encontrado</h2>
          <p className="text-[#94a3b8]">
            El perfil que estas buscando no existe o ha sido eliminado.
          </p>
          <Button
            asChild
            className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"
          >
            <Link to={ROUTES.HOME}>
              <ArrowLeft className="mr-2 h-4 w-4" />
              Volver al inicio
            </Link>
          </Button>
        </CardContent>
      </Card>
    </div>
  )
}

export default function ArtistaPublicProfilePage() {
  const { id } = useParams<{ id: string }>()
  const { data: artista, isLoading, isError } = useArtista(id!)

  if (isLoading) {
    return <ArtistaProfileSkeleton />
  }

  if (isError || !artista) {
    return <ArtistaNotFound />
  }

  return (
    <div className="min-h-screen bg-[#1a1a2e]">
      {/* Hero Section */}
      <ArtistaHero artista={artista} />

      {/* Main Content */}
      <div className="container mx-auto px-8 py-8">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Left Column - Bio */}
          <div className="lg:col-span-2">
            <ArtistaBio artista={artista} />
          </div>

          {/* Right Column - Stats Placeholder */}
          <div>
            <Card className="bg-[#0f1729] border-[#334155]">
              <CardContent className="pt-6">
                <h3 className="text-white font-semibold mb-4">Estadisticas</h3>
                <div className="space-y-3 text-[#94a3b8]">
                  <div className="flex justify-between items-center">
                    <span>Campanias</span>
                    <span className="font-bold text-white">-</span>
                  </div>
                  <div className="flex justify-between items-center">
                    <span>Backers</span>
                    <span className="font-bold text-white">-</span>
                  </div>
                  <div className="flex justify-between items-center">
                    <span>Fondos Recaudados</span>
                    <span className="font-bold text-white">-</span>
                  </div>
                </div>
                <p className="text-[#64748b] text-sm mt-4 italic">
                  Estadisticas disponibles proximamente
                </p>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    </div>
  )
}
