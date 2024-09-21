using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;



namespace ATM
{
    public delegate void TransactionHandler(object sender, EventArgs args, Account toAccount, decimal amount);

    public class Account
    {
        public string Owner { get; set; }
        public decimal Balance { get; set; }
        public string PhoneNumber { get; set; }

       
        public event TransactionHandler OnTransfer;
        public event TransactionHandler OnWithdraw;

        public Account(string owner, decimal initialBalance, string phoneNumber)
        {
            Owner = owner;
            Balance = initialBalance;
            PhoneNumber = phoneNumber; 
        }

        // Phương thức rút tiền
        public void Withdraw(decimal amount)
        {
            if (amount <= Balance)
            {
                Balance -= amount;
                // Kích hoạt sự kiện OnWithdraw
                OnWithdraw?.Invoke(this, EventArgs.Empty, null, amount);
                Console.WriteLine($"Bạn đã rút {amount}. Số dư TK mới: {Balance}");
            }
            else
            {
                Console.WriteLine("Số dư không đủ để thực hiện giao dịch!");
            }
        }

        // Phương thức chuyển tiền
        public void Transfer(decimal amount, Account toAccount, Account fromAccount)
        {
            if (amount <= Balance)
            {
                Balance -= amount;
                toAccount.Balance += amount;
                // Kích hoạt sự kiện OnTransfer
                OnTransfer?.Invoke(this, EventArgs.Empty, toAccount, amount);
                Console.WriteLine($"Bạn đã chuyển {amount} đến {toAccount.Owner}. Số dư TK mới: {Balance}");
            }
            else
            {
                Console.WriteLine("Số dư không đủ để thực hiện chuyển tiền!");
            }
        }

        public void PrintAccountInfo()
        {
            Console.WriteLine($"\nChủ tài khoản: {Owner}");
            Console.WriteLine($"Số dư: {Balance}");
            Console.WriteLine($"Số điện thoại: {PhoneNumber}");
        }
    }

    // Lớp ATM thực hiện giao dịch rút tiền và chuyển tiền
    public class ATM
    {
        private Account account;

        public ATM(Account acc)
        {
            account = acc;
            // Đăng ký các phương thức xử lý sự kiện
            account.OnWithdraw += HandleWithdraw;
            account.OnTransfer += HandleTransfer;
        }

        // Phương thức rút tiền từ tài khoản
        public void WithdrawMoney(decimal amount)
        {
            account.Withdraw(amount);
        }

        // Phương thức chuyển tiền từ tài khoản này sang tài khoản khác
        public void TransferMoney(decimal amount, Account toAccount, Account fromAccount)
        {
            account.Transfer(amount, toAccount, fromAccount);
        }

        // Xử lý sự kiện rút tiền
        private void HandleWithdraw(object sender, EventArgs e, Account unused, decimal amount)
        {
            Account account = (Account)sender;
            Console.WriteLine($"\n--> Sự kiện: {account.Owner} đã thực hiện rút tiền. \nSMS: Thông báo gửi đến {account.Owner} ({account.PhoneNumber}).");
        }

        // Xử lý sự kiện chuyển tiền
        private void HandleTransfer(object sender, EventArgs e, Account toAccount, decimal amount)
        {
            Account fromAccount = (Account)sender;
            Console.WriteLine($"\n--> Sự kiện: {fromAccount.Owner} đã chuyển tiền cho {toAccount.Owner}.\nSMS: Thông báo gửi đến {toAccount.Owner} ({toAccount.PhoneNumber})");
            Console.WriteLine($"Bạn đã nhận {amount} từ {fromAccount.Owner}. Số dư TK mới: {toAccount.Balance}");
            Console.WriteLine($"\nSMS: Thông báo gửi đến {fromAccount.Owner} ({fromAccount.PhoneNumber}).");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            
            Account account1 = new Account("Sherry", 5000000, "0325467883"); 
            Account account2 = new Account("Harley", 2000000, "0922340954"); 
            Console.WriteLine("-- Thông tin tài khoản:");
            account1.PrintAccountInfo();
            account2.PrintAccountInfo();

            ATM atm = new ATM(account1);

            // Thực hiện rút tiền
            atm.WithdrawMoney(1000000); 

            // Thực hiện chuyển tiền
            atm.TransferMoney(1500000, account2, account1); 
            Console.ReadKey();
        }
    }
}




