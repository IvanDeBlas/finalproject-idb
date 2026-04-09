import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table"
import { Badge } from "@/components/ui/badge"
import { Users } from "lucide-react"
import type { PromoProgramaPromotorSummary } from "@shared/types"

interface PromoProgramaPromotoresTabProps {
    promotores: PromoProgramaPromotorSummary[]
}

function getEstadoBadge(promotor: PromoProgramaPromotorSummary) {
    if (promotor.esBloqueado) {
        return <Badge className="bg-red-500/20 text-red-400 text-xs">Bloqueado</Badge>
    }
    if (promotor.esAprobado) {
        return <Badge className="bg-emerald-500/20 text-emerald-400 text-xs">Aprobado</Badge>
    }
    return <Badge className="bg-yellow-500/20 text-yellow-400 text-xs">Pendiente</Badge>
}

export function PromoProgramaPromotoresTab({ promotores }: PromoProgramaPromotoresTabProps) {
    if (promotores.length === 0) {
        return (
            <div className="flex flex-col items-center justify-center py-12 text-center">
                <div className="rounded-full bg-[#1e1e38] p-4 mb-4">
                    <Users className="h-8 w-8 text-zinc-400" />
                </div>
                <p className="text-sm text-zinc-400">
                    Aun no hay promotores inscritos en este programa
                </p>
            </div>
        )
    }

    return (
        <div className="rounded-md border border-zinc-800">
            <Table>
                <TableHeader>
                    <TableRow className="border-zinc-800 hover:bg-transparent">
                        <TableHead className="text-zinc-400">Promotor</TableHead>
                        <TableHead className="text-zinc-400">Tipo</TableHead>
                        <TableHead className="text-zinc-400">Estado</TableHead>
                        <TableHead className="text-zinc-400">Fecha de alta</TableHead>
                    </TableRow>
                </TableHeader>
                <TableBody>
                    {promotores.map((promotor) => (
                        <TableRow key={promotor.id} className="border-zinc-800">
                            <TableCell>
                                <div className="flex items-center gap-2">
                                    <div className="h-8 w-8 rounded-full bg-purple-500/20 flex items-center justify-center text-xs font-medium text-purple-400">
                                        {promotor.promotorNombre.charAt(0).toUpperCase()}
                                    </div>
                                    <span className="text-sm text-white">{promotor.promotorNombre}</span>
                                </div>
                            </TableCell>
                            <TableCell className="text-sm text-zinc-400">
                                {promotor.tipoPromotorNombre}
                            </TableCell>
                            <TableCell>{getEstadoBadge(promotor)}</TableCell>
                            <TableCell className="text-sm text-zinc-400">
                                {new Date(promotor.fechaAlta).toLocaleDateString("es-ES")}
                            </TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </div>
    )
}
