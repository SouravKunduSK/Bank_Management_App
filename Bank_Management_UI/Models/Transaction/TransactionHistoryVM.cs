using Bank_Management_Api.Models.Transactions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bank_Management_UI.Models.Transaction
{
    public class TransactionHistoryVM
    {
        public string SelectedAccountNumber { get; set; }
        public List<SelectListItem> Accounts { get; set; }
        public List<TransactionResponse> Transactions { get; set; }
    }
}
