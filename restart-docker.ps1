# Clean up failed containers and restart
Write-Host "Stopping and removing containers..." -ForegroundColor Yellow
docker-compose down -v

Write-Host "`nStarting services..." -ForegroundColor Green
docker-compose up -d

Write-Host "`nWaiting for services to be healthy..." -ForegroundColor Cyan
Start-Sleep -Seconds 10

Write-Host "`nChecking container status:" -ForegroundColor Green
docker-compose ps

Write-Host "`nChecking if models are being pulled..." -ForegroundColor Cyan
Start-Sleep -Seconds 5
docker-compose logs ollama-setup

Write-Host "`n`nTo view live ollama-setup logs, run:" -ForegroundColor Yellow
Write-Host "docker-compose logs -f ollama-setup" -ForegroundColor White

Write-Host "`nTo check downloaded models, run:" -ForegroundColor Yellow
Write-Host "docker exec -it letschat-ollama ollama list" -ForegroundColor White
