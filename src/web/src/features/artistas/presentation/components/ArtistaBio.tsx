import { Card, CardHeader, CardTitle, CardContent } from "@/components/ui/card"
import { MapPin, Globe, Music } from "lucide-react"
import type { Artista } from "../../domain"

interface ArtistaBioProps {
  artista: Artista
}

export function ArtistaBio({ artista }: ArtistaBioProps) {
  const hasLocation = artista.ciudad || artista.pais

  return (
    <Card className="bg-[#0f1729] border-[#334155]">
      <CardHeader>
        <CardTitle className="text-white">Sobre el Artista</CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        {/* Descripcion/Biografia */}
        {artista.descripcion ? (
          <p className="text-[#94a3b8] leading-relaxed">{artista.descripcion}</p>
        ) : (
          <p className="text-[#64748b] italic">No hay descripcion disponible</p>
        )}

        {/* Ubicacion */}
        {hasLocation && (
          <div className="flex items-center gap-2 text-[#94a3b8]">
            <MapPin className="h-4 w-4 text-[#64748b]" />
            <span>
              {[artista.ciudad, artista.pais].filter(Boolean).join(", ")}
            </span>
          </div>
        )}

        {/* Genero Musical */}
        {artista.generoMusical && (
          <div className="flex items-center gap-2 text-[#94a3b8]">
            <Music className="h-4 w-4 text-[#64748b]" />
            <span>{artista.generoMusical}</span>
          </div>
        )}

        {/* Placeholder para Redes Sociales */}
        <div className="pt-4 border-t border-[#334155]">
          <h4 className="text-white font-medium mb-3">Redes Sociales</h4>
          <div className="flex items-center gap-2 text-[#64748b]">
            <Globe className="h-4 w-4" />
            <span className="italic">Proximamente</span>
          </div>
        </div>
      </CardContent>
    </Card>
  )
}
