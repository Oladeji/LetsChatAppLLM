// Parse citations from AI response text
export interface Citation {
  filename: string
  pageNumber: number
  quote?: string
}

export function parseCitations(text: string): { content: string; citations: Citation[] } {
  const citations: Citation[] = []

  // Match old XML citation tags (for backward compatibility)
  const citationRegex = /<citation filename=['"]([^'"]+)['"] page_number=['"](\d+)['"]>([^<]+)<\/citation>/g

  let match
  while ((match = citationRegex.exec(text)) !== null) {
    citations.push({
      filename: match[1],
      pageNumber: parseInt(match[2]),
      quote: match[3].trim(),
    })
  }

  // Parse new markdown sources format
  // Format: **documentname.pdf** (Pages: 1, 2, 3)
  const markdownSourcesRegex = /\*\*([^*]+)\*\*\s*\(Pages?:\s*([0-9,\s]+)\)/g

  while ((match = markdownSourcesRegex.exec(text)) !== null) {
    const filename = match[1].trim()
    const pagesStr = match[2]

    // Parse multiple pages
    const pages = pagesStr.split(',').map(p => parseInt(p.trim())).filter(p => !isNaN(p))

    // Create a citation for each page
    pages.forEach(page => {
      // Avoid duplicates
      if (!citations.some(c => c.filename === filename && c.pageNumber === page)) {
        citations.push({
          filename,
          pageNumber: page,
        })
      }
    })
  }

  // Remove citation tags and sources section from content
  let cleanContent = text.replace(citationRegex, '').trim()

  // Remove the sources section (everything after the --- separator)
  const sourcesSeparatorIndex = cleanContent.indexOf('\n\n---\n\n**Sources:**')
  if (sourcesSeparatorIndex !== -1) {
    cleanContent = cleanContent.substring(0, sourcesSeparatorIndex).trim()
  }

  return { content: cleanContent, citations }
}
