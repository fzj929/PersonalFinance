using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.API.Models;
using PersonalFinance.API.Services;
using System.Security.Claims;
using System.Text;

namespace PersonalFinance.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
        {
            try
            {
                var createdTransaction = await _transactionService.CreateTransaction(request, GetUserId());
                return CreatedAtAction(nameof(GetTransaction), new { id = createdTransaction.Id }, createdTransaction);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? type = null,
            [FromQuery] int? limit = null)
        {
            var transactions = await _transactionService.GetUserTransactions(GetUserId(), startDate, endDate, type, limit);
            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransaction(int id)
        {
            var transaction = await _transactionService.GetTransactionById(id, GetUserId());
            if (transaction == null) return NotFound();
            return Ok(transaction);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, [FromBody] Transaction transaction)
        {
            if (id != transaction.Id) return BadRequest("ID不匹配");

            var success = await _transactionService.UpdateTransaction(transaction, GetUserId());
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var success = await _transactionService.DeleteTransaction(id, GetUserId());
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var statistics = await _transactionService.GetStatistics(GetUserId(), startDate, endDate);
            return Ok(statistics);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportTransactions()
        {
            var transactions = await _transactionService.GetUserTransactions(GetUserId());

            // 创建CSV内容，使用UTF-8编码
            var csvContent = new System.Text.StringBuilder();

            // 添加UTF-8 BOM
            csvContent.Append("\uFEFF");

            // 添加CSV头部
            csvContent.AppendLine("日期,类型,类别,金额,描述");

            foreach (var transaction in transactions)
            {
                // 处理CSV转义 - 如果描述包含逗号或引号，需要用引号包围
                var description = string.IsNullOrEmpty(transaction.Description)
                    ? ""
                    : $"\"{transaction.Description.Replace("\"", "\"\"")}\"";

                csvContent.AppendLine($"{transaction.TransactionDate:yyyy-MM-dd},{transaction.Type},{transaction.Category},{transaction.Amount},{description}");
            }

            // 设置正确的Content-Type和文件名
            var bytes = Encoding.UTF8.GetBytes(csvContent.ToString());
            return File(bytes, "text/csv; charset=utf-8", $"transactions_{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}