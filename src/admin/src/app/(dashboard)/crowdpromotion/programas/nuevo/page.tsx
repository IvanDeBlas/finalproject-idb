import type { Metadata } from "next"
import { PromoProgramaWizardContainer } from "../../components/wizard/PromoProgramaWizardContainer"

export const metadata: Metadata = {
    title: "Crear Programa de Promocion | WePlay Rises",
    description: "Crea un nuevo programa de promocion para tu campana",
}

export default function NuevoProgramaPage() {
    return <PromoProgramaWizardContainer mode="create" />
}
