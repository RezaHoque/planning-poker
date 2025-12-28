# Self-Hosting Planning Poker with Docker

This guide will help you self-host the Planning Poker application using Docker.

## Prerequisites

- Docker installed on your system ([Install Docker](https://docs.docker.com/get-docker/))
- Docker Compose (usually included with Docker Desktop)
- A GIPHY API key (optional, for GIF reactions) - Get one from [GIPHY Developers](https://developers.giphy.com/)

## Quick Start

### Option 1: Using Pre-built Image from GHCR (Recommended)

The easiest way to self-host is using the pre-built image from GitHub Container Registry.

1. **Create a directory** for your deployment

   ```bash
   mkdir planning-poker-deploy
   cd planning-poker-deploy
   ```

2. **Create `.env` file** in that directory (optional, for GIF reactions):

   ```
   GIPHY_API_BASE_URL=https://api.giphy.com/v1/gifs/search?api_key=YOUR_API_KEY
   GIPHY_API_QUERY=limit=5&offset=0&rating=g&lang=en&bundle=low_bandwidth
   ```

   **Note:** Replace `YOUR_API_KEY` with your actual GIPHY API key. If you don't want GIF reactions, you can skip creating the `.env` file - the application will work without it.

3. **Run the container**

   **Linux/macOS:**

   ```bash
   docker run -d \
     --name planning-poker \
     -p 8080:8080 \
     -v $(pwd)/database:/app/Database \
     -v $(pwd)/logs:/app/Logs \
     --env-file .env \
     --restart unless-stopped \
     ghcr.io/rezahoque/planning-poker:latest
   ```

   **Windows PowerShell:**

   ```powershell
   docker run -d `
     --name planning-poker `
     -p 8080:8080 `
     -v ${PWD}/database:/app/Database `
     -v ${PWD}/logs:/app/Logs `
     --env-file .env `
     --restart unless-stopped `
     ghcr.io/rezahoque/planning-poker:latest
   ```

   **Note:** If you didn't create a `.env` file, remove the `--env-file .env` line from the command.

4. **Access the application**

   Open your browser and navigate to: `http://localhost:8080`

5. **Stop the container** (when needed)

   ```bash
   docker stop planning-poker
   docker rm planning-poker
   ```

---

### Option 2: Using Docker Compose

If you prefer using Docker Compose, you can use it with either the pre-built image or build from source.

#### Using Pre-built Image with Docker Compose

1. **Create a directory** for your deployment

   ```bash
   mkdir planning-poker-deploy
   cd planning-poker-deploy
   ```

2. **Create `docker-compose.yml`** file:

   ```yaml
   services:
     planning-poker:
       image: ghcr.io/rezahoque/planning-poker:latest
       container_name: planning-poker
       ports:
         - "8080:8080"
       volumes:
         - ./database:/app/Database
         - ./logs:/app/Logs
       env_file:
         - .env
       restart: unless-stopped
       environment:
         - ASPNETCORE_ENVIRONMENT=Production
         - ASPNETCORE_URLS=http://+:8080
   ```

3. **Create `.env` file** in the same directory (optional, for GIF reactions):

   ```
   GIPHY_API_BASE_URL=https://api.giphy.com/v1/gifs/search?api_key=YOUR_API_KEY
   GIPHY_API_QUERY=limit=5&offset=0&rating=g&lang=en&bundle=low_bandwidth
   ```

   **Note:** Replace `YOUR_API_KEY` with your actual GIPHY API key. The GIF feature is optional.

4. **Start the application**

   ```bash
   docker-compose up -d
   ```

5. **Access the application**

   - Open your browser and navigate to: `http://localhost:8080`

6. **Stop the application**

   ```bash
   docker-compose down
   ```

#### Building from Source with Docker Compose

If you want to build from source or customize the application:

1. **Clone or download this repository**

2. **Navigate to the project directory**

   ```bash
   cd PlanningPoker
   ```

3. **Create `.env` file** in the same directory as `docker-compose.yml`:

   ```
   GIPHY_API_BASE_URL=https://api.giphy.com/v1/gifs/search?api_key=YOUR_API_KEY
   GIPHY_API_QUERY=limit=5&offset=0&rating=g&lang=en&bundle=low_bandwidth
   ```

   **Note:** Replace `YOUR_API_KEY` with your actual GIPHY API key. The GIF feature is optional.

4. **Start the application**

   ```bash
   docker-compose up -d
   ```

5. **Access the application**

   - Open your browser and navigate to: `http://localhost:8080`

6. **Stop the application**

   ```bash
   docker-compose down
   ```

---

## Configuration

### GIPHY API Configuration (Optional)

The application supports GIF reactions when votes are revealed. This feature is **optional** and the application will work perfectly without it.

To enable GIF reactions:

1. **Get a GIPHY API key** from [GIPHY Developers](https://developers.giphy.com/)

2. **Create a `.env` file** with your GIPHY configuration:

   ```
   GIPHY_API_BASE_URL=https://api.giphy.com/v1/gifs/search?api_key=YOUR_API_KEY
   GIPHY_API_QUERY=limit=5&offset=0&rating=g&lang=en&bundle=low_bandwidth
   ```

   Replace `YOUR_API_KEY` with your actual GIPHY API key.

3. **Where to create the `.env` file:**

   - **If using docker-compose**: Create it in the same directory as your `docker-compose.yml` file
   - **If using `docker run`**: Create it in the directory where you run the command, then use `--env-file .env`

4. **Restart the container** to apply changes:

   ```bash
   docker-compose restart
   ```

   Or if using `docker run`:

   ```bash
   docker restart planning-poker
   ```

**Note:** Without GIPHY API configuration, the application will work normally but GIF reactions will be disabled (no error will occur).

### Changing the Port

To run the application on a different port, modify the port mapping:

**Docker Compose:**

```yaml
ports:
  - "YOUR_PORT:8080" # Change YOUR_PORT to your desired port
```

**Docker Run:**

```bash
docker run -d -p YOUR_PORT:8080 ...
```

---

## Data Persistence

The application stores data in two locations:

- **Database**: Stored in `./database` directory (SQLite database)
- **Logs**: Stored in `./logs` directory

These directories are automatically created and persisted across container restarts using Docker volumes. Make sure to back up the `./database` directory regularly for production deployments.

---

## Production Self-Hosting

For production self-hosting, consider the following:

### 1. Use a Reverse Proxy

Set up a reverse proxy (nginx, Traefik, Caddy, etc.) for SSL/TLS termination:

**Example nginx configuration:**

```nginx
server {
    listen 80;
    server_name your-domain.com;

    location / {
        proxy_pass http://localhost:8080;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### 2. Set Up Regular Backups

Regularly back up the `./database` directory where your SQLite database is stored:

```bash
# Example backup script
cp -r ./database ./backups/database-$(date +%Y%m%d-%H%M%S)
```

### 3. Configure Firewall Rules

Restrict access to your server and only allow necessary ports (e.g., 80, 443 for web traffic).

### 4. Monitor Container Health

Monitor your container using Docker's built-in monitoring or external tools like Portainer, Prometheus, etc.

### 5. Set Resource Limits

Add resource limits in `docker-compose.yml`:

```yaml
deploy:
  resources:
    limits:
      cpus: "1"
      memory: 512M
```

### 6. Keep the Image Updated

Regularly pull the latest image to get security updates and new features:

```bash
docker pull ghcr.io/rezahoque/planning-poker:latest
docker-compose up -d
```

---

## Updating the Application

### Using Pre-built Image

```bash
docker pull ghcr.io/rezahoque/planning-poker:latest
docker-compose up -d
```

### Building from Source

```bash
git pull
docker-compose up -d --build
```

---

## Troubleshooting

### Check Container Logs

```bash
docker-compose logs -f
```

Or for a specific container:

```bash
docker logs planning-poker
```

### Check if Container is Running

```bash
docker ps
```

### Restart the Container

```bash
docker-compose restart
```

Or:

```bash
docker restart planning-poker
```

### Access Container Shell (for debugging)

```bash
docker exec -it planning-poker /bin/bash
```

### Check Environment Variables

```bash
docker exec planning-poker env | grep GIPHY
```

### View Application Logs

Application logs are also stored in the `./logs` directory on your host machine.

---

## Support

For issues or questions:

1. Check the application logs in the `./logs` directory
2. Check container logs: `docker logs planning-poker`
3. Verify your `.env` file is correctly formatted (no quotes around values)
4. Ensure the GIPHY API key is valid if using GIF reactions
