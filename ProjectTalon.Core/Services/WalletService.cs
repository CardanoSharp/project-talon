using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Enums;
using CardanoSharp.Wallet.Extensions.Models;
using ProjectTalon.Core.Common;
using ProjectTalon.Core.Data;
using ProjectTalon.Core.Data.Models;
using SQLite;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjectTalon.Core.Services
{
    public interface IWalletService
    {

        Task AddWallet(string name, string recoveryPhrase, string spendingPassword);
    }

    public class WalletService : IWalletService
    {
        private readonly SQLiteAsyncConnection database;
        private IMnemonicService _mnemonicService;
        private IWalletDatabase _walletDatabase;
        private IWalletKeyDatabase _walletKeyDatabase;

        public WalletService(
            IMnemonicService mnemonicService,
            IWalletKeyDatabase walletKeyDatabase,
            IWalletDatabase walletDatabase)
        {
            _mnemonicService = mnemonicService;
            _walletDatabase = walletDatabase;
            _walletKeyDatabase = walletKeyDatabase;
        }

        public async Task AddWallet(string name, string recoveryPhrase, string spendingPassword)
        {
            // Restore a Mnemonic
            var mnemonic = _mnemonicService.Restore(recoveryPhrase);
            Wallet newlyCreatedWallet;
            int walletId = 0;
            int walletKeyId = 0;

            if (await _walletDatabase.WalletExistsAsync(name))
            {
                throw new Exception("Wallet already exists");
            }

            //await database.RunInTransactionAsync(async (SQLiteConnection transaction) =>
            //{
            //transaction.BeginTransaction();
            int accountIx = 0;

            walletId = await _walletDatabase.SaveWalletAsync(new Wallet
            {
                Name = name,
                WalletType = (int)WalletType.HD,
            });

            newlyCreatedWallet = await _walletDatabase.GetWalletByNameAsync(name);

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

            //    transaction.Commit();
            //});

            var wallets = await _walletDatabase.GetWalletsAsync();
            var wallet = await _walletDatabase.GetWalletAsync(walletId);
            var walletKey = await _walletKeyDatabase.GetWalletKeyAsync(walletKeyId);
        }
    }
}
