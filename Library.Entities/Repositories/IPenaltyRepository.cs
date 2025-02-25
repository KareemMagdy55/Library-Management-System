﻿using Library.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Entities.Repositories
{
    public interface IPenaltyRepository : IGenericRepository<Penalty>
    {
        public Task<IEnumerable<Penalty>> GetAllPenalties();

        public Task<Penalty> GetFirstPenalty(int id);

        public Task<IEnumerable<Penalty>> GetAllPenaltiesFilterdByUsernnameAsync(string username);
    }
}