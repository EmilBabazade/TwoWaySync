﻿using Data.Entities;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO: interface
// TODO: unit tests

namespace Data.Repos;
public class UsersRepo
{
    private readonly DataContext _dataContext;

    public UsersRepo(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<UserEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await _dataContext.Users
            .Include(u => u.Address.Geo)
            .Include(u => u.Company)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<ICollection<UserEntity>> GetAllAsync(CancellationToken cancellation = default) =>
        await _dataContext.Users
            .Include(u => u.Address.Geo)
            .Include(u => u.Company)
            .ToListAsync(cancellation);
}
