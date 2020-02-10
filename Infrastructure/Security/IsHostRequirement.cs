using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Persistence;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    public class IsHostRequirement : IAuthorizationRequirement { }

    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirement>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly DataContext dataContext;

        public IsHostRequirementHandler(IHttpContextAccessor httpContextAccessor, DataContext dataContext)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.dataContext = dataContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
        {
            if (context.Resource is AuthorizationFilterContext authContext)
            {
                var currentUserName = httpContextAccessor.HttpContext.User?.Claims?.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                var activityId = Guid.Parse(authContext.RouteData.Values["id"].ToString());
                var activity = dataContext.Activities.Find(activityId);
                var host = activity.UserActivities.FirstOrDefault(x => x.IsHost);
                if (host?.AppUser?.UserName == currentUserName) context.Succeed(requirement);
            }
            else context.Fail();
            return Task.CompletedTask;
        }
    }
}