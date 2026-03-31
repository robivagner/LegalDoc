using LegalDoc.Application.Abstractions;
using MediatR;

namespace LegalDoc.Application.Lawyer.Commands;

public class UpdateLawyerActivityCommandHandler(ILawyerRepository repository) : IRequestHandler<UpdateLawyerActivityCommand>
{
    public async Task Handle(UpdateLawyerActivityCommand request, CancellationToken cancellationToken)
    {
        var lawyer = await repository.FindAsync(request.LawyerId, cancellationToken);
        
        if (lawyer == null)
            throw new Exception("Lawyer not found.");
        
        lawyer.UpdateLawyerActivity(request.IsActive);
        
        await repository.UpdateAsync(lawyer, cancellationToken);
    }
}