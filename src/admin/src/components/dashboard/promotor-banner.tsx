"use client"

import { useState, useEffect } from "react"
import Link from "next/link"
import { Card, CardContent } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Megaphone, X } from "lucide-react"
import { usePromotor } from "@/hooks/use-promotor"

const STORAGE_KEY = "promotor-banner-dismissed"

export function PromotorBanner() {
    const { data: promotor, isLoading } = usePromotor()
    const [dismissed, setDismissed] = useState(false)
    const [mounted, setMounted] = useState(false)

    useEffect(() => {
        setMounted(true)
        if (typeof window !== "undefined") {
            const isDismissed = localStorage.getItem(STORAGE_KEY) === "true"
            setDismissed(isDismissed)
        }
    }, [])

    const handleDismiss = () => {
        localStorage.setItem(STORAGE_KEY, "true")
        setDismissed(true)
    }

    if (!mounted || isLoading || dismissed) {
        return null
    }

    if (promotor?.esActivo === true) {
        return null
    }

    if (promotor && !promotor.esActivo) {
        return (
            <Card className="border-amber-500/50 bg-amber-950/10">
                <CardContent className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 pt-6">
                    <div className="flex items-start gap-3">
                        <Megaphone className="h-5 w-5 text-amber-400 mt-0.5 shrink-0" />
                        <div>
                            <h3 className="font-semibold">Tu perfil de promotor esta inactivo</h3>
                            <p className="text-sm text-muted-foreground mt-1">
                                Tu perfil ha sido desactivado. Contacta soporte si deseas reactivarlo.
                            </p>
                        </div>
                    </div>
                    <Button
                        size="sm"
                        variant="ghost"
                        onClick={handleDismiss}
                        className="text-muted-foreground hover:text-white shrink-0"
                    >
                        <X className="h-4 w-4" />
                    </Button>
                </CardContent>
            </Card>
        )
    }

    return (
        <Card className="bg-gradient-to-r from-purple-500/10 to-pink-600/10 border-purple-500/50">
            <CardContent className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 pt-6">
                <div className="flex items-start gap-3">
                    <Megaphone className="h-5 w-5 text-purple-400 mt-0.5 shrink-0" />
                    <div>
                        <h3 className="font-semibold">Conviertete en promotor</h3>
                        <p className="text-sm text-muted-foreground mt-1">
                            Promueve artistas y gana comisiones por cada campania exitosa.
                        </p>
                    </div>
                </div>
                <div className="flex items-center gap-3 shrink-0">
                    <Link href="/promotor">
                        <Button
                            size="sm"
                            className="bg-gradient-to-r from-purple-500 to-pink-600 hover:from-purple-600 hover:to-pink-700"
                        >
                            Saber mas
                        </Button>
                    </Link>
                    <Button
                        size="sm"
                        variant="ghost"
                        onClick={handleDismiss}
                        className="text-muted-foreground hover:text-white"
                    >
                        <X className="h-4 w-4" />
                    </Button>
                </div>
            </CardContent>
        </Card>
    )
}
