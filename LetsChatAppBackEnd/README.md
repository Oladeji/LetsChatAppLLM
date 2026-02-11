# LetsChatAppBackEnd

ASP.NET Core Minimal API backend for the Let's Chat application with RAG (Retrieval-Augmented Generation) capabilities.

## Features

- **SignalR Hub** for real-time chat streaming
- **Semantic Search** using vector embeddings (SQLite with vec extension)
- **Document Management** (upload, list, delete PDFs)
- **AI Integration** using Ollama (llama3.2 for chat, all-minilm for embeddings)
- **RESTful API** endpoints
- **CORS** configured for React frontend

## Prerequisites

- .NET 9 SDK
- Ollama running locally on `http://localhost:11434`
  - Required models: `llama3.2`, `all-minilm`

## API Endpoints

### SignalR Hub
- `/chatHub` - Real-time chat connection

### REST Endpoints
- `POST /api/search` - Semantic search in documents
- `GET /api/documents` - List all ingested documents
- `DELETE /api/documents/{documentId}` - Delete a document
- `POST /api/documents/upload` - Upload and ingest a PDF
- `POST /api/documents/reingest` - Re-ingest all documents

## Running the Application

```bash
cd LetsChatAppBackEnd
dotnet run
```

Default URL: `https://localhost:7001` or `http://localhost:5001`

## Configuration

Update `appsettings.json` or `appsettings.Development.json` to configure:
- Ollama endpoint
- Model names
- Vector store path
- Upload directory

## Project Structure

```
LetsChatAppBackEnd/
├── Hubs/
│   └── ChatHub.cs           # SignalR hub for chat
├── Models/
│   └── ApiModels.cs         # DTOs for API
├── Services/
│   ├── IngestedChunk.cs     # Vector store model for chunks
│   ├── IngestedDocument.cs  # Vector store model for documents
│   ├── SemanticSearch.cs    # Search service
│   └── Ingestion/
│       ├── DataIngestor.cs       # Ingestion orchestrator
│       ├── IIngestionSource.cs   # Ingestion source interface
│       ├── PDFDirectorySource.cs # Directory-based ingestion
│       └── PDFUploadSource.cs    # Upload-based ingestion
└── Program.cs               # Application entry point
```
