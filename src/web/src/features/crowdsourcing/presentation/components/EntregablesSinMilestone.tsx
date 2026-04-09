import { FC } from "react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Separator } from "@/components/ui/separator"
import { EntregableItem } from "./EntregableItem"
import type { Entregable, RolAcuerdo } from "../../domain"

interface EntregablesSinMilestoneProps {
    entregables: Entregable[]
    miRol: RolAcuerdo
    estadoAcuerdoId: number
    onAprobarEntregable: (entregable: Entregable) => void
    onRechazarEntregable: (entregable: Entregable) => void
}

export const EntregablesSinMilestone: FC<EntregablesSinMilestoneProps> = ({
    entregables,
    miRol,
    estadoAcuerdoId,
    onAprobarEntregable,
    onRechazarEntregable,
}) => {
    if (entregables.length === 0) return null

    return (
        <Card className="bg-[#0f1729] border-[#334155]">
            <CardHeader className="pb-3">
                <CardTitle className="text-white text-base">
                    Entregables generales
                </CardTitle>
            </CardHeader>
            <CardContent>
                {entregables.map((entregable, index) => (
                    <div key={entregable.id}>
                        {index > 0 && <Separator className="bg-slate-700" />}
                        <EntregableItem
                            entregable={entregable}
                            miRol={miRol}
                            estadoAcuerdoId={estadoAcuerdoId}
                            onAprobar={onAprobarEntregable}
                            onRechazar={onRechazarEntregable}
                        />
                    </div>
                ))}
            </CardContent>
        </Card>
    )
}
