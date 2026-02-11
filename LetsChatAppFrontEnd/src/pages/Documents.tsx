import React, { useEffect, useState, useRef } from "react";
import {
  Box,
  Button,
  Typography,
  Paper,
  List,
  ListItem,
  ListItemText,
  IconButton,
  Alert,
  CircularProgress,
  Divider,
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import UploadFileIcon from "@mui/icons-material/UploadFile";
import RefreshIcon from "@mui/icons-material/Refresh";
import { documentService } from "../services/documentService";
import { DocumentInfo } from "../types";

const Documents = () => {
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [documents, setDocuments] = useState<DocumentInfo[]>([]);
  const [loading, setLoading] = useState(false);
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  useEffect(() => {
    loadDocuments();
  }, []);

  const loadDocuments = async () => {
    setLoading(true);
    setError(null);
    try {
      const docs = await documentService.getDocuments();
      setDocuments(docs);
    } catch (err) {
      setError("Failed to load documents");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleFileUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    if (!file.name.toLowerCase().endsWith(".pdf")) {
      setError("Only PDF files are allowed");
      return;
    }

    setUploading(true);
    setError(null);
    setSuccess(null);

    try {
      const response = await documentService.uploadDocument(file);
      if (response.success) {
        setSuccess(`Document "${file.name}" uploaded successfully`);
        await loadDocuments();
      } else {
        setError(response.message);
      }
    } catch (err) {
      setError("Failed to upload document");
      console.error(err);
    } finally {
      setUploading(false);
      event.target.value = "";
    }
  };

  const handleDelete = async (documentId: string) => {
    if (!window.confirm(`Are you sure you want to delete "${documentId}"?`)) {
      return;
    }

    setError(null);
    setSuccess(null);

    try {
      await documentService.deleteDocument(documentId);
      setSuccess(`Document "${documentId}" deleted successfully`);
      await loadDocuments();
    } catch (err) {
      setError("Failed to delete document");
      console.error(err);
    }
  };

  const handleReingest = async () => {
    setLoading(true);
    setError(null);
    setSuccess(null);

    try {
      await documentService.reingestDocuments();
      setSuccess("Documents re-ingested successfully");
      await loadDocuments();
    } catch (err) {
      setError("Failed to re-ingest documents");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box sx={{ height: "100%", display: "flex", flexDirection: "column" }}>
      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 2 }}>
        <Typography variant="h4">Document Management</Typography>
        <Box sx={{ display: "flex", gap: 1 }}>
          <Button
            variant="outlined"
            startIcon={<RefreshIcon />}
            onClick={handleReingest}
            disabled={loading || uploading}
          >
            Re-ingest
          </Button>
          <Button
            variant="outlined"
            startIcon={<RefreshIcon />}
            onClick={loadDocuments}
            disabled={loading || uploading}
          >
            Refresh
          </Button>
          <Button
            variant="contained"
            startIcon={<UploadFileIcon />}
            onClick={() => fileInputRef.current?.click()}
            disabled={uploading}
          >
            {uploading ? "Uploading..." : "Upload PDF"}
          </Button>
          {/* @ts-ignore */}
          <input ref={fileInputRef} type="file" accept=".pdf" onChange={handleFileUpload} style={{ display: "none" }} />
        </Box>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError(null)}>
          {error}
        </Alert>
      )}

      {success && (
        <Alert severity="success" sx={{ mb: 2 }} onClose={() => setSuccess(null)}>
          {success}
        </Alert>
      )}

      <Paper sx={{ flex: 1, overflow: "auto", p: 2 }}>
        {loading && !uploading ? (
          <Box sx={{ display: "flex", justifyContent: "center", my: 4 }}>
            <CircularProgress />
          </Box>
        ) : documents.length === 0 ? (
          <Box sx={{ textAlign: "center", my: 4 }}>
            <Typography variant="h6" color="text.secondary">
              No documents found. Upload a PDF to get started.
            </Typography>
          </Box>
        ) : (
          <List>
            {documents.map((doc, index) => (
              <Box key={`${doc.documentId}-${index}`}>
                <ListItem
                  secondaryAction={
                    <IconButton edge="end" aria-label="delete" onClick={() => handleDelete(doc.documentId)}>
                      <DeleteIcon />
                    </IconButton>
                  }
                >
                  <ListItemText
                    primary={doc.documentId}
                    secondary={`Source: ${doc.sourceId} | Version: ${new Date(doc.documentVersion).toLocaleString()}`}
                  />
                </ListItem>
                {index < documents.length - 1 && <Divider />}
              </Box>
            ))}
          </List>
        )}
      </Paper>
    </Box>
  );
};

export default Documents;
