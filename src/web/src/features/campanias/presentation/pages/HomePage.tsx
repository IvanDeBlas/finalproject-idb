import { Link } from "react-router-dom"
import { Button } from "@/components/ui/button"
import { ROUTES } from "@/lib/constants"
import { useCampanias } from "../../application/useCampanias"
import { CampaniaList } from "../components/CampaniaList"
import { Music, Users, TrendingUp } from "lucide-react"

export default function HomePage() {
    const { data: campanias, isLoading } = useCampanias()

    const campaniasFeatured = campanias?.slice(0, 6) ?? []

    return (
        <div>
            {/* Hero Section */}
            <section className="relative overflow-hidden bg-gradient-to-b from-background to-muted py-24">
                <div className="container">
                    <div className="mx-auto max-w-3xl text-center">
                        <h1 className="text-4xl font-bold tracking-tight sm:text-6xl">
                            Impulsa la musica que amas
                        </h1>
                        <p className="mt-6 text-lg text-muted-foreground">
                            WePlay Rises conecta artistas musicales con fans que quieren
                            apoyar sus proyectos creativos. Financia el proximo gran album,
                            gira o videoclip.
                        </p>
                        <div className="mt-10 flex items-center justify-center gap-4">
                            <Link to={ROUTES.EXPLORAR}>
                                <Button size="lg">Explorar campanias</Button>
                            </Link>
                            <Link to={ROUTES.REGISTER}>
                                <Button variant="outline" size="lg">
                                    Soy artista
                                </Button>
                            </Link>
                        </div>
                    </div>
                </div>
            </section>

            {/* Stats Section */}
            <section className="border-y bg-background py-12">
                <div className="container">
                    <div className="grid gap-8 sm:grid-cols-3">
                        <div className="text-center">
                            <div className="flex justify-center mb-2">
                                <Music className="h-8 w-8 text-primary" />
                            </div>
                            <div className="text-3xl font-bold">100+</div>
                            <div className="text-sm text-muted-foreground">
                                Proyectos financiados
                            </div>
                        </div>
                        <div className="text-center">
                            <div className="flex justify-center mb-2">
                                <Users className="h-8 w-8 text-primary" />
                            </div>
                            <div className="text-3xl font-bold">5,000+</div>
                            <div className="text-sm text-muted-foreground">
                                Fans participantes
                            </div>
                        </div>
                        <div className="text-center">
                            <div className="flex justify-center mb-2">
                                <TrendingUp className="h-8 w-8 text-primary" />
                            </div>
                            <div className="text-3xl font-bold">500K EUR</div>
                            <div className="text-sm text-muted-foreground">
                                Total recaudado
                            </div>
                        </div>
                    </div>
                </div>
            </section>

            {/* Featured Campaigns */}
            <section className="py-16">
                <div className="container">
                    <div className="mb-8 flex items-center justify-between">
                        <div>
                            <h2 className="text-2xl font-bold">Campanias destacadas</h2>
                            <p className="text-muted-foreground">
                                Descubre proyectos musicales que necesitan tu apoyo
                            </p>
                        </div>
                        <Link to={ROUTES.EXPLORAR}>
                            <Button variant="ghost">Ver todas</Button>
                        </Link>
                    </div>

                    <CampaniaList campanias={campaniasFeatured} isLoading={isLoading} />
                </div>
            </section>
        </div>
    )
}
