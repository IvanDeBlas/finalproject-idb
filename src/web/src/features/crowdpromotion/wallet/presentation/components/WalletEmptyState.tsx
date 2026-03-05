import type { FC } from "react"
import { Link } from "react-router-dom"
import { Button } from "@/components/ui/button"
import { Wallet } from "lucide-react"

export const WalletEmptyState: FC = () => {
    return (
        <div className="flex flex-col items-center justify-center py-16 gap-4">
            <div className="flex items-center justify-center h-16 w-16 rounded-full bg-[#151525] border border-[#334155]">
                <Wallet className="h-8 w-8 text-[#94a3b8]" />
            </div>
            <h3 className="text-lg font-semibold text-white">Aun no tienes transacciones</h3>
            <p className="text-sm text-[#94a3b8] text-center max-w-md">
                Cuando completes tareas de promocion y generes comisiones, aqui podras ver tu historial de
                transacciones y solicitar cobros.
            </p>
            <Button asChild className="bg-[#a855f7] hover:bg-[#9333ea] text-white mt-2">
                <Link to="/crowdpromotion/explorar">Explorar programas</Link>
            </Button>
        </div>
    )
}
