import type { FC } from "react"
import type { UseFormRegister, FieldErrors } from "react-hook-form"
import { Input } from "@/components/ui/input"
import { AlertCircle } from "lucide-react"
import type { CreatePromotorFormData } from "@shared/schemas/crowdpromotion.schema"

interface PromotorSocialFieldsProps {
    register: UseFormRegister<CreatePromotorFormData>
    errors: FieldErrors<CreatePromotorFormData>
    disabled?: boolean
    onLastFieldBlur?: () => void
}

const socialFields = [
    { name: "urlInstagram" as const, label: "Instagram", icon: "IG", placeholder: "https://instagram.com/..." },
    { name: "urlTikTok" as const, label: "TikTok", icon: "TT", placeholder: "https://tiktok.com/@..." },
    { name: "urlYouTube" as const, label: "YouTube", icon: "YT", placeholder: "https://youtube.com/..." },
    { name: "urlTwitter" as const, label: "Twitter/X", icon: "X", placeholder: "https://x.com/..." },
]

export const PromotorSocialFields: FC<PromotorSocialFieldsProps> = ({
    register,
    errors,
    disabled,
    onLastFieldBlur,
}) => {
    return (
        <div>
            <p className="text-sm font-medium text-[#cbd5e1] mb-3">
                Redes sociales
            </p>
            {socialFields.map(({ name, label, icon, placeholder }, index) => {
                const isLast = index === socialFields.length - 1
                const registration = register(name)
                return (
                    <div key={name} className="mb-3">
                        <div className="flex items-center gap-3">
                            <span className="w-8 h-8 rounded-md flex items-center justify-center bg-[#1e1e38] text-[#94a3b8] flex-shrink-0 text-xs font-bold">
                                {icon}
                            </span>
                            <span className="text-sm text-[#94a3b8] w-20 flex-shrink-0">
                                {label}
                            </span>
                            <Input
                                type="url"
                                placeholder={placeholder}
                                disabled={disabled}
                                className="bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] focus:border-[#a855f7] h-10 flex-1 focus-visible:ring-0 focus-visible:ring-offset-0"
                                {...registration}
                                onBlur={(e) => {
                                    registration.onBlur(e)
                                    if (isLast && onLastFieldBlur) {
                                        onLastFieldBlur()
                                    }
                                }}
                            />
                        </div>
                        {errors[name]?.message && (
                            <p className="text-xs text-red-400 mt-1 ml-11 flex items-center gap-1" role="alert">
                                <AlertCircle className="w-3 h-3 flex-shrink-0" />
                                {errors[name]?.message as string}
                            </p>
                        )}
                    </div>
                )
            })}
        </div>
    )
}
