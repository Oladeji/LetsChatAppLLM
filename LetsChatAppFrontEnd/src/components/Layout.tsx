import { ReactNode } from 'react'
import { Link as RouterLink } from 'react-router-dom'
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

interface LayoutProps {
  children: ReactNode
}

const Layout = ({ children }: LayoutProps) => {
  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', height: '100vh' }}>
      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            Let's Chat App
          </Typography>
          <Button
            color="inherit"
            component={RouterLink}
            to="/"
            startIcon={<ChatIcon />}
          >
            Chat
          </Button>
          <Button
            color="inherit"
            component={RouterLink}
            to="/documents"
            startIcon={<FolderIcon />}
          >
            Documents
          </Button>
        </Toolbar>
      </AppBar>
      <Container
        maxWidth="xl"
        sx={{
          flex: 1,
          display: 'flex',
          flexDirection: 'column',
          overflow: 'hidden',
          py: 2,
        }}
      >
        {children}
      </Container>
    </Box>
  )
}

export default Layout
