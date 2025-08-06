using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InventorySystem
{
    // 5b. Marker Interface for Logging
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    // 5a. Immutable Inventory Record
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // 5c. Generic Inventory Logger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private List<T> _log = new List<T>();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll()
        {
            return new List<T>(_log);
        }

        public void SaveToFile()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new JsonStringEnumConverter() }
                };

                string jsonString = JsonSerializer.Serialize(_log, options);
                File.WriteAllText(_filePath, jsonString);
                Console.WriteLine($"Successfully saved {_log.Count} items to {_filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine($"File not found: {_filePath}");
                    return;
                }

                string jsonString = File.ReadAllText(_filePath);
                _log = JsonSerializer.Deserialize<List<T>>(jsonString) ?? new List<T>();
                Console.WriteLine($"Successfully loaded {_log.Count} items from {_filePath}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }
        }
    }

    // 5f. Integration Layer - InventoryApp
    public class InventoryApp
    {
        private readonly InventoryLogger<InventoryItem> _logger;

        public InventoryApp()
        {
            _logger = new InventoryLogger<InventoryItem>("inventory.json");
        }

        public void SeedSampleData()
        {
            Console.WriteLine("Seeding sample data...");
            
            _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now.AddDays(-30)));
            _logger.Add(new InventoryItem(2, "Smartphone", 25, DateTime.Now.AddDays(-15)));
            _logger.Add(new InventoryItem(3, "Monitor", 15, DateTime.Now.AddDays(-7)));
            _logger.Add(new InventoryItem(4, "Keyboard", 50, DateTime.Now.AddDays(-3)));
            _logger.Add(new InventoryItem(5, "Mouse", 75, DateTime.Now));

            Console.WriteLine("Added 5 sample inventory items.");
        }

        public void SaveData()
        {
            _logger.SaveToFile();
        }

        public void LoadData()
        {
            _logger.LoadFromFile();
        }

        public void PrintAllItems()
        {
            var items = _logger.GetAll();
            Console.WriteLine("\nCurrent Inventory:");
            Console.WriteLine("-----------------");
            
            foreach (var item in items)
            {
                Console.WriteLine($"ID: {item.Id}");
                Console.WriteLine($"Name: {item.Name}");
                Console.WriteLine($"Quantity: {item.Quantity}");
                Console.WriteLine($"Date Added: {item.DateAdded:yyyy-MM-dd}");
                Console.WriteLine("-----------------");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("Inventory Management System");
            Console.WriteLine("==========================");

            var app = new InventoryApp();

            // Initial session
            Console.WriteLine("\n[Initial Session]");
            app.SeedSampleData();
            app.SaveData();

            // Clear memory and simulate new session
            Console.WriteLine("\n[New Session - Loading Data]");
            app = new InventoryApp(); // Simulate new session
            app.LoadData();
            app.PrintAllItems();
        }
    }
}