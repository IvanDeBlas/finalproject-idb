import { Skeleton } from "@/components/ui/skeleton"

export function CampaniaDetailSkeleton() {
    return (
        <div className="min-h-screen bg-[#1a1a2e]">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
                {/* Hero skeleton */}
                <Skeleton className="w-full h-64 sm:h-80 lg:h-96 mb-8 rounded-lg" />

                <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                    {/* Content column */}
                    <div className="lg:col-span-2 space-y-6">
                        {/* Title */}
                        <Skeleton className="h-10 w-3/4" />

                        {/* Artist info */}
                        <div className="flex items-center gap-3">
                            <Skeleton className="w-10 h-10 rounded-full" />
                            <Skeleton className="h-4 w-32" />
                            <Skeleton className="h-5 w-20 rounded-full" />
                        </div>

                        {/* Stats */}
                        <div className="space-y-3">
                            <Skeleton className="h-8 w-48" />
                            <Skeleton className="h-3 w-full" />
                            <Skeleton className="h-4 w-32" />
                            <div className="flex gap-6">
                                <Skeleton className="h-4 w-24" />
                                <Skeleton className="h-4 w-32" />
                            </div>
                        </div>

                        {/* Tabs */}
                        <div className="flex gap-4 border-b border-[#334155] pb-2">
                            <Skeleton className="h-6 w-20" />
                            <Skeleton className="h-6 w-32" />
                            <Skeleton className="h-6 w-28" />
                            <Skeleton className="h-6 w-16" />
                        </div>

                        {/* Content lines */}
                        <div className="space-y-3 py-4">
                            {[...Array(8)].map((_, i) => (
                                <Skeleton
                                    key={i}
                                    className="h-4"
                                    style={{ width: `${Math.random() * 30 + 70}%` }}
                                />
                            ))}
                        </div>

                        {/* Artist card */}
                        <div className="space-y-3 p-6 border border-[#334155] rounded-lg">
                            <Skeleton className="h-6 w-32" />
                            <div className="flex gap-4">
                                <Skeleton className="w-16 h-16 rounded-full" />
                                <div className="flex-1 space-y-2">
                                    <Skeleton className="h-5 w-40" />
                                    <Skeleton className="h-4 w-full" />
                                    <Skeleton className="h-4 w-3/4" />
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Sidebar column */}
                    <div className="lg:col-span-1 space-y-4">
                        <div className="p-6 border border-[#334155] rounded-lg space-y-4">
                            <Skeleton className="h-12 w-full rounded-md" />
                            <Skeleton className="h-4 w-24 mx-auto" />
                            <Skeleton className="h-px w-full" />
                            <Skeleton className="h-6 w-28" />
                            <Skeleton className="h-32 w-full rounded-md" />
                            <Skeleton className="h-32 w-full rounded-md" />
                            <Skeleton className="h-32 w-full rounded-md" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}
