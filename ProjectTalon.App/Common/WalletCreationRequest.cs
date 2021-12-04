using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTalon.App.Common
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
