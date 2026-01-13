<div align="center">

# 📄 PaperlessMCP

**Model Context Protocol Server for Paperless-ngx**

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![MCP](https://img.shields.io/badge/MCP-0.2.0--preview.1-blue)](https://modelcontextprotocol.io/)

*Seamlessly integrate your Paperless-ngx document management system with Claude via the Model Context Protocol*

[Features](#-features) • [Installation](#-installation) • [Configuration](#-configuration) • [API Reference](docs/API_REFERENCE.md) • [Contributing](#-contributing)

</div>

---

## 🎯 Overview

PaperlessMCP is a Model Context Protocol (MCP) server that bridges [Paperless-ngx](https://github.com/paperless-ngx/paperless-ngx) with AI assistants like Claude. It provides comprehensive document management capabilities through a modern, type-safe .NET implementation.

### What is MCP?

The [Model Context Protocol](https://modelcontextprotocol.io/) enables AI models to securely interact with external data sources and tools. PaperlessMCP implements this protocol to give Claude direct access to your document management system.

---

## ✨ Features

| Category | Capabilities |
|----------|-------------|
| **Documents** | Search, upload, download, update, delete, bulk operations, reprocess OCR |
| **Tags** | Create, manage, auto-tagging rules, bulk delete |
| **Correspondents** | Track document sources with auto-assignment |
| **Document Types** | Classify documents with custom types |
| **Storage Paths** | Organize files with template-based paths |
| **Custom Fields** | Define metadata fields (string, date, boolean, monetary, etc.) |

**Developer Features:** Dual transport (stdio/HTTP), pagination, dry-run mode, retry logic, comprehensive tests

---

## 🚀 Installation

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A running [Paperless-ngx](https://github.com/paperless-ngx/paperless-ngx) instance
- Paperless-ngx API token ([How to get one](https://docs.paperless-ngx.com/api/#authorization))

### Quick Start

```bash
git clone https://github.com/barryw/PaperlessMCP.git
cd PaperlessMCP
dotnet build
```

### Claude Desktop Integration

Add to your Claude Desktop config (`~/Library/Application Support/Claude/claude_desktop_config.json` on macOS):

```json
{
  "mcpServers": {
    "paperless": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/PaperlessMCP/PaperlessMCP", "--", "--stdio"],
      "env": {
        "PAPERLESS_BASE_URL": "https://your-paperless-instance.com",
        "PAPERLESS_API_TOKEN": "your-token-here"
      }
    }
  }
}
```

### Docker

```bash
docker build -t paperless-mcp ./PaperlessMCP
docker run -e PAPERLESS_BASE_URL=https://... -e PAPERLESS_API_TOKEN=... -p 5000:5000 paperless-mcp
```

---

## ⚙️ Configuration

| Variable | Required | Default | Description |
|----------|----------|---------|-------------|
| `PAPERLESS_BASE_URL` | Yes | - | Base URL of your Paperless-ngx instance |
| `PAPERLESS_API_TOKEN` | Yes | - | API authentication token |
| `MAX_PAGE_SIZE` | No | 100 | Maximum items per page |
| `MCP_PORT` | No | 5000 | HTTP server port (HTTP mode only) |

**Transport Modes:**
- `dotnet run -- --stdio` — For Claude Desktop
- `dotnet run` — HTTP mode at `http://localhost:5000/mcp`

---

## 💻 Usage

Once configured, ask Claude to interact with your documents:

```
"Search for all invoices from 2024"
"Upload this PDF and tag it as 'Receipt'"
"Show me documents tagged 'Important' with no correspondent"
"Create a new tag called 'Urgent' with a red color"
```

---

## 📖 API Reference

PaperlessMCP provides **43+ MCP tools** across these categories:

| Category | Tools |
|----------|-------|
| Health | `ping`, `capabilities` |
| Documents | `search`, `get`, `upload`, `upload_from_path`, `update`, `delete`, `bulk_update`, `download`, `preview`, `thumbnail`, `reprocess` |
| Tags | `list`, `get`, `create`, `update`, `delete`, `bulk_delete` |
| Correspondents | `list`, `get`, `create`, `update`, `delete`, `bulk_delete` |
| Document Types | `list`, `get`, `create`, `update`, `delete`, `bulk_delete` |
| Storage Paths | `list`, `get`, `create`, `update`, `delete`, `bulk_delete` |
| Custom Fields | `list`, `get`, `create`, `update`, `delete`, `assign` |

**[📚 Full API Reference →](docs/API_REFERENCE.md)**

---

## 🛠️ Development

```bash
dotnet restore      # Restore dependencies
dotnet build        # Build
dotnet test         # Run tests
```

### CI/CD

This project uses [Woodpecker CI](https://woodpecker-ci.org/) with trunk-based development:

| Event | Actions |
|-------|---------|
| **Pull Request** | Build → Test → Docker verify |
| **Merge to main** | Build → Test → Version → Package → Docker → Tag → Release |

**Automatic Versioning:** Version bumps are determined by [Conventional Commits](https://www.conventionalcommits.org/):

| Commit Type | Version Bump | Example |
|-------------|--------------|---------|
| `fix:` | Patch (0.0.X) | `fix: handle null response` |
| `feat:` | Minor (0.X.0) | `feat: add bulk export` |
| `feat!:` | Major (X.0.0) | `feat!: change API format` |

**Docker Images:**
- `ghcr.io/barryw/paperlessmcp:latest` — Latest release
- `ghcr.io/barryw/paperlessmcp:vX.Y.Z` — Specific version

### Project Structure

```
PaperlessMCP/
├── PaperlessMCP/           # Main application
│   ├── Client/             # Paperless API client
│   ├── Models/             # Data models
│   ├── Tools/              # MCP tool implementations
│   └── Program.cs          # Entry point
├── PaperlessMCP.Tests/     # Test project
└── docs/                   # Documentation
```

---

## 🤝 Contributing

Contributions welcome! We use trunk-based development with conventional commits.

See **[CONTRIBUTING.md](CONTRIBUTING.md)** for guidelines on:
- Commit message format
- Pull request process
- Local development setup

---

## 📄 License

MIT License - see [LICENSE](LICENSE) for details.

---

## 🙏 Acknowledgments

- [Paperless-ngx](https://github.com/paperless-ngx/paperless-ngx)
- [Model Context Protocol](https://modelcontextprotocol.io/)
- [Anthropic](https://www.anthropic.com/)

---

<div align="center">

**[⬆ back to top](#-paperlessmcp)**

Made with ❤️ for the Paperless-ngx community

</div>
