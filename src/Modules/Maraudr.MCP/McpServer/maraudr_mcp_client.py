#!/usr/bin/env python3
"""
Client MCP pour le chatbot de gestion de stock
Connecte OpenAI/OpenRouter avec le serveur MCP
"""

import asyncio
import json
import os
from typing import Dict, List, Any, Optional
from openai import AsyncOpenAI
from mcp.client.session import ClientSession
from mcp.client.stdio import stdio_client

class CRMChatbot:
    def __init__(self, api_key: str, model: str = "gpt-4o-mini", base_url: Optional[str] = None):
        """
        Initialise le chatbot
        
        Args:
            api_key: Clé API OpenAI ou OpenRouter
            model: Modèle à utiliser (ex: gpt-4o-mini pour OpenAI, openai/gpt-4o-mini pour OpenRouter)
            base_url: URL de base (None pour OpenAI, https://openrouter.ai/api/v1 pour OpenRouter)
        """
        self.client = AsyncOpenAI(
            api_key=api_key,
            base_url=base_url
        )
        self.model = model
        self.mcp_session: Optional[ClientSession] = None
        self.conversation_history = []
    
    async def start_mcp_server(self):
        """Démarre la connexion avec le serveur MCP"""
        try:
            # Lance le serveur MCP en subprocess
            server_params = stdio_client(
                command="python",
                args=["crm_mcp_server.py"],
                env=None
            )
            
            self.mcp_session = ClientSession(server_params[0], server_params[1])
            await self.mcp_session.initialize()
            
            print("✅ Connexion MCP établie")
            
            # Liste les outils disponibles
            tools = await self.mcp_session.list_tools()
            print(f"🔧 Outils disponibles: {[tool.name for tool in tools.tools]}")
            
        except Exception as e:
            print(f"❌ Erreur lors de la connexion MCP: {e}")
            raise
    
    async def get_available_tools(self) -> List[Dict[str, Any]]:
        """Récupère les outils MCP au format OpenAI Functions"""
        if not self.mcp_session:
            return []
        
        tools_response = await self.mcp_session.list_tools()
        openai_tools = []
        
        for tool in tools_response.tools:
            openai_tool = {
                "type": "function",
                "function": {
                    "name": tool.name,
                    "description": tool.description,
                    "parameters": tool.inputSchema
                }
            }
            openai_tools.append(openai_tool)
        
        return openai_tools
    
    async def call_mcp_tool(self, tool_name: str, arguments: Dict[str, Any]) -> str:
        """Appelle un outil MCP et retourne le résultat"""
        if not self.mcp_session:
            return "Erreur: Aucune session MCP active"
        
        try:
            result = await self.mcp_session.call_tool(tool_name, arguments)
            
            # Extrait le texte des résultats
            text_results = []
            for content in result.content:
                if hasattr(content, 'text'):
                    text_results.append(content.text)
            
            return "\n".join(text_results)
            
        except Exception as e:
            return f"Erreur lors de l'appel de l'outil {tool_name}: {str(e)}"
    
    async def chat(self, user_message: str) -> str:
        """Traite un message utilisateur et retourne la réponse"""
        try:
            # Ajoute le message utilisateur à l'historique
            self.conversation_history.append({
                "role": "user",
                "content": user_message
            })
            
            # Récupère les outils disponibles
            tools = await self.get_available_tools()
            
            # Messages système pour le contexte
            system_message = {
                "role": "system",
                "content": """Tu es un assistant spécialisé dans la gestion de stock d'un CRM.
                
                Tu peux utiliser les outils suivants pour répondre aux questions sur le stock:
                - get_stock_overview: Vue d'ensemble du stock
                - get_critical_stock: Items avec quantité critique
                - get_items_by_category: Items par catégorie (nourriture, électronique, etc.)
                - search_items: Recherche d'items par nom
                - get_stock_value: Valeur totale du stock

                Réponds de manière naturelle et conviviale en français. Utilise les outils quand nécessaire pour fournir des informations précises et actuelles."""
            }
            
            # Appel à OpenAI/OpenRouter
            messages = [system_message] + self.conversation_history
            
            response = await self.client.chat.completions.create(
                model=self.model,
                messages=messages,
                tools=tools if tools else None,
                tool_choice="auto" if tools else None,
                temperature=0.7
            )
            
            message = response.choices[0].message
            
            # Vérifie si des outils doivent être appelés
            if message.tool_calls:
                # Ajoute la réponse de l'assistant avec les appels d'outils
                self.conversation_history.append({
                    "role": "assistant",
                    "content": message.content,
                    "tool_calls": [
                        {
                            "id": tc.id,
                            "type": tc.type,
                            "function": {
                                "name": tc.function.name,
                                "arguments": tc.function.arguments
                            }
                        } for tc in message.tool_calls
                    ]
                })
                
                # Exécute les appels d'outils
                for tool_call in message.tool_calls:
                    function_name = tool_call.function.name
                    function_args = json.loads(tool_call.function.arguments)
                    
                    print(f"🔧 Appel outil: {function_name} avec {function_args}")
                    
                    # Appelle l'outil MCP
                    tool_result = await self.call_mcp_tool(function_name, function_args)
                    
                    # Ajoute le résultat à l'historique
                    self.conversation_history.append({
                        "role": "tool",
                        "tool_call_id": tool_call.id,
                        "content": tool_result
                    })
                
                # Fait un nouvel appel pour générer la réponse finale
                final_response = await self.client.chat.completions.create(
                    model=self.model,
                    messages=[system_message] + self.conversation_history,
                    temperature=0.7
                )
                
                final_message = final_response.choices[0].message.content
                
                # Ajoute la réponse finale à l'historique
                self.conversation_history.append({
                    "role": "assistant",
                    "content": final_message
                })
                
                return final_message
            
            else:
                # Pas d'appel d'outil nécessaire
                self.conversation_history.append({
                    "role": "assistant",
                    "content": message.content
                })
                
                return message.content
                
        except Exception as e:
            error_msg = f"Erreur lors du traitement: {str(e)}"
            print(f"❌ {error_msg}")
            return error_msg
    
    async def cleanup(self):
        """Nettoyage des ressources"""
        if self.mcp_session:
            await self.mcp_session.close()

