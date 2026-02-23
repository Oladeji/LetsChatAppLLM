import * as signalR from '@microsoft/signalr'
import { ChatMessage } from '../types'

const HUB_URL = 'http://localhost:5010/chatHub'

export class ChatService {
  private connection: signalR.HubConnection | null = null
  private messageChunkCallback: ((chunk: string) => void) | null = null
  private messageEndCallback: ((conversationId: string | null) => void) | null = null
  private errorCallback: ((error: string) => void) | null = null

  async connect(): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      return
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL, {
        skipNegotiation: false,
        transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.ServerSentEvents | signalR.HttpTransportType.LongPolling,
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build()

    this.connection.on('ReceiveMessageStart', () => {
      console.log('📨 Message streaming started')
    })

    this.connection.on('ReceiveMessageChunk', (chunk: string) => {
      console.log('📝 Received chunk:', chunk.substring(0, 50) + (chunk.length > 50 ? '...' : ''))
      if (this.messageChunkCallback) {
        this.messageChunkCallback(chunk)
      }
    })

    this.connection.on('ReceiveMessageEnd', (conversationId: string | null) => {
      console.log('✅ Message streaming ended')
      if (this.messageEndCallback) {
        this.messageEndCallback(conversationId)
      }
    })

    this.connection.on('ReceiveError', (error: string) => {
      console.error('❌ Server error:', error)
      if (this.errorCallback) {
        this.errorCallback(error)
      }
    })

    try {
      await this.connection.start()
      console.log('✅ SignalR Connected')
    } catch (err) {
      console.error('❌ SignalR connection failed:', err)
      throw err
    }
  }

  async disconnect(): Promise<void> {
    if (this.connection) {
      await this.connection.stop()
      this.connection = null
    }
  }

  async sendMessage(
    message: string,
    conversationHistory: ChatMessage[],
    conversationId: string | null,
    onChunk: (chunk: string) => void,
    onEnd: (conversationId: string | null) => void,
    onError?: (error: string) => void
  ): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      throw new Error('Not connected to chat hub')
    }

    console.log('🚀 Sending message:', message)

    this.messageChunkCallback = onChunk
    this.messageEndCallback = onEnd
    this.errorCallback = onError || null

    const history = conversationHistory.map(m => ({
      role: m.role,
      content: m.content,
    }))

    try {
      console.log('📤 Invoking SendMessage on server with conversationId:', conversationId)
      console.log('📤 Message content:', message)
      await this.connection.invoke('SendMessage', message, history, conversationId)
      console.log('✅ Message sent to server')
    } catch (err) {
      console.error('❌ Failed to send message:', err)
      throw err
    }
  }

  isConnected(): boolean {
    const connected = this.connection?.state === signalR.HubConnectionState.Connected
    console.log('?? isConnected check:', connected, 'State:', this.connection?.state)
    return connected
  }
}

export const chatService = new ChatService()

