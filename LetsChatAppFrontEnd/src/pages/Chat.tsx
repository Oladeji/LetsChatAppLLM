import{ useEffect, useRef, useState } from 'react'
import { Box, Button, CircularProgress, Typography, Alert } from '@mui/material'
import RefreshIcon from '@mui/icons-material/Refresh'
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
    // Open PDF in new tab
    const url = `http://localhost:5010/api/documents/view/${encodeURIComponent(filename)}#page=${pageNumber}`
    window.open(url, '_blank')
  }

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h4">Chat</Typography>
        <Button
          startIcon={<RefreshIcon />}
          onClick={handleReset}
          variant="outlined"
        >
          New Chat
        </Button>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
          {error}
        </Alert>
      )}

      <Box
        sx={{
          flex: 1,
          overflowY: 'auto',
          mb: 2,
          p: 2,
          border: '1px solid',
          borderColor: 'divider',
          borderRadius: 1,
        }}
      >
        {messages.length === 0 && !streamingMessage && (
          <Box sx={{ textAlign: 'center', mt: 4 }}>
            <Typography variant="h6" color="text.secondary">
              Start a conversation by asking a question about your documents
            </Typography>
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
          <Box sx={{ display: 'flex', justifyContent: 'center', my: 2 }}>
            <CircularProgress />
          </Box>
        )}

        <Box ref={messagesEndRef} />
      </Box>

      <ChatInput onSend={handleSendMessage} disabled={isLoading} />
    </Box>
  )
}

export default Chat

