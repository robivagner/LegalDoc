using LegalDoc.Application.Abstractions;
using MediatR;

namespace LegalDoc.Application.Lawyer.Queries;

public class GetLawyersQueryHandler(ILawyerRepository repository) : IRequestHandler<GetLawyersQuery, List<LawyerDto>>
{
    public Task<List<LawyerDto>> Handle(GetLawyersQuery request, CancellationToken cancellationToken)
    {
        var lawyers = repository.Query().Select(l => new LawyerDto(l.Id, l.Name, l.BarNumber, l.Email, l.IsActive)).ToList();
        return Task.FromResult(lawyers);
    }
}