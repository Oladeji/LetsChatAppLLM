
import { Box, Paper, Typography, Avatar } from '@mui/material'
import PersonIcon from '@mui/icons-material/Person'
import SmartToyIcon from '@mui/icons-material/SmartToy'
import ReactMarkdown from 'react-markdown'
import { ChatMessage } from '../types'
import { parseCitations } from '../utils/citationParser'
import CitationList from './CitationList'

interface ChatMessageItemProps {
  message: ChatMessage
  onViewDocument?: (filename: string, pageNumber: number) => void
}

const ChatMessageItem = ({ message, onViewDocument }: ChatMessageItemProps) => {
  const isUser = message.role === 'user'

  // Parse citations from assistant messages
  const { content, citations } = isUser 
    ? { content: message.content, citations: [] }
    : parseCitations(message.content)

  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: isUser ? 'flex-end' : 'flex-start',
        mb: 2,
      }}
    >
      <Box
        sx={{
          display: 'flex',
          flexDirection: isUser ? 'row-reverse' : 'row',
          alignItems: 'flex-start',
          maxWidth: '80%',
        }}
      >
        <Avatar
          sx={{
            bgcolor: isUser ? 'primary.main' : 'secondary.main',
            mx: 1,
          }}
        >
          {isUser ? <PersonIcon /> : <SmartToyIcon />}
        </Avatar>
        <Box sx={{ display: 'flex', flexDirection: 'column' }}>
          <Paper
            elevation={1}
            sx={{
              p: 2,
              bgcolor: isUser ? 'primary.light' : 'grey.100',
              color: isUser ? 'white' : 'text.primary',
            }}
          >
            {isUser ? (
              <Typography variant="body1">{content}</Typography>
            ) : (
              <Box sx={{ '& p': { m: 0 }, '& pre': { bgcolor: 'grey.900', p: 1, borderRadius: 1 } }}>
                <ReactMarkdown>{content}</ReactMarkdown>
              </Box>
            )}
          </Paper>
          {!isUser && citations.length > 0 && onViewDocument && (
            <CitationList citations={citations} onViewDocument={onViewDocument} />
          )}
        </Box>
      </Box>
    </Box>
  )
}

export default ChatMessageItem
