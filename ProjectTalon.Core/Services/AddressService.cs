using CardanoSharp.Koios.Sdk;
using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using ProjectTalon.Core.Common;
using ProjectTalon.Core.Data;
using ProjectTalon.Core.Data.Models;
using ProjectTalon.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjectTalon.Core.Services
{
    public interface IAddressService
    {
        Task<string?> GetWalletAddress(IWalletKeyDatabase keyDatabase, int? addressIndex = null);
        Task<List<Asset>> GetUtxos(string address);
        Task AddWallet(string name, string recoveryPhrase, string spendingPassword);
    }

    public class AddressService: IAddressService
    {
        private readonly IAddressClient _addressClient;
        private IMnemonicService _mnemonicService;
        private IWalletDatabase _walletDatabase;
        private IWalletKeyDatabase _walletKeyDatabase;

        public AddressService(
            IAddressClient addressClient, 
            IMnemonicService mnemonicService, 
            IWalletDatabase walletDatabase, 
            IWalletKeyDatabase walletKeyDatabase)
        {
            _addressClient = addressClient;
            _mnemonicService = mnemonicService;
            _walletDatabase = walletDatabase;
            _walletKeyDatabase = walletKeyDatabase;
        }
        public async Task<string?> GetWalletAddress(IWalletKeyDatabase keyDatabase, int? addressIndex = null)
        {
            addressIndex ??= 0;

            var wallet = await keyDatabase.GetWalletKeysAsync(1);
            var publicKey = JsonSerializer.Deserialize<PublicKey>(wallet.First().Vkey);
            if (publicKey is null)
                throw new Exception("Wallet not found");
            var payment = publicKey
                .Derive(RoleType.ExternalChain)
                .Derive((int)addressIndex);

            var stake = publicKey
                .Derive(RoleType.Staking)
                .Derive(0);

            var address = new CardanoSharp.Wallet.AddressService()
                .GetBaseAddress(payment.PublicKey, stake.PublicKey, NetworkType.Testnet);

            return address.ToString();
        }

        public async Task<List<Asset>> GetUtxos(string address)
        {
            try
            {
                var addressInfo = await _addressClient.GetAddressInformation(address);
                var currentAssets = new List<Asset>()
                {
                    new Asset("lovelace", "", (ulong)addressInfo.Sum(x => Convert.ToInt64(x.Balance)))
                };

                foreach (var utxo in addressInfo.SelectMany(x => x.UtxoSets).SelectMany(x => x.AssetList))
                {
                    var asset = currentAssets.FirstOrDefault(x => x.PolicyId == utxo.PolicyId && x.AssetName == utxo.AssetName);
                    if (asset is null)
                    {
                        asset = new Asset(utxo.PolicyId, utxo.AssetName, (ulong)Convert.ToInt64(utxo.Quantity));
                    }
                    else
                    {
                        asset.Quantity = asset.Quantity + (ulong)Convert.ToInt64(utxo.Quantity);
                    }
                }

                return currentAssets;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task AddWallet(string name, string recoveryPhrase, string spendingPassword)
        {
            // Restore a Mnemonic
            var mnemonic = _mnemonicService.Restore(recoveryPhrase);
            Wallet newlyCreatedWallet;
            int walletId = 0;
            int walletKeyId = 0;

            if (await _walletDatabase.ExistsAsync(name))
            {
                throw new Exception("Wallet already exists");
            }

            int accountIx = 0;

            walletId = await _walletDatabase.SaveAsync(new Wallet
            {
                Name = name,
                WalletType = (int)WalletType.HD,
            });

            newlyCreatedWallet = await _walletDatabase.GetByNameAsync(name);

            var accountNode = mnemonic.GetMasterNode()
                .Derive(PurposeType.Shelley)
                .Derive(CoinType.Ada)
                .Derive(accountIx);
            accountNode.SetPublicKey();

            walletKeyId = await _walletKeyDatabase.SaveWalletAsync(new WalletKey
            {
                WalletId = newlyCreatedWallet.Id,
                KeyType = (int)KeyType.Account,
                Skey = JsonSerializer.Serialize(accountNode.PrivateKey.Encrypt(spendingPassword)),
                Vkey = JsonSerializer.Serialize(accountNode.PublicKey),
                KeyIndex = accountIx,
                AccountIndex = accountIx
            });


            var wallets = await _walletDatabase.ListAsync();
            var wallet = await _walletDatabase.GetByIdAsync(walletId);
            var walletKey = await _walletKeyDatabase.GetWalletKeyAsync(walletKeyId);
        }
    }
}
