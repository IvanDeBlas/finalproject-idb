import { type FC, useState, useCallback } from "react"
import { Button } from "@/components/ui/button"
import {
    Tooltip,
    TooltipContent,
    TooltipProvider,
    TooltipTrigger,
} from "@/components/ui/tooltip"
import { Copy, Check } from "lucide-react"
import { toast } from "sonner"

interface UrlTrackingBlockProps {
    url: string
}

export const UrlTrackingBlock: FC<UrlTrackingBlockProps> = ({ url }) => {
    const [copied, setCopied] = useState(false)

    const handleCopy = useCallback(async () => {
        try {
            await navigator.clipboard.writeText(url)
            setCopied(true)
            toast.success("URL de tracking copiada al portapapeles")
            setTimeout(() => setCopied(false), 1500)
        } catch {
            toast.error("No se pudo copiar la URL")
        }
    }, [url])

    return (
        <div>
            <p className="text-xs font-medium text-[#94a3b8] mb-1.5">
                URL de tracking:
            </p>
            <div className="flex items-center gap-2">
                <TooltipProvider>
                    <Tooltip>
                        <TooltipTrigger asChild>
                            <div
                                className="flex-1 flex items-center bg-[#0f0f1f] border border-[#334155] rounded-lg px-3 h-10 text-sm text-[#94a3b8] overflow-hidden cursor-default"
                                role="textbox"
                                aria-readonly="true"
                                aria-label="URL de tracking"
                            >
                                <span className="truncate">{url}</span>
                            </div>
                        </TooltipTrigger>
                        <TooltipContent
                            className="max-w-md break-all bg-[#1e1e38] border-[#334155] text-[#94a3b8] text-xs"
                            side="bottom"
                        >
                            {url}
                        </TooltipContent>
                    </Tooltip>
                </TooltipProvider>
                <Button
                    variant="ghost"
                    size="icon"
                    className="h-10 w-10 p-0 text-[#64748b] hover:text-[#a855f7] hover:bg-[#a855f7]/10 shrink-0"
                    onClick={handleCopy}
                    aria-label={copied ? "URL copiada" : "Copiar URL de tracking"}
                >
                    {copied
                        ? <Check className="w-4 h-4 text-green-400" aria-hidden="true" />
                        : <Copy className="w-4 h-4" aria-hidden="true" />
                    }
                </Button>
            </div>
        </div>
    )
}
