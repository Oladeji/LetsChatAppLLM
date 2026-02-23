import { ReactNode } from 'react'
import { Link as RouterLink, useLocation } from 'react-router-dom'
import {
  AppBar,
  Toolbar,
  Typography,
  Button,
  Box,
  Container,
} from '@mui/material'
import ChatIcon from '@mui/icons-material/Chat'
import FolderIcon from '@mui/icons-material/Folder'
import AutoAwesomeIcon from '@mui/icons-material/AutoAwesome'

interface LayoutProps {
  children: ReactNode
}

const Layout = ({ children }: LayoutProps) => {
  const location = useLocation()

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', height: '100vh' }}>
      <AppBar 
        position="static" 
        elevation={0}
        sx={{ 
          background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
          borderBottom: '1px solid rgba(255, 255, 255, 0.1)',
        }}
      >
        <Toolbar sx={{ py: 1 }}>
          <AutoAwesomeIcon sx={{ mr: 1.5, fontSize: 28 }} />
          <Typography 
            variant="h6" 
            component="div" 
            sx={{ 
              flexGrow: 1,
              fontWeight: 700,
              letterSpacing: '-0.02em',
              background: 'linear-gradient(to right, #fff, rgba(255,255,255,0.9))',
              backgroundClip: 'text',
              WebkitBackgroundClip: 'text',
              WebkitTextFillColor: 'transparent',
            }}
          >
            Let's Chat App
          </Typography>
          <Box sx={{ display: 'flex', gap: 1 }}>
            <Button
              color="inherit"
              component={RouterLink}
              to="/"
              startIcon={<ChatIcon />}
              sx={{
                borderRadius: 2,
                px: 2.5,
                bgcolor: location.pathname === '/' ? 'rgba(255,255,255,0.2)' : 'transparent',
                backdropFilter: location.pathname === '/' ? 'blur(10px)' : 'none',
                '&:hover': {
                  bgcolor: 'rgba(255,255,255,0.15)',
                },
                transition: 'all 0.3s ease',
              }}
            >
              Chat
            </Button>
            <Button
              color="inherit"
              component={RouterLink}
              to="/documents"
              startIcon={<FolderIcon />}
              sx={{
                borderRadius: 2,
                px: 2.5,
                bgcolor: location.pathname === '/documents' ? 'rgba(255,255,255,0.2)' : 'transparent',
                backdropFilter: location.pathname === '/documents' ? 'blur(10px)' : 'none',
                '&:hover': {
                  bgcolor: 'rgba(255,255,255,0.15)',
                },
                transition: 'all 0.3s ease',
              }}
            >
              Documents
            </Button>
          </Box>
        </Toolbar>
      </AppBar>
      <Container
        maxWidth="xl"
        sx={{
          flex: 1,
          display: 'flex',
          flexDirection: 'column',
          overflow: 'hidden',
          py: 3,
        }}
      >
        {children}
      </Container>
    </Box>
  )
}

export default Layout
