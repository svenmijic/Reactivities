using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Delete
    {
        public class Command : IRequest
        {
            public string Id { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext dataContext;
            private readonly IUserAccessor userAccessor;
            private readonly IPhotoAccessor photoAccessor;

            public Handler(DataContext dataContext, IUserAccessor userAccessor, IPhotoAccessor photoAccessor)
            {
                this.dataContext = dataContext;
                this.userAccessor = userAccessor;
                this.photoAccessor = photoAccessor;
            }
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await dataContext.Users.SingleOrDefaultAsync(x => x.UserName == userAccessor.GetCurrentUsername());
                var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id);
                if (photo == null) throw new RestException(HttpStatusCode.NotFound, new { Photo = "Not found!" });
                if (photo.IsMain) throw new RestException(HttpStatusCode.BadRequest, new { Photo = "You cannot delete your main photo!" });
                var result = photoAccessor.DeletePhoto(photo.Id);
                if (result == null) throw new Exception("Problem deleting the photo!");
                user.Photos.Remove(photo);
                var success = await dataContext.SaveChangesAsync() > 0;
                if (success) return Unit.Value;
                throw new Exception("Problem saving changes!");
            }
        }
    }
}