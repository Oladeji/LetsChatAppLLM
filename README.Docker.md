# Docker Setup Guide

This guide explains how to run the Let's Chat App using Docker containers.

## Prerequisites

- Docker Desktop (Windows/Mac) or Docker Engine (Linux)
- Docker Compose v2.0 or higher
- At least 8GB of free disk space for Ollama models
- At least 4GB of available RAM

## Quick Start

### 1. Build and Start All Services

```bash
docker-compose up -d
```

This command will:
- Pull the Qdrant vector database image
- Pull the Ollama image
- Build the backend (.NET 9 application)
- Build the frontend (React application)
- Start all services
- Automatically download the required LLM models (llama3.2 and all-minilm)

### 2. Wait for Model Download

The first time you run the application, Ollama needs to download the models. This can take several minutes depending on your internet connection.

Check the logs:
```bash
docker-compose logs -f ollama-setup
```

### 3. Access the Application

- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **Ollama API**: http://localhost:11434

## Managing Services

### View Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f backend
docker-compose logs -f frontend
docker-compose logs -f ollama
```

### Stop Services

```bash
docker-compose down
```

### Stop and Remove Volumes (Clean Reset)

```bash
docker-compose down -v
```

**Warning**: This will delete all uploaded documents and the vector database.

### Restart a Single Service

```bash
docker-compose restart backend
```

### Rebuild Services

```bash
# Rebuild all services
docker-compose up -d --build

# Rebuild specific service
docker-compose up -d --build backend
```

## Architecture

The Docker setup includes five services:

1. **qdrant**: Vector database for storing embeddings and semantic search
2. **ollama**: The LLM service running llama3.2 and all-minilm models
3. **backend**: .NET 9 API with SignalR for real-time chat
4. **frontend**: React application served by Nginx
5. **ollama-setup**: One-time service to download required models

### Network

All services communicate through a bridge network called `letschat-network`.

### Volumes

- `qdrant-data`: Stores Qdrant vector database (persists between restarts)
- `ollama-data`: Stores Ollama models (persists between restarts)
- `backend-uploads`: Stores uploaded PDF documents

## Environment Configuration

### Backend Environment Variables

You can modify these in `docker-compose.yml`:

```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Development
  - ASPNETCORE_URLS=http://+:8080
  - OLLAMA_ENDPOINT=http://ollama:11434
  - QDRANT_ENDPOINT=http://qdrant:6333
  - ALLOWED_ORIGINS=http://localhost:3000,http://frontend:80
```

### Frontend Environment Variables

```yaml
environment:
  - VITE_API_URL=http://localhost:5000
```

## Troubleshooting

### Models Not Loading

If the models fail to download, manually pull them:

```bash
docker exec -it letschat-ollama ollama pull llama3.2
docker exec -it letschat-ollama ollama pull all-minilm
```

### Backend Connection Issues

Check if Ollama and Qdrant are healthy:
```bash
docker-compose ps
curl http://localhost:11434/api/tags
curl http://localhost:6333/healthz
```

### Qdrant Dashboard

Access the Qdrant web UI at http://localhost:6333/dashboard to view collections and manage vector data.

### Port Conflicts

If ports 3000, 5000, 6333, 6334, or 11434 are already in use, modify the port mappings in `docker-compose.yml`:

```yaml
ports:
  - "YOUR_PORT:80"  # Change YOUR_PORT to an available port
```

### Permission Issues (Linux)

If you encounter permission issues with volumes:
```bash
sudo chown -R $USER:$USER ./backend-uploads
```

## Production Deployment

For production deployment:

1. Update `ASPNETCORE_ENVIRONMENT` to `Production`
2. Add proper SSL/TLS certificates
3. Configure proper CORS origins
4. Use Docker secrets for sensitive configuration
5. Set up proper logging and monitoring
6. Consider using Docker Swarm or Kubernetes for orchestration

### Production Docker Compose Example

```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Production
  - ASPNETCORE_URLS=https://+:443;http://+:80
  - ALLOWED_ORIGINS=https://yourdomain.com
```

## Advanced Usage

### Scaling Services

While the Ollama and backend services are stateful, you can scale the frontend:

```bash
docker-compose up -d --scale frontend=3
```

### Custom Ollama Models

To use different models, modify the `ollama-setup` command in `docker-compose.yml`:

```yaml
command:
  - |
    ollama pull your-model-name --ollama-host http://ollama:11434
```

### Backup Volumes

```bash
# Backup uploads
docker run --rm -v letschat_backend-uploads:/data -v $(pwd):/backup alpine tar czf /backup/uploads-backup.tar.gz /data

# Backup Qdrant database
docker run --rm -v letschat_qdrant-data:/data -v $(pwd):/backup alpine tar czf /backup/qdrant-backup.tar.gz /data
```

## Resource Requirements

- **Qdrant**: 512MB RAM minimum (1GB+ recommended for large datasets)
- **Ollama**: 2GB RAM minimum (4GB+ recommended)
- **Backend**: 512MB RAM
- **Frontend**: 128MB RAM
- **Disk Space**: 8GB+ for models and data

## Health Checks

All services include health checks. View their status:

```bash
docker-compose ps
```

## Support

For issues and questions, refer to:
- Main project documentation
- TROUBLESHOOTING.md
- GitHub issues
