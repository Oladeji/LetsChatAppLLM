# SignalR Connection Diagnostics

## ✅ Changes Applied

### Backend (LetsChatAppBackEnd/Hubs/ChatHub.cs)
- ✅ Added comprehensive logging
- ✅ Added try-catch error handling
- ✅ Added console output for debugging
- ✅ Added `ReceiveError` event for client errors

### Frontend (LetsChatAppFrontEnd/src/services/chatService.ts)
- ✅ Added detailed logging for all events
- ✅ Added error callback handling
- ✅ Better connection diagnostics

### Frontend (LetsChatAppFrontEnd/src/pages/Chat.tsx)
- ✅ Added error handling callback
- ✅ Display server errors to user

## 🔍 How to Diagnose

### 1. **Restart Backend (REQUIRED)**
```sh
# Stop backend (Ctrl+C)
cd LetsChatAppBackEnd
dotnet run
```

### 2. **Restart Frontend (REQUIRED)**
```sh
# Stop frontend (Ctrl+C)
cd LetsChatAppFrontEnd
npm run dev
```

### 3. **Check Backend Console**
When you send a message, you should see:
```
📨 Received message: [your message]
✅ Started streaming response
✅ Finished streaming response
```

If you see errors instead:
```
❌ Error in SendMessage: [error message]
```

### 4. **Check Frontend Console**
When you send a message, you should see:
```
🚀 Sending message: [your message]
✅ Message sent to server
📨 Message streaming started
📝 Received chunk: [text]...
✅ Message streaming ended
```

## 🐛 Common Issues & Solutions

### Issue 1: "Ollama connection error"
**Backend Console Shows:**
```
❌ Error in SendMessage: Failed to connect to Ollama
```

**Solution:**
```sh
# Check if Ollama is running
curl http://localhost:11434/api/tags

# If not running, start it:
ollama serve

# Verify models are available:
ollama list
```

### Issue 2: "No documents to search"
**If you haven't uploaded documents yet:**

1. Go to Documents page
2. Click "Upload PDF"
3. Upload at least one PDF
4. Wait for success message
5. Go back to Chat page
6. Try asking about the document

### Issue 3: "Function calling error"
**Backend Console Shows:**
```
❌ Error in SearchAsync
```

**Solution:**
- Ensure SQLite vector database is initialized
- Check that documents were ingested successfully
- Restart backend to re-initialize database

### Issue 4: "SignalR negotiation error" (Your Current Issue)
**This error is harmless if:**
- ✅ You see "SignalR Connected" after the error
- ✅ WebSocket connection succeeds

**But no responses means:**
- ❌ Backend method is throwing an error
- ❌ Ollama is not running
- ❌ No connection to AI model

**Check:**
1. **Is Ollama running?**
   ```sh
   curl http://localhost:11434/api/generate -d '{"model":"llama3.2","prompt":"hi"}'
   ```

2. **Are models pulled?**
   ```sh
   ollama pull llama3.2
   ollama pull all-minilm
   ```

3. **Backend logs?**
   - Look for errors in backend console
   - Should see "📨 Received message" when you send

## 📊 Expected Flow

### Successful Message Flow:

1. **Frontend sends:**
   ```
   🚀 Sending message: What is this document about?
   ```

2. **Backend receives:**
   ```
   📨 Received message: What is this document about?
   ✅ Started streaming response
   ```

3. **Frontend receives chunks:**
   ```
   📨 Message streaming started
   📝 Received chunk: This document...
   📝 Received chunk: describes...
   📝 Received chunk: a product...
   ✅ Message streaming ended
   ```

4. **Backend completes:**
   ```
   ✅ Finished streaming response
   ```

## 🔧 Quick Test

### Test 1: Check Backend is Running
```sh
curl http://localhost:5010/api/documents
```
**Expected:** JSON array of documents (or empty array `[]`)

### Test 2: Check Ollama
```sh
curl http://localhost:11434/api/tags
```
**Expected:** List of available models

### Test 3: Check SignalR
Open browser console and navigate to Chat page.
**Expected:** "✅ SignalR Connected"

### Test 4: Send Simple Message
1. Type "hello" in chat
2. Press Send

**Backend Console Should Show:**
```
📨 Received message: hello
✅ Started streaming response
```

**Frontend Console Should Show:**
```
🚀 Sending message: hello
✅ Message sent to server
```

## ❌ If Nothing Happens

### Checklist:
- [ ] Backend running on http://localhost:5010
- [ ] Frontend running on http://localhost:5173
- [ ] Ollama running on http://localhost:11434
- [ ] Models `llama3.2` and `all-minilm` pulled
- [ ] No firewall blocking connections
- [ ] Browser console shows "✅ SignalR Connected"
- [ ] Backend console shows "📨 Received message" when you type

### Debug Steps:

1. **Check Backend Console** - Should show received messages
2. **Check Frontend Console** - Should show detailed logs
3. **Check Network Tab** (F12 → Network):
   - Look for `chatHub` WebSocket
   - Should show as "101 Switching Protocols"
   - Should be green/active

4. **Try Restarting Everything:**
   ```sh
   # Terminal 1: Stop and restart backend
   cd LetsChatAppBackEnd
   dotnet run
   
   # Terminal 2: Stop and restart frontend
   cd LetsChatAppFrontEnd
   npm run dev
   
   # Terminal 3: Ensure Ollama is running
   ollama serve
   ```

## 📝 What the Logs Mean

### Backend Logs
- `📨 Received message` = Backend got your message from frontend
- `✅ Started streaming response` = AI is generating response
- `✅ Finished streaming response` = AI completed response
- `❌ Error in SendMessage` = Something went wrong (check error details)

### Frontend Logs
- `✅ SignalR Connected` = Connection established
- `🚀 Sending message` = Your message is being sent
- `📨 Message streaming started` = Backend started responding
- `📝 Received chunk` = Receiving AI response in chunks
- `✅ Message streaming ended` = Response complete

## 🎯 Most Likely Issue

Based on your error, the most likely issues are:

1. **Ollama not running** - Start with `ollama serve`
2. **Models not pulled** - Run `ollama pull llama3.2` and `ollama pull all-minilm`
3. **Backend error** - Check backend console for error messages

## 🚀 Next Steps

1. **Restart both apps** (backend and frontend)
2. **Check all logs** (backend console, frontend console)
3. **Verify Ollama** is running with models
4. **Send a test message** and observe the logs
5. **Report back** what you see in the logs

The detailed logging will help us identify exactly where it's failing!
