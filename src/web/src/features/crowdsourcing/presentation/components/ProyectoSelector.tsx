import { FC } from "react"
import { Card } from "@/components/ui/card"
import { Label } from "@/components/ui/label"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"

interface ProyectoArtistico {
    id: string
    nombre: string
}

interface ProyectoSelectorProps {
    value: string | undefined
    onChange: (id: string) => void
    proyectos: ProyectoArtistico[]
}

export const ProyectoSelector: FC<ProyectoSelectorProps> = ({
    value,
    onChange,
    proyectos,
}) => {
    return (
        <Card className="bg-[#0f1729] border-[#334155] p-4 mt-6 max-w-4xl mx-auto">
            <Label className="text-sm font-medium text-[#cbd5e1] mb-2 block">
                Proyecto Artistico (opcional)
            </Label>
            <p className="text-xs text-[#94a3b8] mb-3">
                Vincula estas necesidades a un proyecto existente o crea uno nuevo
            </p>
            <Select value={value} onValueChange={onChange}>
                <SelectTrigger className="bg-[#1a1a2e] border-[#334155] text-white">
                    <SelectValue placeholder="Selecciona un proyecto..." />
                </SelectTrigger>
                <SelectContent className="bg-[#16213e] border-[#334155]">
                    {proyectos.map((proyecto) => (
                        <SelectItem key={proyecto.id} value={proyecto.id}>
                            {proyecto.nombre}
                        </SelectItem>
                    ))}
                    <SelectItem value="__create_new__">
                        + Crear nuevo proyecto
                    </SelectItem>
                </SelectContent>
            </Select>
        </Card>
    )
}
