import type { BackingPublicDto } from "@shared/types/backing"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { User, EyeOff } from "lucide-react"
import { formatCurrency, formatRelativeTime } from "../../application/utils"
import { cn } from "@/lib/utils"

interface BackingRecenteCardProps {
    backing: BackingPublicDto
    className?: string
}

export function BackingRecenteCard({ backing, className }: BackingRecenteCardProps) {
    const isAnonimo = backing.nombreBacker === "Anonimo"

    return (
        <div className={cn("flex items-start gap-3 p-3 bg-[#1a1a2e] rounded-lg", className)}>
            <Avatar className="w-10 h-10 bg-[#334155]">
                <AvatarFallback className="bg-[#334155] text-[#94a3b8]">
                    {isAnonimo ? (
                        <EyeOff className="w-4 h-4" />
                    ) : (
                        <User className="w-4 h-4" />
                    )}
                </AvatarFallback>
            </Avatar>

            <div className="flex-1 min-w-0">
                <p className="text-sm font-semibold text-white">
                    {backing.nombreBacker}
                </p>
                <p className="text-xs text-[#94a3b8]">
                    Aporto {formatCurrency(backing.monto)}
                    {backing.rewardNombre && ` · ${backing.rewardNombre}`}
                </p>
                {backing.mensaje && (
                    <p className="text-xs text-[#64748b] italic mt-1 line-clamp-2">
                        &ldquo;{backing.mensaje}&rdquo;
                    </p>
                )}
            </div>

            <span className="text-xs text-[#64748b] whitespace-nowrap flex-shrink-0">
                {formatRelativeTime(backing.fechaCreacion)}
            </span>
        </div>
    )
}
