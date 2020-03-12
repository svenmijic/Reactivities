using System.Threading;
using MediatR;
using Persistence;
using System;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Application.Errors;
using System.Net;
using Domain;

namespace Application.Followers
{
    public class Delete
    {
        public class Command : IRequest
        {
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext context;
            private readonly IUserAccessor userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                this.userAccessor = userAccessor;
                this.context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var observer = await context.Users.SingleOrDefaultAsync(x => x.UserName == userAccessor.GetCurrentUsername());
                var target = await context.Users.SingleOrDefaultAsync(x => x.UserName == request.Username);
                if (target == null) throw new RestException(HttpStatusCode.NotFound, new { User = "Not found!" });
                var following = await context.Followings.SingleOrDefaultAsync(x => x.ObserverId == observer.Id && x.TargetId == target.Id);
                if (following == null) throw new RestException(HttpStatusCode.BadRequest, new { User = "You are not following this user!" });
                if (following != null) context.Followings.Remove(following);
                var success = await context.SaveChangesAsync() > 0;
                if (success) return Unit.Value;
                throw new Exception("Problem saving changes!");
            }
        }
    }
}