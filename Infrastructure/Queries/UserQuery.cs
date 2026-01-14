using Application.DTOs.Commons;
using Application.DTOs.Identity;
using Application.Interfaces.Queries;
using Infrastructure.Context;
using Shared.Extensions;
using Shared.Results;

namespace Infrastructure.Queries
{
    public class UserQuery : IUserQuery
    {
        private readonly ApplicationDbContext _context;
        public UserQuery(ApplicationDbContext context) {
            _context = context;
        }

        public PagedResult<AccountDto> GetList(CommonQueryParameters parameters, params string[] searchProperties)
        {
            var query = (from user in _context.Users
                         where user.IsActive == true
                         join ur in _context.UserRole on user.Id equals ur.UserId into urJoin
                         from ur in urJoin.DefaultIfEmpty()
                         join r in _context.ApplicationRole on ur.RoleId equals r.Id into roleJoin
                         from r in roleJoin.DefaultIfEmpty()
                         select new
                         {
                             user.Id,
                             user.FullName,
                             user.UserName,
                             user.Email,
                             user.EmailConfirmed,
                             RoleName = r.Name,
                             RoleId = r.Id,
                             RoleDescription = r.Description
                         })
                        .GroupBy(x => new
                        {
                            x.Id,
                            x.FullName,
                            x.UserName,
                            x.Email,
                            x.EmailConfirmed,
                        })
                        .Select(g => new AccountDto
                        {
                            Id = g.Key.Id,
                            FullName = g.Key.FullName,
                            UserName = g.Key.UserName,
                            Email = g.Key.Email,
                            EmailConfirmed = g.Key.EmailConfirmed,
                            Roles = g.Select(x => new RoleDto
                            {
                                Id = x.RoleId,
                                Name = x.RoleName,
                                Description = x.RoleDescription
                            }).Distinct().ToList()
                        });
            query = query.ApplySearch(parameters.Search, searchProperties);
            query = query.ApplySorting(parameters.SortBy, parameters.SortDirection);
            return query.ToPagedResult(parameters.Index, parameters.PageSize);
        }
    }
}
