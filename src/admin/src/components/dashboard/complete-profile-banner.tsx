"use client"

import { useState, useEffect } from "react"
import Link from "next/link"
import { Card, CardContent } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Music, X } from "lucide-react"
import { useMyArtistProfile } from "@/hooks/use-artista"

const STORAGE_KEY = "profile-banner-dismissed"

export function CompleteProfileBanner() {
    const { data: artista, isLoading } = useMyArtistProfile()
    const [dismissed, setDismissed] = useState(false)

    useEffect(() => {
        if (typeof window !== "undefined") {
            const isDismissed = localStorage.getItem(STORAGE_KEY) === "true"
            setDismissed(isDismissed)
        }
    }, [])

    const handleDismiss = () => {
        localStorage.setItem(STORAGE_KEY, "true")
        setDismissed(true)
    }

    if (isLoading || artista || dismissed) {
        return null
    }

    return (
        <Card className="bg-gradient-to-r from-pink-500/10 to-purple-600/10 border-pink-500/50">
            <CardContent className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4 pt-6">
                <div className="flex items-start gap-3">
                    <Music className="h-5 w-5 text-pink-500 mt-0.5 shrink-0" />
                    <div>
                        <h3 className="font-semibold text-white">
                            Completa tu perfil de artista
                        </h3>
                        <p className="text-sm text-muted-foreground mt-1">
                            Para crear campanias de crowdfunding, necesitas completar tu perfil artistico.
                        </p>
                    </div>
                </div>
                <div className="flex items-center gap-3 shrink-0">
                    <Link href="/artista/perfil/crear">
                        <Button
                            size="sm"
                            className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"
                        >
                            Completar perfil
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
