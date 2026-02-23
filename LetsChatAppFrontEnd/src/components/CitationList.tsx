import { Box, Chip, Paper, Typography, Tooltip } from '@mui/material'
import ArticleIcon from '@mui/icons-material/Article'
import OpenInNewIcon from '@mui/icons-material/OpenInNew'
import { Citation } from '../utils/citationParser'

interface CitationListProps {
  citations: Citation[]
  onViewDocument: (filename: string, pageNumber: number) => void
}

const CitationList = ({ citations, onViewDocument }: CitationListProps) => {
  if (citations.length === 0) {
    return null
  }

  // Group citations by filename
  const groupedCitations = citations.reduce((acc, citation) => {
    if (!acc[citation.filename]) {
      acc[citation.filename] = []
    }
    acc[citation.filename].push(citation.pageNumber)
    return acc
  }, {} as Record<string, number[]>)

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
      <Typography variant="subtitle2" fontWeight="bold" gutterBottom sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
        📚 Sources
        <Typography component="span" variant="caption" color="text.secondary">
          (click to view)
        </Typography>
      </Typography>
      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
        {Object.entries(groupedCitations).map(([filename, pages]) => (
          <Box key={filename} sx={{ display: 'flex', flexDirection: 'column', gap: 0.5 }}>
            <Typography variant="caption" color="text.secondary" sx={{ fontWeight: 'medium' }}>
              {filename}
            </Typography>
            <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
              {pages.map((pageNumber, index) => (
                <Tooltip key={index} title={`Open ${filename} at page ${pageNumber}`} arrow>
                  <Chip
                    icon={<ArticleIcon fontSize="small" />}
                    deleteIcon={<OpenInNewIcon fontSize="small" />}
                    label={`Page ${pageNumber}`}
                    onClick={() => onViewDocument(filename, pageNumber)}
                    onDelete={() => onViewDocument(filename, pageNumber)}
                    clickable
                    size="small"
                    color="primary"
                    variant="outlined"
                    sx={{
                      '&:hover': {
                        bgcolor: 'primary.light',
                        borderColor: 'primary.main',
                        '& .MuiChip-deleteIcon': {
                          color: 'primary.main',
                        },
                      },
                      '& .MuiChip-deleteIcon': {
                        fontSize: '1rem',
                      },
                    }}
                  />
                </Tooltip>
              ))}
            </Box>
          </Box>
        ))}
      </Box>
    </Paper>
  )
}

export default CitationList
