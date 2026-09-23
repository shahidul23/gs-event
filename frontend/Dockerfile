# Build stage
FROM node:22-alpine AS build

WORKDIR /app

# Copy package files first for better Docker layer caching
COPY package*.json ./

RUN npm ci

# Copy application source
COPY . .

# Build Vue application
RUN npm run build


# Production stage
FROM nginx:alpine

# Remove default nginx configuration
RUN rm /etc/nginx/conf.d/default.conf

# Copy Vue build files
COPY --from=build /app/dist /usr/share/nginx/html

# Copy custom nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
