# Troubleshooting Guide - Fixed Issues

## Issues Fixed

### 1. ✅ React Router Deprecation Warnings
**Problem:** React Router v6 showing future flag warnings

**Solution:** Added future flags to `BrowserRouter` in `main.tsx`:
```typescript
<BrowserRouter
  future={{
    v7_startTransition: true,
    v7_relativeSplatPath: true,
  }}
>
```

### 2. ✅ SignalR Connection Error
**Problem:** "The connection was stopped during negotiation"

**Solutions Applied:**

#### Backend (`Program.cs`)
- ✅ Reordered middleware: `UseHttpsRedirection()` before `UseCors()`
- ✅ Added CORS to SignalR hub: `.RequireCors("AllowReactApp")`

```csharp
app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

app.MapHub<ChatHub>("/chatHub").RequireCors("AllowReactApp");
```

#### Frontend (`chatService.ts`)
- ✅ Added multiple transport fallbacks (WebSockets → SSE → LongPolling)
- ✅ Added connection logging
- ✅ Better error handling

```typescript
.withUrl(HUB_URL, {
  skipNegotiation: false,
  transport: signalR.HttpTransportType.WebSockets | 
            signalR.HttpTransportType.ServerSentEvents | 
            signalR.HttpTransportType.LongPolling,
})
.withAutomaticReconnect()
.configureLogging(signalR.LogLevel.Information)
```

## How to Test

### 1. Restart Backend
```sh
# Stop current backend (Ctrl+C)
cd LetsChatAppBackEnd
dotnet run
```

### 2. Restart Frontend
```sh
# Stop current frontend (Ctrl+C)
cd LetsChatAppFrontEnd
npm run dev
```

### 3. Check Browser Console
You should now see:
```
✅ SignalR Connected
```

## Verification Checklist

- [ ] No React Router warnings in console
- [ ] SignalR connects successfully (see "✅ SignalR Connected")
- [ ] Backend running on `http://localhost:5010`
- [ ] Frontend running on `http://localhost:5173`
- [ ] Scalar API explorer accessible at `http://localhost:5010/scalar/v1`
- [ ] Can navigate between Chat and Documents pages
- [ ] No CORS errors in console

## Common Issues

### Issue: Still getting SignalR errors
**Check:**
1. Backend is running on port 5010
2. No firewall blocking connections
3. Browser console shows the correct URL: `ws://localhost:5010/chatHub`

### Issue: CORS errors
**Check:**
1. Frontend is running on port 5173
2. Backend CORS policy includes `http://localhost:5173`
3. Middleware order is correct (UseHttpsRedirection → UseCors)

### Issue: 404 on /chatHub
**Check:**
1. Backend Program.cs has `app.MapHub<ChatHub>("/chatHub")`
2. ChatHub.cs exists in Hubs folder
3. Backend is running in Development mode

## Expected Console Output

### Backend Console
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5010
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7226
```

### Frontend Console
```
[2026-02-11T15:XX:XX.XXXZ] Information: WebSocket connected to ws://localhost:5010/chatHub
✅ SignalR Connected
```

## Port Configuration Summary

| Service | URL | Purpose |
|---------|-----|---------|
| Backend HTTP | http://localhost:5010 | API & SignalR |
| Backend HTTPS | https://localhost:7226 | API & SignalR (SSL) |
| Frontend | http://localhost:5173 | React SPA |
| Scalar API | http://localhost:5010/scalar/v1 | API Documentation |
| SignalR Hub | ws://localhost:5010/chatHub | Real-time Chat |

## Next Steps

After verifying everything works:

1. **Upload a document:**
   - Go to Documents page
   - Click "Upload PDF"
   - Select a PDF file
   - Wait for success message

2. **Test chat:**
   - Go to Chat page
   - Type a question about your document
   - Watch the response stream in real-time

3. **Check Scalar:**
   - Navigate to http://localhost:5010/scalar/v1
   - Explore API endpoints
   - Test endpoints directly from the UI

## Additional Improvements Applied

### Better Error Messages
- SignalR connection now logs detailed errors
- Console shows connection status clearly

### Multiple Transport Support
- Tries WebSockets first (fastest)
- Falls back to Server-Sent Events if WebSocket fails
- Falls back to Long Polling if both fail

### Automatic Reconnection
- SignalR automatically reconnects if connection drops
- No manual intervention needed

## Debugging Tips

### Enable Verbose SignalR Logging
In `chatService.ts`, change:
```typescript
.configureLogging(signalR.LogLevel.Trace) // More verbose
```

### Check Network Tab
1. Open browser DevTools (F12)
2. Go to Network tab
3. Filter for "chatHub"
4. Look for WebSocket connection
5. Check status (should be 101 Switching Protocols)

### Backend Logging
The backend automatically logs SignalR events. Check the terminal for:
- Connection attempts
- Method invocations
- Disconnections

## Success Indicators

✅ No warnings in console  
✅ "✅ SignalR Connected" message  
✅ Can send messages in chat  
✅ Messages stream in real-time  
✅ Can upload documents  
✅ Documents appear in list  
✅ Scalar API explorer works  

If all indicators are green, the app is working correctly! 🎉
