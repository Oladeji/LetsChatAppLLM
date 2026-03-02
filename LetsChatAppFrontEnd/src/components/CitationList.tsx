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
        p: 2.5,
        bgcolor: 'grey.50',
        border: '1px solid',
        borderColor: 'grey.200',
        borderRadius: 2,
        transition: 'all 0.3s ease',
        '&:hover': {
          bgcolor: 'grey.100',
        },
      }}
    >
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1.5 }}>
        <ArticleIcon sx={{ fontSize: 18, color: 'primary.main' }} />
        <Typography 
          variant="subtitle2" 
          sx={{ 
            fontWeight: 700,
            color: 'text.primary',
            letterSpacing: '0.3px',
          }}
        >
          Sources
        </Typography>
        <Typography 
          component="span" 
          variant="caption" 
          sx={{ 
            color: 'text.secondary',
            fontStyle: 'italic',
          }}
        >
          (click to view)
        </Typography>
      </Box>
      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.5 }}>
        {Object.entries(groupedCitations).map(([filename, pages]) => (
          <Box key={filename}>
            <Typography 
              variant="body2" 
              sx={{ 
                mb: 0.8,
                fontWeight: 600,
                color: 'text.primary',
                fontSize: '0.875rem',
              }}
            >
              {filename}
            </Typography>
            <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.8 }}>
              {pages.map((pageNumber, index) => (
                <Tooltip 
                  key={index} 
                  title={`Open ${filename} at page ${pageNumber}`} 
                  arrow
                >
                  <Chip
                    icon={<ArticleIcon sx={{ fontSize: 16 }} />}
                    deleteIcon={<OpenInNewIcon sx={{ fontSize: 16 }} />}
                    label={`Page ${pageNumber}`}
                    onClick={() => onViewDocument(filename, pageNumber)}
                    onDelete={() => onViewDocument(filename, pageNumber)}
                    clickable
                    size="small"
                    sx={{
                      bgcolor: 'white',
                      border: '1.5px solid',
                      borderColor: 'primary.light',
                      color: 'primary.main',
                      fontWeight: 600,
                      fontSize: '0.75rem',
                      transition: 'all 0.2s ease',
                      '&:hover': {
                        bgcolor: 'primary.main',
                        borderColor: 'primary.main',
                        color: 'white',
                        transform: 'translateY(-2px)',
                        boxShadow: '0 4px 12px rgba(99, 102, 241, 0.3)',
                        '& .MuiChip-icon': {
                          color: 'white',
                        },
                        '& .MuiChip-deleteIcon': {
                          color: 'white',
                        },
                      },
                      '& .MuiChip-icon': {
                        color: 'primary.main',
                        transition: 'color 0.2s ease',
                      },
                      '& .MuiChip-deleteIcon': {
                        color: 'primary.main',
                        transition: 'color 0.2s ease',
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
