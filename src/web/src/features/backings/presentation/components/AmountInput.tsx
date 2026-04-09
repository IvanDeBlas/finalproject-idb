import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { cn } from "@/lib/utils"

interface AmountInputProps {
    value: number
    onChange: (value: number) => void
    minAmount?: number
    maxAmount?: number
    error?: string
    hint?: string
    className?: string
}

export function AmountInput({
    value,
    onChange,
    minAmount = 1,
    maxAmount = 100000,
    error,
    hint,
    className,
}: AmountInputProps) {
    return (
        <div className={className}>
            <Label htmlFor="monto" className="text-sm font-medium text-[#cbd5e1]">
                Monto a Aportar
            </Label>
            <div className="relative mt-2">
                <span className="absolute left-4 top-1/2 -translate-y-1/2 text-2xl text-[#94a3b8]">
                    €
                </span>
                <Input
                    id="monto"
                    type="number"
                    step="1"
                    value={value || ""}
                    onChange={(e) => {
                        const val = Number(e.target.value)
                        if (!isNaN(val)) onChange(val)
                    }}
                    className={cn(
                        "bg-[#1a1a2e] border-[#334155] text-white text-2xl font-bold text-center py-4 pl-12 pr-4 focus:border-primary",
                        error && "border-red-500 focus:border-red-500"
                    )}
                    min={minAmount}
                    max={maxAmount}
                    aria-invalid={!!error}
                    aria-describedby={error ? "monto-error" : hint ? "monto-hint" : undefined}
                />
            </div>
            {error && (
                <p id="monto-error" role="alert" className="text-xs text-red-500 mt-1">
                    {error}
                </p>
            )}
            {hint && !error && (
                <p id="monto-hint" className="text-xs text-[#64748b] mt-1 italic">
                    {hint}
                </p>
            )}
        </div>
    )
}
