# File Upload Security Considerations

## Current State

**Status:** No file upload functionality implemented yet

**This Document:** Planning for future feature addition

## Threat Model

**Common File Upload Attacks:**
1. **Malware Upload**: Executable files (ransomware, backdoors)
2. **Web Shell Upload**: PHP/ASPX files executed on server
3. **Path Traversal**: `../../etc/passwd` in filenames
4. **Zip Bombs**: 42KB file that decompresses to 4.5PB
5. **XXE Attacks**: Malicious XML in SVG, Office docs
6. **Image Exploits**: Crafted images triggering parser vulnerabilities
7. **Content-Type Spoofing**: Executable disguised as image

## Security Requirements

**File Validation (Required):**
1. Validate MIME type (Content-Type header)
2. Validate file extension (whitelist only)
3. Validate file signature (magic bytes)
4. Check file size limits
5. Sanitize filename (remove path traversal)

**Content Scanning (Recommended):**
1. Antivirus scanning (ClamAV, Windows Defender API)
2. Image reprocessing (strip EXIF, regenerate)
3. Document parsing validation

**Storage Security (Required):**
1. Store outside web root (prevent direct execution)
2. Generate random filenames (prevent enumeration)
3. Set strict permissions (no execute bit)
4. Use separate storage domain (prevent cookie theft)

## Implementation Phases

**Phase 1 - Basic Upload:**
- Whitelist file extensions: `.jpg, .png, .pdf, .txt`
- Max file size: 5 MB (already configured in RequestSizeLimits)
- Store in `/uploads` outside web root
- Generate UUID filenames

**Phase 2 - Content Validation:**
- Validate magic bytes match extension
- Reprocess images using `ImageSharp`
- Parse and validate document structure
- Reject files with mismatched signatures

**Phase 3 - Advanced Security:**
- Integrate antivirus scanning (ClamAV)
- Store files in Azure Blob Storage or AWS S3
- Serve files through CDN with signed URLs
- Implement virus quarantine queue

## File Type Validation

**Extension Whitelist (Never Blacklist):**
```csharp
private static readonly HashSet<string> AllowedExtensions = new()
{
    ".jpg", ".jpeg", ".png", ".gif", ".webp",  // Images
    ".pdf",                                      // Documents
    ".txt", ".md"                                // Text
};
```

**MIME Type Validation:**
```csharp
private static readonly Dictionary<string, string[]> MimeTypeMap = new()
{
    { ".jpg", new[] { "image/jpeg" } },
    { ".png", new[] { "image/png" } },
    { ".pdf", new[] { "application/pdf" } }
};
```

**Magic Bytes Validation (File Signature):**
```csharp
// Detect file type by content, not extension
// Example: JPEG starts with FF D8 FF
// PNG starts with 89 50 4E 47
private static bool IsValidJpeg(byte[] fileBytes)
{
    return fileBytes.Length > 3
        && fileBytes[0] == 0xFF
        && fileBytes[1] == 0xD8
        && fileBytes[2] == 0xFF;
}
```

## Filename Sanitization

**Remove Path Traversal:**
```csharp
public string SanitizeFilename(string filename)
{
    // Remove directory separators
    filename = Path.GetFileName(filename);

    // Remove null bytes
    filename = filename.Replace("\0", "");

    // Remove invalid characters
    var invalidChars = Path.GetInvalidFileNameChars();
    filename = string.Concat(filename.Split(invalidChars));

    // Limit length
    if (filename.Length > 255)
        filename = filename.Substring(0, 255);

    return filename;
}
```

**Generate Safe Storage Name:**
```csharp
public string GenerateStorageFilename(string originalFilename)
{
    var extension = Path.GetExtension(originalFilename);
    return $"{Guid.NewGuid()}{extension}";
}
```

## Storage Architecture

**Option 1: Local File System (Simple)**
```
/app
  /wwwroot (web root - no uploads here!)
  /storage
    /uploads
      /images
      /documents
```

**Pros:** Simple, no dependencies
**Cons:** No scaling, no CDN, backup complexity

**Option 2: Cloud Storage (Recommended)**
- Azure Blob Storage or AWS S3
- Separate domain: `uploads.pageplay.com`
- Signed URLs for access (prevent hotlinking)
- Automatic CDN integration
- Built-in malware scanning (Azure Defender)

**Pros:** Scalable, secure, CDN, backups
**Cons:** Cost, complexity, vendor lock-in

## Image Processing Security

**Why Reprocess Images:**
- Strip malicious EXIF data
- Prevent parser exploits
- Normalize format
- Generate thumbnails

**Using ImageSharp:**
```csharp
using (var image = await Image.LoadAsync(uploadStream))
{
    // This decodes and re-encodes, stripping exploits
    image.Mutate(x => x.Resize(new ResizeOptions
    {
        Mode = ResizeMode.Max,
        Size = new Size(2000, 2000)
    }));

    await image.SaveAsync(outputPath, new JpegEncoder());
}
```

## Antivirus Integration

**ClamAV (Open Source):**
- Run as Docker container
- Scan files via REST API
- Quarantine on detection
- Cost: Free (self-hosted)

**Cloud AV Services:**
- Azure Defender for Storage
- AWS S3 Malware Scanning
- VirusTotal API
- Cost: Pay per scan

**Implementation:**
```csharp
public async Task<bool> ScanForMalwareAsync(Stream fileStream)
{
    // Send to ClamAV REST API
    var result = await _clamAvClient.ScanAsync(fileStream);

    if (result.IsInfected)
    {
        _logger.Warn("Malware detected: {Virus}", result.VirusName);
        // Move to quarantine, alert admins
        return false;
    }

    return true;
}
```

## Access Control

**Upload Permissions:**
- Require authentication
- Rate limit per user (5 files/minute)
- Quota per user (100 MB total)
- Log all uploads with user ID

**Download Permissions:**
- Serve through controller, not direct file access
- Check ownership or public status
- Generate temporary signed URLs (expire in 1 hour)
- Log downloads for audit

**Example:**
```csharp
[Authorize]
[RequestSizeLimit(10_485_760)] // 10 MB for file uploads
public async Task<IActionResult> UploadFile(IFormFile file)
{
    // Validate, scan, store
}

public async Task<IActionResult> DownloadFile(Guid fileId)
{
    // Check permissions
    // Return File() or Redirect to signed URL
}
```

## Current Architecture Integration

**Repository Pattern:**
- Add `IFileRepository` for file metadata
- Store: filename, size, content type, owner, upload date

**Performer Pattern:**
- `UploadFilePerformer`: validation, scanning, storage
- `DownloadFilePerformer`: permission check, logging

**Middleware:**
- Already configured: 5 MB request body limit
- Override with `[RequestSizeLimit]` for uploads

## Open Questions

- Which cloud storage provider? (Azure Blob, AWS S3, Cloudflare R2)
- Self-hosted vs cloud antivirus scanning?
- Image-only uploads or allow documents?
- Per-user storage quotas?
- File retention policy (auto-delete old files)?