async def main():
    """Interface en ligne de commande pour tester le chatbot"""
    
    # Configuration - à adapter selon tes clés API
    API_KEY = os.getenv("OPENAI_API_KEY") or os.getenv("OPENROUTER_API_KEY")
    if not API_KEY:
        print("❌ Veuillez définir OPENAI_API_KEY ou OPENROUTER_API_KEY")
        return
    
    # Pour OpenAI
    MODEL = "gpt-4o-mini"
    BASE_URL = None
    
    # Pour OpenRouter (décommenter si nécessaire)
    # MODEL = "openai/gpt-4o-mini"
    # BASE_URL = "https://openrouter.ai/api/v1"
    
    print("🤖 Initialisation du chatbot CRM multi-modules...")
    
    chatbot = CRMChatbot(API_KEY, MODEL, BASE_URL)
    
    try:
        # Démarre la connexion MCP
        await chatbot.start_mcp_server()
        
        print("\n💬 Chatbot prêt! Tapez 'quit' pour quitter.\n")
        
        while True:
            try:
                user_input = input("Vous: ").strip()
                
                if user_input.lower() in ['quit', 'exit', 'q']:
                    break
                
                if not user_input:
                    continue
                
                print("🤖 Assistant: ", end="", flush=True)
                response = await chatbot.chat(user_input)
                print(response)
                print()
                
            except KeyboardInterrupt:
                break
            except EOFError:
                break
    
    except Exception as e:
        print(f"❌ Erreur: {e}")
    
    finally:
        await chatbot.cleanup()
        print("\n👋 Au revoir!")

if __name__ == "__main__":
    asyncio.run(main())