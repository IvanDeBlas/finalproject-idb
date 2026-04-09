import { FC } from "react"
import { Music } from "lucide-react"
import { Button } from "@/components/ui/button"
import { TemplateCard } from "./TemplateCard"
import { TemplateCardSkeleton } from "./TemplateCardSkeleton"
import type { PlantillaProyectoList } from "../../domain"

interface TemplateGalleryProps {
    templates: PlantillaProyectoList[]
    onSelectTemplate: (id: string) => void
    isLoading?: boolean
}

export const TemplateGallery: FC<TemplateGalleryProps> = ({
    templates,
    onSelectTemplate,
    isLoading,
}) => {
    if (isLoading) {
        return (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 md:gap-6 max-w-7xl mx-auto">
                {Array.from({ length: 6 }).map((_, i) => (
                    <TemplateCardSkeleton key={i} />
                ))}
            </div>
        )
    }

    if (templates.length === 0) {
        return (
            <div className="flex flex-col items-center justify-center py-16 text-center">
                <Music className="w-16 h-16 text-[#64748b] mb-4" />
                <h3 className="text-xl font-semibold text-white mb-2">
                    No hay plantillas disponibles
                </h3>
                <p className="text-sm text-[#94a3b8] mb-6">
                    Crea tu primera plantilla personalizada
                </p>
                <Button className="bg-gradient-to-r from-pink-500 to-purple-600">
                    Crear plantilla personalizada
                </Button>
            </div>
        )
    }

    return (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 md:gap-6 max-w-7xl mx-auto">
            {templates.map((template) => (
                <TemplateCard
                    key={template.id}
                    template={template}
                    onSelect={onSelectTemplate}
                />
            ))}
        </div>
    )
}
