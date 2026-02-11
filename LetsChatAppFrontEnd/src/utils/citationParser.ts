// Parse citations from AI response text
export interface Citation {
  filename: string
  pageNumber: number
  quote: string
}

export function parseCitations(text: string): { content: string; citations: Citation[] } {
  const citations: Citation[] = []
  
  // Match citation XML tags
  const citationRegex = /<citation filename=['"]([^'"]+)['"] page_number=['"](\d+)['"]>([^<]+)<\/citation>/g
  
  let match
  while ((match = citationRegex.exec(text)) !== null) {
    citations.push({
      filename: match[1],
      pageNumber: parseInt(match[2]),
      quote: match[3].trim(),
    })
  }
  
  // Remove citation tags from content
  const cleanContent = text.replace(citationRegex, '').trim()
  
  return { content: cleanContent, citations }
}
