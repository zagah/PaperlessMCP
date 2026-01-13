# PaperlessMCP

A Model Context Protocol (MCP) server that provides AI-first tooling for managing [Paperless-ngx](https://docs.paperless-ngx.com/) instances via the official REST API.

## Features

- **Full Document Management**: Search, upload, download, update, and delete documents
- **Metadata Management**: CRUD operations for tags, correspondents, document types, storage paths, and custom fields
- **Bulk Operations**: Batch updates and deletions with dry-run support
- **Safety Guardrails**: Destructive operations require explicit confirmation
- **Dual Transport**: Supports both HTTP (for remote access) and stdio (for local Claude Desktop)
- **Structured Output**: All responses follow a consistent JSON envelope format

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (Preview)
- A running [Paperless-ngx](https://docs.paperless-ngx.com/) instance
- API token from your Paperless-ngx instance

## Getting Started

### Configuration

Set the following environment variables:

| Variable | Required | Default | Description |
|----------|----------|---------|-------------|
| `PAPERLESS_BASE_URL` | Yes | - | Base URL of your Paperless-ngx instance |
| `PAPERLESS_API_TOKEN` | Yes | - | API token for authentication |
| `MCP_LOG_LEVEL` | No | `Information` | Logging level |
| `MAX_PAGE_SIZE` | No | `100` | Maximum page size for paginated requests |
| `MCP_PORT` | No | `5000` | HTTP port for remote transport |

### Getting an API Token

1. Log into your Paperless-ngx instance
2. Go to Settings > Administration
3. Create a new API token or use an existing one

### Running Locally (stdio transport)

For use with Claude Desktop or other local MCP clients:

```bash
# Clone and build
cd PaperlessMCP
dotnet build

# Run with stdio transport
PAPERLESS_BASE_URL=https://your-instance.com \
PAPERLESS_API_TOKEN=your-token \
dotnet run -- --stdio
```

### Running as HTTP Server

For remote access or containerized deployments:

```bash
# Run with HTTP transport (default)
PAPERLESS_BASE_URL=https://your-instance.com \
PAPERLESS_API_TOKEN=your-token \
dotnet run
```

The MCP endpoint will be available at `http://localhost:5000/mcp`

### Docker

```bash
# Using docker-compose
echo "PAPERLESS_BASE_URL=https://your-instance.com" > .env
echo "PAPERLESS_API_TOKEN=your-token" >> .env
docker-compose up -d

# Or build and run directly
docker build -t paperless-mcp .
docker run -p 5000:5000 \
  -e PAPERLESS_BASE_URL=https://your-instance.com \
  -e PAPERLESS_API_TOKEN=your-token \
  paperless-mcp
```

## Claude Desktop Configuration

Add to your Claude Desktop MCP configuration (`~/.config/claude/claude_desktop_config.json` on macOS/Linux or `%APPDATA%\Claude\claude_desktop_config.json` on Windows):

```json
{
  "mcpServers": {
    "paperless": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/PaperlessMCP", "--", "--stdio"],
      "env": {
        "PAPERLESS_BASE_URL": "https://your-paperless-instance.com",
        "PAPERLESS_API_TOKEN": "your-api-token"
      }
    }
  }
}
```

## Available Tools

### Health & Capability

| Tool | Description |
|------|-------------|
| `paperless.ping` | Verify connectivity and authentication |
| `paperless.capabilities` | List supported endpoints and features |

### Documents

| Tool | Description |
|------|-------------|
| `paperless.documents.search` | Full-text search with filters |
| `paperless.documents.get` | Get document by ID |
| `paperless.documents.download` | Get download URLs |
| `paperless.documents.preview` | Get preview URL |
| `paperless.documents.thumbnail` | Get thumbnail URL |
| `paperless.documents.upload` | Upload new document |
| `paperless.documents.update` | Update document metadata |
| `paperless.documents.delete` | Delete document (requires confirmation) |
| `paperless.documents.bulk_update` | Bulk operations (with dry-run) |
| `paperless.documents.reprocess` | Reprocess document OCR |

### Tags

| Tool | Description |
|------|-------------|
| `paperless.tags.list` | List all tags |
| `paperless.tags.get` | Get tag by ID |
| `paperless.tags.create` | Create new tag |
| `paperless.tags.update` | Update tag |
| `paperless.tags.delete` | Delete tag (requires confirmation) |
| `paperless.tags.bulk_delete` | Bulk delete tags |

### Correspondents

| Tool | Description |
|------|-------------|
| `paperless.correspondents.list` | List all correspondents |
| `paperless.correspondents.get` | Get correspondent by ID |
| `paperless.correspondents.create` | Create new correspondent |
| `paperless.correspondents.update` | Update correspondent |
| `paperless.correspondents.delete` | Delete correspondent (requires confirmation) |
| `paperless.correspondents.bulk_delete` | Bulk delete correspondents |

### Document Types

| Tool | Description |
|------|-------------|
| `paperless.document_types.list` | List all document types |
| `paperless.document_types.get` | Get document type by ID |
| `paperless.document_types.create` | Create new document type |
| `paperless.document_types.update` | Update document type |
| `paperless.document_types.delete` | Delete document type (requires confirmation) |
| `paperless.document_types.bulk_delete` | Bulk delete document types |

### Storage Paths

| Tool | Description |
|------|-------------|
| `paperless.storage_paths.list` | List all storage paths |
| `paperless.storage_paths.get` | Get storage path by ID |
| `paperless.storage_paths.create` | Create new storage path |
| `paperless.storage_paths.update` | Update storage path |
| `paperless.storage_paths.delete` | Delete storage path (requires confirmation) |
| `paperless.storage_paths.bulk_delete` | Bulk delete storage paths |

### Custom Fields

| Tool | Description |
|------|-------------|
| `paperless.custom_fields.list` | List all custom field definitions |
| `paperless.custom_fields.get` | Get custom field by ID |
| `paperless.custom_fields.create` | Create new custom field |
| `paperless.custom_fields.update` | Update custom field |
| `paperless.custom_fields.delete` | Delete custom field (requires confirmation) |
| `paperless.custom_fields.assign` | Assign field value to document |

## Response Format

All tools return responses in a consistent format:

### Success Response

```json
{
  "ok": true,
  "result": { ... },
  "meta": {
    "request_id": "uuid",
    "page": 1,
    "page_size": 25,
    "total": 123,
    "next": null,
    "paperless_base_url": "https://your-instance.com"
  },
  "warnings": []
}
```

### Error Response

```json
{
  "ok": false,
  "error": {
    "code": "NOT_FOUND",
    "message": "Document with ID 123 not found",
    "details": null
  },
  "meta": {
    "request_id": "uuid",
    "paperless_base_url": "https://your-instance.com"
  }
}
```

### Error Codes

| Code | Description |
|------|-------------|
| `AUTH_FAILED` | Authentication failed |
| `NOT_FOUND` | Resource not found |
| `VALIDATION` | Invalid input |
| `UPSTREAM_ERROR` | Paperless API error |
| `RATE_LIMIT` | Rate limited |
| `CONFIRMATION_REQUIRED` | Destructive operation requires confirmation |
| `UNKNOWN` | Unknown error |

## Example Tool Calls

### Search for Documents

```json
{
  "tool": "paperless.documents.search",
  "arguments": {
    "query": "invoice",
    "tags": "1,2",
    "correspondent": 5,
    "page": 1,
    "pageSize": 25
  }
}
```

### Upload a Document

```json
{
  "tool": "paperless.documents.upload",
  "arguments": {
    "fileContent": "base64-encoded-content",
    "fileName": "invoice.pdf",
    "title": "January Invoice",
    "tags": "1,3",
    "correspondent": 5
  }
}
```

### Bulk Add Tag (Dry Run)

```json
{
  "tool": "paperless.documents.bulk_update",
  "arguments": {
    "documentIds": "1,2,3,4,5",
    "operation": "add_tag",
    "value": 10,
    "dryRun": true,
    "confirm": false
  }
}
```

### Bulk Add Tag (Execute)

```json
{
  "tool": "paperless.documents.bulk_update",
  "arguments": {
    "documentIds": "1,2,3,4,5",
    "operation": "add_tag",
    "value": 10,
    "dryRun": false,
    "confirm": true
  }
}
```

### Delete Document with Confirmation

```json
{
  "tool": "paperless.documents.delete",
  "arguments": {
    "id": 123,
    "confirm": true
  }
}
```

## Testing with curl

### Test HTTP endpoint

```bash
# Check if server is running
curl http://localhost:5000/mcp

# The MCP protocol uses JSON-RPC, so you'd typically
# connect using an MCP client rather than raw curl
```

## Development

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run tests (if any)
dotnet test

# Run in development mode
dotnet run
```

## License

MIT

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.
