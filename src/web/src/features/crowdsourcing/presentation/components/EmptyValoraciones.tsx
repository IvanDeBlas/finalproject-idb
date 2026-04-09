import { FC } from "react"
import { Star } from "lucide-react"

export const EmptyValoraciones: FC = () => {
    return (
        <div
            role="status"
            className="bg-[#0f1729] border border-[#334155] rounded-xl p-10 flex flex-col items-center"
        >
            <Star
                className="w-10 h-10 text-[#334155] mx-auto mb-3"
                aria-hidden="true"
            />
            <p className="text-[#94a3b8] text-center text-sm">
                Este usuario aun no tiene valoraciones
            </p>
            <p className="text-[#64748b] text-center text-xs mt-1">
                Completa un acuerdo para recibir tu primera valoracion
            </p>
        </div>
    )
}
