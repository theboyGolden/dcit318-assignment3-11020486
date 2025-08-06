using System;
using System.Collections.Generic;

namespace WarehouseInventorySystem
{
    // 3a. Marker Interface for Inventory Items
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // 3b. ElectronicItem class
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Quantity: {Quantity}, Brand: {Brand}, Warranty: {WarrantyMonths} months";
        }
    }

    // 3c. GroceryItem class
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Quantity: {Quantity}, Expiry: {ExpiryDate.ToShortDateString()}";
        }
    }

    // 3e. Custom Exceptions
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // 3d. Generic Inventory Repository
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private Dictionary<int, T> _items = new Dictionary<int, T>();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
            {
                throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
            }
            _items.Add(item.Id, item);
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out T item))
            {
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            }
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.ContainsKey(id))
            {
                throw new ItemNotFoundException($"Item with ID {id} not found.");
            }
            _items.Remove(id);
        }

        public List<T> GetAllItems()
        {
            return new List<T>(_items.Values);
        }

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
            {
                throw new InvalidQuantityException("Quantity cannot be negative.");
            }

            var item = GetItemById(id);
            item.Quantity = newQuantity;
        }
    }

    // 3f. WarehouseManager class
   public class WarehouseManager
    {
        public InventoryRepository<ElectronicItem> Electronics { get; } = new InventoryRepository<ElectronicItem>();
        public InventoryRepository<GroceryItem> Groceries { get; } = new InventoryRepository<GroceryItem>();

        public void SeedData()
        {
            // Add electronic items
            Electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            Electronics.AddItem(new ElectronicItem(2, "Smartphone", 25, "Samsung", 12));
            Electronics.AddItem(new ElectronicItem(3, "Headphones", 50, "Sony", 6));

            // Add grocery items
            Groceries.AddItem(new GroceryItem(101, "Milk", 100, DateTime.Now.AddDays(14)));
            Groceries.AddItem(new GroceryItem(102, "Bread", 75, DateTime.Now.AddDays(7)));
            Groceries.AddItem(new GroceryItem(103, "Eggs", 60, DateTime.Now.AddDays(30)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            Console.WriteLine($"All {typeof(T).Name}s:");
            Console.WriteLine("---------------------");
            foreach (var item in repo.GetAllItems())
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Increased stock for item {id} by {quantity}. New quantity: {item.Quantity + quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error increasing stock: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Item {id} removed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing item: {ex.Message}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var manager = new WarehouseManager();

            // Seed initial data
            manager.SeedData();

            // Print all items
            manager.PrintAllItems(manager.Groceries);
            manager.PrintAllItems(manager.Electronics);

            // Demonstrate exception handling
            Console.WriteLine("Testing error scenarios:");
            Console.WriteLine("-----------------------");

            // Try to add a duplicate item
            try
            {
                Console.WriteLine("\nAttempting to add duplicate electronic item...");
                manager.Electronics.AddItem(new ElectronicItem(1, "Duplicate Laptop", 5, "HP", 12));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"Caught exception: {ex.Message}");
            }

            // Try to remove non-existent item
            try
            {
                Console.WriteLine("\nAttempting to remove non-existent grocery item...");
                manager.RemoveItemById(manager.Groceries, 999);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught exception: {ex.Message}");
            }

            // Try to update with invalid quantity
            try
            {
                Console.WriteLine("\nAttempting to update with negative quantity...");
                manager.Groceries.UpdateQuantity(101, -5);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"Caught exception: {ex.Message}");
            }

            // Successful operations
            Console.WriteLine("\nTesting successful operations:");
            Console.WriteLine("-----------------------------");

            // Increase stock
            manager.IncreaseStock(manager.Electronics, 2, 10);

            // Remove existing item
            manager.RemoveItemById(manager.Groceries, 102);

            // Print updated lists
            Console.WriteLine("\nUpdated inventory:");
            manager.PrintAllItems(manager.Groceries);
            manager.PrintAllItems(manager.Electronics);
        }
    }
}