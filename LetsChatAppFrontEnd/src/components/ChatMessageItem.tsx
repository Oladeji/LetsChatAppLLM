
import { Box, Paper, Typography, Avatar, Fade } from '@mui/material'
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
    <Fade in timeout={500}>
      <Box
        sx={{
          display: 'flex',
          justifyContent: isUser ? 'flex-end' : 'flex-start',
          mb: 3,
        }}
      >
        <Box
          sx={{
            display: 'flex',
            flexDirection: isUser ? 'row-reverse' : 'row',
            alignItems: 'flex-start',
            maxWidth: '80%',
            gap: 1.5,
          }}
        >
          <Avatar
            sx={{
              background: isUser 
                ? 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'
                : 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
              width: 40,
              height: 40,
              boxShadow: '0 4px 12px rgba(0,0,0,0.15)',
            }}
          >
            {isUser ? <PersonIcon /> : <SmartToyIcon />}
          </Avatar>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5 }}>
            <Paper
              elevation={0}
              sx={{
                p: 2.5,
                background: isUser 
                  ? 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'
                  : '#ffffff',
                color: isUser ? 'white' : 'text.primary',
                borderRadius: 3,
                border: isUser ? 'none' : '1px solid',
                borderColor: 'divider',
                boxShadow: isUser 
                  ? '0 4px 12px rgba(102, 126, 234, 0.3)'
                  : '0 2px 8px rgba(0,0,0,0.05)',
                transition: 'all 0.3s ease',
                '&:hover': {
                  boxShadow: isUser
                    ? '0 6px 16px rgba(102, 126, 234, 0.4)'
                    : '0 4px 12px rgba(0,0,0,0.1)',
                  transform: 'translateY(-1px)',
                },
              }}
            >
              {isUser ? (
                <Typography variant="body1" sx={{ lineHeight: 1.6 }}>
                  {content}
                </Typography>
              ) : (
                <Box 
                  sx={{ 
                    '& p': { m: 0, mb: 1, lineHeight: 1.7, '&:last-child': { mb: 0 } },
                    '& pre': { 
                      bgcolor: 'grey.900', 
                      color: 'grey.100',
                      p: 2, 
                      borderRadius: 2,
                      overflow: 'auto',
                      fontSize: '0.9em',
                    },
                    '& code': {
                      bgcolor: 'grey.100',
                      px: 0.8,
                      py: 0.3,
                      borderRadius: 1,
                      fontSize: '0.9em',
                      fontFamily: 'monospace',
                    },
                    '& pre code': {
                      bgcolor: 'transparent',
                      p: 0,
                    },
                    '& ul, & ol': {
                      pl: 3,
                      my: 1,
                    },
                    '& li': {
                      mb: 0.5,
                    },
                    '& strong': {
                      fontWeight: 600,
                      color: 'primary.main',
                    },
                  }}
                >
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
    </Fade>
  )
}

export default ChatMessageItem
