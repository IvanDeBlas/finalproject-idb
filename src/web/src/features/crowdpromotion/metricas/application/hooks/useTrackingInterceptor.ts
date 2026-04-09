import { useEffect } from "react"
import {
    TRACKING_PARAMS,
    TRACKING_STORAGE_KEYS,
    TRACKING_COOKIE_KEY,
    TRACKING_COOKIE_TTL_MINUTES,
    TIPO_EVENTO_PROMO,
} from "@shared/constants"
import { trackingService } from "../../infrastructure/tracking.service"
import type { RegistrarEventoRequest } from "../../domain"

function clearTrackingStorage(): void {
    sessionStorage.removeItem(TRACKING_STORAGE_KEYS.ref)
    sessionStorage.removeItem(TRACKING_STORAGE_KEYS.utmSource)
    sessionStorage.removeItem(TRACKING_STORAGE_KEYS.utmMedium)
    sessionStorage.removeItem(TRACKING_STORAGE_KEYS.utmCampaign)
    sessionStorage.removeItem(TRACKING_STORAGE_KEYS.campaniaId)
}

export function useTrackingInterceptor(): void {
    useEffect(() => {
        const params = new URLSearchParams(window.location.search)
        const ref = params.get(TRACKING_PARAMS.ref)

        if (!ref) return

        sessionStorage.setItem(TRACKING_STORAGE_KEYS.ref, ref)

        const utmSource = params.get(TRACKING_PARAMS.utmSource) || undefined
        const utmMedium = params.get(TRACKING_PARAMS.utmMedium) || undefined
        const utmCampaign = params.get(TRACKING_PARAMS.utmCampaign) || undefined

        if (utmSource) sessionStorage.setItem(TRACKING_STORAGE_KEYS.utmSource, utmSource)
        if (utmMedium) sessionStorage.setItem(TRACKING_STORAGE_KEYS.utmMedium, utmMedium)
        if (utmCampaign) sessionStorage.setItem(TRACKING_STORAGE_KEYS.utmCampaign, utmCampaign)

        const maxAge = TRACKING_COOKIE_TTL_MINUTES * 60
        document.cookie = `${TRACKING_COOKIE_KEY}=${ref}; max-age=${maxAge}; path=/; SameSite=Lax`

        const request: RegistrarEventoRequest = {
            tipoEventoPromoId: TIPO_EVENTO_PROMO.CLICK,
            codigoReferido: ref,
            urlOrigen: window.location.href,
            urlReferer: document.referrer || undefined,
            utmSource,
            utmMedium,
            utmCampaign,
        }

        trackingService.registrarEvento(request).catch((err: Error & { errorCode?: string }) => {
            if (err.errorCode?.startsWith("1")) {
                clearTrackingStorage()
            }
        })
    }, [])
}
