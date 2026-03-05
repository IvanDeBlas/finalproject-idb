import { Avatar, AvatarImage, AvatarFallback } from "@/components/ui/avatar"
import { User } from "lucide-react"
import type { Artista } from "../../domain"

interface ArtistaHeroProps {
  artista: Artista
}

export function ArtistaHero({ artista }: ArtistaHeroProps) {
  return (
    <div className="relative">
      {/* Hero Banner with Gradient */}
      <div className="h-64 bg-gradient-to-r from-purple-900 via-purple-800 to-pink-900" />

      {/* Avatar positioned over banner */}
      <div className="absolute bottom-0 left-8 transform translate-y-1/2">
        <Avatar className="h-32 w-32 border-4 border-[#1a1a2e]">
          <AvatarImage src={artista.imagenUrl} alt={artista.nombreArtistico} />
          <AvatarFallback className="bg-[#0f1729] text-white">
            <User className="h-16 w-16" />
          </AvatarFallback>
        </Avatar>
      </div>

      {/* Artista Name and Genre */}
      <div className="mt-20 px-8 pb-6">
        <h1 className="text-4xl font-bold text-white mb-2">
          {artista.nombreArtistico}
        </h1>
        {artista.generoMusical && (
          <p className="text-[#94a3b8] text-lg">{artista.generoMusical}</p>
        )}
      </div>
    </div>
  )
}
