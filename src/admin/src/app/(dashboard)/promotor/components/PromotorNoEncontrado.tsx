"use client"

import { Card, CardContent } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Megaphone } from "lucide-react"

const LANDING_URL = process.env.NEXT_PUBLIC_LANDING_URL || "http://localhost:3000"

export function PromotorNoEncontrado() {
    return (
        <div className="space-y-6">
            <div>
                <h1 className="text-3xl font-bold">Mi Perfil Promotor</h1>
                <p className="text-muted-foreground">
                    Gestiona tu perfil y visibilidad como promotor
                </p>
            </div>

            <Card className="max-w-lg mx-auto">
                <CardContent className="flex flex-col items-center text-center py-12 space-y-4">
                    <div className="rounded-full bg-muted p-4">
                        <Megaphone className="h-8 w-8 text-muted-foreground" aria-hidden="true" />
                    </div>
                    <h2 className="text-xl font-semibold">
                        No tienes un perfil de promotor
                    </h2>
                    <p className="text-sm text-muted-foreground max-w-sm">
                        Para empezar a promover artistas y ganar comisiones, primero debes
                        registrarte como promotor desde la landing.
                    </p>
                    <a href={`${LANDING_URL}/promotor/registro`} target="_blank" rel="noopener noreferrer">
                        <Button className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700">
                            <Megaphone className="mr-2 h-4 w-4" aria-hidden="true" />
                            Registrarme como promotor
                        </Button>
                    </a>
                </CardContent>
            </Card>
        </div>
    )
}
