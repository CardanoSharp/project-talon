using System.ComponentModel.DataAnnotations;

namespace ProjectTalon.App.Models
{
    public class WalletCreationRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string RecoveryPhrase { get; set; }

        [Required]
        public string SpendingPassword { get; set; }

        [Required]
        public string ConfirmSpendingPassword { get; set; }
    }
}
