"use client"

import { DollarSign, Users, TrendingUp } from "lucide-react"
import { Card, CardHeader, CardTitle, CardContent, Skeleton } from "@/components/ui"
import { formatCurrency } from "@shared/utils"
import type { CampaniaStats } from "@shared/types"

interface BackingStatsGridProps {
    stats: CampaniaStats | undefined
    isLoading: boolean
}

export function BackingStatsGrid({ stats, isLoading }: BackingStatsGridProps) {
    if (isLoading) {
        return (
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                {Array.from({ length: 3 }).map((_, i) => (
                    <Card key={i}>
                        <CardHeader className="flex flex-row items-center justify-between pb-2">
                            <Skeleton className="h-4 w-24" />
                            <Skeleton className="h-4 w-4" />
                        </CardHeader>
                        <CardContent>
                            <Skeleton className="h-8 w-20" />
                        </CardContent>
                    </Card>
                ))}
            </div>
        )
    }

    const totalRecaudado = stats?.totalRecaudado ?? 0
    const totalBackers = stats?.totalBackers ?? 0
    const promedioAporte = stats?.promedioAporte ?? 0

    return (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <Card>
                <CardHeader className="flex flex-row items-center justify-between pb-2">
                    <CardTitle className="text-sm font-medium text-muted-foreground">
                        Total Recaudado
                    </CardTitle>
                    <DollarSign className="h-4 w-4 text-muted-foreground" />
                </CardHeader>
                <CardContent>
                    <div className="text-2xl font-bold text-foreground">
                        {formatCurrency(totalRecaudado)}
                    </div>
                </CardContent>
            </Card>

            <Card>
                <CardHeader className="flex flex-row items-center justify-between pb-2">
                    <CardTitle className="text-sm font-medium text-muted-foreground">
                        Total Apoyos
                    </CardTitle>
                    <Users className="h-4 w-4 text-muted-foreground" />
                </CardHeader>
                <CardContent>
                    <div className="text-2xl font-bold text-foreground">
                        {totalBackers}
                    </div>
                </CardContent>
            </Card>

            <Card>
                <CardHeader className="flex flex-row items-center justify-between pb-2">
                    <CardTitle className="text-sm font-medium text-muted-foreground">
                        Promedio por Aporte
                    </CardTitle>
                    <TrendingUp className="h-4 w-4 text-muted-foreground" />
                </CardHeader>
                <CardContent>
                    <div className="text-2xl font-bold text-foreground">
                        {formatCurrency(promedioAporte)}
                    </div>
                </CardContent>
            </Card>
        </div>
    )
}
