"use client"

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { Badge } from "@/components/ui/badge"
import { formatCurrency, formatRelativeDate } from "@shared/utils"
import { Users } from "lucide-react"
import { useRecentBackings } from "@/hooks/use-recent-backings"

interface RecentBackingsProps {
    campaniaId?: string
}

export function RecentBackings({ campaniaId }: RecentBackingsProps) {
    const { data: backings, isLoading } = useRecentBackings(campaniaId)

    return (
        <Card>
            <CardHeader>
                <CardTitle>Ultimos Backings</CardTitle>
            </CardHeader>
            <CardContent>
                {isLoading ? (
                    <div className="space-y-4">
                        {[1, 2, 3].map((i) => (
                            <div key={i} className="flex items-center gap-3 p-3">
                                <Skeleton className="h-10 w-10 rounded-full" />
                                <div className="flex-1 space-y-2">
                                    <Skeleton className="h-4 w-24" />
                                    <Skeleton className="h-3 w-16" />
                                </div>
                                <Skeleton className="h-4 w-16" />
                            </div>
                        ))}
                    </div>
                ) : backings && backings.length > 0 ? (
                    <div className="space-y-4">
                        {backings.map((backing) => (
                            <div
                                key={backing.id}
                                className="flex items-center gap-3 rounded-lg border border-border p-3 hover:bg-card transition-colors"
                            >
                                <Avatar className="h-10 w-10">
                                    <AvatarFallback className="bg-primary/20 text-primary text-sm">
                                        {backing.nombreBacker.charAt(0).toUpperCase()}
                                    </AvatarFallback>
                                </Avatar>

                                <div className="flex-1 min-w-0">
                                    <div className="flex items-center gap-2">
                                        <p className="text-sm font-medium truncate">
                                            {backing.esAnonimo ? (
                                                <span className="text-muted-foreground italic">Anonimo</span>
                                            ) : (
                                                backing.nombreBacker
                                            )}
                                        </p>
                                        {backing.esAnonimo && (
                                            <Badge variant="secondary" className="text-xs">
                                                Anonimo
                                            </Badge>
                                        )}
                                    </div>
                                    <p className="text-xs text-muted-foreground">
                                        {backing.rewardNombre ?? "Sin recompensa"}
                                    </p>
                                </div>

                                <div className="text-right">
                                    <p className="text-sm font-medium">
                                        {formatCurrency(backing.monto)}
                                    </p>
                                    <p className="text-xs text-muted-foreground">
                                        {formatRelativeDate(backing.fechaCreacion)}
                                    </p>
                                </div>
                            </div>
                        ))}
                    </div>
                ) : (
                    <div className="text-center py-8">
                        <Users className="h-12 w-12 text-muted-foreground mx-auto mb-4" />
                        <p className="text-muted-foreground">
                            No hay backings recientes
                        </p>
                    </div>
                )}
            </CardContent>
        </Card>
    )
}
