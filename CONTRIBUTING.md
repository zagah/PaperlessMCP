# Contributing to PaperlessMCP

Thank you for your interest in contributing! This document outlines our development workflow and guidelines.

## Development Workflow

We use **trunk-based development**:

1. **Create a feature branch** from `main`
2. **Make your changes** with conventional commits
3. **Open a pull request** to `main`
4. **CI runs** build and tests
5. **Merge to main** triggers automatic release

```
main ←── feature/add-search-filters
     ←── fix/upload-timeout
     ←── feat/bulk-export
```

## Conventional Commits

We use [Conventional Commits](https://www.conventionalcommits.org/) for automatic versioning. Your commit messages determine the version bump:

### Commit Format

```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

### Types and Version Bumps

| Type | Description | Version Bump |
|------|-------------|--------------|
| `fix:` | Bug fixes | Patch (0.0.X) |
| `feat:` | New features | Minor (0.X.0) |
| `feat!:` | Breaking changes | Major (X.0.0) |
| `docs:` | Documentation only | Patch |
| `style:` | Code style (formatting) | Patch |
| `refactor:` | Code refactoring | Patch |
| `test:` | Adding/updating tests | Patch |
| `chore:` | Maintenance tasks | Patch |

### Examples

```bash
# Bug fix → v1.0.1
git commit -m "fix: handle null response from API"

# New feature → v1.1.0
git commit -m "feat: add document export functionality"

# Breaking change → v2.0.0
git commit -m "feat!: change API response format"

# Or with BREAKING CHANGE footer
git commit -m "feat: redesign search API

BREAKING CHANGE: search now returns paginated results by default"
```

### Scopes (Optional)

Use scopes to indicate the affected area:

```bash
git commit -m "fix(upload): increase timeout for large files"
git commit -m "feat(tags): add bulk delete operation"
git commit -m "docs(readme): update installation instructions"
```

## Pull Request Process

1. **Branch naming**: Use descriptive names
   - `feat/description` for features
   - `fix/description` for bug fixes
   - `docs/description` for documentation

2. **PR title**: Use conventional commit format
   - The PR title becomes the merge commit message
   - Example: `feat: add document preview endpoint`

3. **Description**: Include
   - What changes were made
   - Why the changes were needed
   - How to test the changes

4. **CI checks**: Ensure all checks pass
   - Build succeeds
   - Tests pass
   - Docker builds

## Local Development

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Docker (optional, for container testing)

### Setup

```bash
# Clone the repo
git clone https://github.com/barryw/PaperlessMCP.git
cd PaperlessMCP

# Restore and build
dotnet restore
dotnet build

# Run tests
dotnet test
```

### Running Locally

```bash
# Set environment variables
export PAPERLESS_BASE_URL=https://your-instance.com
export PAPERLESS_API_TOKEN=your-token

# Run in stdio mode (for testing with Claude)
dotnet run --project PaperlessMCP/PaperlessMCP -- --stdio

# Run in HTTP mode
dotnet run --project PaperlessMCP/PaperlessMCP
```

### Running Tests

```bash
# All tests
dotnet test

# With verbose output
dotnet test --logger "console;verbosity=detailed"

# Specific test class
dotnet test --filter "FullyQualifiedName~DocumentToolsTests"
```

## Code Style

- Follow existing code patterns
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Keep methods focused and small

## Questions?

Open an issue for questions or discussions about contributing.
