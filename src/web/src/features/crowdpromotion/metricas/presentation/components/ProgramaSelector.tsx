import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import { Skeleton } from "@/components/ui/skeleton"
import type { ProgramaSelectorItem } from "../../application/hooks/useMisProgramasParaSelector"

interface ProgramaSelectorProps {
    programas: ProgramaSelectorItem[]
    selectedId: string | undefined
    onSelect: (programaId: string) => void
    isLoading?: boolean
}

export function ProgramaSelector({ programas, selectedId, onSelect, isLoading }: ProgramaSelectorProps) {
    if (isLoading) {
        return <Skeleton className="w-full sm:w-64 h-10 rounded-md bg-[#1e1e38]" />
    }

    return (
        <div className="flex items-center gap-3">
            <label
                htmlFor="programa-select"
                className="text-sm text-[#94a3b8] shrink-0"
            >
                Programa:
            </label>
            <Select
                value={selectedId}
                onValueChange={onSelect}
            >
                <SelectTrigger
                    id="programa-select"
                    aria-label="Selecciona un programa de promocion"
                    className="w-full sm:w-64 bg-[#0f0f1f] border-[#334155] text-white text-sm focus:border-[#a855f7] focus:ring-[#a855f7]"
                >
                    <SelectValue placeholder="Selecciona un programa" />
                </SelectTrigger>
                <SelectContent>
                    {programas.map(p => (
                        <SelectItem key={p.id} value={p.id} className="text-sm">
                            {p.nombrePrograma}
                        </SelectItem>
                    ))}
                </SelectContent>
            </Select>
        </div>
    )
}
