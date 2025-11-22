# Syntax & Style Guide

**Version:** 1.0
**Last Updated:** 2025-11-22
**Status:** Foundation Document

---

## Core Principle

**Consistency in syntax enables pattern recognition.** When code follows uniform style conventions, developers can focus on logic rather than syntax variations. These rules reduce cognitive load and make the codebase feel cohesive.

---

## Method Naming Conventions

### Private Methods - Lower Camel Case

All private methods use lower camel case (first letter lowercase).

```csharp
// ✅ Correct
private async Task<User> getUserByEmail(string email) =>
    await _userRepository.GetByEmailAsync(email);

private bool isEmailValid(string email) =>
    !string.IsNullOrEmpty(email) && email.Contains("@");

private IApplicationResult<RegisterResponse> buildSuccessResponse() =>
    ApplicationResult<RegisterResponse>.Succeed(
        new RegisterResponse { Message = "Account created successfully." }
    );

// ❌ Incorrect
private async Task<User> GetUserByEmail(string email) =>  // Should be getUserByEmail
    await _userRepository.GetByEmailAsync(email);

private bool IsEmailValid(string email) =>  // Should be isEmailValid
    !string.IsNullOrEmpty(email) && email.Contains("@");
```

**Why:** Private methods are internal implementation details. Lower camel case visually distinguishes them from public API methods, making the public surface area immediately obvious when scanning code.

### Public Methods - Upper Camel Case (PascalCase)

All public methods use upper camel case (PascalCase).

```csharp
// ✅ Correct
public async Task<IApplicationResult<LoginResponse>> Perform(LoginRequest request)
{
    var validationResult = await validate(request);
    if (!validationResult.IsValid)
        return buildErrorResponse(validationResult);

    return await processLogin(request);
}

// ❌ Incorrect
public async Task<IApplicationResult<LoginResponse>> perform(LoginRequest request)  // Should be Perform
{
    // ...
}
```

**Why:** Public methods are the API surface. PascalCase follows C# conventions and makes the contract boundary clear.

---

## Constructor Style - Primary Constructors

Use C# 12+ primary constructors with underscore-prefixed parameter names.

### Multi-Line Formatting (2+ Dependencies)

When a class has **two or more dependencies**, format the constructor with each parameter on its own line and closing parenthesis on a separate line:

```csharp
// ✅ Correct - Multi-line with closing paren on own line
public class LoginWorkflow(
    IUserRepository _userRepository,
    IPasswordHasher _passwordHasher,
    IJwtTokenService _jwtTokenService,
    IValidator<LoginRequest> _validator
) : IWorkflow<LoginRequest, LoginResponse>
{
    public async Task<IApplicationResult<LoginResponse>> Perform(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        var isValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
        // ...
    }
}

// ❌ Incorrect - Closing paren on same line as last parameter
public class LoginWorkflow(
    IUserRepository _userRepository,
    IPasswordHasher _passwordHasher) : IWorkflow<LoginRequest, LoginResponse>
{
    // ...
}

// ❌ Incorrect - All on one line (only acceptable for single dependency)
public class LoginWorkflow(IUserRepository _userRepository, IPasswordHasher _passwordHasher) : IWorkflow<LoginRequest, LoginResponse>
{
    // ...
}
```

### Single Dependency Formatting

For classes with **only one dependency**, use a single line:

```csharp
// ✅ Correct - Single dependency on one line
public class UserRepository(AppDbContext _context) : Repository<User>(_context), IUserRepository
{
    public async Task<User> GetByEmailAsync(string email) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
}
```

### Traditional Constructor (Incorrect)

```csharp
// ❌ Incorrect - Traditional constructor with field assignments
public class LoginWorkflow : IWorkflow<LoginRequest, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public LoginWorkflow(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }
}
```

**Why:** Primary constructors eliminate boilerplate field declarations and assignments. The underscore prefix maintains consistency with traditional field naming while making dependency injection parameters immediately recognizable. This reduces a 10-line constructor to 1 line.

**Formatting Rationale:**
- **Closing paren on own line:** Makes it clear where parameters end and base class/interfaces begin
- **Vertical alignment:** Easy to scan dependencies at a glance
- **Diff-friendly:** Adding/removing dependencies changes minimal lines in version control
- **Single dependency exception:** No need for multi-line when there's only one parameter

