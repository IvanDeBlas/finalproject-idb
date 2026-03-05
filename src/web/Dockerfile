FROM node:20-alpine AS build
WORKDIR /app

COPY shared/ ./shared/

COPY web/package.json web/package-lock.json ./web/
WORKDIR /app/web
RUN npm install

# Symlink so shared resolves peer deps from web's node_modules
RUN ln -s /app/web/node_modules /app/shared/node_modules

COPY web/ .
ARG VITE_API_URL=/api
ENV VITE_API_URL=${VITE_API_URL}
RUN npx vite build

FROM nginx:alpine
COPY --from=build /app/web/dist /usr/share/nginx/html
COPY web/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
