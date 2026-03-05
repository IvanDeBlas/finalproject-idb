import { FC } from "react"
import { Music, Video, MapPin, TrendingUp, Disc, Globe, Layers, LucideIcon } from "lucide-react"
import { Card, CardHeader, CardContent, CardTitle, CardDescription } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { formatCurrency } from "@/features/campanias/application/utils"
import type { PlantillaProyectoList } from "../../domain"

interface TemplateCardProps {
    template: PlantillaProyectoList
    onSelect: (id: string) => void
    className?: string
}

const ICON_MAP: Record<string, LucideIcon> = {
    music: Music,
    video: Video,
    route: MapPin,
    trending: TrendingUp,
    disc: Disc,
    globe: Globe,
}

function getTemplateIcon(icono?: string): LucideIcon {
    if (!icono) return Music
    const key = icono.toLowerCase()
    for (const [match, icon] of Object.entries(ICON_MAP)) {
        if (key.includes(match)) return icon
    }
    return Music
}

export const TemplateCard: FC<TemplateCardProps> = ({ template, onSelect, className }) => {
    const Icon = getTemplateIcon(template.icono)

    return (
        <Card
            role="button"
            tabIndex={0}
            onClick={() => onSelect(template.id)}
            onKeyDown={(e) => {
                if (e.key === "Enter" || e.key === " ") {
                    e.preventDefault()
                    onSelect(template.id)
                }
            }}
            aria-label={`Template: ${template.nombre}. ${template.cantidadNecesidades} necesidades. Presupuesto ${formatCurrency(template.precioMinTotal)} a ${formatCurrency(template.precioMaxTotal)}`}
            className={`group bg-[#0f1729] border-[#334155] hover:bg-[#1e2a42] hover:border-[#a855f7] hover:shadow-[0_0_20px_rgba(168,85,247,0.3)] transition-all duration-200 cursor-pointer hover:scale-[1.02] active:scale-[0.98] ${className ?? ""}`}
        >
            <CardHeader className="text-center">
                <div className="w-16 h-16 mx-auto mb-4">
                    <Icon className="w-full h-full text-[#a855f7]" />
                </div>
                <CardTitle className="text-xl font-semibold text-white">
                    {template.nombre}
                </CardTitle>
                {template.descripcion && (
                    <CardDescription className="text-sm text-[#94a3b8] line-clamp-2 mt-2">
                        {template.descripcion}
                    </CardDescription>
                )}
            </CardHeader>
            <CardContent className="space-y-3">
                <div className="flex items-center justify-center gap-2 text-sm text-[#94a3b8]">
                    <Layers className="w-4 h-4" />
                    <span>{template.cantidadNecesidades} necesidades</span>
                </div>
                <div className="flex items-center justify-center gap-1 text-base font-medium text-[#a855f7]">
                    <span>
                        {formatCurrency(template.precioMinTotal)} - {formatCurrency(template.precioMaxTotal)}
                    </span>
                </div>
                <Button
                    className="w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold"
                    onClick={(e) => {
                        e.stopPropagation()
                        onSelect(template.id)
                    }}
                >
                    Seleccionar
                </Button>
            </CardContent>
        </Card>
    )
}
