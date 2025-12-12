# Input Validation & Sanitization Considerations

## Current State

**Validation Location:**
- Endpoint-level validation in workflow boundary contracts
- FluentValidation or data annotations
- Validates structure, not malicious content

**Sanitization:**
- Minimal - relies on output encoding
- No centralized input sanitization layer

## Validation vs Sanitization

**Validation**: Is the input acceptable? (reject if not)
- Email format, string length, required fields
- Type constraints, range checks

**Sanitization**: Make input safe by removing/encoding dangerous parts
- Strip HTML tags, encode special characters
- Remove SQL keywords, escape quotes

## Key Concerns

**Where We're Protected:**
- ✅ EF Core parameterization prevents SQL injection
- ✅ Razor encoding prevents basic XSS on output
- ✅ JWT validation prevents token tampering

**Where We're Vulnerable:**
- ⚠️ No input sanitization before storage (stored XSS if encoding fails)
- ⚠️ No centralized validation middleware (inconsistent validation)
- ⚠️ No defense against polyglot payloads (valid JSON + malicious content)
- ⚠️ No validation of content type vs actual content (file upload spoofing)

## Implementation Phases

**Phase 1 (Current):**
- ✅ Endpoint-level validation in workflows
- Consider: Add `[StringLength]`, `[RegularExpression]` attributes

**Phase 2 (Next):**
- Add centralized input sanitization for user-generated content
- Sanitize before storage: strip HTML, trim whitespace, normalize unicode
- Create `IInputSanitizer` service for reusable sanitization rules

**Phase 3 (Production):**
- Add WAF layer (Cloudflare, Azure Application Gateway)
- Implement content security validation (detect injection attempts)
- Add rate limiting per input pattern (slow brute force detection)

## Sanitization Strategies

**HTML Content (if allowing rich text):**
- Use `HtmlSanitizer` library (AngleSharp-based)
- Whitelist allowed tags: `<p>, <strong>, <em>, <a>`
- Strip all attributes except `href` on `<a>`

**Plain Text (current use case):**
```csharp
public string SanitizePlainText(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return string.Empty;

    return input
        .Trim()
        .Replace("\0", "") // Remove null bytes
        .Normalize(NormalizationForm.FormC); // Normalize unicode
}
```

**Email Addresses:**
- Validate format with regex
- Normalize: trim, lowercase
- Block disposable email domains (if needed)

## OWASP Recommendations

**Always Validate:**
1. Input type (string, int, email)
2. Length constraints (prevent buffer overflow)
3. Format (regex patterns)
4. Range (min/max values)
5. Whitelisted values (enums, dropdown options)

**Always Sanitize:**
1. Before storage (defense in depth)
2. Before rendering (HTML encoding)
3. Before SQL (parameterization)
4. Before logging (prevent log injection)

## Current Architecture Integration

**Where to Add Sanitization:**
- `PerformerRequest` base class: Add `Sanitize()` method
- `Repository.SaveAsync()`: Sanitize before persistence
- Endpoint filters: Add `SanitizeInputFilter`

**Validation Libraries:**
- FluentValidation (current recommendation)
- `System.ComponentModel.DataAnnotations`
- Custom validators for complex rules

## Open Questions

- Should we sanitize on input or before storage? (Both = defense in depth)
- Do we need rich text editor support? (Requires HTML sanitizer)
- Should we log rejected inputs for security monitoring?
- What level of unicode normalization? (FormC = most compatible)
