import { FC } from "react"
import { Button } from "@/components/ui/button"
import { Link } from "react-router-dom"

interface PerfilProfesionalCTAProps {
    className?: string
}

export const PerfilProfesionalCTA: FC<PerfilProfesionalCTAProps> = ({
    className = "",
}) => {
    return (
        <div
            className={`bg-[#1e2a42] border border-[#334155] rounded-lg p-4 ${className}`}
        >
            <p className="text-sm text-[#94a3b8] mb-3">
                Necesitas un perfil profesional para enviar propuestas
            </p>
            <Link to="/perfil-profesional/crear">
                <Button
                    variant="outline"
                    className="w-full border-purple-500 text-purple-300 hover:bg-purple-900/20"
                >
                    Crear perfil profesional
                </Button>
            </Link>
        </div>
    )
}
