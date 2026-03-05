import type { FC } from "react"
import { Link } from "react-router-dom"
import { Card, CardContent, CardFooter, CardHeader } from "@/components/ui/card"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { Badge } from "@/components/ui/badge"
import { Separator } from "@/components/ui/separator"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import { Mail, Globe } from "lucide-react"
import { APP_ROUTES } from "@shared/constants"
import { PromotorSocialLinks } from "./PromotorSocialLinks"
import type { Promotor } from "../../domain"

interface PromotorPerfilCardProps {
    promotor?: Promotor
    isLoading?: boolean
}

function getInitials(name: string): string {
    return name
        .split(" ")
        .map(w => w[0])
        .join("")
        .toUpperCase()
        .slice(0, 2)
}

export const PromotorPerfilCard: FC<PromotorPerfilCardProps> = ({
    promotor,
    isLoading,
}) => {
    if (isLoading || !promotor) {
        return (
            <Card className="bg-[#151525] border-[#334155]">
                <CardHeader className="border-b border-[#334155] pb-4">
                    <Skeleton className="h-5 w-48 bg-[#1e1e38]" />
                </CardHeader>
                <CardContent className="pt-6 space-y-4">
                    <div className="flex items-center gap-4">
                        <Skeleton className="w-16 h-16 rounded-full bg-[#1e1e38]" />
                        <div className="space-y-2">
                            <Skeleton className="h-5 w-40 bg-[#1e1e38]" />
                            <Skeleton className="h-4 w-24 bg-[#1e1e38]" />
                        </div>
                    </div>
                    <Skeleton className="h-px w-full bg-[#1e1e38]" />
                    <Skeleton className="h-4 w-56 bg-[#1e1e38]" />
                    <Skeleton className="h-4 w-44 bg-[#1e1e38]" />
                </CardContent>
            </Card>
        )
    }

    const esActivo = promotor.esActivo

    return (
        <Card className="bg-[#151525] border-[#334155]">
            <CardHeader className="border-b border-[#334155] pb-4">
                <h2 className="text-lg font-semibold text-white">Mi Perfil de Promotor</h2>
            </CardHeader>
            <CardContent className="pt-6">
                <div className="flex flex-col items-center text-center sm:flex-row sm:items-start sm:text-left gap-4">
                    <Avatar className="w-16 h-16">
                        <AvatarFallback className="bg-gradient-to-br from-pink-500 to-purple-600 text-white text-xl font-bold">
                            {getInitials(promotor.nombrePublico)}
                        </AvatarFallback>
                    </Avatar>
                    <div>
                        <div className="flex items-center gap-2 flex-wrap">
                            <p className="text-xl font-semibold text-white">{promotor.nombrePublico}</p>
                            <Badge className={esActivo
                                ? "bg-green-950/50 text-green-400 border border-green-800/50"
                                : "bg-slate-800 text-slate-400 border border-slate-700"
                            }>
                                {esActivo ? "ACTIVO" : "INACTIVO"}
                            </Badge>
                        </div>
                        <p className="text-sm text-[#94a3b8] mt-1">{promotor.tipoPromotorNombre}</p>
                    </div>
                </div>

                <Separator className="bg-[#334155] my-4" />

                <div className="space-y-3">
                    <div className="flex items-start gap-3">
                        <Mail className="w-4 h-4 text-[#64748b] mt-0.5 flex-shrink-0" />
                        <span className="text-sm text-[#cbd5e1]">
                            {promotor.emailContacto || <span className="text-[#64748b] italic">--</span>}
                        </span>
                    </div>
                    <div className="flex items-start gap-3">
                        <Globe className="w-4 h-4 text-[#64748b] mt-0.5 flex-shrink-0" />
                        {promotor.urlSitioWeb ? (
                            <a
                                href={promotor.urlSitioWeb}
                                target="_blank"
                                rel="noopener noreferrer"
                                className="text-sm text-[#a855f7] hover:underline truncate max-w-[200px] inline-block"
                            >
                                {promotor.urlSitioWeb}
                            </a>
                        ) : (
                            <span className="text-sm text-[#64748b] italic">--</span>
                        )}
                    </div>

                    <Separator className="bg-[#334155] my-3" />

                    <PromotorSocialLinks
                        urlInstagram={promotor.urlInstagram}
                        urlTikTok={promotor.urlTikTok}
                        urlYouTube={promotor.urlYouTube}
                        urlTwitter={promotor.urlTwitter}
                    />
                </div>
            </CardContent>
            <CardFooter className="pt-4 border-t border-[#334155] flex gap-3">
                <Button
                    variant="outline"
                    className="border-[#334155] text-[#cbd5e1] hover:bg-[#1e1e38] hover:text-white"
                    asChild
                >
                    <Link to={APP_ROUTES.landing.promotor.perfil}>Editar perfil</Link>
                </Button>
                <Button
                    className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white"
                    disabled={!esActivo}
                >
                    Ver programas
                </Button>
            </CardFooter>
        </Card>
    )
}
