from mcp_maraudr_server import mcp
import requests
import json
from typing import Optional, Dict, Any
import uuid

# Base configuration
BASE_URL = "http://localhost:8081"  # Adjust to your API URL
API_HEADERS = {
    "Content-Type": "application/json",
    "Authorization": "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJjY2Y1OWM0YS1mZmFiLTQ4M2MtOTA1MS01MTVmZGI0NmIyNjkiLCJlbWFpbCI6ImtoYWxpbG1ha2hsb3VmaUBnbWFpbC5jb20iLCJqdGkiOiI3YzViYjdjMi0xNDQzLTRkNTYtOTdmNS1jMDdkNDlhNmQ3YzAiLCJuYmYiOjE3NDk3NDA0MjYsImV4cCI6MTc0OTc0NDAyNiwiaWF0IjoxNzQ5NzQwNDI2LCJpc3MiOiJhdXRoLWFwaSIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcxNDAifQ.jb5PKJnPvNrHcwKDThIFPqQAcpxRPVDCgG4IzeGBgxI"  # Replace with actual token
}



@mcp.tool()
def get_items_by_category(category: str) -> Dict[str, Any]:
    """
    Retrieve items by category type.
    Category 1 is for food items.
    Args:
        category: The category/type of items to retrieve
    
    Returns:
        Dict containing list of items or error message
    """
    try:
        url = f"{BASE_URL}/item/type/{category}"
        response = requests.get(url, headers=API_HEADERS)
        
        if response.status_code == 200:
            return {"success": True, "data": response.json()}
        else:
            return {"success": False, "message": f"Error: {response.status_code}"}
            
    except Exception as e:
        return {"success": False, "message": f"Request failed: {str(e)}"}

@mcp.tool()
def get_item_by_barcode(barcode: str) -> Dict[str, Any]:
    """
    Retrieve an item by its barcode.
    
    Args:
        barcode: The barcode of the item to retrieve
    
    Returns:
        Dict containing item details or error message
    """
    try:
        url = f"{BASE_URL}/item/barcode/{barcode}"
        response = requests.get(url, headers=API_HEADERS)
        
        if response.status_code == 200:
            return {"success": True, "data": response.json()}
        else:
            return {"success": False, "message": f"Error: {response.status_code}"}
            
    except Exception as e:
        return {"success": False, "message": f"Request failed: {str(e)}"}



@mcp.tool()
def get_stock_items(association_id: str, category: Optional[str] = None, 
                   name: Optional[str] = None) -> Dict[str, Any]:
    """
    Get all items in stock for an association with optional filtering.
    
    Args:
        association_id: The association ID to get items for
        category: Optional category filter
        name: Optional name filter
    
    Returns:
        Dict containing list of stock items or error message
    """
    try:
        # Validate UUID
        uuid.UUID(association_id)
        
        url = f"{BASE_URL}/stock/items"
        params = {"associationId": association_id}
        
        if category:
            params["category"] = category
        if name:
            params["name"] = name
        
        response = requests.get(url, headers=API_HEADERS, params=params)
        
        if response.status_code == 200:
            return {"success": True, "data": response.json()}
        elif response.status_code == 400:
            return {"success": False, "message": "Missing or invalid association ID"}
        else:
            return {"success": False, "message": f"Error: {response.status_code}"}
            
    except ValueError:
        return {"success": False, "message": "Invalid UUID format for association_id"}
    except Exception as e:
        return {"success": False, "message": f"Request failed: {str(e)}"}




@mcp.tool()
def search_items_by_name_and_category(association_id: str, search_term: str) -> Dict[str, Any]:
    """
    Search items by name and category for a specific association.
    
    Args:
        association_id: The association ID to search items for
        search_term: The term to search for in both name and category
    
    Returns:
        Dict containing matching items or error message
    """
    try:
        # Search by name
        items_by_name = get_stock_items(association_id, name=search_term)
        # Search by category
        items_by_category = get_stock_items(association_id, category=search_term)
        
        if items_by_name.get("success") and items_by_category.get("success"):
            # Combine results and remove duplicates
            all_items = []
            seen_ids = set()
            
            for item in items_by_name.get("data", []):
                if item.get("id") not in seen_ids:
                    all_items.append(item)
                    seen_ids.add(item.get("id"))
            
            for item in items_by_category.get("data", []):
                if item.get("id") not in seen_ids:
                    all_items.append(item)
                    seen_ids.add(item.get("id"))
            
            return {"success": True, "data": all_items, "search_term": search_term}
        else:
            return {"success": False, "message": "Search failed"}
            
    except Exception as e:
        return {"success": False, "message": f"Search failed: {str(e)}"}
