import type { BackingPublicDto } from "@shared/types/backing"
import { Skeleton } from "@/components/ui/skeleton"
import { Heart } from "lucide-react"
import { BackingRecenteCard } from "./BackingRecenteCard"
import { cn } from "@/lib/utils"

interface BackingsRecentesListProps {
    backings: BackingPublicDto[]
    isLoading?: boolean
    emptyMessage?: string
    className?: string
}

export function BackingsRecentesList({
    backings,
    isLoading = false,
    emptyMessage = "Se el primero en apoyar esta campana",
    className,
}: BackingsRecentesListProps) {
    return (
        <div className={cn("space-y-3", className)}>
            <h3 className="text-lg font-bold text-white mb-4">
                Apoyos Recientes
            </h3>

            {isLoading ? (
                <div className="space-y-3">
                    {Array.from({ length: 3 }).map((_, i) => (
                        <div key={i} className="flex items-start gap-3 p-3">
                            <Skeleton className="w-10 h-10 rounded-full bg-[#1e293b]" />
                            <div className="flex-1 space-y-2">
                                <Skeleton className="h-4 w-24 bg-[#1e293b]" />
                                <Skeleton className="h-3 w-32 bg-[#1e293b]" />
                            </div>
                        </div>
                    ))}
                </div>
            ) : backings.length === 0 ? (
                <div className="text-center py-6">
                    <Heart className="w-8 h-8 text-[#64748b] mx-auto mb-2" />
                    <p className="text-sm text-[#64748b]">{emptyMessage}</p>
                </div>
            ) : (
                backings.map((backing) => (
                    <BackingRecenteCard key={backing.id} backing={backing} />
                ))
            )}
        </div>
    )
}
