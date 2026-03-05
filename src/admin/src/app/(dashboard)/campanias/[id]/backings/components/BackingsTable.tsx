"use client"

import { useMemo } from "react"
import { Search } from "lucide-react"
import {
    Card,
    CardHeader,
    CardContent,
    Input,
    Select,
    SelectTrigger,
    SelectValue,
    SelectContent,
    SelectItem,
    Badge,
    Table,
    TableHeader,
    TableBody,
    TableRow,
    TableHead,
    TableCell,
    Skeleton,
    Avatar,
    AvatarFallback,
    Tooltip,
    TooltipTrigger,
    TooltipContent,
    TooltipProvider,
} from "@/components/ui"
import { formatCurrency, formatDateShort } from "@shared/utils"
import type { BackingPublicDto } from "@shared/types"

interface BackingsTableProps {
    backings: BackingPublicDto[]
    isLoading: boolean
    searchTerm: string
    onSearchChange: (term: string) => void
    selectedReward: string | null
    onRewardFilterChange: (id: string | null) => void
}

export function BackingsTable({
    backings,
    isLoading,
    searchTerm,
    onSearchChange,
    selectedReward,
    onRewardFilterChange,
}: BackingsTableProps) {
    const uniqueRewards = useMemo(() => {
        const names = new Set<string>()
        backings.forEach((b) => {
            if (b.rewardNombre) names.add(b.rewardNombre)
        })
        return Array.from(names).sort()
    }, [backings])

    const filteredBackings = useMemo(() => {
        return backings.filter((backing) => {
            const matchesSearch = backing.nombreBacker
                .toLowerCase()
                .includes(searchTerm.toLowerCase())

            const matchesReward = selectedReward
                ? backing.rewardNombre === selectedReward
                : true

            return matchesSearch && matchesReward
        })
    }, [backings, searchTerm, selectedReward])

    if (isLoading) {
        return (
            <Card>
                <CardHeader>
                    <div className="flex flex-col md:flex-row gap-3">
                        <Skeleton className="h-10 flex-1" />
                        <Skeleton className="h-10 w-[200px]" />
                    </div>
                </CardHeader>
                <CardContent>
                    <div className="space-y-3">
                        {Array.from({ length: 5 }).map((_, i) => (
                            <div key={i} className="flex items-center gap-4">
                                <Skeleton className="h-10 w-10 rounded-full" />
                                <Skeleton className="h-4 flex-1" />
                                <Skeleton className="h-4 w-20" />
                                <Skeleton className="h-4 w-24" />
                            </div>
                        ))}
                    </div>
                </CardContent>
            </Card>
        )
    }

    return (
        <Card>
            <CardHeader>
                <div className="flex flex-col md:flex-row gap-3">
                    <div className="relative flex-1 md:max-w-xs">
                        <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                        <Input
                            type="search"
                            placeholder="Buscar por nombre..."
                            value={searchTerm}
                            onChange={(e) => onSearchChange(e.target.value)}
                            className="pl-9"
                            aria-label="Buscar backings por nombre de backer"
                        />
                    </div>

                    {uniqueRewards.length > 0 && (
                        <Select
                            value={selectedReward ?? "all"}
                            onValueChange={(val) =>
                                onRewardFilterChange(val === "all" ? null : val)
                            }
                        >
                            <SelectTrigger
                                className="w-full md:w-[200px]"
                                aria-label="Filtrar por recompensa"
                            >
                                <SelectValue placeholder="Todas las recompensas" />
                            </SelectTrigger>
                            <SelectContent>
                                <SelectItem value="all">
                                    Todas las recompensas
                                </SelectItem>
                                {uniqueRewards.map((name) => (
                                    <SelectItem key={name} value={name}>
                                        {name}
                                    </SelectItem>
                                ))}
                            </SelectContent>
                        </Select>
                    )}
                </div>
            </CardHeader>

            <CardContent>
                {filteredBackings.length === 0 ? (
                    <div className="text-center py-8">
                        <p className="text-muted-foreground">
                            {backings.length === 0
                                ? "No hay apoyos aun"
                                : "No se encontraron apoyos con los filtros aplicados"}
                        </p>
                    </div>
                ) : (
                    <TooltipProvider>
                        <div className="overflow-x-auto">
                            <Table aria-label="Lista de backings">
                                <TableHeader>
                                    <TableRow>
                                        <TableHead>Backer</TableHead>
                                        <TableHead>Monto</TableHead>
                                        <TableHead>Recompensa</TableHead>
                                        <TableHead>Mensaje</TableHead>
                                        <TableHead className="text-right">
                                            Fecha
                                        </TableHead>
                                    </TableRow>
                                </TableHeader>
                                <TableBody>
                                    {filteredBackings.map((backing) => (
                                        <TableRow
                                            key={backing.id}
                                            className="hover:bg-muted/50 transition-colors"
                                        >
                                            <TableCell>
                                                <div className="flex items-center gap-3">
                                                    <Avatar className="h-8 w-8">
                                                        <AvatarFallback className="bg-primary/20 text-primary text-xs">
                                                            {backing.nombreBacker
                                                                .charAt(0)
                                                                .toUpperCase()}
                                                        </AvatarFallback>
                                                    </Avatar>
                                                    <div>
                                                        <span className="font-medium">
                                                            {backing.nombreBacker ===
                                                            "Anonimo" ? (
                                                                <span className="text-muted-foreground italic">
                                                                    Anonimo
                                                                </span>
                                                            ) : (
                                                                backing.nombreBacker
                                                            )}
                                                        </span>
                                                        {backing.nombreBacker === "Anonimo" && (
                                                            <Badge
                                                                variant="secondary"
                                                                className="text-xs ml-2"
                                                            >
                                                                Anonimo
                                                            </Badge>
                                                        )}
                                                    </div>
                                                </div>
                                            </TableCell>
                                            <TableCell className="font-semibold">
                                                {formatCurrency(backing.monto)}
                                            </TableCell>
                                            <TableCell className="text-muted-foreground">
                                                {backing.rewardNombre ?? (
                                                    <span className="italic">
                                                        Sin recompensa
                                                    </span>
                                                )}
                                            </TableCell>
                                            <TableCell>
                                                {backing.mensaje ? (
                                                    <Tooltip>
                                                        <TooltipTrigger asChild>
                                                            <p className="text-sm text-muted-foreground truncate max-w-[200px] cursor-help">
                                                                {backing.mensaje}
                                                            </p>
                                                        </TooltipTrigger>
                                                        <TooltipContent className="max-w-sm">
                                                            <p>{backing.mensaje}</p>
                                                        </TooltipContent>
                                                    </Tooltip>
                                                ) : (
                                                    <span className="text-muted-foreground italic text-sm">
                                                        -
                                                    </span>
                                                )}
                                            </TableCell>
                                            <TableCell className="text-right text-muted-foreground">
                                                {formatDateShort(
                                                    backing.fechaCreacion
                                                )}
                                            </TableCell>
                                        </TableRow>
                                    ))}
                                </TableBody>
                            </Table>
                        </div>
                    </TooltipProvider>
                )}
            </CardContent>
        </Card>
    )
}
