using MediatR;
using Shared.Kernel;
using Shared.Kernel.Dtos;

namespace E_Menu.Internal.Queries.GetAllMenus;

public record class GetAllMenusInternalRequest() : IRequest<Result<IEnumerable<MenuDto>>>;
