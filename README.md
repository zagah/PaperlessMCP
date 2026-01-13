<div align="center">

# 📄 PaperlessMCP

**Model Context Protocol Server for Paperless-ngx**

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![MCP](https://img.shields.io/badge/MCP-0.2.0--preview.1-blue)](https://modelcontextprotocol.io/)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com/)

*Seamlessly integrate your Paperless-ngx document management system with Claude via the Model Context Protocol*

[Features](#-features) • [Installation](#-installation) • [Configuration](#-configuration) • [Usage](#-usage) • [API Reference](#-api-reference) • [Contributing](#-contributing)

</div>

---

## 🎯 Overview

PaperlessMCP is a powerful Model Context Protocol (MCP) server that bridges [Paperless-ngx](https://github.com/paperless-ngx/paperless-ngx) with AI assistants like Claude. It provides comprehensive document management capabilities through a modern, type-safe .NET implementation with built-in retry logic and error handling.

### What is MCP?

The [Model Context Protocol](https://modelcontextprotocol.io/) enables AI models to securely interact with external data sources and tools. PaperlessMCP implements this protocol to give Claude direct access to your document management system.

---

## ✨ Features

### 📚 Document Operations
- **Search & Discovery** - Full-text search with advanced filtering (tags, dates, correspondents, types)
- **Document Management** - Create, read, update, delete documents with metadata
- **Bulk Operations** - Process multiple documents at once (add/remove tags, set properties, reprocess)
- **File Uploads** - Support for base64 content or local file paths with automatic retries
- **Download URLs** - Get preview, thumbnail, and original file URLs

### 🏷️ Metadata Management
- **Tags** - Create, organize, and manage document tags with auto-tagging rules
- **Correspondents** - Track document sources and senders
- **Document Types** - Classify documents with custom types
- **Storage Paths** - Organize files with template-based storage paths
- **Custom Fields** - Define and assign custom metadata fields (string, date, boolean, monetary, etc.)

### 🔧 Developer Features
- **Dual Transport** - Supports both stdio (Claude Desktop) and HTTP transports
- **Pagination** - Efficient handling of large datasets with configurable page sizes
- **Dry Run Mode** - Preview destructive operations before execution
- **Retry Logic** - Built-in exponential backoff for transient failures
- **Health Checks** - Verify connectivity and discover API capabilities
- **Comprehensive Tests** - Full test suite with 100% coverage

---

## 🚀 Installation

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (or later)
- A running [Paperless-ngx](https://github.com/paperless-ngx/paperless-ngx) instance
- Paperless-ngx API token ([How to get one](https://docs.paperless-ngx.com/api/#authorization))

### Method 1: Clone and Build

```bash
# Clone the repository
git clone https://github.com/barryw/PaperlessMCP.git
cd PaperlessMCP

# Build the project
dotnet build

# Run tests (optional)
dotnet test
```

### Method 2: Docker

```bash
# Build the Docker image
docker build -t paperless-mcp ./PaperlessMCP

# Run with environment variables
docker run -d \
  -e PAPERLESS_BASE_URL=https://your-paperless-instance.com \
  -e PAPERLESS_API_TOKEN=your-token-here \
  -p 5000:5000 \
  paperless-mcp
```

### Method 3: Claude Desktop Integration

1. Build the project:
   ```bash
   cd PaperlessMCP
   dotnet build -c Release
   ```

2. Add to your Claude Desktop configuration (`claude_desktop_config.json`):

   **macOS**: `~/Library/Application Support/Claude/claude_desktop_config.json`

   **Windows**: `%APPDATA%\Claude\claude_desktop_config.json`

   ```json
   {
     "mcpServers": {
       "paperless": {
         "command": "dotnet",
         "args": [
           "run",
           "--project",
           "/absolute/path/to/PaperlessMCP/PaperlessMCP",
           "--",
           "--stdio"
         ],
         "env": {
           "PAPERLESS_BASE_URL": "https://your-paperless-instance.com",
           "PAPERLESS_API_TOKEN": "your-token-here"
         }
       }
     }
   }
   ```

3. Restart Claude Desktop

---

## ⚙️ Configuration

### Environment Variables

PaperlessMCP supports multiple configuration methods. Environment variables take precedence over `appsettings.json`.

| Variable | Aliases | Required | Default | Description |
|----------|---------|----------|---------|-------------|
| `PAPERLESS_BASE_URL` | `PAPERLESS_URL` | ✅ Yes | - | Base URL of your Paperless-ngx instance |
| `PAPERLESS_API_TOKEN` | `PAPERLESS_TOKEN` | ✅ Yes | - | API authentication token |
| `MAX_PAGE_SIZE` | - | ❌ No | `100` | Maximum items per page for paginated requests |
| `MCP_PORT` | - | ❌ No | `5000` | HTTP server port (HTTP mode only) |

### appsettings.json

Alternatively, configure via `appsettings.json`:

```json
{
  "Paperless": {
    "BaseUrl": "https://your-paperless-instance.com",
    "ApiToken": "your-token-here",
    "MaxPageSize": 100
  },
  "Mcp": {
    "Port": 5000
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "ModelContextProtocol": "Debug"
    }
  }
}
```

### Transport Modes

- **stdio mode** (for Claude Desktop): `dotnet run -- --stdio`
- **HTTP mode** (for remote access): `dotnet run` (default)

---

## 💻 Usage

### With Claude Desktop

Once configured, simply ask Claude to interact with your documents:

```
"Search for all invoices from 2024"
"Upload this PDF to Paperless and tag it as 'Receipt'"
"Show me documents with the tag 'Important' that have no correspondent"
"Create a new tag called 'Urgent' with a red color"
```

### HTTP Endpoint

When running in HTTP mode, the MCP endpoint is available at:

```
http://localhost:5000/mcp
```

### Running the Server

```bash
# Stdio mode (Claude Desktop)
dotnet run --project PaperlessMCP/PaperlessMCP -- --stdio

# HTTP mode (remote access)
dotnet run --project PaperlessMCP/PaperlessMCP

# Docker
docker run -e PAPERLESS_BASE_URL=... -e PAPERLESS_API_TOKEN=... -p 5000:5000 paperless-mcp
```

---

## 📖 API Reference

### Health & Capabilities

#### `paperless.ping`
Verify connectivity and authentication with Paperless-ngx.

**Returns:** Connection status and server version

#### `paperless.capabilities`
Return supported API endpoints and detected Paperless-ngx version information.

**Returns:** Available endpoints, bulk operations, and server capabilities

---

### Document Operations

#### `paperless.documents.search`
Search for documents with full-text search and filters.

**Parameters:**
- `query` (string, optional) - Full-text search query
- `tags` (string, optional) - Filter by tag IDs (comma-separated)
- `tagsExclude` (string, optional) - Exclude tag IDs (comma-separated)
- `correspondent` (int, optional) - Filter by correspondent ID
- `documentType` (int, optional) - Filter by document type ID
- `storagePath` (int, optional) - Filter by storage path ID
- `createdAfter` (string, optional) - Filter by creation date (YYYY-MM-DD)
- `createdBefore` (string, optional) - Filter by creation date (YYYY-MM-DD)
- `addedAfter` (string, optional) - Filter by added date (YYYY-MM-DD)
- `addedBefore` (string, optional) - Filter by added date (YYYY-MM-DD)
- `archiveSerialNumber` (int, optional) - Filter by archive serial number
- `page` (int, default: 1) - Page number
- `pageSize` (int, default: 25, max: 100) - Results per page
- `ordering` (string, optional) - Sort field (e.g., 'created', '-created', 'title')
- `includeContent` (bool, default: false) - Include document content in results
- `contentMaxLength` (int, default: 500) - Max content length when `includeContent=true`

**Returns:** Paginated list of document summaries

#### `paperless.documents.get`
Get a document by its ID.

**Parameters:**
- `id` (int, required) - Document ID

**Returns:** Complete document details including content

#### `paperless.documents.download`
Get download URLs for a document's original file, preview, and thumbnail.

**Parameters:**
- `id` (int, required) - Document ID

**Returns:** Object with download URLs (`original_url`, `preview_url`, `thumbnail_url`)

#### `paperless.documents.preview`
Get the preview URL for a document.

**Parameters:**
- `id` (int, required) - Document ID

**Returns:** Preview URL

#### `paperless.documents.thumbnail`
Get the thumbnail URL for a document.

**Parameters:**
- `id` (int, required) - Document ID

**Returns:** Thumbnail URL

#### `paperless.documents.upload`
Upload a new document to Paperless-ngx via base64-encoded content.

**Parameters:**
- `fileContent` (string, required) - Base64-encoded file content
- `fileName` (string, required) - Original filename with extension
- `title` (string, optional) - Document title
- `correspondent` (int, optional) - Correspondent ID
- `documentType` (int, optional) - Document type ID
- `storagePath` (int, optional) - Storage path ID
- `tags` (string, optional) - Tag IDs (comma-separated)
- `archiveSerialNumber` (int, optional) - Archive serial number
- `created` (string, optional) - Created date (YYYY-MM-DD)

**Returns:** Task ID and upload status

**Note:** For large files, use `paperless.documents.upload_from_path` instead.

#### `paperless.documents.upload_from_path`
Upload a document from a local file path. More reliable for large files.

**Parameters:**
- `filePath` (string, required) - Absolute path to the file
- `title` (string, optional) - Document title (defaults to filename)
- `correspondent` (int, optional) - Correspondent ID
- `documentType` (int, optional) - Document type ID
- `storagePath` (int, optional) - Storage path ID
- `tags` (string, optional) - Tag IDs (comma-separated)
- `archiveSerialNumber` (int, optional) - Archive serial number
- `created` (string, optional) - Created date (YYYY-MM-DD)

**Returns:** Task ID, upload status, and file information

**Features:** Supports `~/` expansion, automatic retries, file validation

#### `paperless.documents.update`
Update document metadata.

**Parameters:**
- `id` (int, required) - Document ID
- `title` (string, optional) - New title
- `correspondent` (int, optional) - Correspondent ID (use -1 to clear)
- `documentType` (int, optional) - Document type ID (use -1 to clear)
- `storagePath` (int, optional) - Storage path ID (use -1 to clear)
- `tags` (string, optional) - Tag IDs to set (comma-separated)
- `archiveSerialNumber` (int, optional) - Archive serial number
- `created` (string, optional) - Created date (YYYY-MM-DD)

**Returns:** Updated document

#### `paperless.documents.delete`
Delete a document. Requires explicit confirmation.

**Parameters:**
- `id` (int, required) - Document ID
- `confirm` (bool, default: false) - Must be true to confirm deletion

**Returns:** Deletion status or dry-run preview

**Safety:** Without `confirm=true`, returns a dry-run preview showing what would be deleted.

#### `paperless.documents.bulk_update`
Perform bulk operations on multiple documents.

**Parameters:**
- `documentIds` (string, required) - Document IDs (comma-separated)
- `operation` (string, required) - Operation: `add_tag`, `remove_tag`, `set_correspondent`, `set_document_type`, `set_storage_path`, `delete`, `reprocess`
- `value` (int, optional) - Parameter value (e.g., tag ID, correspondent ID)
- `dryRun` (bool, default: true) - Preview changes without applying
- `confirm` (bool, default: false) - Must be true to execute

**Returns:** Affected document IDs and operation status

**Safety:** Defaults to dry-run mode to prevent accidental changes.

#### `paperless.documents.reprocess`
Reprocess a document's OCR and content extraction.

**Parameters:**
- `id` (int, required) - Document ID
- `confirm` (bool, default: false) - Must be true to confirm reprocessing

**Returns:** Processing status

---

### Tag Operations

#### `paperless.tags.list`
List all tags with pagination.

**Parameters:**
- `page` (int, default: 1) - Page number
- `pageSize` (int, default: 25, max: 100) - Results per page
- `ordering` (string, optional) - Sort field (e.g., 'name', '-document_count')

**Returns:** Paginated list of tags

#### `paperless.tags.get`
Get a tag by its ID.

**Parameters:**
- `id` (int, required) - Tag ID

**Returns:** Tag details

#### `paperless.tags.create`
Create a new tag.

**Parameters:**
- `name` (string, required) - Tag name
- `color` (string, optional) - Hex color (e.g., '#ff0000')
- `match` (string, optional) - Match pattern for auto-tagging
- `matchingAlgorithm` (int, optional) - Matching algorithm (0=None, 1=Any, 2=All, 3=Literal, 4=Regex, 5=Fuzzy, 6=Auto)
- `isInboxTag` (bool, optional) - Mark as inbox tag

**Returns:** Created tag

#### `paperless.tags.update`
Update an existing tag.

**Parameters:**
- `id` (int, required) - Tag ID
- `name` (string, optional) - New name
- `color` (string, optional) - Hex color
- `match` (string, optional) - Match pattern
- `matchingAlgorithm` (int, optional) - Matching algorithm
- `isInboxTag` (bool, optional) - Inbox tag status

**Returns:** Updated tag

#### `paperless.tags.delete`
Delete a tag. Requires explicit confirmation.

**Parameters:**
- `id` (int, required) - Tag ID
- `confirm` (bool, default: false) - Must be true to confirm deletion

**Returns:** Deletion status or dry-run preview

#### `paperless.tags.bulk_delete`
Delete multiple tags.

**Parameters:**
- `tagIds` (string, required) - Tag IDs (comma-separated)
- `dryRun` (bool, default: true) - Preview changes without applying
- `confirm` (bool, default: false) - Must be true to execute

**Returns:** Affected tag IDs and operation status

---

### Correspondent Operations

#### `paperless.correspondents.list`
List all correspondents with pagination.

**Parameters:**
- `page` (int, default: 1) - Page number
- `pageSize` (int, default: 25, max: 100) - Results per page
- `ordering` (string, optional) - Sort field (e.g., 'name', '-document_count', 'last_correspondence')

**Returns:** Paginated list of correspondents

#### `paperless.correspondents.get`
Get a correspondent by its ID.

**Parameters:**
- `id` (int, required) - Correspondent ID

**Returns:** Correspondent details

#### `paperless.correspondents.create`
Create a new correspondent.

**Parameters:**
- `name` (string, required) - Correspondent name
- `match` (string, optional) - Match pattern for auto-assignment
- `matchingAlgorithm` (int, optional) - Matching algorithm (0=None, 1=Any, 2=All, 3=Literal, 4=Regex, 5=Fuzzy, 6=Auto)

**Returns:** Created correspondent

#### `paperless.correspondents.update`
Update an existing correspondent.

**Parameters:**
- `id` (int, required) - Correspondent ID
- `name` (string, optional) - New name
- `match` (string, optional) - Match pattern
- `matchingAlgorithm` (int, optional) - Matching algorithm

**Returns:** Updated correspondent

#### `paperless.correspondents.delete`
Delete a correspondent. Requires explicit confirmation.

**Parameters:**
- `id` (int, required) - Correspondent ID
- `confirm` (bool, default: false) - Must be true to confirm deletion

**Returns:** Deletion status or dry-run preview

#### `paperless.correspondents.bulk_delete`
Delete multiple correspondents.

**Parameters:**
- `correspondentIds` (string, required) - Correspondent IDs (comma-separated)
- `dryRun` (bool, default: true) - Preview changes without applying
- `confirm` (bool, default: false) - Must be true to execute

**Returns:** Affected correspondent IDs and operation status

---

### Document Type Operations

#### `paperless.document_types.list`
List all document types with pagination.

**Parameters:**
- `page` (int, default: 1) - Page number
- `pageSize` (int, default: 25, max: 100) - Results per page
- `ordering` (string, optional) - Sort field (e.g., 'name', '-document_count')

**Returns:** Paginated list of document types

#### `paperless.document_types.get`
Get a document type by its ID.

**Parameters:**
- `id` (int, required) - Document type ID

**Returns:** Document type details

#### `paperless.document_types.create`
Create a new document type.

**Parameters:**
- `name` (string, required) - Document type name
- `match` (string, optional) - Match pattern for auto-assignment
- `matchingAlgorithm` (int, optional) - Matching algorithm (0=None, 1=Any, 2=All, 3=Literal, 4=Regex, 5=Fuzzy, 6=Auto)

**Returns:** Created document type

#### `paperless.document_types.update`
Update an existing document type.

**Parameters:**
- `id` (int, required) - Document type ID
- `name` (string, optional) - New name
- `match` (string, optional) - Match pattern
- `matchingAlgorithm` (int, optional) - Matching algorithm

**Returns:** Updated document type

#### `paperless.document_types.delete`
Delete a document type. Requires explicit confirmation.

**Parameters:**
- `id` (int, required) - Document type ID
- `confirm` (bool, default: false) - Must be true to confirm deletion

**Returns:** Deletion status or dry-run preview

#### `paperless.document_types.bulk_delete`
Delete multiple document types.

**Parameters:**
- `documentTypeIds` (string, required) - Document type IDs (comma-separated)
- `dryRun` (bool, default: true) - Preview changes without applying
- `confirm` (bool, default: false) - Must be true to execute

**Returns:** Affected document type IDs and operation status

---

### Storage Path Operations

#### `paperless.storage_paths.list`
List all storage paths with pagination.

**Parameters:**
- `page` (int, default: 1) - Page number
- `pageSize` (int, default: 25, max: 100) - Results per page
- `ordering` (string, optional) - Sort field (e.g., 'name', '-document_count')

**Returns:** Paginated list of storage paths

#### `paperless.storage_paths.get`
Get a storage path by its ID.

**Parameters:**
- `id` (int, required) - Storage path ID

**Returns:** Storage path details

#### `paperless.storage_paths.create`
Create a new storage path.

**Parameters:**
- `name` (string, required) - Storage path name
- `path` (string, required) - Path template (e.g., `{correspondent}/{document_type}`)
- `match` (string, optional) - Match pattern for auto-assignment
- `matchingAlgorithm` (int, optional) - Matching algorithm (0=None, 1=Any, 2=All, 3=Literal, 4=Regex, 5=Fuzzy, 6=Auto)

**Returns:** Created storage path

#### `paperless.storage_paths.update`
Update an existing storage path.

**Parameters:**
- `id` (int, required) - Storage path ID
- `name` (string, optional) - New name
- `path` (string, optional) - Path template
- `match` (string, optional) - Match pattern
- `matchingAlgorithm` (int, optional) - Matching algorithm

**Returns:** Updated storage path

#### `paperless.storage_paths.delete`
Delete a storage path. Requires explicit confirmation.

**Parameters:**
- `id` (int, required) - Storage path ID
- `confirm` (bool, default: false) - Must be true to confirm deletion

**Returns:** Deletion status or dry-run preview

#### `paperless.storage_paths.bulk_delete`
Delete multiple storage paths.

**Parameters:**
- `storagePathIds` (string, required) - Storage path IDs (comma-separated)
- `dryRun` (bool, default: true) - Preview changes without applying
- `confirm` (bool, default: false) - Must be true to execute

**Returns:** Affected storage path IDs and operation status

---

### Custom Field Operations

#### `paperless.custom_fields.list`
List all custom field definitions with pagination.

**Parameters:**
- `page` (int, default: 1) - Page number
- `pageSize` (int, default: 25, max: 100) - Results per page

**Returns:** Paginated list of custom field definitions

#### `paperless.custom_fields.get`
Get a custom field definition by its ID.

**Parameters:**
- `id` (int, required) - Custom field ID

**Returns:** Custom field details

#### `paperless.custom_fields.create`
Create a new custom field definition.

**Parameters:**
- `name` (string, required) - Custom field name
- `dataType` (string, required) - Data type: `string`, `url`, `date`, `boolean`, `integer`, `float`, `monetary`, `documentlink`, `select`
- `selectOptions` (string, optional) - Select options (comma-separated, for 'select' type only)
- `defaultCurrency` (string, optional) - Default currency (for 'monetary' type only)

**Returns:** Created custom field

#### `paperless.custom_fields.update`
Update an existing custom field definition.

**Parameters:**
- `id` (int, required) - Custom field ID
- `name` (string, optional) - New name
- `selectOptions` (string, optional) - Select options (comma-separated)
- `defaultCurrency` (string, optional) - Default currency

**Returns:** Updated custom field

#### `paperless.custom_fields.delete`
Delete a custom field definition. Requires explicit confirmation.

**Parameters:**
- `id` (int, required) - Custom field ID
- `confirm` (bool, default: false) - Must be true to confirm deletion

**Returns:** Deletion status or dry-run preview

#### `paperless.custom_fields.assign`
Assign a custom field value to a document.

**Parameters:**
- `documentId` (int, required) - Document ID
- `fieldId` (int, required) - Custom field ID
- `value` (string, required) - Value to assign (string, number, boolean, or date depending on field type)

**Returns:** Assignment status and assigned value

---

## 🛠️ Development

### Building from Source

```bash
# Clone the repository
git clone https://github.com/barryw/PaperlessMCP.git
cd PaperlessMCP

# Restore dependencies
dotnet restore

# Build
dotnet build

# Run tests
dotnet test

# Build for release
dotnet build -c Release
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true

# Run specific test file
dotnet test --filter "FullyQualifiedName~DocumentToolsTests"
```

### Project Structure

```
PaperlessMCP/
├── PaperlessMCP/                 # Main application
│   ├── Client/                   # Paperless API client
│   │   ├── PaperlessClient.cs    # Main API client implementation
│   │   └── PaperlessAuthHandler.cs # Authentication handler
│   ├── Configuration/            # Configuration options
│   │   └── PaperlessOptions.cs   # Connection configuration
│   ├── Models/                   # Data models
│   │   ├── Common/               # Shared models
│   │   ├── Documents/            # Document models
│   │   ├── Tags/                 # Tag models
│   │   ├── Correspondents/       # Correspondent models
│   │   ├── DocumentTypes/        # Document type models
│   │   ├── StoragePaths/         # Storage path models
│   │   └── CustomFields/         # Custom field models
│   ├── Tools/                    # MCP tool implementations
│   │   ├── HealthTools.cs        # Health checks
│   │   ├── DocumentTools.cs      # Document operations
│   │   ├── TagTools.cs           # Tag operations
│   │   ├── CorrespondentTools.cs # Correspondent operations
│   │   ├── DocumentTypeTools.cs  # Document type operations
│   │   ├── StoragePathTools.cs   # Storage path operations
│   │   └── CustomFieldTools.cs   # Custom field operations
│   ├── Program.cs                # Application entry point
│   ├── Dockerfile                # Docker configuration
│   └── appsettings.json          # Default configuration
├── PaperlessMCP.Tests/           # Test project
│   ├── Client/                   # Client tests
│   ├── Tools/                    # Tool tests
│   └── Fixtures/                 # Test fixtures
└── PaperlessMCP.sln              # Solution file
```

### Technology Stack

- **.NET 10** - Modern, cross-platform framework
- **ModelContextProtocol 0.2.0-preview.1** - MCP server implementation
- **Polly 8.5.2** - Resilience and transient fault handling
- **xUnit** - Testing framework

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

### Guidelines

1. **Fork the repository** and create your branch from `main`
2. **Write tests** for any new functionality
3. **Ensure all tests pass** with `dotnet test`
4. **Follow the existing code style** and conventions
5. **Update documentation** for any API changes
6. **Submit a pull request** with a clear description of changes

### Development Setup

```bash
# Fork and clone your fork
git clone https://github.com/YOUR-USERNAME/PaperlessMCP.git
cd PaperlessMCP

# Create a feature branch
git checkout -b feature/your-feature-name

# Make changes and test
dotnet test

# Commit and push
git add .
git commit -m "Add your feature"
git push origin feature/your-feature-name
```

---

## 🐛 Troubleshooting

### Common Issues

#### Connection Errors

**Problem:** "Failed to connect to Paperless instance"

**Solution:**
- Verify `PAPERLESS_BASE_URL` is correct and accessible
- Ensure your Paperless-ngx instance is running
- Check network connectivity and firewall rules
- Verify API endpoint: `curl https://your-instance/api/`

#### Authentication Errors

**Problem:** "401 Unauthorized"

**Solution:**
- Verify your `PAPERLESS_API_TOKEN` is correct
- Generate a new token in Paperless-ngx: Settings → API → Tokens
- Ensure the token has necessary permissions

#### Upload Failures

**Problem:** Document uploads timeout or fail

**Solution:**
- For large files, use `paperless.documents.upload_from_path` instead of base64 upload
- Check file size limits in Paperless-ngx configuration
- Verify disk space on Paperless-ngx server
- Check Paperless-ngx logs for processing errors

#### Claude Desktop Integration

**Problem:** Server doesn't appear in Claude Desktop

**Solution:**
- Verify JSON syntax in `claude_desktop_config.json`
- Use absolute paths (no relative paths or `~`)
- Restart Claude Desktop completely
- Check Claude Desktop logs for errors

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- [Paperless-ngx](https://github.com/paperless-ngx/paperless-ngx) - The excellent document management system
- [Model Context Protocol](https://modelcontextprotocol.io/) - For the MCP specification
- [Anthropic](https://www.anthropic.com/) - For Claude and the MCP implementation

---

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/barryw/PaperlessMCP/issues)
- **Discussions**: [GitHub Discussions](https://github.com/barryw/PaperlessMCP/discussions)
- **Paperless-ngx Docs**: [docs.paperless-ngx.com](https://docs.paperless-ngx.com/)
- **MCP Docs**: [modelcontextprotocol.io](https://modelcontextprotocol.io/)

---

<div align="center">

**[⬆ back to top](#-paperlessmcp)**

Made with ❤️ for the Paperless-ngx and MCP communities

</div>
