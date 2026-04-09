import { Link } from "react-router-dom"
import { Card } from "@/components/ui/card"
import { Avatar, AvatarImage, AvatarFallback } from "@/components/ui/avatar"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { CheckCircle } from "lucide-react"
import { cn } from "@/lib/utils"

interface ArtistCardProps {
    artistaId: string
    nombre: string
    avatar?: string
    bio?: string
    verified?: boolean
    className?: string
}

export function ArtistCard({
    artistaId,
    nombre,
    avatar,
    bio,
    verified = false,
    className,
}: ArtistCardProps) {
    const initials = nombre
        .split(" ")
        .map((n) => n[0])
        .join("")
        .slice(0, 2)
        .toUpperCase()

    return (
        <Card className={cn("bg-[#0f1729] border-[#334155] p-6", className)}>
            <h3 className="text-lg font-bold text-white mb-4">Sobre el Artista</h3>

            <div className="flex items-start gap-4 mb-4">
                <Avatar className="w-16 h-16">
                    {avatar && <AvatarImage src={avatar} alt={nombre} />}
                    <AvatarFallback className="bg-[#334155] text-white text-lg">
                        {initials}
                    </AvatarFallback>
                </Avatar>

                <div className="flex-1 min-w-0">
                    <div className="flex items-center gap-2 mb-1">
                        <h4 className="font-semibold text-white truncate">{nombre}</h4>
                        {verified && (
                            <Badge className="bg-primary/20 text-primary border-primary/50 shrink-0">
                                <CheckCircle className="w-3 h-3 mr-1" />
                                Verificado
                            </Badge>
                        )}
                    </div>
                    {bio && (
                        <p className="text-sm text-[#94a3b8] line-clamp-3">
                            {bio}
                        </p>
                    )}
                </div>
            </div>

            <Button
                variant="outline"
                size="sm"
                className="w-full border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e2a42]"
                asChild
            >
                <Link to={`/artistas/${artistaId}`}>Ver perfil</Link>
            </Button>
        </Card>
    )
}
