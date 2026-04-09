import { Routes, Route } from "react-router-dom"
import { lazy, Suspense } from "react"
import { ROUTES } from "@/lib/constants"

// Layouts
import { PublicLayout } from "@/components/layout/PublicLayout"
import { DashboardLayout } from "@/components/layout/DashboardLayout"

// Lazy load pages
const HomePage = lazy(() => import("@/features/campanias/presentation/pages/HomePage"))
const CampaniasPage = lazy(() => import("@/features/campanias/presentation/pages/CampaniasPage"))
const CampaniaDetailPage = lazy(() => import("@/features/campanias/presentation/pages/CampaniaDetailPage"))
const CampaniaNewPage = lazy(() => import("@/features/campanias/presentation/pages/CampaniaNewPage"))
const ExplorarPage = lazy(() => import("@/features/campanias/presentation/pages/ExplorarPage"))
const LoginPage = lazy(() => import("@/features/auth/presentation/pages/LoginPage"))
const RegisterPage = lazy(() => import("@/features/auth/presentation/pages/RegisterPage"))
const DashboardPage = lazy(() => import("@/features/artistas/presentation/pages/DashboardPage"))
const ArtistaPerfilPage = lazy(() => import("@/features/artistas/presentation/pages/ArtistaPerfilPage"))
const ArtistaPublicProfilePage = lazy(() => import("@/features/artistas/presentation/pages/ArtistaPublicProfilePage"))
const BackingConfirmationPage = lazy(() => import("@/features/backings/presentation/pages/BackingConfirmationPage"))
const NuevoProyectoPage = lazy(() => import("@/features/crowdsourcing/presentation/pages/NuevoProyectoPage"))
const ExplorarNecesidadesPage = lazy(() => import("@/features/crowdsourcing/presentation/pages/ExplorarNecesidadesPage"))
const NecesidadDetallePage = lazy(() => import("@/features/crowdsourcing/presentation/pages/NecesidadDetallePage"))
const MisPropuestasPage = lazy(() => import("@/features/crowdsourcing/presentation/pages/MisPropuestasPage"))
const AcuerdoDetallePage = lazy(() => import("@/features/crowdsourcing/presentation/pages/AcuerdoDetallePage"))
const MensajesPage = lazy(() => import("@/features/crowdsourcing/presentation/pages/MensajesPage"))
const ConversacionChatPage = lazy(() => import("@/features/crowdsourcing/presentation/pages/ConversacionChatPage"))
const PerfilProfesionalPage = lazy(() => import("@/features/crowdsourcing/presentation/pages/PerfilProfesionalPage"))
const PromotorRegistroPage = lazy(() => import("@/features/promotor/presentation/pages/PromotorRegistroPage"))
const PromotorDashboardPage = lazy(() => import("@/features/promotor/presentation/pages/PromotorDashboardPage"))
const PromotorPerfilPage = lazy(() => import("@/features/promotor/presentation/pages/PromotorPerfilPage"))
const ExplorarProgramasPage = lazy(() => import("@/features/crowdpromotion/presentation/pages/ExplorarProgramasPage"))
const MisProgramasPage = lazy(() => import("@/features/crowdpromotion/presentation/pages/MisProgramasPage"))
const MisTareasPage = lazy(() => import("@/features/crowdpromotion/tareas/presentation/pages/MisTareasPage"))
const PromotorMetricasPage = lazy(() => import("@/features/crowdpromotion/metricas/presentation/pages/PromotorMetricasPage"))
const PromotorWalletPage = lazy(() => import("@/features/crowdpromotion/wallet/presentation/pages/PromotorWalletPage"))

function PageLoader() {
    return (
        <div className="flex h-screen items-center justify-center">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary" />
        </div>
    )
}

export function AppRouter() {
    return (
        <Suspense fallback={<PageLoader />}>
            <Routes>
                {/* Public routes */}
                <Route element={<PublicLayout />}>
                    <Route path={ROUTES.HOME} element={<HomePage />} />
                    <Route path={ROUTES.CAMPANIAS} element={<CampaniasPage />} />
                    <Route path={ROUTES.CAMPANIA_DETAIL} element={<CampaniaDetailPage />} />
                    <Route path={ROUTES.BACKING_CONFIRMATION} element={<BackingConfirmationPage />} />
                    <Route path={ROUTES.EXPLORAR} element={<ExplorarPage />} />
                    <Route path="/artistas/:id" element={<ArtistaPublicProfilePage />} />
                    <Route path="/crowdsourcing/nuevo-proyecto" element={<NuevoProyectoPage />} />
                    <Route path={ROUTES.CROWDSOURCING_NECESIDADES} element={<ExplorarNecesidadesPage />} />
                    <Route path={ROUTES.CROWDSOURCING_NECESIDAD_DETAIL} element={<NecesidadDetallePage />} />
                    <Route path={ROUTES.CROWDSOURCING_MIS_PROPUESTAS} element={<MisPropuestasPage />} />
                    <Route path="/crowdsourcing/mensajes" element={<MensajesPage />} />
                    <Route path="/crowdsourcing/mensajes/:id" element={<ConversacionChatPage />} />
                    <Route path="/crowdsourcing/profesionales/:userId" element={<PerfilProfesionalPage />} />
                    <Route path="/crowdpromotion/explorar" element={<ExplorarProgramasPage />} />
                    <Route path={ROUTES.LOGIN} element={<LoginPage />} />
                    <Route path={ROUTES.REGISTER} element={<RegisterPage />} />
                </Route>

                {/* Protected dashboard routes */}
                <Route element={<DashboardLayout />}>
                    <Route path={ROUTES.DASHBOARD} element={<DashboardPage />} />
                    <Route path={ROUTES.CAMPANIA_NEW} element={<CampaniaNewPage />} />
                    <Route path={ROUTES.ARTISTA_PERFIL} element={<ArtistaPerfilPage />} />
                    <Route path={ROUTES.CROWDSOURCING_ACUERDO_DETAIL} element={<AcuerdoDetallePage />} />
                    <Route path="/promotor/registro" element={<PromotorRegistroPage />} />
                    <Route path="/promotor/dashboard" element={<PromotorDashboardPage />} />
                    <Route path="/promotor/perfil" element={<PromotorPerfilPage />} />
                    <Route path="/promotor/mis-programas" element={<MisProgramasPage />} />
                    <Route path="/promotor/programas/:programaId/tareas" element={<MisTareasPage />} />
                    <Route path="/promotor/metricas" element={<PromotorMetricasPage />} />
                    <Route path="/promotor/wallet" element={<PromotorWalletPage />} />
                </Route>
            </Routes>
        </Suspense>
    )
}
