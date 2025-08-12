FROM node:18.13 AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . .
RUN npm run build --prod

FROM nginx:latest
COPY --from=build /app/dist/e-menu.client /usr/share/nginx/html
EXPOSE 80
