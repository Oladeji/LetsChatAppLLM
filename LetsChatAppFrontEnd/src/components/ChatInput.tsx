import React, { useState } from 'react'
import { Box, TextField, IconButton, Paper } from '@mui/material'
import SendIcon from '@mui/icons-material/Send'

interface ChatInputProps {
  onSend: (message: string) => void
  disabled?: boolean
}

const ChatInput = ({ onSend, disabled = false }: ChatInputProps) => {
  const [input, setInput] = useState('')

  const handleSend = () => {
    if (input.trim() && !disabled) {
      onSend(input.trim())
      setInput('')
    }
  }

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault()
      handleSend()
    }
  }

  return (
    <Paper elevation={3} sx={{ p: 2 }}>
      <Box sx={{ display: 'flex', gap: 1 }}>
        <TextField
          fullWidth
          multiline
          maxRows={4}
          value={input}
          onChange={(e) => setInput(e.target.value)}
          onKeyPress={handleKeyPress}
          placeholder="Type your message..."
          disabled={disabled}
          variant="outlined"
        />
        <IconButton
          color="primary"
          onClick={handleSend}
          disabled={disabled || !input.trim()}
          sx={{ alignSelf: 'flex-end' }}
        >
          <SendIcon />
        </IconButton>
      </Box>
    </Paper>
  )
}

export default ChatInput
