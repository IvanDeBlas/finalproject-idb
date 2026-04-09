import type { Metadata } from "next"
import { PromotorPageClient } from "./components/PromotorPageClient"

export const metadata: Metadata = {
    title: "Mi Perfil Promotor | WePlay Rises",
    description: "Gestiona tu perfil de promotor en WePlay Rises",
}

export default function PromotorPage() {
    return <PromotorPageClient />
}
