# Planning Poker 🃏

A **zero-friction, open-source Planning Poker** tool for agile teams.

- 🚀 No accounts, no logins
- 🔗 Join via a single shared link
- 👥 Real-time voting (SignalR)
- 🏠 Self-hosted & privacy-friendly
- 🐳 Docker-ready

Perfect for Scrum teams, consultants, and companies that want a **simple and private** estimation tool.

---

## ✨ Features

- Create a planning poker room instantly
- Share a single URL with your team
- Real-time voting & reveal
- Duplicate names handled automatically (`John`, `John_1`, etc.)
- Works great for remote and hybrid teams
- Open source

---

## 🚀 Live Demo (Hosted Version)

A free hosted version is available here:

👉 **https://your-hosted-url.com**

This version is intended for:

- quick demos
- public usage
- trying out the tool

For **private or company usage**, see self-hosting below.

---

## 🏠 Self-Hosting (Recommended for Teams)

Planning Poker is designed to run on **one shared server** (VPS, cloud VM, or on-prem server).  
All team members access it via a browser using the server’s URL.

> ❗ This is **not** installed on each team member’s computer.

---

### 🔧 Requirements

- Docker
- Docker Compose (optional but recommended)

That's it.

---

### 📦 Self-Hosting Steps

#### Quick Start with Docker Compose

1. **Clone this repository**

   ```bash
   git clone <repository-url>
   cd planning-poker/PlanningPoker/PlanningPoker
   ```

2. **Create `.env` file** (optional - only if you want GIF reactions)

   ```
   GIPHY_API_BASE_URL=https://api.giphy.com/v1/gifs/search?api_key=YOUR_API_KEY
   GIPHY_API_QUERY=limit=5&offset=0&rating=g&lang=en&bundle=low_bandwidth
   ```

   > **Note:** Get a free API key from [GIPHY Developers](https://developers.giphy.com/). The app works fine without it.

3. **Start the application**

   ```bash
   docker-compose up -d
   ```

4. **Access the application**

   - Open your browser: `http://localhost:8080`
   - Or use your server's IP/domain: `http://your-server-ip:8080`

5. **Stop the application**
   ```bash
   docker-compose down
   ```

#### Using Pre-built Docker Image

If you prefer using the pre-built image instead of building from source:

```bash
docker run -d \
  --name planning-poker \
  -p 8080:8080 \
  -v $(pwd)/database:/app/Database \
  -v $(pwd)/logs:/app/Logs \
  --restart unless-stopped \
  ghcr.io/rezahoque/planning-poker:latest
```

> **Note:** For Windows PowerShell, use backticks (`) instead of backslashes (\) for line continuation.

#### Production Tips

- **Use a reverse proxy** (nginx, Caddy) for SSL/TLS
- **Backup the `./database` directory** regularly
- **Change the port** by modifying `8080:8080` in docker-compose.yml
- **Update the app**: `docker-compose pull && docker-compose up -d`

For detailed instructions, see [README-DOCKER.md](PlanningPoker/PlanningPoker/README-DOCKER.md).
