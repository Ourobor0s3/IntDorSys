FROM node:20-alpine AS node
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN APP_VER=$(node -p "require('./package.json').version + '.' + Math.floor(Date.now()/1000)") && \
    sed -i "s/APP_VERSION_PLACEHOLDER/$APP_VER/g" src/environments/environment.prod.ts && \
    npm run build

FROM nginx:alpine
RUN apk add --no-cache openssl && \
    mkdir -p /etc/nginx/certs

COPY --from=node /app/dist/endless /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80 443

CMD ["nginx", "-g", "daemon off;"]
