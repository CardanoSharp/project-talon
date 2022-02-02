using Blockfrost.Api.Services;
using CardanoSharp.Koios.Sdk;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Models.Addresses;
using CardanoSharp.Wallet.Models.Transactions;
using Newtonsoft.Json;
using ProjectTalon.Core.Data.Models;
using ProjectTalon.Core.Dto;
using ProjectTalon.Core.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTalon.Core.Services
{
    public interface ITransactionService
    {
        Task<string> SubmitTransactionAsync(string address, TransactionRequest transactionRequest);
    }

    public class TransactionService : ITransactionService
    {
        private readonly ICardanoService _blockfrostService;
        private readonly IEpochClient _epochClient;
        private readonly IAddressClient _addressClient;
        private readonly IAddressService _addressService;


        public TransactionService(ICardanoService blockfrostService, IEpochClient epochClient, IAddressClient addressClient, IAddressService addressService)
        {
            _blockfrostService = blockfrostService;
            _epochClient = epochClient;
            _addressClient = addressClient;
            _addressService = addressService;
        }

        public async Task<string> SubmitTransactionAsync(string address, TransactionRequest transactionRequest)
        {
            //1. Determine what needs to be sent
            var requestedOutputs = GetRequestedOutputs(transactionRequest);

            //2. Get UTxOs
            var utxos = await _addressService.GetUtxos(address);
            if (utxos is null)
                return null;

            //3. Make sure user has the funds
            var hasFunds = DetermineIfWeHaveTheFunds(utxos, requestedOutputs);

            //4. Create Inputs / Track their totals

            //5. Create Outputs

            //6. Get Tip and Protocol Parameters

            //7. Set TTL

            //8. Add Witnesses

            //9. Build Draft TX

            //10. Calculate Fee

            //11. Build Raw TX

            //12. Sign/Submit TX

            return "";
        }

        private bool DetermineIfWeHaveTheFunds(List<Asset> utxos, List<TransactionOutput> requestedOutputs)
        {
            var canSend = false;



            return canSend;
        }

        private List<TransactionOutput> GetRequestedOutputs(TransactionRequest transactionRequest)
        {
            var outputs = new List<TransactionOutput>();

            var requestedOutputs = JsonConvert.DeserializeObject<List<TransactionSubmitOutputRequest>>(
            (JsonConvert.DeserializeObject<Dictionary<string, object>>(transactionRequest.Parameters))["Outputs"].ToString());
            foreach (var ro in requestedOutputs)
            {
                var output = new TransactionOutput();
                output.Address = new Address(ro.Address).GetBytes();
                output.Value = new TransactionOutputValue();

                foreach (var asset in ro.Assets)
                {
                    if (asset.AssetName == "lovelace")
                    {
                        output.Value.Coin = (ulong)asset.Quantity;
                    }
                    else
                    {
                        if (output.Value.MultiAsset is null)
                            output.Value.MultiAsset = new Dictionary<byte[], NativeAsset>();

                        output.Value.MultiAsset.Add(asset.PolicyId.ToBytes(), new NativeAsset
                        {
                            Token = new Dictionary<byte[], ulong>()
                            {
                                { asset.AssetName.ToBytes(), (ulong)asset.Quantity }
                            }
                        });
                    }
                }

                outputs.Add(output);
            }

            return outputs;
        }
    }
}
