FROM node:20-alpine AS build
WORKDIR /app

COPY shared/ ./shared/

COPY admin/package.json admin/package-lock.json ./admin/
WORKDIR /app/admin
RUN npm install --ignore-scripts

# Symlink so shared resolves peer deps from admin's node_modules
RUN ln -s /app/admin/node_modules /app/shared/node_modules

WORKDIR /app/admin
COPY admin/ .
ENV NEXT_TELEMETRY_DISABLED=1
ENV INTERNAL_API_URL=http://api:8080
RUN npm run build

FROM node:20-alpine
WORKDIR /app

ENV NODE_ENV=production
ENV NEXT_TELEMETRY_DISABLED=1

COPY --from=build /app/admin/package.json ./package.json
COPY --from=build /app/admin/package-lock.json ./package-lock.json
COPY --from=build /app/admin/next.config.mjs ./next.config.mjs
COPY --from=build /app/admin/.next ./.next
COPY --from=build /app/admin/node_modules ./node_modules

EXPOSE 3000
CMD ["npx", "next", "start", "-p", "3000"]
