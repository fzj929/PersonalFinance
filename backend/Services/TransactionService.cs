using Microsoft.EntityFrameworkCore;
using PersonalFinance.API.Data;
using PersonalFinance.API.Models;

namespace PersonalFinance.API.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _context;

        public TransactionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction> CreateTransaction(Transaction transaction, int userId)
        {
            transaction.UserId = userId;
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> CreateTransaction(CreateTransactionRequest request, int userId)
        {
            var transaction = new Transaction
            {
                Amount = request.Amount,
                Type = request.Type,
                Category = request.Category,
                Description = request.Description,
                TransactionDate = request.TransactionDate,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<IEnumerable<Transaction>> GetUserTransactions(int userId, DateTime? startDate = null, DateTime? endDate = null, string? type = null, int? limit = null)
        {
            var query = _context.Transactions
                .Where(t => t.UserId == userId);

            if (startDate.HasValue)
                query = query.Where(t => t.TransactionDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.TransactionDate <= endDate.Value);

            if (!string.IsNullOrEmpty(type))
                query = query.Where(t => t.Type == type);

            var orderedQuery = query.OrderByDescending(t => t.TransactionDate);

            if (limit.HasValue)
                return await orderedQuery.Take(limit.Value).ToListAsync();

            return await orderedQuery.ToListAsync();
        }

        public async Task<Transaction?> GetTransactionById(int id, int userId)
        {
            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<bool> UpdateTransaction(Transaction transaction, int userId)
        {
            var existingTransaction = await GetTransactionById(transaction.Id, userId);
            if (existingTransaction == null) return false;

            existingTransaction.Amount = transaction.Amount;
            existingTransaction.Type = transaction.Type;
            existingTransaction.Category = transaction.Category;
            existingTransaction.Description = transaction.Description;
            existingTransaction.TransactionDate = transaction.TransactionDate;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTransaction(int id, int userId)
        {
            var transaction = await GetTransactionById(id, userId);
            if (transaction == null) return false;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<object> GetStatistics(int userId, DateTime startDate, DateTime endDate)
        {
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId && t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .ToListAsync();

            var income = transactions.Where(t => t.Type == "income").Sum(t => t.Amount);
            var expense = transactions.Where(t => t.Type == "expense").Sum(t => t.Amount);
            var balance = income - expense;

            var categoryStats = transactions
                .GroupBy(t => t.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(t => t.Amount),
                    Type = g.First().Type,
                    Count = g.Count()
                })
                .ToList();

            return new
            {
                TotalIncome = income,
                TotalExpense = expense,
                Balance = balance,
                CategoryStatistics = categoryStats,
                TransactionCount = transactions.Count
            };
        }
    }
}