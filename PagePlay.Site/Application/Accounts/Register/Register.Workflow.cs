using FluentValidation;
using FluentValidation.Results;
using PagePlay.Site.Application.Accounts.Domain.Models;
using PagePlay.Site.Infrastructure.Core.Application;
using PagePlay.Site.Infrastructure.Data.Repositories;
using PagePlay.Site.Infrastructure.Security;

namespace PagePlay.Site.Application.Accounts.Register;

public class RegisterWorkflow(
    IPasswordHasher _passwordHasher,
    IRepository _repository,
    IValidator<RegisterWorkflowRequest> _validator
) : WorkflowBase<RegisterWorkflowRequest, RegisterWorkflowResponse>, IWorkflow<RegisterWorkflowRequest, RegisterWorkflowResponse>
{
    public async Task<IApplicationResult<RegisterWorkflowResponse>> Perform(RegisterWorkflowRequest workflowRequest)
    {
        var validationResult = await validate(workflowRequest);
        if (!validationResult.IsValid)
            return Fail(validationResult);

        // TODO: remove once we have move this into patterns/examples. This does not need a transaction.
        User user = null;
        await using(var scope = _repository.BeginTransactionScope())
        {
            user = createUser(workflowRequest);

            var emailExists = await checkEmailExists(user.Email);
            if (emailExists)
                return Fail("An account with this email already exists.");

            await saveUser(user);
            await scope.CompleteTransaction();
        }

        return Succeed(buildResponse(user));
    }

    private async Task<ValidationResult> validate(RegisterWorkflowRequest workflowRequest) =>
        await _validator.ValidateAsync(workflowRequest);

    private async Task<bool> checkEmailExists(string email) =>
        await _repository.Exists(User.ByEmail(email));

    private User createUser(RegisterWorkflowRequest workflowRequest) =>
        User.Create(workflowRequest.Email, _passwordHasher.HashPassword(workflowRequest.Password));

    private async Task saveUser(User user)
    {
        await _repository.Add(user);
        await _repository.SaveChanges();
    }
    
    private RegisterWorkflowResponse buildResponse(User user) =>
        new RegisterWorkflowResponse { UserId = user.Id };
}