**Benefits:**
- **Less boilerplate:** No field declarations, no assignments
- **Clearer intent:** Dependencies are visible at class definition
- **Consistent naming:** `_param` works both as parameter and field
- **Easier refactoring:** Add/remove dependencies in one place

---

## Field Naming - Underscore Prefix

Private fields use underscore prefix with lower camel case.

```csharp
// ✅ Correct
private readonly IUserRepository _userRepository;
private readonly string _connectionString;
private int _retryCount;

// ❌ Incorrect
private readonly IUserRepository userRepository;  // Missing underscore
private readonly IUserRepository m_userRepository;  // Hungarian notation not used
```

**Note:** With primary constructors, you rarely declare fields explicitly—parameters become fields automatically.

---

## Local Variables and Parameters - Lower Camel Case

```csharp
// ✅ Correct
public async Task<User> ProcessRegistration(RegisterRequest request)
{
    var hashedPassword = _passwordHasher.HashPassword(request.Password);
    var newUser = User.Create(request.Email, hashedPassword);
    var savedUser = await _userRepository.AddAsync(newUser);
    return savedUser;
}

// ❌ Incorrect
public async Task<User> ProcessRegistration(RegisterRequest Request)  // Parameter should be lowercase
{
    var HashedPassword = _passwordHasher.HashPassword(Request.Password);  // Should be hashedPassword
}
```

---

## Expression-Bodied Members

Prefer expression-bodied members for simple operations.

```csharp
// ✅ Correct - Expression-bodied for simple operations
private IApplicationResult<RegisterResponse> buildErrorResponse(string message) =>
    ApplicationResult<RegisterResponse>.Fail(message);

private bool isAuthenticated() =>
    _currentUser != null;

private string getUserDisplayName() =>
    _currentUser?.Email ?? "Guest";

// ✅ Also correct - Block body for complex logic
private async Task<bool> validateUserCredentials(string email, string password)
{
    var user = await _userRepository.GetByEmailAsync(email);
    if (user == null)
        return false;

    return _passwordHasher.VerifyPassword(password, user.PasswordHash);
}

// ❌ Incorrect - Block body for simple operation
private bool isAuthenticated()
{
    return _currentUser != null;
}
```

**Guideline:** Use expression-bodied members when the body is a single expression or simple return statement. Use block bodies when you need multiple statements, early returns, or complex logic.

---

## Pattern Examples

### Workflow Pattern

```csharp
public class RegisterWorkflow(
    IPasswordHasher _passwordHasher,
    IUserRepository _userRepository,
    IValidator<RegisterRequest> _validator
) : IWorkflow<RegisterRequest, RegisterResponse>
{
    public async Task<IApplicationResult<RegisterResponse>> Perform(RegisterRequest request)
    {
        var validationResult = await validate(request);
        if (!validationResult.IsValid)
            return buildErrorResponse(validationResult);

        var user = createUser(request);
        var emailExists = await checkEmailExists(user.Email);
        if (emailExists)
            return buildErrorResponse("An account with this email already exists.");

        await saveUser(user);
        return buildSuccessResponse();
    }

    private async Task<ValidationResult> validate(RegisterRequest request) =>
        await _validator.ValidateAsync(request);

    private async Task<bool> checkEmailExists(string email) =>
        await _userRepository.EmailExistsAsync(email);

    private User createUser(RegisterRequest request) =>
        User.Create(request.Email, _passwordHasher.HashPassword(request.Password));

    private async Task saveUser(User user)
    {
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    private IApplicationResult<RegisterResponse> buildErrorResponse(ValidationResult result) =>
        ApplicationResult<RegisterResponse>.Fail(result);

    private IApplicationResult<RegisterResponse> buildErrorResponse(string message) =>
        ApplicationResult<RegisterResponse>.Fail(message);

    private IApplicationResult<RegisterResponse> buildSuccessResponse() =>
        ApplicationResult<RegisterResponse>.Succeed(
            new RegisterResponse { Message = "Account created successfully. You can now log in." }
        );
}
```

### Repository Pattern

```csharp
public class UserRepository(AppDbContext _context) : Repository<User>(_context), IUserRepository
{
    public async Task<User> GetByEmailAsync(string email) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User> GetByIdAsync(long id) =>
        await _context.Users.FindAsync(id);

    public async Task<bool> EmailExistsAsync(string email) =>
        await _context.Users.AnyAsync(u => u.Email == email);
}
```

---

## Nullability - Explicit Intent Over Suppression

### Nullable Reference Types - Disabled Project-Wide

