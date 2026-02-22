# Local Changes to PaperlessMCP

This file documents the modifications made to the local PaperlessMCP build to support the Paperless-ngx reorganization project.

## [2026-02-20] - Nested Tag Support & Logging

### Added
- **Nested Tag Support:**
    - Updated `PaperlessMCP/Models/Tags/Tag.cs` to include the `parent` property in `Tag`, `TagCreateRequest`, and `TagUpdateRequest` records.
    - Updated `PaperlessMCP/Tools/TagTools.cs` to expose the `parent` parameter in the `Create` and `Update` MCP tools.
- **Diagnostic Logging:**
    - Modified `PaperlessMCP/Client/PaperlessClient.cs` to log the URL and JSON body of all `POST` and `PATCH` requests. This was used to verify that request bodies were being serialized correctly.

### Fixed
- Resolved an issue where tag creation/updates were failing with "This field is required" errors due to missing fields in the serialization models.

### Deployment Notes
- These changes are currently uncommitted in the local git repository.
- The `paperless-mcp` Docker container was rebuilt from source to apply these changes.
