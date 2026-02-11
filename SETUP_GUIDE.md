# Let's Chat App - Complete Setup Guide

This guide covers the setup of both backend and frontend projects.

## Architecture Overview

```
┌─────────────────────────────┐
│  LetsChatAppFrontEnd        │
│  (React + TypeScript + MUI) │
│  Port: 5173                 │
└──────────┬──────────────────┘
           │
           │ SignalR + REST API
           │
┌──────────▼──────────────────┐
│  LetsChatAppBackEnd         │
│  (ASP.NET Core Minimal API) │
│  Port: 5001/7001            │
└──────────┬──────────────────┘
           │
           ├─► Ollama (AI Models)
           │   Port: 11434
           │
           └─► SQLite Vector Store
               (vector-store.db)
```

## Prerequisites

### Required Software
1. **.NET 9 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
2. **Node.js 18+** - [Download](https://nodejs.org/)
3. **Ollama** - [Download](https://ollama.ai/)

### Required Ollama Models
```bash
ollama pull llama3.2
ollama pull all-minilm
```

## Backend Setup (LetsChatAppBackEnd)

### 1. Navigate to Backend Directory
```bash
cd LetsChatAppBackEnd
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Build the Project
```bash
dotnet build
```

### 4. Run the Backend
```bash
dotnet run
```

The backend will start on:
- HTTP: `http://localhost:5001`
- HTTPS: `https://localhost:7001`

### 5. Verify Backend is Running
Open browser to: `http://localhost:5001/api/documents`

You should see an empty array `[]` if no documents are uploaded yet.

## Frontend Setup (LetsChatAppFrontEnd)

### 1. Navigate to Frontend Directory
```bash
cd LetsChatAppFrontEnd
```

### 2. Install Dependencies
```bash
npm install
```

### 3. Start Development Server
```bash
npm run dev
```

The frontend will start on: `http://localhost:5173`

## Usage

### 1. Upload Documents
- Navigate to the **Documents** page
- Click **Upload PDF** button
- Select a PDF file
- Wait for ingestion to complete

### 2. Start Chatting
- Navigate to the **Chat** page
- Type your question about the uploaded documents
- Press Enter or click Send
- Watch the AI response stream in real-time

### 3. Manage Documents
- View all uploaded documents
- Delete documents
- Re-ingest documents if needed

## API Endpoints

### SignalR Hub
- `ws://localhost:5001/chatHub` - Real-time chat connection

### REST Endpoints
- `GET /api/documents` - List all documents
- `POST /api/documents/upload` - Upload a PDF
- `DELETE /api/documents/{documentId}` - Delete a document
- `POST /api/documents/reingest` - Re-ingest all documents
- `POST /api/search` - Semantic search

## Configuration

### Backend (appsettings.json)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### Frontend (vite.config.ts)
```typescript
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5001',
        changeOrigin: true,
        secure: false,
      }
    }
  }
})
```

## Troubleshooting

### Backend Issues

**1. Ollama Connection Error**
```
Error: Failed to connect to Ollama
```
**Solution:** Ensure Ollama is running:
```bash
ollama serve
```

**2. Model Not Found**
```
Error: Model 'llama3.2' not found
```
**Solution:** Pull the required models:
```bash
ollama pull llama3.2
ollama pull all-minilm
```

**3. SQLite Error**
```
Error: Unable to open database file
```
**Solution:** Ensure the application has write permissions to the directory.

### Frontend Issues

**1. SignalR Connection Failed**
```
Error: Failed to connect to chat server
```
**Solution:**
- Ensure backend is running on `http://localhost:5001`
- Check CORS configuration in backend
- Check browser console for detailed errors

**2. API Request Failed**
```
Error: Failed to load documents
```
**Solution:**
- Verify backend is running
- Check network tab in browser dev tools
- Ensure API URLs match in `documentService.ts` and `chatService.ts`

**3. Module Not Found**
```
Error: Cannot find module '@mui/material'
```
**Solution:**
```bash
cd LetsChatAppFrontEnd
rm -rf node_modules package-lock.json
npm install
```

## Development Tips

### Hot Reload
Both frontend and backend support hot reload:
- **Frontend:** Vite automatically reloads on file changes
- **Backend:** Use `dotnet watch run` for automatic restart

### Debugging

**Backend:**
- Use Visual Studio or VS Code with C# extension
- Set breakpoints in Program.cs or service files
- Press F5 to start debugging

**Frontend:**
- Use React DevTools browser extension
- Use browser's built-in DevTools (F12)
- Check Console and Network tabs

### Adding New Features

**Backend:**
1. Add new endpoint in `Program.cs`
2. Create service in `Services/` folder
3. Update models in `Models/` folder

**Frontend:**
1. Create new component in `src/components/`
2. Add new page in `src/pages/`
3. Update routing in `App.tsx`

## Production Build

### Backend
```bash
cd LetsChatAppBackEnd
dotnet publish -c Release -o ./publish
```

### Frontend
```bash
cd LetsChatAppFrontEnd
npm run build
```

The production files will be in `LetsChatAppFrontEnd/dist/`

## Security Considerations

⚠️ **Important:** This is a development setup. For production:

1. **Add Authentication** - Implement JWT or OAuth
2. **Configure HTTPS** - Use proper SSL certificates
3. **Validate Uploads** - Scan PDFs for malicious content
4. **Rate Limiting** - Prevent abuse of API endpoints
5. **Environment Variables** - Use for sensitive configuration
6. **Content Security** - Sanitize all user inputs
7. **CORS** - Restrict to specific domains only

## Next Steps

- [ ] Add user authentication
- [ ] Implement conversation history persistence
- [ ] Add file size limits for uploads
- [ ] Add support for more file types (DOCX, TXT)
- [ ] Implement search filters and advanced queries
- [ ] Add export functionality for conversations
- [ ] Deploy to cloud (Azure, AWS, etc.)

## Support

For issues or questions:
1. Check this README
2. Review error logs in browser console and backend terminal
3. Verify all prerequisites are installed
4. Ensure all services (Ollama, Backend, Frontend) are running

## License

This project is for educational purposes.
