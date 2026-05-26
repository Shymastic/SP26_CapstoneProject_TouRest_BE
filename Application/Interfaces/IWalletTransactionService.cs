using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Wallet;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Interfaces
{
    public interface IWalletTransactionService
    {
        Task<List<WalletTransactionDTO>> GetTransactionsAsync(Guid userId);
    }
}
