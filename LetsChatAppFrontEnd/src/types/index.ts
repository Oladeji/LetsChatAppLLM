export interface ChatMessage {
  role: 'user' | 'assistant'
  content: string
  timestamp: Date
}

export interface SearchResult {
  documentId: string
  pageNumber: number
  text: string
}

export interface DocumentInfo {
  documentId: string
  sourceId: string
  documentVersion: string
}

export interface UploadResponse {
  success: boolean
  message: string
  documentId?: string
}
