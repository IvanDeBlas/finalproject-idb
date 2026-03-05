import type { FC } from "react"

interface PromotorSocialLinksProps {
    urlInstagram: string | null
    urlTikTok: string | null
    urlYouTube: string | null
    urlTwitter: string | null
}

const socialNetworks = [
    { key: "urlInstagram", label: "Instagram", icon: "IG" },
    { key: "urlTikTok", label: "TikTok", icon: "TT" },
    { key: "urlYouTube", label: "YouTube", icon: "YT" },
    { key: "urlTwitter", label: "Twitter/X", icon: "X" },
] as const

export const PromotorSocialLinks: FC<PromotorSocialLinksProps> = ({
    urlInstagram,
    urlTikTok,
    urlYouTube,
    urlTwitter,
}) => {
    const urls: Record<string, string | null> = {
        urlInstagram,
        urlTikTok,
        urlYouTube,
        urlTwitter,
    }

    const hasAny = Object.values(urls).some(Boolean)

    if (!hasAny) {
        return (
            <p className="text-sm text-[#64748b] italic">
                Sin redes sociales configuradas
            </p>
        )
    }

    return (
        <div className="flex items-center gap-2">
            {socialNetworks.map(({ key, label, icon }) => {
                const url = urls[key]
                if (!url) return null
                return (
                    <a
                        key={key}
                        href={url}
                        target="_blank"
                        rel="noopener noreferrer"
                        title={label}
                        className="w-8 h-8 flex items-center justify-center rounded-md bg-[#1e1e38] text-[#94a3b8] hover:text-white hover:bg-[#334155] transition-colors text-xs font-bold"
                    >
                        {icon}
                    </a>
                )
            })}
        </div>
    )
}
