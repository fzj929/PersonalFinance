using PersonalFinance.API.Models;

namespace PersonalFinance.API.Services
{
    public interface ITransactionService
    {
        Task<Transaction> CreateTransaction(Transaction transaction, int userId);
        Task<Transaction> CreateTransaction(CreateTransactionRequest request, int userId);
        Task<IEnumerable<Transaction>> GetUserTransactions(int userId, DateTime? startDate = null, DateTime? endDate = null, string? type = null, int? limit = null);
        Task<Transaction?> GetTransactionById(int id, int userId);
        Task<bool> UpdateTransaction(Transaction transaction, int userId);
        Task<bool> DeleteTransaction(int id, int userId);
        Task<object> GetStatistics(int userId, DateTime startDate, DateTime endDate);
    }
}