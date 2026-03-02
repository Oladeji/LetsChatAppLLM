import{ useEffect, useRef, useState } from 'react'
import { Box, Button, CircularProgress, Typography, Alert, Paper } from '@mui/material'
import RefreshIcon from '@mui/icons-material/Refresh'
import AutoAwesomeIcon from '@mui/icons-material/AutoAwesome'
import { useChat } from '../contexts/ChatContext'
import { chatService } from '../services/chatService'
import ChatMessageItem from '../components/ChatMessageItem'
import ChatInput from '../components/ChatInput'
import { ChatMessage } from '../types'

const Chat = () => {
  const { messages, conversationId, isLoading, addMessage, setConversationId, setIsLoading, resetConversation } = useChat()
  const [streamingMessage, setStreamingMessage] = useState<string>('')
  const [error, setError] = useState<string | null>(null)
  const messagesEndRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    const connectToHub = async () => {
      try {
        await chatService.connect()
      } catch (err) {
        setError('Failed to connect to chat server')
        console.error(err)
      }
    }

    connectToHub()

    // Don't disconnect on cleanup - keep connection alive
    // return () => {
    //   chatService.disconnect()
    // }
  }, [])

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' })
  }, [messages, streamingMessage])

  const handleSendMessage = async (content: string) => {
    console.log('🎯 handleSendMessage called with:', content)

    if (!chatService.isConnected()) {
      console.error('❌ Not connected to chat server')
      setError('Not connected to chat server')
      return
    }

    console.log('✅ Chat service is connected')

    const userMessage: ChatMessage = {
      role: 'user',
      content,
      timestamp: new Date(),
    }

    addMessage(userMessage)
    setIsLoading(true)
    setStreamingMessage('')
    setError(null)

    let fullMessage = ''

    try {
      console.log('📤 Attempting to send message via SignalR...')
      await chatService.sendMessage(
        content,
        messages,
        conversationId,
        (chunk) => {
          console.log('📦 Received chunk')
          fullMessage += chunk
          setStreamingMessage(fullMessage)
        },
        (newConversationId) => {
          console.log('🏁 Message streaming completed')
          const assistantMessage: ChatMessage = {
            role: 'assistant',
            content: fullMessage,
            timestamp: new Date(),
          }
          addMessage(assistantMessage)
          setStreamingMessage('')
          setIsLoading(false)
          if (newConversationId) {
            setConversationId(newConversationId)
          }
        },
        (error) => {
          setError(`Server error: ${error}`)
          setIsLoading(false)
        }
      )
    } catch (err) {
      setError('Failed to send message')
      setIsLoading(false)
      console.error(err)
    }
  }

  const handleReset = () => {
    resetConversation()
    setStreamingMessage('')
    setError(null)
  }

  const handleViewDocument = (filename: string, pageNumber: number) => {
    // Open PDF in new tab using the backend API endpoint
    // Backend is running on port 5010 (from launchSettings.json http profile)
    const apiUrl = import.meta.env.VITE_API_URL || 'http://localhost:5010'
    const url = `${apiUrl}/api/documents/view/${encodeURIComponent(filename)}#page=${pageNumber}`
    window.open(url, '_blank')
  }

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography 
          variant="h4" 
          sx={{ 
            fontWeight: 700,
            background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
            backgroundClip: 'text',
            WebkitBackgroundClip: 'text',
            WebkitTextFillColor: 'transparent',
          }}
        >
          AI Chat
        </Typography>
        <Button
          startIcon={<RefreshIcon />}
          onClick={handleReset}
          variant="outlined"
          sx={{
            borderRadius: 2,
            px: 2.5,
            borderWidth: 2,
            '&:hover': {
              borderWidth: 2,
              transform: 'translateY(-2px)',
              boxShadow: '0 4px 12px rgba(0,0,0,0.1)',
            },
            transition: 'all 0.3s ease',
          }}
        >
          New Chat
        </Button>
      </Box>

      {error && (
        <Alert 
          severity="error" 
          sx={{ 
            mb: 2,
            borderRadius: 2,
            border: '1px solid',
            borderColor: 'error.light',
          }} 
          onClose={() => setError(null)}
        >
          {error}
        </Alert>
      )}

      <Paper
        elevation={0}
        sx={{
          flex: 1,
          overflowY: 'auto',
          mb: 2,
          p: 3,
          border: '1px solid',
          borderColor: 'divider',
          borderRadius: 3,
          bgcolor: 'background.default',
          '&::-webkit-scrollbar': {
            width: '8px',
          },
          '&::-webkit-scrollbar-track': {
            bgcolor: 'transparent',
          },
          '&::-webkit-scrollbar-thumb': {
            bgcolor: 'grey.300',
            borderRadius: '4px',
            '&:hover': {
              bgcolor: 'grey.400',
            },
          },
        }}
      >
        {messages.length === 0 && !streamingMessage && (
          <Box 
            sx={{ 
              textAlign: 'center', 
              mt: 8,
              display: 'flex',
              flexDirection: 'column',
              alignItems: 'center',
              gap: 3,
            }}
          >
            <Box
              sx={{
                width: 80,
                height: 80,
                borderRadius: '20px',
                background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                boxShadow: '0 8px 24px rgba(102, 126, 234, 0.3)',
              }}
            >
              <AutoAwesomeIcon sx={{ fontSize: 40, color: 'white' }} />
            </Box>
            <Box>
              <Typography 
                variant="h5" 
                sx={{ 
                  fontWeight: 600, 
                  mb: 1,
                  color: 'text.primary',
                }}
              >
                Start a conversation
              </Typography>
              <Typography variant="body1" color="text.secondary">
                Ask me anything about your documents
              </Typography>
            </Box>
          </Box>
        )}

        {messages.map((message, index) => (
          <ChatMessageItem 
            key={index} 
            message={message} 
            onViewDocument={handleViewDocument}
          />
        ))}

        {streamingMessage && (
          <ChatMessageItem
            message={{
              role: 'assistant',
              content: streamingMessage,
              timestamp: new Date(),
            }}
            onViewDocument={handleViewDocument}
          />
        )}

        {isLoading && !streamingMessage && (
          <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
            <CircularProgress 
              sx={{
                color: 'primary.main',
              }}
            />
          </Box>
        )}

        <Box ref={messagesEndRef} />
      </Paper>

      <ChatInput onSend={handleSendMessage} disabled={isLoading} />
    </Box>
  )
}

export default Chat

