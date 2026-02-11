import React, { createContext, useContext, useState, ReactNode } from 'react'
import { ChatMessage } from '../types'

interface ChatContextType {
  messages: ChatMessage[]
  conversationId: string | null
  isLoading: boolean
  addMessage: (message: ChatMessage) => void
  setMessages: (messages: ChatMessage[]) => void
  setConversationId: (id: string | null) => void
  setIsLoading: (loading: boolean) => void
  resetConversation: () => void
}

const ChatContext = createContext<ChatContextType | undefined>(undefined)

export const ChatProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [messages, setMessages] = useState<ChatMessage[]>([])
  const [conversationId, setConversationId] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(false)

  const addMessage = (message: ChatMessage) => {
    setMessages(prev => [...prev, message])
  }

  const resetConversation = () => {
    setMessages([])
    setConversationId(null)
    setIsLoading(false)
  }

  return (
    <ChatContext.Provider
      value={{
        messages,
        conversationId,
        isLoading,
        addMessage,
        setMessages,
        setConversationId,
        setIsLoading,
        resetConversation,
      }}
    >
      {children}
    </ChatContext.Provider>
  )
}

export const useChat = () => {
  const context = useContext(ChatContext)
  if (context === undefined) {
    throw new Error('useChat must be used within a ChatProvider')
  }
  return context
}
