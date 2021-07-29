FROM cypress/included:7.7.0

WORKDIR /app

COPY package.json package-lock.json tsconfig.json ./

RUN npm install

COPY . .

ENV XDG_CONFIG_HOME /app

ENTRYPOINT npm run cyrun -- --env FRONTEND_URL=http://frontend:3000,API_URL=http://backend:5000,AUTH_URL=http://mock-auth:8080
