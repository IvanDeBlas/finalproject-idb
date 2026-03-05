"use client"

import { Package, Lightbulb } from "lucide-react"
import {
    Card,
    CardContent,
    CardHeader,
    CardTitle,
    Button,
} from "@/components/ui"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { EmptyState } from "../shared/EmptyState"

interface RewardsStepProps {
    campaniaId?: string
    onNext: () => void
    onBack: () => void
}

export function RewardsStep({ onNext, onBack }: RewardsStepProps) {
    return (
        <Card className="bg-card border-border">
            <CardHeader>
                <CardTitle className="text-2xl font-bold">
                    Recompensas para tus Backers
                </CardTitle>
                <p className="text-sm text-muted-foreground">
                    Crea niveles de apoyo con beneficios atractivos
                </p>
            </CardHeader>
            <CardContent className="space-y-6">
                {/* Empty State - MVP placeholder */}
                <EmptyState
                    icon={<Package className="w-16 h-16" />}
                    title="Aun no has creado recompensas"
                    description="Las recompensas incentivan a tus fans a apoyarte. Podras crear recompensas despues de crear tu campania."
                />

                {/* Info alert */}
                <Alert className="bg-primary/5 border-l-4 border-primary">
                    <Lightbulb className="h-4 w-4" />
                    <AlertDescription className="text-sm">
                        <strong>Consejo:</strong> Crea 3-5 niveles de
                        recompensas con beneficios progresivos. Por ejemplo:
                        Descarga digital (&euro;5), CD firmado (&euro;25),
                        Concierto privado (&euro;100).
                    </AlertDescription>
                </Alert>

                {/* MVP notice */}
                <div className="bg-yellow-500/10 border border-yellow-500/30 rounded-lg p-4 text-center">
                    <p className="text-sm text-yellow-400">
                        La gestion de recompensas estara disponible tras crear
                        tu campania. Puedes continuar sin recompensas.
                    </p>
                </div>

                {/* Footer Navigation */}
                <div className="flex items-center justify-between pt-6 border-t border-border">
                    <Button
                        type="button"
                        variant="outline"
                        onClick={onBack}
                        className="border-border text-muted-foreground hover:text-foreground hover:bg-secondary"
                    >
                        &larr; Anterior
                    </Button>

                    <span className="text-sm text-muted-foreground">
                        Paso 4 de 5
                    </span>

                    <Button
                        type="button"
                        onClick={onNext}
                        className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8"
                    >
                        Siguiente &rarr;
                    </Button>
                </div>
            </CardContent>
        </Card>
    )
}
