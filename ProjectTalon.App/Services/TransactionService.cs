using Blockfrost.Api.Services;
using CardanoSharp.Koios.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTalon.App.Services
{
    public interface ITransactionService
    {
        string SubmitTransaction();
    }

    public class TransactionService : ITransactionService
    {
        private readonly ITransactionsService _transactionsService;
        private readonly IEpochClient _epochClient;
        private readonly IAddressClient _addressClient;


        public TransactionService(ITransactionsService transactionsService, IEpochClient epochClient, IAddressClient addressClient)
        {
            _transactionsService = transactionsService;
            _epochClient = epochClient;
            _addressClient = addressClient;
        }

        public string SubmitTransaction()
        {
            throw new NotImplementedException();
        }
    }
}
