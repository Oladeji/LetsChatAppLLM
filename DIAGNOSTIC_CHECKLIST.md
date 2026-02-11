# 🚨 SignalR Connection Issues - Complete Diagnostic Guide

## Current Status
- ✅ SignalR connects successfully (you see "✅ SignalR Connected")
- ❌ Messages not reaching the server

## 🔍 Step-by-Step Diagnosis

### Step 1: Verify Backend is Running
**Open backend terminal and check for:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5010
```

**If NOT running:**
```sh
cd LetsChatAppBackEnd
dotnet run
```

### Step 2: Verify Ollama is Running
```sh
# Test Ollama
curl http://localhost:11434/api/tags

# If it fails, start Ollama
ollama serve

# Pull required models
ollama pull llama3.2
ollama pull all-minilm
```

### Step 3: Test Message Flow

1. **Open Browser Console** (F12)
2. **Navigate to Chat page**
3. **Type "test" and send**

**You should see in Browser Console:**
```
🎯 handleSendMessage called with: test
✅ Chat service is connected
📤 Attempting to send message via SignalR...
🚀 Sending message: test
✅ Message sent to server
📨 Message streaming started
📦 Received chunk
📦 Received chunk
...
🏁 Message streaming completed
```

**You should see in Backend Console:**
```
📨 Received message: test
✅ Started streaming response
✅ Finished streaming response
```

### Step 4: Identify the Failure Point

#### A) If you see in browser console:
```
🎯 handleSendMessage called with: test
❌ Not connected to chat server
```
**Problem:** SignalR disconnected after initial connection
**Solution:** 
- Restart both frontend and backend
- Check CORS configuration

#### B) If you see in browser console:
```
🎯 handleSendMessage called with: test
✅ Chat service is connected
📤 Attempting to send message via SignalR...
❌ Failed to send message: [error]
```
**Problem:** SignalR invoke() failing
**Solution:** Check backend hub method signature

#### C) If you see in browser console everything up to "✅ Message sent to server" but no chunks:
**Problem:** Backend hub method failing
**Solution:** Check backend console for errors

#### D) If you see in backend console:
```
📨 Received message: test
✅ Started streaming response
❌ Error in SendMessage: [error]
```
**Problem:** AI chat client or Ollama connection issue
**Solution:** 
1. Verify Ollama is running
2. Check models are available
3. Test Ollama directly:
```sh
curl http://localhost:11434/api/generate -d '{"model":"llama3.2","prompt":"hello","stream":false}'
```

## 🔧 Quick Fixes

### Fix 1: Restart Everything
```sh
# Terminal 1: Backend
cd LetsChatAppBackEnd
# Ctrl+C to stop
dotnet run

# Terminal 2: Frontend  
cd LetsChatAppFrontEnd
# Ctrl+C to stop
npm run dev

# Terminal 3: Ollama
ollama serve
```

### Fix 2: Clear Browser Cache
1. Press F12
2. Go to Network tab
3. Check "Disable cache"
4. Refresh page (Ctrl+R)

### Fix 3: Verify CORS
Check backend Program.cs has:
```csharp
app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

app.MapHub<ChatHub>("/chatHub").RequireCors("AllowReactApp");
```

### Fix 4: Test SignalR Hub Directly
**In backend, add test endpoint:**
```csharp
app.MapGet("/api/test", () => "Backend is working!");
```

**Test it:**
```sh
curl http://localhost:5010/api/test
```

## 📊 Network Tab Analysis

1. Open Browser DevTools (F12)
2. Go to **Network** tab
3. Filter for "chatHub"
4. Send a message
5. Click on the WebSocket connection

**What to look for:**
- **Status:** Should be "101 Switching Protocols"
- **Frames tab:** Should show messages being sent/received
- **Green dot:** Connection is active

**If you see frames being sent but no response:**
- Backend hub method is failing
- Check backend console for errors

**If you see no frames at all:**
- Message not reaching SignalR
- Check browser console for errors

## 🎯 Most Common Causes

### 1. Ollama Not Running (90% of cases)
```sh
ollama serve
ollama pull llama3.2
ollama pull all-minilm
```

### 2. Backend Not Running
```sh
cd LetsChatAppBackEnd
dotnet run
```

### 3. SignalR Connection Negotiation Issues
- Clear browser cache
- Restart both apps
- Check CORS

### 4. Port Mismatch
- Backend should be on **5010**
- Frontend should be on **5173**
- Check `chatService.ts` has `http://localhost:5010/chatHub`
- Check `documentService.ts` has `http://localhost:5010/api`

## 🧪 Test Script

Run this to test all components:

```sh
# Test Backend
curl http://localhost:5010/api/documents

# Test Ollama
curl http://localhost:11434/api/tags

# Test Ollama Generate
curl http://localhost:11434/api/generate -d '{"model":"llama3.2","prompt":"hi","stream":false}'
```

**All three should return data without errors!**

## 📝 What to Report Back

Please check and report:

1. **Backend Console Output** when you send a message
2. **Browser Console Output** when you send a message
3. **Results of test commands** above
4. **Network Tab** - do you see WebSocket frames?

This will help us pinpoint the exact issue! 🎯
