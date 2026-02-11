import { Box, Chip, Paper, Typography } from '@mui/material'
import ArticleIcon from '@mui/icons-material/Article'
import { Citation } from '../utils/citationParser'

interface CitationListProps {
  citations: Citation[]
  onViewDocument: (filename: string, pageNumber: number) => void
}

const CitationList = ({ citations, onViewDocument }: CitationListProps) => {
  if (citations.length === 0) {
    return null
  }

  return (
    <Paper
      elevation={0}
      sx={{
        mt: 2,
        p: 2,
        bgcolor: 'grey.50',
        borderLeft: '4px solid',
        borderColor: 'primary.main',
      }}
    >
      <Typography variant="subtitle2" fontWeight="bold" gutterBottom>
        📚 Sources:
      </Typography>
      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
        {citations.map((citation, index) => (
          <Chip
            key={index}
            icon={<ArticleIcon />}
            label={`${citation.filename} (Page ${citation.pageNumber})`}
            onClick={() => onViewDocument(citation.filename, citation.pageNumber)}
            clickable
            color="primary"
            variant="outlined"
            sx={{
              justifyContent: 'flex-start',
              '& .MuiChip-label': {
                flex: 1,
                textAlign: 'left',
              },
            }}
          />
        ))}
      </Box>
    </Paper>
  )
}

export default CitationList
