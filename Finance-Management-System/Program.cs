using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    // 1a. Transaction record type
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);
    
    // 1b. Transaction Processor Interface
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }
    
    // 1c. Concrete processor implementations
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Processing bank transfer: ${transaction.Amount} for {transaction.Category}");
        }
    }
    
    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Processing mobile money payment: ${transaction.Amount} for {transaction.Category}");
        }
    }
    
    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"Processing crypto transaction: ${transaction.Amount} for {transaction.Category}");
        }
    }
    
    // 1d. Base Account class
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }
        
        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }
        
        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. New balance: ${Balance}");
        }
    }
    
    // 1e. Sealed SavingsAccount class
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) 
            : base(accountNumber, initialBalance) { }
        
        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
            }
            else
            {
                base.ApplyTransaction(transaction);
            }
        }
    }
    
    // 1f. FinanceApp class
    public class FinanceApp
    {
        private List<Transaction> _transactions = new List<Transaction>();
        
        public void Run()
        {
            // i. Create savings account
            var savingsAccount = new SavingsAccount("SAV123456", 1000m);
            Console.WriteLine($"Created savings account with balance: ${savingsAccount.Balance}");
            
            // ii. Create sample transactions
            var transactions = new List<Transaction>
            {
                new Transaction(1, DateTime.Now, 150m, "Groceries"),
                new Transaction(2, DateTime.Now, 75m, "Utilities"),
                new Transaction(3, DateTime.Now, 200m, "Entertainment")
            };
            
            // iii. Create processors
            var processors = new ITransactionProcessor[]
            {
                new MobileMoneyProcessor(),
                new BankTransferProcessor(),
                new CryptoWalletProcessor()
            };
            
            // Process and apply transactions
            for (int i = 0; i < transactions.Count; i++)
            {
                Console.WriteLine($"\nProcessing Transaction {i+1}:");
                processors[i].Process(transactions[i]);
                savingsAccount.ApplyTransaction(transactions[i]);
                _transactions.Add(transactions[i]);
            }
            
            // Display summary
            Console.WriteLine("\nTransaction Summary:");
            foreach (var transaction in _transactions)
            {
                Console.WriteLine($"{transaction.Id}: {transaction.Date} - {transaction.Category} - ${transaction.Amount}");
            }
            Console.WriteLine($"Final balance: ${savingsAccount.Balance}");
        }
        
        public static void Main()
        {
            var app = new FinanceApp();
            app.Run();
        }
    }
}