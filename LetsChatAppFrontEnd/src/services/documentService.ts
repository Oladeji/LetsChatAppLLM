import axios from 'axios'
import { DocumentInfo, UploadResponse } from '../types'

const API_BASE_URL = 'http://localhost:5010/api'

export const documentService = {
  async getDocuments(): Promise<DocumentInfo[]> {
    const response = await axios.get<DocumentInfo[]>(`${API_BASE_URL}/documents`)
    return response.data
  },

  async uploadDocument(file: File): Promise<UploadResponse> {
    const formData = new FormData()
    formData.append('file', file)
    
    const response = await axios.post<UploadResponse>(
      `${API_BASE_URL}/documents/upload`,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    )
    return response.data
  },

  async deleteDocument(documentId: string): Promise<void> {
    await axios.delete(`${API_BASE_URL}/documents/${documentId}`)
  },

  async reingestDocuments(): Promise<void> {
    await axios.post(`${API_BASE_URL}/documents/reingest`)
  },
}
