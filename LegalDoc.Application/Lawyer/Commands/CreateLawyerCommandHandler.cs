using LegalDoc.Application.Abstractions;
using MediatR;

namespace LegalDoc.Application.Lawyer.Commands;

public class CreateLawyerCommandHandler(ILawyerRepository repository) : IRequestHandler<CreateLawyerCommand, Guid>
{
    public async Task<Guid> Handle(CreateLawyerCommand request, CancellationToken cancellationToken)
    {
        var lawyer = Domain.Entities.Lawyer.Create(request.Name, request.BarNumber, request.Email);
        await repository.AddAsync(lawyer, cancellationToken);
        return lawyer.Id;
    }
}