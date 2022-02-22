FROM node:16.13.0 AS build

WORKDIR /app

RUN npm install -g @angular/cli

COPY ./package.json .

RUN npm install --force

COPY . .

RUN ng build --prod

FROM nginx AS runtime

COPY --from=build /app/dist/LetsDate-Frontend /usr/share/nginx/html