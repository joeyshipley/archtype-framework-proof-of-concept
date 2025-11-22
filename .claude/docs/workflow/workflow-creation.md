You are helping the user create a new workflow following the vertical slice architecture pattern.

## User will provide:
- Feature name (e.g., "UpdateProfile", "CreateProject")
- Domain area (e.g., "Accounts", "Projects")

## Your tasks:

1. **Determine folder structure:**
   - Site: `PagePlay.Site/Application/{Domain}/{Feature}/`
   - Test: `PagePlay.Tests/Application/{Domain}/{Feature}/`

2. **Check what files already exist:**
   - Use Glob to find existing files in both locations
   - Identify which files need to be created

3. **Create the vertical slice files:**

   **A. {Feature}.BoundaryContracts.cs** (if not exists):
   ```csharp
   using FluentValidation;
   using PagePlay.Site.Infrastructure.Application;

   namespace PagePlay.Site.Application.{Domain}.{Feature};

   public class {Feature}Response : IResponse
   {
       // TODO: Add response properties
   }

   public class {Feature}Request : IRequest
   {
       // TODO: Add request properties
   }

   public class {Feature}RequestValidator : AbstractValidator<{Feature}Request>
   {
       public {Feature}RequestValidator()
       {
           // TODO: Add validation rules
       }
   }
   ```

   **B. {Feature}.Workflow.cs** (if not exists):
   ```csharp
   using FluentValidation;
   using PagePlay.Site.Infrastructure.Application;

   namespace PagePlay.Site.Application.{Domain}.{Feature};

   public class {Feature}Workflow(
       IValidator<{Feature}Request> _validator
   ) : IWorkflow<{Feature}Request, {Feature}Response>
   {
       public async Task<IApplicationResult<{Feature}Response>> Perform({Feature}Request request)
       {
           var validationResult = await validate(request);
           if (!validationResult.IsValid)
               return response(validationResult);

           // TODO: Implement business logic

           return response();
       }

       private async Task<FluentValidation.Results.ValidationResult> validate({Feature}Request request) =>
           await _validator.ValidateAsync(request);

       private IApplicationResult<{Feature}Response> response(FluentValidation.Results.ValidationResult validationResult) =>
           ApplicationResult<{Feature}Response>.Fail(validationResult);

       private IApplicationResult<{Feature}Response> response(string errorMessage) =>
           ApplicationResult<{Feature}Response>.Fail(errorMessage);

       private IApplicationResult<{Feature}Response> response()
       {
           // TODO: Build success response
           return ApplicationResult<{Feature}Response>.Succeed(
               new {Feature}Response { }
           );
       }
   }
   ```

   **C. {Feature}.Endpoint.cs** (if not exists):
   ```csharp
   using PagePlay.Site.Infrastructure.Application;
   using PagePlay.Site.Infrastructure.Routing;

   namespace PagePlay.Site.Application.{Domain}.{Feature};

   public class {Feature}Endpoint(IWorkflow<{Feature}Request, {Feature}Response> _workflow) : I{Domain}Endpoint
   {
       public void Map(IEndpointRouteBuilder endpoints) =>
           endpoints.Register<{Feature}Response>("/{endpoint-route}", handle);

       private async Task<IResult> handle({Feature}Request request) =>
           Respond.With(await _workflow.Perform(request));
   }
   ```

4. **Provide next steps:**
   - Tell user which files were created
   - Remind them to update TODOs in the templates
   - Suggest running `/new-test {Feature}` to create matching test files
   - Note: Workflow is automatically registered via `AutoRegisterWorkflows()` in DependencyResolver

## Important notes:
- Follow vertical slice pattern: one feature = one folder with 3 files
- Use primary constructor syntax with `_` prefix for dependencies
- Use lower camel case for private methods
- Follow Revealing Intent pattern in workflow
- Keep all TODO comments so user knows what to fill in
