#!/usr/bin/env python3
"""
Building a Local MCP Client with LlamaIndex

This Python script creates a local MCP (Model Context Protocol) client that can 
chat with a database through tools exposed by an MCP server—completely on your machine.
"""

import asyncio
import nest_asyncio
from llama_index.llms.ollama import Ollama
from llama_index.core import Settings
from llama_index.tools.mcp import BasicMCPClient, McpToolSpec
from llama_index.core.agent.workflow import FunctionAgent, ToolCallResult, ToolCall
from llama_index.core.workflow import Context

# Apply nest_asyncio for Jupyter-like async support
nest_asyncio.apply()

# System prompt for the AI assistant
SYSTEM_PROMPT = """\
You are an AI assistant for a humanitarian association management application.

This application manages key areas such as stock and inventory, user and volunteer data, events organization, and field operations (like street outreach and food distribution missions, also known as "maraudes").

Before helping the user, you must first interact with internal tools and APIs to retrieve or update relevant information from the system.
"""

def setup_llm():
    """Setup the local LLM using Ollama."""
    llm = Ollama(model="llama3.2:1b", request_timeout=120.0)
    Settings.llm = llm
    return llm

async def get_agent(tools: McpToolSpec):
    """Creates a FunctionAgent wired up with the MCP tool list and chosen LLM."""
    tool_list = await tools.to_tool_list_async()
    agent = FunctionAgent(
        name="Agent",
        description="An agent that can work with our APIS.",
        tools=tool_list,
        system_prompt=SYSTEM_PROMPT,
    )
    return agent

async def handle_user_message(
    message_content: str,
    agent: FunctionAgent,
    agent_context: Context,
    verbose: bool = False,
):
    """
    Streams intermediate tool calls (for transparency) and returns the final response.
    """
    handler = agent.run(message_content, ctx=agent_context)
    async for event in handler.stream_events():
        if verbose and type(event) == ToolCall:
            print(f"Calling tool {event.tool_name} with kwargs {event.tool_kwargs}")
        elif verbose and type(event) == ToolCallResult:
            print(f"Tool {event.tool_name} returned {event.tool_output}")

    response = await handler
    return str(response)

async def list_available_tools(mcp_tools: McpToolSpec):
    """List all available tools from the MCP server."""
    print("Available tools:")
    tools = await mcp_tools.to_tool_list_async()
    for tool in tools:
        print(f"- {tool.metadata.name}: {tool.metadata.description}")
    print()

async def main():
    """Main function to run the MCP client."""
    print("Building a Local MCP Client with LlamaIndex")
    print("=" * 50)
    
    print("Setting up local LLM...")
    setup_llm()
    
    print("Initializing MCP client...")
    mcp_client = BasicMCPClient("http://localhost:8000/sse")
    mcp_tools = McpToolSpec(client=mcp_client)
    
    await list_available_tools(mcp_tools)
    
    print("Building agent...")
    agent = await get_agent(mcp_tools)
    agent_context = Context(agent)
    
    print("Agent ready! Type 'exit' to quit.\n")
    
    while True:
        try:
            user_input = input("Enter your message: ")
            if user_input.lower() in ['exit', 'quit', 'q']:
                print("Goodbye!")
                break
            
            print(f"User: {user_input}")
            response = await handle_user_message(user_input, agent, agent_context, verbose=True)
            print(f"Agent: {response}\n")
            
        except KeyboardInterrupt:
            print("\nGoodbye!")
            break
        except Exception as e:
            print(f"Error: {e}")
            print("Continuing...\n")

if __name__ == "__main__":
    asyncio.run(main())