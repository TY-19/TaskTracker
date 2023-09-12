FROM node:16-alpine AS build
WORKDIR /app

RUN npm install -g @angular/cli

COPY ./package.json .
RUN NODE_ENV=development npm i
COPY . .
RUN npm run build-docker
# build-docker is defined in package.json (section scripts)
# docker configuration is defined in angular.json
#   (projects:WebUI:architect:build:configurations:docker)
# environment.ts file is replaced with environment.docker.ts that contains url of backend server

FROM nginx AS runtime
COPY --from=build /app/dist/WebUI /usr/share/nginx/html

ENTRYPOINT ["nginx", "-g", "daemon off;"]