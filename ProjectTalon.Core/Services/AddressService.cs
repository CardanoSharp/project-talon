using CardanoSharp.Koios.Sdk;
using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using ProjectTalon.Core.Common;
using ProjectTalon.Core.Data;
using ProjectTalon.Core.Data.Models;
using CardanoSharpAsset = CardanoSharp.Wallet.Models.Asset;
using CardanoSharp.Wallet.Models;
using Newtonsoft.Json;

namespace ProjectTalon.Core.Services
{
    public interface IAddressService
    {
        Task<string?> GetWalletAddress(int? addressIndex = null);
        Task<List<Utxo>> GetUtxos(string address);
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
        
        public async Task<string?> GetWalletAddress(int? addressIndex = null)
        {
            addressIndex ??= 0;

            var wallet = await _walletKeyDatabase.GetWalletKeysAsync(1);
            var publicKey = JsonConvert.DeserializeObject<PublicKey>(wallet.First().Vkey);
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

        public async Task<List<Utxo>> GetUtxos(string address)
        {
            try
            {
                var addressInfo = (await _addressClient.GetAddressInformation(address)).Content;
                var utxos = new List<Utxo>();

                foreach (var ai in addressInfo.SelectMany(x => x.UtxoSets))
                {
                    if(ai is null) continue;
                    var utxo = new Utxo()
                    {
                        TxIndex = ai.TxIndex,
                        TxHash = ai.TxHash,
                        Balance = new Balance()
                        {
                            Lovelaces = ulong.Parse(ai.Value)
                        }
                    };

                    var assetList = new List<CardanoSharpAsset>();
                    foreach (var aa in ai.AssetList)
                    {
                        assetList.Add(new CardanoSharpAsset()
                        {
                            Name = aa.AssetName,
                            PolicyId = aa.PolicyId,
                            Quantity = ulong.Parse(aa.Quantity)
                        });
                    }

                    utxo.Balance.Assets = assetList;
                    utxos.Add(utxo);
                }

                return utxos;
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
                Skey = JsonConvert.SerializeObject(accountNode.PrivateKey.Encrypt(spendingPassword)),
                Vkey = JsonConvert.SerializeObject(accountNode.PublicKey),
                KeyIndex = accountIx,
                AccountIndex = accountIx
            });


            var wallets = await _walletDatabase.ListAsync();
            var wallet = await _walletDatabase.GetByIdAsync(walletId);
            var walletKey = await _walletKeyDatabase.GetWalletKeyAsync(walletKeyId);
        }
    }
}
