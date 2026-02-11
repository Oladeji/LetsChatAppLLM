@echo off
echo ====================================
echo Let's Chat App - Quick Start
echo ====================================
echo.

echo Checking prerequisites...
echo.

REM Check .NET
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] .NET SDK not found. Please install .NET 9 SDK.
    echo Download: https://dotnet.microsoft.com/download/dotnet/9.0
    pause
    exit /b 1
)
echo [OK] .NET SDK found

REM Check Node
node --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] Node.js not found. Please install Node.js 18+
    echo Download: https://nodejs.org/
    pause
    exit /b 1
)
echo [OK] Node.js found

REM Check npm
npm --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] npm not found. Please install npm
    pause
    exit /b 1
)
echo [OK] npm found

REM Check Ollama
curl -s http://localhost:11434/api/tags >nul 2>&1
if %errorlevel% neq 0 (
    echo [WARNING] Ollama not responding on localhost:11434
    echo Please ensure Ollama is running: ollama serve
    echo.
) else (
    echo [OK] Ollama is running
)

echo.
echo ====================================
echo Starting Backend...
echo ====================================
echo.

cd LetsChatAppBackEnd

REM Restore and build backend
dotnet restore
if %errorlevel% neq 0 (
    echo [ERROR] Failed to restore backend dependencies
    pause
    exit /b 1
)

dotnet build
if %errorlevel% neq 0 (
    echo [ERROR] Failed to build backend
    pause
    exit /b 1
)

echo.
echo Backend built successfully!
echo Starting backend server...
echo.

REM Start backend in new window
start "LetsChatApp Backend" cmd /k "dotnet run"

cd ..

echo.
echo ====================================
echo Starting Frontend...
echo ====================================
echo.

cd LetsChatAppFrontEnd

REM Check if node_modules exists
if not exist "node_modules\" (
    echo Installing frontend dependencies...
    call npm install
    if %errorlevel% neq 0 (
        echo [ERROR] Failed to install frontend dependencies
        pause
        exit /b 1
    )
) else (
    echo [OK] Dependencies already installed
)

echo.
echo Starting frontend server...
echo.

REM Start frontend in new window
start "LetsChatApp Frontend" cmd /k "npm run dev"

cd ..

echo.
echo ====================================
echo Applications Started!
echo ====================================
echo.
echo Backend:  http://localhost:5001
echo Frontend: http://localhost:5173
echo.
echo Press any key to view the setup guide...
pause >nul

type SETUP_GUIDE.md
echo.
pause
