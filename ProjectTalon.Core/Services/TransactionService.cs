using CardanoSharp.Koios.Client;
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
using CardanoSharp.Wallet.CIPs.CIP2;
using CardanoSharp.Wallet.CIPs.CIP2.Models;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Extensions.Models.Transactions;
using CardanoSharp.Wallet.Models;
using CardanoSharp.Wallet.Models.Derivations;
using CardanoSharp.Wallet.Models.Keys;
using CardanoSharp.Wallet.TransactionBuilding;
using ProjectTalon.Core.Common;
using ProjectTalon.Core.Data;

namespace ProjectTalon.Core.Services
{
    public interface ITransactionService
    {
        Task<string> SubmitTransactionAsync(TransactionRequest transactionRequest, string password);
    }

    public class TransactionService : ITransactionService
    {
        private readonly ITransactionClient _transactionClient;
        private readonly IEpochClient _epochClient;
        private readonly IAddressClient _addressClient;
        private readonly INetworkClient _networkClient;
        private readonly IAddressService _addressService;
        private readonly IWalletKeyDatabase _walletKeyDatabase;


        public TransactionService(ITransactionClient transactionClient, 
            IEpochClient epochClient, 
            IAddressClient addressClient, 
            IAddressService addressService, 
            INetworkClient networkClient, 
            IWalletKeyDatabase walletKeyDatabase)
        {
            _transactionClient = transactionClient;
            _epochClient = epochClient;
            _addressClient = addressClient;
            _addressService = addressService;
            _networkClient = networkClient;
            _walletKeyDatabase = walletKeyDatabase;
        }

        public async Task<string> SubmitTransactionAsync(TransactionRequest transactionRequest, string password)
        {
            //0. Get Address
            var (payment, stake) = await GetPaymentAndStakeKeys(password, RoleType.ExternalChain);
            var (change, _) = await GetPaymentAndStakeKeys(password, RoleType.InternalChain);
            var address = new CardanoSharp.Wallet.AddressService()
                .GetBaseAddress(payment.PublicKey, stake.PublicKey, NetworkType.Testnet);
            var changeAddress = new CardanoSharp.Wallet.AddressService()
                .GetBaseAddress(change.PublicKey, stake.PublicKey, NetworkType.Testnet);
            
            //1. Get UTxOs
            var utxos = await _addressService.GetUtxos(address.ToString());
            var changeUtxos = await _addressService.GetUtxos(changeAddress.ToString());
            utxos.AddRange(changeUtxos);
            if (utxos is null)
                return null;
    
            ///2. Create the Body
            var transactionBody = TransactionBodyBuilder.Create;

            //add the requested outputs
            GetRequestedOutputs(transactionBody, transactionRequest);

            //perform coin selection
            var coinSelection = ((TransactionBodyBuilder)transactionBody).UseLargestFirstWithImprove(utxos);
            
            //add the inputs from coin selection to transaction body builder
            AddInputsFromCoinSelection(coinSelection, transactionBody);
            
            //if we have change from coin selection, add to outputs
            if (coinSelection.ChangeOutputs is not null && coinSelection.ChangeOutputs.Any())
            {
                AddChangeOutputs(transactionBody, coinSelection.ChangeOutputs, changeAddress.ToString());
            }

            //get protocol parameters and set default fee
            var epochResponse = await _epochClient.GetEpochInformation();
            var ppResponse = await _epochClient.GetProtocolParameters();
            var protocolParameters = ppResponse.Content.FirstOrDefault();
            transactionBody.SetFee(protocolParameters.MinFeeB.Value);

            //get network tip and set ttl
            var blockSummaries = (await _networkClient.GetChainTip()).Content;
            var ttl = 2500 + (uint)blockSummaries.First().AbsSlot;
            transactionBody.SetTtl(ttl);
            
            ///3. Add Metadata
            IAuxiliaryDataBuilder auxData = null;
            var metadataRequest =
                JsonConvert.DeserializeObject<Dictionary<string, object>>(transactionRequest.Parameters)["Metadata"];
            if (metadataRequest is not null)
            {
                var metadatas = JsonConvert.DeserializeObject<Dictionary<int, object>>(metadataRequest.ToString());
                foreach (var metadata in metadatas)
                {
                    if (auxData is null) auxData = AuxiliaryDataBuilder.Create;
                    ;
                    auxData.AddMetadata(metadata.Key, metadata.Value);
                }
            }

            ///4. Add Witnesses
            var witnessSet = TransactionWitnessSetBuilder.Create;
            witnessSet.AddVKeyWitness(payment.PublicKey, payment.PrivateKey);
            witnessSet.AddVKeyWitness(change.PublicKey, change.PrivateKey);

            ///5. Build Draft TX
            //create transaction builder and add the pieces
            var transaction = TransactionBuilder.Create;
            transaction.SetBody(transactionBody);
            transaction.SetWitnesses(witnessSet);
            if (auxData is not null)
                transaction.SetAuxData(auxData);

            //get a draft transaction to calculate fee
            var draft = transaction.Build(); 
            var fee = draft.CalculateFee(protocolParameters.MinFeeA, protocolParameters.MinFeeB);
            
            //update fee and change output
            transactionBody.SetFee(fee);
            var raw = transaction.Build();
            raw.TransactionBody.TransactionOutputs.Last().Value.Coin -= fee;

            ///6. Sign/Submit TX
            var signed = raw.Serialize();
            
            using (MemoryStream stream = new MemoryStream(signed))
                try
                {
                    var result = (await _transactionClient.Submit(stream));
                    return result.Content;
                }
                catch (Exception e)
                {
                    return null;
                }
        }

