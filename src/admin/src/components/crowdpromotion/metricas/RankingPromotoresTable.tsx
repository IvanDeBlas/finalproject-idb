"use client"

import {
    Table,
    TableHeader,
    TableBody,
    TableRow,
    TableHead,
    TableCell,
} from "@/components/ui/table"
import { Badge } from "@/components/ui/badge"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { ArrowUpDown, ArrowUp, ArrowDown } from "lucide-react"
import { useSortableTable } from "@/hooks/use-sortable-table"
import type { RankingPromotorItem } from "@shared/types/crowdpromotion"

interface RankingPromotoresTableProps {
    items: RankingPromotorItem[]
}

export function RankingPromotoresTable({ items }: RankingPromotoresTableProps) {
    const { sortedData, handleSort, getSortIcon } = useSortableTable<RankingPromotorItem>(
        items,
        "conversiones",
        "desc"
    )

    function renderSortIcon(key: keyof RankingPromotorItem) {
        const state = getSortIcon(key)
        if (state === "asc") return <ArrowUp className="w-3.5 h-3.5 ml-1 inline" />
        if (state === "desc") return <ArrowDown className="w-3.5 h-3.5 ml-1 inline" />
        return <ArrowUpDown className="w-3.5 h-3.5 ml-1 inline text-[#475569]" />
    }

    function getAriaSortValue(key: keyof RankingPromotorItem): "ascending" | "descending" | "none" {
        const state = getSortIcon(key)
        if (state === "asc") return "ascending"
        if (state === "desc") return "descending"
        return "none"
    }

    return (
        <div className="overflow-x-auto">
            <Table>
                <TableHeader>
                    <TableRow className="border-zinc-800 hover:bg-transparent">
                        <TableHead scope="col" className="text-[#64748b] w-12">#</TableHead>
                        <TableHead scope="col" className="text-[#64748b]">Promotor</TableHead>
                        <TableHead
                            scope="col"
                            className="text-[#64748b] cursor-pointer select-none"
                            aria-sort={getAriaSortValue("clicks")}
                            onClick={() => handleSort("clicks")}
                        >
                            Clicks{renderSortIcon("clicks")}
                        </TableHead>
                        <TableHead
                            scope="col"
                            className="text-[#64748b] cursor-pointer select-none"
                            aria-sort={getAriaSortValue("conversiones")}
                            onClick={() => handleSort("conversiones")}
                        >
                            Conv.{renderSortIcon("conversiones")}
                        </TableHead>
                        <TableHead
                            scope="col"
                            className="text-[#64748b] cursor-pointer select-none hidden md:table-cell"
                            aria-sort={getAriaSortValue("valorGenerado")}
                            onClick={() => handleSort("valorGenerado")}
                        >
                            Valor{renderSortIcon("valorGenerado")}
                        </TableHead>
                        <TableHead
                            scope="col"
                            className="text-[#64748b] cursor-pointer select-none hidden md:table-cell"
                            aria-sort={getAriaSortValue("comisionAcumulada")}
                            onClick={() => handleSort("comisionAcumulada")}
                        >
                            Comision{renderSortIcon("comisionAcumulada")}
                        </TableHead>
                    </TableRow>
                </TableHeader>
                <TableBody>
                    {sortedData.length === 0 ? (
                        <TableRow>
                            <TableCell colSpan={6} className="text-center text-[#64748b] py-8">
                                Sin promotores con actividad en este periodo
                            </TableCell>
                        </TableRow>
                    ) : (
                        sortedData.map((item, index) => (
                            <TableRow
                                key={item.promotorId}
                                className="border-zinc-800 hover:bg-[#1e1e38]"
                            >
                                <TableCell className="text-[#64748b] font-mono text-sm">
                                    #{index + 1}
                                </TableCell>
                                <TableCell>
                                    <div className="flex items-center gap-3">
                                        <Avatar className="w-8 h-8 bg-[#1e1e38] border border-zinc-700">
                                            <AvatarFallback className="bg-[#1e1e38] text-[#94a3b8] text-xs">
                                                {item.promotorNombre.charAt(0).toUpperCase()}
                                            </AvatarFallback>
                                        </Avatar>
                                        <div>
                                            <p className="text-sm text-white font-medium">
                                                {item.promotorNombre}
                                            </p>
                                            {item.tipoPromotorNombre && (
                                                <Badge
                                                    variant="secondary"
                                                    className="bg-[#1e1e38] text-[#94a3b8] border border-zinc-700 text-[10px] mt-0.5"
                                                >
                                                    {item.tipoPromotorNombre}
                                                </Badge>
                                            )}
                                        </div>
                                    </div>
                                </TableCell>
                                <TableCell className="text-white text-sm">
                                    {item.clicks.toLocaleString("es-ES")}
                                </TableCell>
                                <TableCell className="text-white text-sm">
                                    {item.conversiones.toLocaleString("es-ES")}
                                </TableCell>
                                <TableCell className="text-[#f59e0b] text-sm hidden md:table-cell">
                                    {item.valorGenerado.toLocaleString("es-ES", {
                                        minimumFractionDigits: 0,
                                        maximumFractionDigits: 2,
                                    })}
                                </TableCell>
                                <TableCell className="text-white text-sm hidden md:table-cell">
                                    {item.comisionAcumulada.toLocaleString("es-ES", {
                                        minimumFractionDigits: 0,
                                        maximumFractionDigits: 2,
                                    })}
                                </TableCell>
                            </TableRow>
                        ))
                    )}
                </TableBody>
            </Table>
        </div>
    )
}
