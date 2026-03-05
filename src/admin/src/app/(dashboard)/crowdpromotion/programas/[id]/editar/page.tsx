import type { Metadata } from "next"
import { PromoProgramaWizardContainer } from "../../../components/wizard/PromoProgramaWizardContainer"

export const metadata: Metadata = {
    title: "Editar Programa de Promocion | WePlay Rises",
    description: "Edita tu programa de promocion",
}

export default async function EditarProgramaPage({
    params,
}: {
    params: Promise<{ id: string }>
}) {
    const { id } = await params
    return <PromoProgramaWizardContainer mode="edit" programaId={id} />
}
