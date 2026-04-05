FROM node:20.9-alpine as node
WORKDIR /app

COPY package.json package-lock.json ./

RUN npm ci

COPY . .

# build admin
RUN npm run build && \
    cp -r /app/dist/endless/ /app

FROM nginx:alpine3.20
COPY --from=node /app /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
