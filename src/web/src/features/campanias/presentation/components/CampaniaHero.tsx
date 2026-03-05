import { useState } from "react"
import { Play, X } from "lucide-react"
import { Button } from "@/components/ui/button"
import { cn } from "@/lib/utils"
import { getVideoEmbedUrl } from "../../application/utils"

interface CampaniaHeroProps {
    imagenUrl: string
    videoUrl?: string
    titulo: string
    alt: string
    className?: string
}

export function CampaniaHero({
    imagenUrl,
    videoUrl,
    titulo,
    alt,
    className,
}: CampaniaHeroProps) {
    const [showVideo, setShowVideo] = useState(false)

    const embedUrl = videoUrl ? getVideoEmbedUrl(videoUrl) : null
    const hasVideo = !!embedUrl

    return (
        <div
            className={cn(
                "w-full h-64 sm:h-80 lg:h-96 relative overflow-hidden rounded-lg",
                className
            )}
        >
            {showVideo && embedUrl ? (
                <div className="relative w-full h-full">
                    <iframe
                        src={embedUrl}
                        title={`Video de ${titulo}`}
                        className="w-full h-full"
                        allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                        allowFullScreen
                    />
                    <Button
                        variant="ghost"
                        size="sm"
                        className="absolute top-3 right-3 bg-black/50 hover:bg-black/70 text-white rounded-full h-8 w-8 p-0"
                        onClick={() => setShowVideo(false)}
                        aria-label="Cerrar video"
                    >
                        <X className="w-4 h-4" />
                    </Button>
                </div>
            ) : (
                <div className="relative w-full h-full group">
                    <img
                        src={imagenUrl}
                        alt={alt}
                        className="w-full h-full object-cover transition-transform duration-300 group-hover:scale-105"
                        loading="lazy"
                    />

                    {/* Gradient overlay */}
                    <div className="absolute inset-0 bg-gradient-to-t from-black/30 to-transparent" />

                    {/* Play button overlay */}
                    {hasVideo && (
                        <button
                            className="absolute inset-0 flex items-center justify-center"
                            onClick={() => setShowVideo(true)}
                            aria-label={`Reproducir video de ${titulo}`}
                        >
                            <div className="w-16 h-16 rounded-full bg-white/90 hover:bg-white flex items-center justify-center transition-all hover:scale-110 shadow-lg">
                                <Play className="w-7 h-7 text-[#1a1a2e] ml-1" />
                            </div>
                        </button>
                    )}
                </div>
            )}
        </div>
    )
}
