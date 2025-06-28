#!/usr/bin/env python3
"""
Serveur MCP modulaire pour CRM multi-API
Expose les outils pour Stock, Planning, Géolocalisation et une API générale
"""

import asyncio
import json
import httpx
from typing import Any, Dict, List, Optional
from mcp.server.models import InitializationOptions
from mcp.server import NotificationOptions, Server
from mcp.types import (
    Resource,
    Tool,
    TextContent,
    ImageContent,
    EmbeddedResource,
    LoggingLevel
)
import mcp.types as types

# Configuration du CRM
CRM_BASE_URL = "http://localhost:8082"

class CRMServer:
    def __init__(self):
        self.server = Server("crm-multi-api")
        self.http_client = httpx.AsyncClient(timeout=30.0)
        self.setup_handlers()
    
    def setup_handlers(self):
        """Configure les handlers du serveur MCP"""
        
        @self.server.list_tools()
        async def handle_list_tools() -> List[Tool]:
            tools = []
            
            # === OUTILS STOCK ===
            tools.extend([
                Tool(
                    name="stock_get_overview",
                    description="Obtient une vue d'ensemble du stock (total items, valeur, catégories)",
                    inputSchema={
                        "type": "object",
                        "properties": {},
                        "required": []
                    }
                ),
                Tool(
                    name="stock_get_critical",
                    description="Trouve les items avec une quantité critique (stock faible)",
                    inputSchema={
                        "type": "object",
                        "properties": {
                            "threshold": {
                                "type": "integer",
                                "description": "Seuil de quantité critique (défaut: 5)",
                                "default": 5
                            }
                        },
                        "required": []
                    }
                ),
                Tool(
                    name="stock_get_by_category",
                    description="Récupère les items par catégorie (nourriture, électronique, etc.)",
                    inputSchema={
                        "type": "object",
                        "properties": {
                            "category": {
                                "type": "string",
                                "description": "Catégorie des items à rechercher"
                            }
                        },
                        "required": ["category"]
                    }
                ),
                Tool(
                    name="stock_search_items",
                    description="Recherche des items par nom ou description",
                    inputSchema={
                        "type": "object",
                        "properties": {
                            "query": {
                                "type": "string",
                                "description": "Terme de recherche"
                            }
                        },
                        "required": ["query"]
                    }
                ),
                Tool(
                    name="stock_add_item",
                    description="Ajoute un nouvel item au stock",
                    inputSchema={
                        "type": "object",
                        "properties": {
                            "name": {"type": "string", "description": "Nom de l'item"},
                            "category": {"type": "string", "description": "Catégorie de l'item"},
                            "quantity": {"type": "integer", "description": "Quantité initiale"},
                            "price": {"type": "number", "description": "Prix unitaire"},
                            "description": {"type": "string", "description": "Description de l'item"}
                        },
                        "required": ["name", "category", "quantity"]
                    }
                ),
                Tool(
                    name="stock_update_quantity",
                    description="Met à jour la quantité d'un item",
                    inputSchema={
                        "type": "object",
                        "properties": {
                            "item_id": {"type": "string", "description": "ID de l'item"},
                            "quantity": {"type": "integer", "description": "Nouvelle quantité"},
                            "operation": {
                                "type": "string", 
                                "enum": ["set", "add", "remove"],
                                "description": "Type d'opération: set (définir), add (ajouter), remove (retirer)"
                            }
                        },
                        "required": ["item_id", "quantity", "operation"]
                    }
                )
            ])
            
            # === OUTILS PLANNING ===
            tools.extend([
                Tool(
                    name="planning_get_schedule",
                    description="Récupère le planning pour une période donnée",
                    inputSchema={
                        "type": "object",
                        "properties": {
                            "start_date": {"type": "string", "description": "Date de début (YYYY-MM-DD)"},
                            "end_date": {"type": "string", "description": "Date de fin (YYYY-MM-DD)"},
                            "user_id": {"type": "string", "description": "ID utilisateur (optionnel)"}
                        },
                        "required": ["start_date", "end_date"]
                    }
                ),
                Tool(
                    name="planning_create_event",
                    description="Crée un nouvel événement dans le planning",
                    inputSchema={
                        "type": "object",
                        "properties": {
                            "title": {"type": "string", "description": "Titre de l'événement"},
                            "start_time": {"type": "string", "description": "Heure de début (YYYY-MM-DD HH:MM)"},
                            "end_time": {"type": "string", "description": "Heure de fin (YYYY-MM-DD HH:MM)"},
                            "description": {"type": "string", "description": "Description de l'événement"},
                            "location": {"type": "string", "description": "Lieu de l'événement"},
                            "attendees": {"type": "array", "items": {"type": "string"}, "description": "Liste des participants"}
                        },
                        "required": ["title", "start_time", "end_time"]
                    }
                ),
                Tool(
                    name="planning_get_conflicts",
                    description="Vérifie les conflits de planning pour une période",
                    inputSchema={
                        "type": "object",
                        "properties": {
                            "start_time": {"type": "string", "description": "Heure de début (YYYY-MM-DD HH:MM)"},
                            "end_time": {"type": "string", "description": "Heure de fin (YYYY-MM-DD HH:MM)"},
                            "user_id": {"type": "string", "description": "ID utilisateur"}
                        },
                        "required": ["start_time", "end_time", "user_id"]
                    }
                ),
                Tool(
                    name="planning_get_availability",
                    description="Trouve les créneaux disponibles pour une période",
                    inputSchema={
                        "type": "object",
                        "properties": {
                            "date": {"type": "string", "description": "Date à vérifier (YYYY-MM-DD)"},
                            "duration": {"type": "integer", "description": "Durée en minutes"},
                            "user_id": {"type": "string", "description": "ID utilisateur"}
                        },
                        "required": ["date", "duration", "user_id"]
                    }
                )
            ])
            
            # === OUTILS GÉOLOCALISATION ===
            tools.extend([
                Tool(
                    name="geo_get_locations",
                    description="Récupère toutes les localisations enregistrées",
                    inputSchema={
                        "type": "object",
                        "properties": {
                            "category": {"type": "string", "description": "Catégorie de lieux (optionnel)"}
                        },
                        "required": []
                    }
                ),
                Tool(
                    name="geo_add_location",
                    description="Ajoute une nouvelle localisation",
                    inputSchema={
                        "type": "object",
                        "properties": {
                            "name": {"type": "string", "description": "Nom du lieu"},
                            "address": {"type": "string", "description": "Adresse complète"},
                            "latitude": {"type": "number", "description": "Latitude"},
                            "longitude": {"type": "number", "description": "Longitude"},
                            "category": {"type": "string", "description": "Catégorie du lieu"},
                            "description": {"type": "string", "description": "Description du lieu"}
                        },
                        "required": ["name", "address", "latitude", "longitude"]
                    }
                ),
                Tool(
                    name="geo_find_nearby",
                    description="Trouve les lieux proches d'une position",
                    inputSchema={
                        "type": "object",
                        "properties": {
                            "latitude": {"type": "number", "description": "Latitude de référence"},
                            "longitude": {"type": "number", "description": "Longitude de référence"},
                            "radius": {"type": "number", "description": "Rayon de recherche en km (défaut: 5)"},
                            "category": {"type": "string", "description": "Catégorie de lieux à chercher"}
                        },
                        "required": ["latitude", "longitude"]
                    }
                ),
                Tool(
                    name="geo_calculate_distance",
                    description="Calcule la distance entre deux points",
                    inputSchema={
                        "type": "object",
                        "properties": {
                            "from_lat": {"type": "number", "description": "Latitude point de départ"},
                            "from_lng": {"type": "number", "description": "Longitude point de départ"},
                            "to_lat": {"type": "number", "description": "Latitude point d'arrivée"},
                            "to_lng": {"type": "number", "description": "Longitude point d'arrivée"}
                        },
                        "required": ["from_lat", "from_lng", "to_lat", "to_lng"]
                    }
                ),
                Tool(
                    name="geo_get_route",
                    description="Calcule un itinéraire entre deux points",
                    inputSchema={
                        "type": "object",
                        "properties": {
                            "from_address": {"type": "string", "description": "Adresse de départ"},
                            "to_address": {"type": "string", "description": "Adresse d'arrivée"},
                            "transport_mode": {
                                "type": "string",
                                "enum": ["driving", "walking", "cycling", "transit"],
                                "description": "Mode de transport"
                            }
                        },
                        "required": ["from_address", "to_address"]
                    }
                )
            ])
            
            # === OUTILS GÉNÉRAUX CRM ===
            tools.extend([
                Tool(
                    name="crm_get_dashboard",
                    description="Récupère les données du tableau de bord CRM",
                    inputSchema={
                        "type": "object",
                        "properties": {},
                        "required": []
                    }
                ),
                Tool(
                    name="crm_search_global",
                    description="Recherche globale dans toutes les données CRM",
                    inputSchema={
                        "type": "object",
                        "properties": {
                            "query": {"type": "string", "description": "Terme de recherche"},
                            "modules": {
                                "type": "array",
                                "items": {"type": "string", "enum": ["stock", "planning", "geo"]},
                                "description": "Modules à inclure dans la recherche"
                            }
                        },
                        "required": ["query"]
                    }
                )
            ])
            
            return tools
        
        @self.server.call_tool()
        async def handle_call_tool(name: str, arguments: Dict[str, Any]) -> List[types.TextContent]:
            try:
                # Route vers le bon handler selon le préfixe
                if name.startswith("stock_"):
                    return await self.handle_stock_tool(name, arguments)
                elif name.startswith("planning_"):
                    return await self.handle_planning_tool(name, arguments)
                elif name.startswith("geo_"):
                    return await self.handle_geo_tool(name, arguments)
                elif name.startswith("crm_"):
                    return await self.handle_crm_tool(name, arguments)
                else:
                    raise ValueError(f"Outil inconnu: {name}")
                    
            except Exception as e:
                return [types.TextContent(
                    type="text",
                    text=f"Erreur lors de l'exécution de {name}: {str(e)}"
                )]
    
    # === HANDLERS STOCK ===
    async def handle_stock_tool(self, name: str, arguments: Dict[str, Any]) -> List[types.TextContent]:
        """Gère les outils liés au stock"""
        
        if name == "stock_get_overview":
            return await self._api_call("GET", "/api/stock/overview")
        
        elif name == "stock_get_critical":
            threshold = arguments.get("threshold", 5)
            return await self._api_call("GET", f"/api/stock/critical?threshold={threshold}")
        
        elif name == "stock_get_by_category":
            category = arguments["category"]
            return await self._api_call("GET", f"/api/stock/category/{category}")
        
        elif name == "stock_search_items":
            query = arguments["query"]
            return await self._api_call("GET", f"/api/stock/search?q={query}")
        
        elif name == "stock_add_item":
            return await self._api_call("POST", "/api/stock/items", data=arguments)
        
        elif name == "stock_update_quantity":
            item_id = arguments["item_id"]
            return await self._api_call("PUT", f"/api/stock/items/{item_id}/quantity", data=arguments)
        
        else:
            raise ValueError(f"Outil stock inconnu: {name}")
    
    # === HANDLERS PLANNING ===
    async def handle_planning_tool(self, name: str, arguments: Dict[str, Any]) -> List[types.TextContent]:
        """Gère les outils liés au planning"""
        
        if name == "planning_get_schedule":
            params = {
                "start_date": arguments["start_date"],
                "end_date": arguments["end_date"]
            }
            if "user_id" in arguments:
                params["user_id"] = arguments["user_id"]
            
            query_string = "&".join([f"{k}={v}" for k, v in params.items()])
            return await self._api_call("GET", f"/api/planning/schedule?{query_string}")
        
        elif name == "planning_create_event":
            return await self._api_call("POST", "/api/planning/events", data=arguments)
        
        elif name == "planning_get_conflicts":
            params = "&".join([f"{k}={v}" for k, v in arguments.items()])
            return await self._api_call("GET", f"/api/planning/conflicts?{params}")
        
        elif name == "planning_get_availability":
            params = "&".join([f"{k}={v}" for k, v in arguments.items()])
            return await self._api_call("GET", f"/api/planning/availability?{params}")
        
        else:
            raise ValueError(f"Outil planning inconnu: {name}")
    
    # === HANDLERS GÉOLOCALISATION ===
    async def handle_geo_tool(self, name: str, arguments: Dict[str, Any]) -> List[types.TextContent]:
        """Gère les outils liés à la géolocalisation"""
        
        if name == "geo_get_locations":
            url = "/api/geo/locations"
            if "category" in arguments:
                url += f"?category={arguments['category']}"
            return await self._api_call("GET", url)
        
        elif name == "geo_add_location":
            return await self._api_call("POST", "/api/geo/locations", data=arguments)
        
        elif name == "geo_find_nearby":
            params = "&".join([f"{k}={v}" for k, v in arguments.items()])
            return await self._api_call("GET", f"/api/geo/nearby?{params}")
        
        elif name == "geo_calculate_distance":
            params = "&".join([f"{k}={v}" for k, v in arguments.items()])
            return await self._api_call("GET", f"/api/geo/distance?{params}")
        
        elif name == "geo_get_route":
            params = "&".join([f"{k}={v}" for k, v in arguments.items()])
            return await self._api_call("GET", f"/api/geo/route?{params}")
        
        else:
            raise ValueError(f"Outil géo inconnu: {name}")
    
    # === HANDLERS CRM GÉNÉRAL ===
    async def handle_crm_tool(self, name: str, arguments: Dict[str, Any]) -> List[types.TextContent]:
        """Gère les outils CRM généraux"""
        
        if name == "crm_get_dashboard":
            return await self._api_call("GET", "/api/dashboard")
        
        elif name == "crm_search_global":
            query = arguments["query"]
            url = f"/api/search?q={query}"
            
            if "modules" in arguments:
                modules = ",".join(arguments["modules"])
                url += f"&modules={modules}"
            
            return await self._api_call("GET", url)
        
        else:
            raise ValueError(f"Outil CRM inconnu: {name}")
    
    # === HELPER METHODS ===
    async def _api_call(self, method: str, endpoint: str, data: Dict[str, Any] = None) -> List[types.TextContent]:
        """Helper pour faire des appels API"""
        try:
            url = f"{CRM_BASE_URL}{endpoint}"
            
            if method == "GET":
                response = await self.http_client.get(url)
            elif method == "POST":
                response = await self.http_client.post(url, json=data)
            elif method == "PUT":
                response = await self.http_client.put(url, json=data)
            elif method == "DELETE":
                response = await self.http_client.delete(url)
            else:
                raise ValueError(f"Méthode HTTP non supportée: {method}")
            
            response.raise_for_status()
            
            # Essaie de parser en JSON, sinon retourne le texte brut
            try:
                result = response.json()
                return [types.TextContent(
                    type="text",
                    text=json.dumps(result, indent=2, ensure_ascii=False)
                )]
            except:
                return [types.TextContent(
                    type="text",
                    text=response.text
                )]
                
        except Exception as e:
            return [types.TextContent(
                type="text",
                text=f"Erreur API {method} {endpoint}: {str(e)}"
            )]
    
    async def cleanup(self):
        """Nettoyage des ressources"""
        await self.http_client.aclose()

async def main():
    """Point d'entrée principal"""
    crm_server = CRMServer()
    
    # Options d'initialisation
    options = InitializationOptions(
        server_name="crm-multi-api",
        server_version="1.0.0",
        capabilities=crm_server.server.get_capabilities(
            notification_options=NotificationOptions(),
            experimental_capabilities={}
        )
    )
    
    try:
        # Démarre le serveur
        async with mcp.server.stdio.stdio_server() as (read_stream, write_stream):
            await crm_server.server.run(
                read_stream,
                write_stream,
                options
            )
    except KeyboardInterrupt:
        pass
    finally:
        await crm_server.cleanup()

if __name__ == "__main__":
    asyncio.run(main())