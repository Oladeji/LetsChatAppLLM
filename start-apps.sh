#!/bin/bash

echo "===================================="
echo "Let's Chat App - Quick Start"
echo "===================================="
echo ""

echo "Checking prerequisites..."
echo ""

# Check .NET
if ! command -v dotnet &> /dev/null; then
    echo "[ERROR] .NET SDK not found. Please install .NET 9 SDK."
    echo "Download: https://dotnet.microsoft.com/download/dotnet/9.0"
    exit 1
fi
echo "[OK] .NET SDK found"

# Check Node
if ! command -v node &> /dev/null; then
    echo "[ERROR] Node.js not found. Please install Node.js 18+"
    echo "Download: https://nodejs.org/"
    exit 1
fi
echo "[OK] Node.js found"

# Check npm
if ! command -v npm &> /dev/null; then
    echo "[ERROR] npm not found. Please install npm"
    exit 1
fi
echo "[OK] npm found"

# Check Ollama
if ! curl -s http://localhost:11434/api/tags > /dev/null 2>&1; then
    echo "[WARNING] Ollama not responding on localhost:11434"
    echo "Please ensure Ollama is running: ollama serve"
    echo ""
else
    echo "[OK] Ollama is running"
fi

echo ""
echo "===================================="
echo "Starting Backend..."
echo "===================================="
echo ""

cd LetsChatAppBackEnd || exit

# Restore and build backend
dotnet restore
if [ $? -ne 0 ]; then
    echo "[ERROR] Failed to restore backend dependencies"
    exit 1
fi

dotnet build
if [ $? -ne 0 ]; then
    echo "[ERROR] Failed to build backend"
    exit 1
fi

echo ""
echo "Backend built successfully!"
echo "Starting backend server..."
echo ""

# Start backend in background
dotnet run &
BACKEND_PID=$!

cd ..

echo ""
echo "===================================="
echo "Starting Frontend..."
echo "===================================="
echo ""

cd LetsChatAppFrontEnd || exit

# Check if node_modules exists
if [ ! -d "node_modules" ]; then
    echo "Installing frontend dependencies..."
    npm install
    if [ $? -ne 0 ]; then
        echo "[ERROR] Failed to install frontend dependencies"
        kill $BACKEND_PID
        exit 1
    fi
else
    echo "[OK] Dependencies already installed"
fi

echo ""
echo "Starting frontend server..."
echo ""

# Start frontend
npm run dev &
FRONTEND_PID=$!

cd ..

echo ""
echo "===================================="
echo "Applications Started!"
echo "===================================="
echo ""
echo "Backend:  http://localhost:5001"
echo "Frontend: http://localhost:5173"
echo ""
echo "Press Ctrl+C to stop all services"
echo ""

# Wait for Ctrl+C
trap "kill $BACKEND_PID $FRONTEND_PID; exit" INT
wait
