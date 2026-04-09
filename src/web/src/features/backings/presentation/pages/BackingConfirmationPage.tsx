import { useParams, useSearchParams, useNavigate, Link } from "react-router-dom"
import { useCampania } from "@/features/campanias/application/useCampanias"
import { useAuthStore } from "@/store/auth-store"
import { Card } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import { HeartHandshake, CheckCircle, Mail } from "lucide-react"
import { formatDate } from "@/features/campanias/application/utils"

export default function BackingConfirmationPage() {
    const { id } = useParams<{ id: string }>()
    const [searchParams] = useSearchParams()
    const navigate = useNavigate()
    const { user } = useAuthStore()
    const backingId = searchParams.get("backingId") || ""

    const { data: campania, isLoading } = useCampania(id || "")

    if (isLoading) {
        return (
            <div className="min-h-screen bg-[#0a0a0f] flex items-center justify-center py-8 px-4">
                <Card className="max-w-2xl w-full bg-[#0f1729] border-[#334155] p-8">
                    <div className="space-y-4">
                        <Skeleton className="w-24 h-24 rounded-full mx-auto bg-[#1e293b]" />
                        <Skeleton className="h-8 w-64 mx-auto bg-[#1e293b]" />
                        <Skeleton className="h-4 w-48 mx-auto bg-[#1e293b]" />
                        <Skeleton className="h-48 w-full bg-[#1e293b]" />
                    </div>
                </Card>
            </div>
        )
    }

    return (
        <div className="min-h-screen bg-[#0a0a0f] flex items-center justify-center py-8 px-4">
            <Card className="max-w-2xl mx-auto bg-[#0f1729] border-[#334155] p-8 text-center">
                {/* Success Icon */}
                <div className="w-24 h-24 mx-auto mb-6 rounded-full bg-green-500/20 flex items-center justify-center">
                    <HeartHandshake className="w-12 h-12 text-green-400" />
                </div>

                {/* Title */}
                <h1 className="text-3xl font-bold text-white mb-4">
                    Gracias por tu apoyo!
                </h1>
                <p className="text-lg text-[#cbd5e1] mb-2">
                    Tu contribucion ha sido confirmada con exito.
                </p>
                {campania && (
                    <p className="text-[#94a3b8] mb-8">
                        Has ayudado a alcanzar el objetivo de &ldquo;{campania.titulo}&rdquo;.
                    </p>
                )}

                {/* Summary Card */}
                <Card className="bg-[#1a1a2e] border-[#334155] p-6 mb-6 text-left">
                    <h3 className="text-xl font-bold text-white mb-4 text-center">
                        Resumen de tu Apoyo
                    </h3>

                    <div className="space-y-2">
                        {campania && (
                            <div className="flex justify-between py-2 border-b border-[#334155]">
                                <span className="text-sm text-[#64748b]">Campana:</span>
                                <span className="text-sm text-white font-semibold">
                                    {campania.titulo}
                                </span>
                            </div>
                        )}

                        <div className="flex justify-between py-2 border-b border-[#334155]">
                            <span className="text-sm text-[#64748b]">Fecha:</span>
                            <span className="text-sm text-white font-semibold">
                                {formatDate(new Date().toISOString())}
                            </span>
                        </div>

                        {backingId && (
                            <div className="flex justify-between py-2">
                                <span className="text-sm text-[#64748b]">ID Backing:</span>
                                <span className="text-sm text-white font-mono text-xs">
                                    {backingId.substring(0, 13)}
                                </span>
                            </div>
                        )}
                    </div>

                    {user?.email && (
                        <div className="flex items-start gap-2 bg-[#1e293b] p-3 rounded-lg mt-4">
                            <Mail className="w-5 h-5 text-primary mt-0.5 flex-shrink-0" />
                            <p className="text-xs text-[#cbd5e1]">
                                Te hemos enviado un email de confirmacion a{" "}
                                <strong>{user.email}</strong>
                            </p>
                        </div>
                    )}
                </Card>

                {/* Next Steps */}
                <div className="text-left mb-8">
                    <h4 className="text-lg font-bold text-white mb-3">Que sigue?</h4>
                    <ul className="space-y-2 text-sm text-[#94a3b8]">
                        <li className="flex items-start gap-2">
                            <CheckCircle className="w-4 h-4 text-primary mt-0.5 flex-shrink-0" />
                            <span>El artista procesara tu recompensa</span>
                        </li>
                        <li className="flex items-start gap-2">
                            <CheckCircle className="w-4 h-4 text-primary mt-0.5 flex-shrink-0" />
                            <span>Te notificaremos sobre actualizaciones de la campana</span>
                        </li>
                    </ul>
                </div>

                {/* Actions */}
                <div className="flex gap-3 justify-center flex-wrap">
                    <Button
                        variant="outline"
                        className="border-primary text-primary hover:bg-primary/10 px-8"
                        asChild
                    >
                        <Link to={`/campanias/${id}`}>Ver Campana</Link>
                    </Button>
                    <Button
                        className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8"
                        onClick={() => navigate("/campanias")}
                    >
                        Explorar Mas Campanas
                    </Button>
                </div>
            </Card>
        </div>
    )
}
