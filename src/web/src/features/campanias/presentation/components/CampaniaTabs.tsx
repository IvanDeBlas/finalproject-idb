import { Tabs, TabsList, TabsTrigger, TabsContent } from "@/components/ui/tabs"
import { cn } from "@/lib/utils"
import { CampaniaDescription } from "./CampaniaDescription"

interface CampaniaTabsProps {
    descripcion: string
    defaultTab?: string
    className?: string
}

const tabTriggerClass =
    "rounded-none border-b-2 border-transparent data-[state=active]:border-primary data-[state=active]:text-primary text-[#94a3b8] px-4 py-3 bg-transparent data-[state=active]:bg-transparent data-[state=active]:shadow-none"

export function CampaniaTabs({
    descripcion,
    defaultTab = "historia",
    className,
}: CampaniaTabsProps) {
    return (
        <Tabs defaultValue={defaultTab} className={cn("", className)}>
            <TabsList
                className="bg-transparent border-b border-[#334155] rounded-none w-full justify-start h-auto p-0"
                aria-label="Secciones de la campana"
            >
                <TabsTrigger value="historia" className={tabTriggerClass}>
                    Historia
                </TabsTrigger>
                <TabsTrigger value="actualizaciones" className={tabTriggerClass}>
                    Actualizaciones
                </TabsTrigger>
                <TabsTrigger value="comentarios" className={tabTriggerClass}>
                    Comentarios
                </TabsTrigger>
                <TabsTrigger value="faq" className={tabTriggerClass}>
                    FAQ
                </TabsTrigger>
            </TabsList>

            <TabsContent value="historia" className="py-6">
                <CampaniaDescription htmlContent={descripcion} />
            </TabsContent>

            <TabsContent value="actualizaciones" className="py-6">
                <div className="text-center py-12">
                    <p className="text-[#64748b] text-lg">Proximamente</p>
                    <p className="text-[#64748b] text-sm mt-2">
                        Las actualizaciones del artista apareceran aqui
                    </p>
                </div>
            </TabsContent>

            <TabsContent value="comentarios" className="py-6">
                <div className="text-center py-12">
                    <p className="text-[#64748b] text-lg">Proximamente</p>
                    <p className="text-[#64748b] text-sm mt-2">
                        Los comentarios de los backers apareceran aqui
                    </p>
                </div>
            </TabsContent>

            <TabsContent value="faq" className="py-6">
                <div className="text-center py-12">
                    <p className="text-[#64748b] text-lg">Proximamente</p>
                    <p className="text-[#64748b] text-sm mt-2">
                        Las preguntas frecuentes apareceran aqui
                    </p>
                </div>
            </TabsContent>
        </Tabs>
    )
}
