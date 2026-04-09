import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { getInscripcionErrorMessage } from "@shared/utils/error-messages"
import { inscripcionService } from "../../infrastructure/inscripcion.service"

export function useSolicitarInscripcion() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (programaId: string) => inscripcionService.solicitarInscripcion(programaId),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: ["crowdpromotion", "programas", "explorar"],
            })
            queryClient.invalidateQueries({
                queryKey: ["crowdpromotion", "inscripciones", "mis-programas"],
            })
            toast.success("Solicitud enviada. El artista revisara tu perfil.")
        },
        onError: (error: Error & { errorCode?: string }) => {
            const errorCode = error.errorCode ?? "5000"
            if (errorCode === "4021") {
                toast.info("Ya tienes una solicitud activa para este programa.")
                return
            }
            if (errorCode === "4022") {
                toast.error("No puedes inscribirte en este programa.")
                return
            }
            toast.error(getInscripcionErrorMessage(errorCode))
        },
    })
}
