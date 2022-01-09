using Blockfrost.Api.Models;
using Blockfrost.Api.Services;
using System.Threading.Tasks;

namespace ProjectTalon.App.ViewModel
{
    public interface ITransactionHistoryViewModel
    {
        public Task<AddressContentTotalResponse> GetAllTransactions(string address);
    }
    public class TransactionHistoryViewModel : ITransactionHistoryViewModel
    {
        private readonly IAddressesService _addressesService;

        public TransactionHistoryViewModel(IAddressesService addressesService)
        {
            _addressesService = addressesService;
        }
        public async Task<AddressContentTotalResponse> GetAllTransactions(string address)
        {
            return await _addressesService?.GetTotalAsync(address);
        }
    }
}
