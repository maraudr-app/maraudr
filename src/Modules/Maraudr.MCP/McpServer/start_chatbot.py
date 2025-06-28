#!/usr/bin/env python3
"""
API REST wrapper pour le client MCP CRM
Expose les fonctionnalités du chatbot via HTTP
"""

import asyncio
import json
import os
from typing import Dict, List, Any, Optional
from fastapi import FastAPI, HTTPException, BackgroundTasks
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from contextlib import asynccontextmanager
from dotenv import load_dotenv

# Charge les variables d'environnement
load_dotenv()

# Import du client MCP
from maraudr_mcp_client import CRMChatbot

# Modèles Pydantic pour l'API
class ChatMessage(BaseModel):
    message: str
    session_id: Optional[str] = "default"

class ChatResponse(BaseModel):
    response: str
    session_id: str
    timestamp: str

class ToolCall(BaseModel):
    tool_name: str
    arguments: Dict[str, Any]

class ToolResponse(BaseModel):
    result: str
    success: bool
    error: Optional[str] = None

# Gestionnaire global des sessions de chatbot
chatbot_sessions: Dict[str, CRMChatbot] = {}

@asynccontextmanager
async def lifespan(app: FastAPI):
    """Gestionnaire de cycle de vie de l'application"""
    print("🚀 Démarrage du serveur API MCP...")
    
    # Initialisation (optionnel: créer une session par défaut)
    yield
    
    # Nettoyage
    print("🧹 Nettoyage des sessions...")
    for session_id, chatbot in chatbot_sessions.items():
        try:
            await chatbot.cleanup()
        except Exception as e:
            print(f"Erreur lors du nettoyage de la session {session_id}: {e}")

# Création de l'app FastAPI
app = FastAPI(
    title="MARAUDR MCP API",
    description="API REST pour le chatbot MARAUDR basé sur MCP",
    version="1.0.0",
    lifespan=lifespan
)

# Configuration CORS pour le frontend
app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:3000"],  # React/Vue
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

async def get_or_create_chatbot(session_id: str) -> CRMChatbot:
    """Récupère ou crée un chatbot pour une session"""
    if session_id not in chatbot_sessions:
        # Configuration
        api_key = os.getenv("OPENAI_API_KEY") or os.getenv("OPENROUTER_API_KEY")
        if not api_key:
            raise HTTPException(status_code=500, detail="Aucune clé API configurée")
        
        model = "gpt-4o-mini"
        base_url = None
        
        if os.getenv("OPENROUTER_API_KEY"):
            model = "openai/gpt-4o-mini"
            base_url = "https://openrouter.ai/api/v1"
            api_key = os.getenv("OPENROUTER_API_KEY")
        
        # Création du chatbot
        chatbot = CRMChatbot(api_key, model, base_url)
        await chatbot.start_mcp_server()
        chatbot_sessions[session_id] = chatbot
    
    return chatbot_sessions[session_id]

@app.get("/")
async def root():
    """Point d'entrée de l'API"""
    return {
        "message": "API CRM MCP active",
        "version": "1.0.0",
        "endpoints": {
            "chat": "/chat",
            "tools": "/tools",
            "sessions": "/sessions"
        }
    }

@app.post("/chat", response_model=ChatResponse)
async def chat_endpoint(message: ChatMessage):
    """Endpoint principal pour le chat"""
    try:
        chatbot = await get_or_create_chatbot(message.session_id)
        response = await chatbot.chat(message.message)
        
        from datetime import datetime
        return ChatResponse(
            response=response,
            session_id=message.session_id,
            timestamp=datetime.now().isoformat()
        )
    
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Erreur lors du chat: {str(e)}")

@app.get("/tools")
async def list_tools():
    """Liste les outils MCP disponibles"""
    try:
        # Utilise une session temporaire pour récupérer les outils
        chatbot = await get_or_create_chatbot("temp_tools")
        tools = await chatbot.get_available_tools()
        
        # Formate les outils pour l'affichage
        formatted_tools = []
        for tool in tools:
            formatted_tools.append({
                "name": tool["function"]["name"],
                "description": tool["function"]["description"],
                "parameters": tool["function"]["parameters"]
            })
        
        return {"tools": formatted_tools}
    
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Erreur lors de la récupération des outils: {str(e)}")

@app.post("/tools/call", response_model=ToolResponse)
async def call_tool_endpoint(tool_call: ToolCall):
    """Appelle directement un outil MCP"""
    try:
        chatbot = await get_or_create_chatbot("direct_tool")
        result = await chatbot.call_mcp_tool(tool_call.tool_name, tool_call.arguments)
        
        return ToolResponse(
            result=result,
            success=True
        )
    
    except Exception as e:
        return ToolResponse(
            result="",
            success=False,
            error=str(e)
        )

@app.get("/sessions")
async def list_sessions():
    """Liste les sessions actives"""
    return {
        "active_sessions": list(chatbot_sessions.keys()),
        "total_sessions": len(chatbot_sessions)
    }

@app.delete("/sessions/{session_id}")
async def close_session(session_id: str):
    """Ferme une session spécifique"""
    if session_id in chatbot_sessions:
        try:
            await chatbot_sessions[session_id].cleanup()
            del chatbot_sessions[session_id]
            return {"message": f"Session {session_id} fermée"}
        except Exception as e:
            raise HTTPException(status_code=500, detail=f"Erreur lors de la fermeture: {str(e)}")
    else:
        raise HTTPException(status_code=404, detail="Session non trouvée")

@app.get("/health")
async def health_check():
    """Vérification de l'état de santé"""
    try:
        # Vérifie la connexion au CRM
        import httpx
        async with httpx.AsyncClient() as client:
            response = await client.get("http://localhost:8082/api/dashboard", timeout=5)
            crm_status = "ok" if response.status_code == 200 else "error"
    except:
        crm_status = "unreachable"
    
    return {
        "status": "ok",
        "crm_connection": crm_status,
        "active_sessions": len(chatbot_sessions),
        "mcp_available": True
    }

if __name__ == "__main__":
    import uvicorn
    print("🌐 Démarrage du serveur API MCP...")
    print("📱 Frontend peut se connecter sur: http://localhost:8000")
    print("📚 Documentation API: http://localhost:8000/docs")
    
    uvicorn.run(
        app,
        host="0.0.0.0",
        port=8000,
        reload=True
    )