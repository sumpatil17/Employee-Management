using Employee_Management.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Management.Repository
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DbContext _context;
        protected DbSet<TEntity> _entities;
        DbSet<TEntity> _set;

        public Repository(DbContext context) 
        {
            _context = context;
            _set = context.Set<TEntity>();
            _entities = context.Set<TEntity>();
        }

        public async virtual Task Add(TEntity entity)
        {
            try
            {
                _entities.Add(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

        }

        public async virtual Task AddRange(IEnumerable<TEntity> entities)
        {
            try
            {
                _entities.AddRange(entities);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }

        public async virtual Task Update(TEntity entity)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }


    }
}
