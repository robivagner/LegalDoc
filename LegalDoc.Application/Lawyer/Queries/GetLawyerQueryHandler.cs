using LegalDoc.Application.Abstractions;
using MediatR;

namespace LegalDoc.Application.Lawyer.Queries;

public class GetLawyerQueryHandler(ILawyerRepository repository) : IRequestHandler<GetLawyerQuery, Domain.Entities.Lawyer?>
{
    public async Task<Domain.Entities.Lawyer?> Handle(GetLawyerQuery request, CancellationToken cancellationToken)
    {
        var lawyer = await repository.FindAsync(request.Id, cancellationToken);

        return lawyer;
    }
}