        private async Task<(IIndexNodeDerivation payment, IIndexNodeDerivation stake)> GetPaymentAndStakeKeys(string password, RoleType roleType)
        {
            var wallet = await _walletKeyDatabase.GetFirstAsync();
            var privateKey = JsonConvert.DeserializeObject<PrivateKey>(wallet.Skey).Decrypt(password);
            if (privateKey is null)
                throw new Exception("Wallet not found");
            var payment = privateKey
                .Derive(roleType)
                .Derive(0);
            payment.SetPublicKey();

            var stake = privateKey
                .Derive(RoleType.Staking)
                .Derive(0);
            stake.SetPublicKey();
            return (payment, stake);
        }

        private static void AddInputsFromCoinSelection(CoinSelection coinSelection, ITransactionBodyBuilder transactionBody)
        {
            foreach (var i in coinSelection.Inputs)
            {
                transactionBody.AddInput(i.TransactionId, i.TransactionIndex);
            }
        }

        private void GetRequestedOutputs(ITransactionBodyBuilder ttb, TransactionRequest transactionRequest)
        {
            var requestedOutputs = JsonConvert.DeserializeObject<List<Output>>(
            (JsonConvert.DeserializeObject<Dictionary<string, object>>(transactionRequest.Parameters))["Outputs"].ToString());
            foreach (var o in requestedOutputs)
            {
                var tokenBundle = TokenBundleBuilder.Create;
                ulong lovelaces = 0;
                foreach (var asset in o.Assets)
                {
                    if (asset.AssetName == "lovelace")
                    {
                        lovelaces = asset.Quantity;
                    }
                    else
                    {
                        tokenBundle.AddToken(asset.PolicyId.ToBytes(), asset.AssetName.ToBytes(),
                            (ulong) asset.Quantity);
                    }
                }

                ttb.AddOutput(new Address(o.Address).GetBytes(), lovelaces, tokenBundle);
            }
        }

        private void AddChangeOutputs(ITransactionBodyBuilder ttb, List<TransactionOutput> outputs, string address)
        {
            foreach (var output in outputs)
            {
                ITokenBundleBuilder? assetList = null;

                if (output.Value.MultiAsset is not null)
                {
                    assetList = TokenBundleBuilder.Create;
                    foreach (var ma in output.Value.MultiAsset)
                    {
                        foreach (var na in ma.Value.Token)
                        {
                            assetList.AddToken(ma.Key, na.Key, na.Value);
                        }
                    }
                }

                ttb.AddOutput(new Address(address), output.Value.Coin, assetList);
            }
        }
    }
}
