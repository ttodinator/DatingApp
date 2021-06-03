using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private IMapper mapper;
        private DataContext context;

        public UnitOfWork(DataContext context, IMapper mapper)
        {
            this.mapper = mapper;
            this.context = context;
        }

        public IUserRepository UserRepository => new UserRepository(context, mapper);
        public IMessageRepository MessageRepository => new MessageRepository(context, mapper);
        public ILikesRepository LikesRepository => new LikesRepository(context);


        

        public async Task<bool> Complete()
        {
            return await context.SaveChangesAsync()>0;
        }

        public bool HasChanges()
        {
            return context.ChangeTracker.HasChanges();
        }
    }
}
