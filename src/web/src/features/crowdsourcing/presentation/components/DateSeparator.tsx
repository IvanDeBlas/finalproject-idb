import { FC } from "react"

interface DateSeparatorProps {
    fecha: Date
}

export const DateSeparator: FC<DateSeparatorProps> = ({ fecha }) => {
    const formatted = fecha.toLocaleDateString("es-ES", {
        day: "numeric",
        month: "long",
        year: "numeric",
    })

    return (
        <div className="flex items-center gap-3 my-4">
            <div className="flex-1 h-px bg-[#334155]" />
            <span className="text-xs text-[#64748b] whitespace-nowrap">
                {formatted}
            </span>
            <div className="flex-1 h-px bg-[#334155]" />
        </div>
    )
}
