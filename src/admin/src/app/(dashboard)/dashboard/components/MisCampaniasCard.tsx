"use client"

import Link from "next/link"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import { Music } from "lucide-react"
import { CampaniaProgressItem } from "./CampaniaProgressItem"
import { EmptyStatePlaceholder } from "@/components/dashboard/EmptyStatePlaceholder"
import type { MiCampaniaListItem } from "@shared/types"

interface MisCampaniasCardProps {
    campanias?: MiCampaniaListItem[]
    isLoading?: boolean
}

export function MisCampaniasCard({ campanias, isLoading }: MisCampaniasCardProps) {
    return (
        <Card>
            <CardHeader className="flex flex-row items-center justify-between">
                <CardTitle>Mis Campanias</CardTitle>
                <Link href="/campanias">
                    <Button variant="ghost" size="sm">
                        Ver todas
                    </Button>
                </Link>
            </CardHeader>
            <CardContent>
                {isLoading ? (
                    <div className="space-y-4">
                        {[1, 2, 3].map((i) => (
                            <Skeleton key={i} className="h-20 rounded-lg" />
                        ))}
                    </div>
                ) : campanias && campanias.length > 0 ? (
                    <div className="space-y-3">
                        {campanias.slice(0, 5).map((c) => (
                            <CampaniaProgressItem key={c.id} campania={c} />
                        ))}
                    </div>
                ) : (
                    <EmptyStatePlaceholder
                        icon={Music}
                        title="No tienes campanias aun"
                        description="Crea tu primera campania y empieza a recaudar fondos"
                        action={{
                            label: "Crear campania",
                            href: "/campanias/nueva",
                        }}
                    />
                )}
            </CardContent>
        </Card>
    )
}
