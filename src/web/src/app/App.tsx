import { Toaster } from "sonner"
import { Providers } from "./providers"
import { AppRouter } from "./router"
import { useTrackingInterceptor } from "@/features/crowdpromotion/metricas/application/hooks/useTrackingInterceptor"

function AppWithTracking() {
  useTrackingInterceptor()
  return (
    <>
      <AppRouter />
      <Toaster position="top-right" theme="dark" />
    </>
  )
}

export function App() {
  return (
    <Providers>
      <AppWithTracking />
    </Providers>
  )
}
