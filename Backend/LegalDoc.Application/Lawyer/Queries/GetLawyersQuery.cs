using MediatR;

namespace LegalDoc.Application.Lawyer.Queries;

public class GetLawyersQuery() : IRequest<List<LawyerDto>>;