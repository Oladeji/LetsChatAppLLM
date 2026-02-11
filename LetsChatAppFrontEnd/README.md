# LetsChatAppFrontEnd

React + TypeScript frontend for Let's Chat application with real-time AI chat and document management.

## Features

- **Real-time Chat** using SignalR with streaming responses
- **Document Management** (upload, list, delete PDFs)
- **Material-UI (MUI)** components
- **React Context API** for state management
- **TypeScript** for type safety
- **Responsive Design**

## Prerequisites

- Node.js (v18 or higher)
- npm or yarn
- Backend running at `http://localhost:5001`

## Installation

```bash
cd LetsChatAppFrontEnd
npm install
```

## Running the Application

```bash
npm run dev
```

The app will be available at `http://localhost:5173`

## Build for Production

```bash
npm run build
npm run preview
```

## Project Structure

```
LetsChatAppFrontEnd/
├── src/
│   ├── components/
│   │   ├── Layout.tsx              # Main layout with navigation
│   │   ├── ChatInput.tsx           # Chat input component
│   │   └── ChatMessageItem.tsx     # Individual chat message
│   ├── contexts/
│   │   └── ChatContext.tsx         # Chat state management
│   ├── pages/
│   │   ├── Chat.tsx                # Chat page
│   │   └── Documents.tsx           # Document management page
│   ├── services/
│   │   ├── chatService.ts          # SignalR chat service
│   │   └── documentService.ts      # Document API service
│   ├── types/
│   │   └── index.ts                # TypeScript type definitions
│   ├── App.tsx                     # Main app component
│   └── main.tsx                    # Entry point
├── index.html
├── package.json
├── tsconfig.json
└── vite.config.ts
```

## Configuration

Update the API endpoints in:
- `src/services/chatService.ts` - SignalR hub URL
- `src/services/documentService.ts` - REST API base URL
- `vite.config.ts` - Proxy configuration

## Technologies Used

- **React 18** - UI library
- **TypeScript** - Type safety
- **Material-UI (MUI)** - Component library
- **SignalR** - Real-time communication
- **Axios** - HTTP client
- **React Router** - Routing
- **React Markdown** - Markdown rendering
- **Vite** - Build tool

## Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint
