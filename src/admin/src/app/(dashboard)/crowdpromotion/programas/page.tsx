import type { Metadata } from "next"
import { PromoProgramaListClient } from "../components/list/PromoProgramaListClient"

export const metadata: Metadata = {
    title: "Mis Programas de Promocion | WePlay Rises",
    description: "Gestiona tus programas de promocion y sus promotores",
}

export default function MisProgramasPage() {
    return <PromoProgramaListClient />
}
