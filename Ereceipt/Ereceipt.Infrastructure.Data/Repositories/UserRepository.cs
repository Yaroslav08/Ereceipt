﻿using Ereceipt.Domain.Interfaces;
using Ereceipt.Domain.Models;
using Ereceipt.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ereceipt.Infrastructure.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(EreceiptContext db) : base(db) { }


        public async Task<List<User>> GetAllAsync(int afterId)
        {
            if (afterId == 0)
                return await db.Users.AsNoTracking().OrderByDescending(d => d.CreatedAt).Take(20).ToListAsync();
            return await db.Users.AsNoTracking().Where(d => d.Id > afterId).OrderByDescending(d => d.CreatedAt).Take(20).ToListAsync();
        }

        public async Task<List<User>> SearchUsersAsync(string name, int afterId)
        {
            if (afterId == 0)
                return await db.Users.AsNoTracking().Where(d => d.Name.Contains(name)).Take(20).ToListAsync();
            return await db.Users.AsNoTracking().Where(d => d.Name.Contains(name) && d.Id > afterId).Take(20).ToListAsync();
        }
    }
}