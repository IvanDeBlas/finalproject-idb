import type { Metadata } from "next"
import { PromoProgramaDetailClient } from "./components/PromoProgramaDetailClient"

export const metadata: Metadata = {
    title: "Detalle del Programa | WePlay Rises",
    description: "Ver detalle de programa de promocion",
}

export default async function DetalleProgramaPage({
    params,
}: {
    params: Promise<{ id: string }>
}) {
    const { id } = await params
    return <PromoProgramaDetailClient programaId={id} />
}
