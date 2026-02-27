# Check if Ollama models are downloaded
Write-Host "Checking Ollama container status..." -ForegroundColor Cyan
docker ps -a --filter "name=letschat-ollama"

Write-Host "`nChecking Ollama service health..." -ForegroundColor Cyan
docker inspect letschat-ollama --format='{{.State.Health.Status}}'

Write-Host "`nChecking downloaded models in Ollama..." -ForegroundColor Green
docker exec -it letschat-ollama ollama list

Write-Host "`nChecking ollama-setup logs..." -ForegroundColor Yellow
docker logs letschat-ollama-setup

Write-Host "`nTo manually pull a model, run:" -ForegroundColor Cyan
Write-Host "docker exec -it letschat-ollama ollama pull llama3.2" -ForegroundColor White
