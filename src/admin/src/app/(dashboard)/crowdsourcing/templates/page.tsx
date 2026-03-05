"use client"

import { useState, useMemo } from "react"
import { useRouter } from "next/navigation"
import { Button } from "@/components/ui/button"
import { Plus } from "lucide-react"
import { useTemplates } from "@/hooks/use-templates"
import { useToggleTemplateStatus } from "@/hooks/use-templates-mutations"
import { TemplateTable } from "./components/list/TemplateTable"
import { TemplateFilters } from "./components/list/TemplateFilters"
import { DeleteConfirmDialog } from "./components/shared/DeleteConfirmDialog"
import { APP_ROUTES } from "@shared/constants"

export default function TemplatesPage() {
    const router = useRouter()
    const { data: templates, isLoading } = useTemplates()
    const toggleStatus = useToggleTemplateStatus()

    const [searchQuery, setSearchQuery] = useState("")
    const [statusFilter, setStatusFilter] = useState("all")
    const [toggleDialogId, setToggleDialogId] = useState<string | null>(null)

    const filteredTemplates = useMemo(() => {
        if (!templates) return []

        return templates.filter((t) => {
            const matchesSearch =
                !searchQuery ||
                t.nombre.toLowerCase().includes(searchQuery.toLowerCase())

            const matchesStatus =
                statusFilter === "all" ||
                (statusFilter === "active" && t.orden >= 0) ||
                (statusFilter === "inactive" && t.orden < 0)

            return matchesSearch && matchesStatus
        })
    }, [templates, searchQuery, statusFilter])

    const handleToggleConfirm = () => {
        if (toggleDialogId) {
            toggleStatus.mutate(toggleDialogId, {
                onSettled: () => setToggleDialogId(null),
            })
        }
    }

    return (
        <div className="space-y-6">
            <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
                <div>
                    <h1 className="text-3xl font-bold">Templates de Proyecto</h1>
                    <p className="text-muted-foreground mt-1">
                        Gestiona las plantillas para artistas noveles
                    </p>
                </div>
                <Button
                    onClick={() =>
                        router.push(APP_ROUTES.dashboard.crowdsourcing.templates + "/nuevo")
                    }
                >
                    <Plus className="h-4 w-4 mr-2" />
                    Nuevo Template
                </Button>
            </div>

            <TemplateFilters
                searchQuery={searchQuery}
                onSearchChange={setSearchQuery}
                statusFilter={statusFilter}
                onStatusChange={setStatusFilter}
            />

            <TemplateTable
                templates={filteredTemplates}
                isLoading={isLoading}
                onEdit={(id) =>
                    router.push(
                        APP_ROUTES.dashboard.crowdsourcing.templateDetail(id) + "/editar"
                    )
                }
                onView={(id) =>
                    router.push(APP_ROUTES.dashboard.crowdsourcing.templateDetail(id))
                }
                onToggleStatus={(id) => setToggleDialogId(id)}
            />

            <DeleteConfirmDialog
                open={!!toggleDialogId}
                onOpenChange={(open) => {
                    if (!open) setToggleDialogId(null)
                }}
                onConfirm={handleToggleConfirm}
                title="Cambiar estado del template"
                description="El template se activara o desactivara segun su estado actual. Los artistas solo veran templates activos."
                isPending={toggleStatus.isPending}
            />
        </div>
    )
}
