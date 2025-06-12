from mcp_maraudr_server import mcp

import tools.stock_tools
import argparse

def main():
    # Debug Mode
    #uv run mcp dev server.py

    # Production Mode
    # uv run server.py --server_type=sse

    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--server_type", type=str, default="sse", choices=["sse", "stdio"]
    )

    args = parser.parse_args()
    mcp.run(args.server_type)

if __name__ == "__main__":
    main()
