/* eslint-disable @next/next/no-img-element */
"use client"

import { useState } from "react"
import { CloudUpload, X, Image } from "lucide-react"
import { cn } from "@/lib/utils"
import { Button, Input, Label } from "@/components/ui"

interface ImageUploadFieldProps {
    value: string
    onChange: (url: string) => void
    label?: string
    accept?: string
    maxSize?: number
}

export function ImageUploadField({
    value,
    onChange,
    label,
}: ImageUploadFieldProps) {
    const [isPreviewError, setIsPreviewError] = useState(false)

    const handleRemove = () => {
        onChange("")
        setIsPreviewError(false)
    }

    return (
        <div className="space-y-4">
            {label && <Label>{label}</Label>}

            {/* URL Input (MVP: paste URL) */}
            <div className="space-y-2">
                <div className="relative">
                    <Input
                        type="url"
                        placeholder="Pega la URL de tu imagen (https://...)"
                        value={value || ""}
                        onChange={(e) => {
                            onChange(e.target.value)
                            setIsPreviewError(false)
                        }}
                        className="bg-input border-border pr-10"
                    />
                    {value && (
                        <Button
                            type="button"
                            variant="ghost"
                            size="icon"
                            onClick={handleRemove}
                            className="absolute right-1 top-1/2 -translate-y-1/2 h-7 w-7"
                        >
                            <X className="h-4 w-4" />
                        </Button>
                    )}
                </div>
                <p className="text-xs text-muted-foreground">
                    PNG, JPG recomendado. 1200x630px para mejor visualizacion.
                </p>
            </div>

            {/* Preview or Placeholder */}
            {value && !isPreviewError ? (
                <div className="p-4 bg-muted/10 rounded-lg border border-border">
                    <div className="flex items-start gap-4">
                        <img
                            src={value}
                            alt="Vista previa"
                            className="w-32 h-32 object-cover rounded-lg border border-border"
                            onError={() => setIsPreviewError(true)}
                        />
                        <div className="flex-1">
                            <p className="text-sm text-foreground font-medium">
                                Vista Previa
                            </p>
                            <p className="text-xs text-muted-foreground mt-1">
                                Tu imagen de portada aparecera asi en la
                                campania
                            </p>
                        </div>
                        <Button
                            type="button"
                            variant="ghost"
                            size="icon"
                            onClick={handleRemove}
                        >
                            <X className="w-4 h-4" />
                        </Button>
                    </div>
                </div>
            ) : !value ? (
                <div
                    className={cn(
                        "border-2 border-dashed rounded-lg p-12 text-center transition-all",
                        "border-muted bg-muted/5"
                    )}
                >
                    <CloudUpload className="w-12 h-12 text-muted-foreground mx-auto mb-3" />
                    <p className="text-muted-foreground mb-1">
                        Pega la URL de tu imagen arriba
                    </p>
                    <p className="text-xs text-muted-foreground">
                        PNG, JPG hasta 5MB (1200x630px recomendado)
                    </p>
                </div>
            ) : (
                <div className="p-4 bg-destructive/10 rounded-lg border border-destructive/30 flex items-center gap-3">
                    <Image className="w-8 h-8 text-destructive" />
                    <div>
                        <p className="text-sm text-destructive font-medium">
                            No se pudo cargar la imagen
                        </p>
                        <p className="text-xs text-destructive/70">
                            Verifica que la URL sea correcta y accesible
                        </p>
                    </div>
                </div>
            )}
        </div>
    )
}
