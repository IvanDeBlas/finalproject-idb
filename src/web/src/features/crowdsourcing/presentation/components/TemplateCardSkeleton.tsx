import { FC } from "react"
import { Card, CardHeader, CardContent } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"

export const TemplateCardSkeleton: FC = () => {
    return (
        <Card className="bg-[#0f1729] border-[#334155]">
            <CardHeader className="text-center">
                <Skeleton className="w-16 h-16 rounded-full mx-auto mb-4 bg-[#1e2a42]" />
                <Skeleton className="h-6 w-3/4 mx-auto bg-[#1e2a42]" />
            </CardHeader>
            <CardContent className="space-y-3">
                <Skeleton className="h-4 w-full bg-[#1e2a42]" />
                <Skeleton className="h-4 w-2/3 mx-auto bg-[#1e2a42]" />
                <Skeleton className="h-4 w-1/2 mx-auto bg-[#1e2a42]" />
                <Skeleton className="h-10 w-full mt-4 bg-[#1e2a42]" />
            </CardContent>
        </Card>
    )
}
