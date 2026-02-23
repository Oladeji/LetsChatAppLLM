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
    <Paper 
      elevation={0} 
      sx={{ 
        p: 2,
        border: '1px solid',
        borderColor: 'divider',
        borderRadius: 3,
        background: 'linear-gradient(to bottom, #ffffff, #f8fafc)',
        transition: 'all 0.3s ease',
        '&:hover': {
          borderColor: 'primary.main',
          boxShadow: '0 0 0 3px rgba(99, 102, 241, 0.1)',
        },
        '&:focus-within': {
          borderColor: 'primary.main',
          boxShadow: '0 0 0 3px rgba(99, 102, 241, 0.2)',
        },
      }}
    >
      <Box sx={{ display: 'flex', gap: 1.5, alignItems: 'flex-end' }}>
        <TextField
          fullWidth
          multiline
          maxRows={4}
          value={input}
          onChange={(e) => setInput(e.target.value)}
          onKeyPress={handleKeyPress}
          placeholder="Ask me anything about your documents..."
          disabled={disabled}
          variant="standard"
          InputProps={{
            disableUnderline: true,
            sx: {
              fontSize: '15px',
              '& textarea': {
                '&::placeholder': {
                  color: 'text.secondary',
                  opacity: 0.7,
                },
              },
            },
          }}
        />
        <IconButton
          onClick={handleSend}
          disabled={disabled || !input.trim()}
          sx={{
            bgcolor: input.trim() && !disabled ? 'primary.main' : 'grey.200',
            color: input.trim() && !disabled ? 'white' : 'grey.400',
            width: 42,
            height: 42,
            transition: 'all 0.3s ease',
            '&:hover': {
              bgcolor: input.trim() && !disabled ? 'primary.dark' : 'grey.300',
              transform: 'scale(1.05)',
            },
            '&:disabled': {
              bgcolor: 'grey.200',
            },
          }}
        >
          <SendIcon fontSize="small" />
        </IconButton>
      </Box>
    </Paper>
  )
}

export default ChatInput
