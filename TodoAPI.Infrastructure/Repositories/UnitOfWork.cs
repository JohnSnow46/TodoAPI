using Microsoft.EntityFrameworkCore.Storage;
using TodoAPI.Core.Interfaces;
using TodoAPI.Infrastructure.Data;

namespace TodoAPI.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TodoDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(TodoDbContext context)
        {
            _context = context;
            Tasks = new TaskRepository(_context);
            Users = new UserRepository(_context);
            Categories = new CategoryRepository(_context);
        }

        public ITaskRepository Tasks { get; private set; }
        public IUserRepository Users { get; private set; }
        public ICategoryRepository Categories { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction is in progress.");
            }

            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await _transaction.RollbackAsync();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction is in progress.");
            }

            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}