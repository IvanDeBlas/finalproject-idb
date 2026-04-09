import { cn } from "@/lib/utils"

interface CampaniaDescriptionProps {
    htmlContent: string
    className?: string
}

export function CampaniaDescription({ htmlContent, className }: CampaniaDescriptionProps) {
    // For MVP: render content as plain text with line break support.
    // If HTML content is detected, render it within prose container.
    const isHtml = /<[a-z][\s\S]*>/i.test(htmlContent)

    if (isHtml) {
        return (
            <div
                className={cn(
                    "prose prose-invert max-w-none",
                    "prose-headings:text-white prose-p:text-[#94a3b8]",
                    "prose-strong:text-white prose-a:text-purple-400 prose-a:no-underline hover:prose-a:underline",
                    "prose-img:rounded-lg prose-img:my-6",
                    className
                )}
                dangerouslySetInnerHTML={{ __html: htmlContent }}
            />
        )
    }

    return (
        <div className={cn("text-[#94a3b8] whitespace-pre-wrap leading-relaxed", className)}>
            {htmlContent}
        </div>
    )
}
