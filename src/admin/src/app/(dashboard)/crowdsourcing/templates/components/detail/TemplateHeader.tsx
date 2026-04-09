"use client"

import { Card, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { IconPreview } from "../shared/IconPreview"
import { Edit } from "lucide-react"
import type { PlantillaProyecto } from "@shared/types"

interface TemplateHeaderProps {
    template: PlantillaProyecto
    onEdit?: () => void
}

export function TemplateHeader({ template, onEdit }: TemplateHeaderProps) {
    return (
        <Card>
            <CardHeader className="flex flex-row items-start justify-between">
                <div className="flex items-start gap-4">
                    {template.icono && (
                        <div className="p-3 rounded-lg bg-primary/10">
                            <IconPreview icon={template.icono} size="lg" />
                        </div>
                    )}
                    <div>
                        <div className="flex items-center gap-3 mb-2">
                            <CardTitle className="text-2xl">{template.nombre}</CardTitle>
                            <Badge variant="default">Activo</Badge>
                        </div>
                        {template.descripcion && (
                            <p className="text-muted-foreground">{template.descripcion}</p>
                        )}
                    </div>
                </div>
                {onEdit && (
                    <Button variant="outline" onClick={onEdit}>
                        <Edit className="h-4 w-4 mr-2" />
                        Editar
                    </Button>
                )}
            </CardHeader>
        </Card>
    )
}