This project has **nullable reference types disabled** (`<Nullable>disable</Nullable>`). All reference types are non-nullable by default.

```csharp
// ✅ Correct - No nullable annotations
public class User
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
}

public async Task<User> GetByEmailAsync(string email) =>
    await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

// ❌ Incorrect - Do not add nullable annotations
public async Task<User?> GetByEmailAsync(string email) =>  // Remove the ?
    await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

public class User
{
    public string? Email { get; set; }  // Remove the ?
    public string? PasswordHash { get; set; }  // Remove the ?
}
```

**Why:** We've made the architectural decision to disable nullable reference types. This ensures:
- **Consistency:** No mix of nullable and non-nullable contexts
- **Simplicity:** No cognitive load from nullable warnings
- **Explicit handling:** Null checks happen through logic, not type annotations

### Null-Forgiving Operator (!) - Requires Discussion

The null-forgiving operator (`!`) should **almost never be used**. Its presence indicates one of two problems:

1. **Logic issue:** You're unsure if the value is actually non-null
2. **Type system mismatch:** The type system doesn't understand your guarantee

```csharp
// ❌ Incorrect - Suppressing potential null
public async Task<string> GetUserEmail(long userId)
{
    var user = await _userRepository.GetByIdAsync(userId);
    return user!.Email;  // Dangerous! What if user is null?
}

// ✅ Correct - Handle null explicitly
public async Task<string> GetUserEmail(long userId)
{
    var user = await _userRepository.GetByIdAsync(userId);
    if (user == null)
        throw new InvalidOperationException($"User with ID {userId} not found.");

    return user.Email;
}

// ✅ Also correct - Return early pattern
public async Task<IApplicationResult<string>> GetUserEmail(long userId)
{
    var user = await _userRepository.GetByIdAsync(userId);
    if (user == null)
        return ApplicationResult<string>.Fail("User not found.");

    return ApplicationResult<string>.Succeed(user.Email);
}
```

**When ! might be acceptable (rare cases):**
- Framework code where you have absolute guarantees (e.g., DI-injected services)
- Immediately after null checks where the compiler can't infer safety
- Test code where null scenarios are explicitly prevented by test setup

**Rule:** Before using `!`, ask:
1. Can I restructure the code to eliminate the need?
2. Can I add an explicit null check instead?
3. If I must use it, can I add a comment explaining the guarantee?

**Discussion Required:** Any use of `!` in production code should be discussed during code review. It's a signal that something might be architecturally wrong.

---

## Consistency Checklist

When writing or reviewing code, check:

- [ ] Private methods use lower camel case (`getUserById`, not `GetUserById`)
- [ ] Public methods use PascalCase (`Perform`, not `perform`)
- [ ] Classes use primary constructors with `_param` naming
- [ ] No explicit field declarations when primary constructor suffices
- [ ] Simple methods use expression-bodied syntax (`=>`)
- [ ] Complex methods use block bodies with proper formatting
- [ ] Local variables use lower camel case
- [ ] No `?` nullable annotations on reference types
- [ ] No `!` null-forgiving operators without explicit discussion
- [ ] Null handling is explicit through logic, not type suppression
- [ ] Code matches existing patterns in the codebase

---

## Why These Rules Matter

**For AI Agents:**
- Consistent patterns → accurate code generation
- Clear conventions → fewer corrections needed
- Predictable structure → better context understanding

**For Developers:**
- **Scan efficiency:** Private vs public methods are instantly distinguishable
- **Pattern recognition:** Similar code looks similar
- **Less cognitive load:** No decisions about style, just follow the pattern
- **Faster reviews:** Style is consistent, focus on logic

**For Codebase:**
- **Uniform appearance:** All files feel like one cohesive system
- **Easier refactoring:** Patterns are predictable across features
- **Lower maintenance:** Consistent code is easier to understand 6 months later

---

## Summary

**Method Naming:**
- Private methods: `lowerCamelCase`
- Public methods: `PascalCase`

**Constructors:**
- Use primary constructors: `public class Foo(_dep1, _dep2) {}`
- Parameters use underscore prefix: `_dependency`

**Expression Bodies:**
- Simple operations: Use `=>`
- Complex logic: Use block bodies

**The Goal:** A codebase where style never distracts from intent. When you read any file, it should feel like the same author wrote it—because the same patterns guide everyone.

---

**Next:** Apply these conventions consistently. When in doubt, find a similar feature and match its style.
