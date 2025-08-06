using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareSystem
{
    // 2a. Generic Repository class
    public class Repository<T>
    {
        private List<T> _items = new List<T>();

        public void Add(T item)
        {
            _items.Add(item);
        }

        public List<T> GetAll()
        {
            return _items;
        }

        public T? GetById(Func<T, bool> predicate)
        {
            return _items.FirstOrDefault(predicate);
        }

        public bool Remove(Func<T, bool> predicate)
        {
            var item = _items.FirstOrDefault(predicate);
            if (item != null)
            {
                return _items.Remove(item);
            }
            return false;
        }
    }

    // 2b. Patient class
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
        }
    }

    // 2c. Prescription class
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Patient ID: {PatientId}, Medication: {MedicationName}, Date Issued: {DateIssued.ToShortDateString()}";
        }
    }

    // HealthSystemApp class
    public class HealthSystemApp
    {
        private Repository<Patient> _patientRepo = new Repository<Patient>();
        private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

        // 2g. Seed data method
        public void SeedData()
        {
            // Add patients
            _patientRepo.Add(new Patient(1, "Kubi Emmanuel", 22, "Male"));
            _patientRepo.Add(new Patient(2, "Ewuradjoa Mensah", 23, "Female"));
            _patientRepo.Add(new Patient(3, "Bondzie Mc-Essel", 42, "Male"));

            // Add prescriptions
            _prescriptionRepo.Add(new Prescription(101, 1, "Ibuprofen", new DateTime(2023, 5, 10)));
            _prescriptionRepo.Add(new Prescription(102, 1, "Amoxicillin", new DateTime(2023, 6, 15)));
            _prescriptionRepo.Add(new Prescription(103, 2, "Lisinopril", new DateTime(2023, 4, 20)));
            _prescriptionRepo.Add(new Prescription(104, 2, "Metformin", new DateTime(2023, 7, 5)));
            _prescriptionRepo.Add(new Prescription(105, 3, "Atorvastatin", new DateTime(2023, 3, 12)));
        }

        // 2g. Build prescription map method
        public void BuildPrescriptionMap()
        {
            var allPrescriptions = _prescriptionRepo.GetAll();
            
            _prescriptionMap = allPrescriptions
                .GroupBy(p => p.PatientId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        // 2g. Print all patients method
        public void PrintAllPatients()
        {
            Console.WriteLine("All Patients:");
            Console.WriteLine("------------");
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine(patient);
            }
            Console.WriteLine();
        }

        // 2g. Print prescriptions for patient method
        public void PrintPrescriptionsForPatient(int patientId)
        {
            if (_prescriptionMap.TryGetValue(patientId, out var prescriptions))
            {
                var patient = _patientRepo.GetById(p => p.Id == patientId);
                Console.WriteLine($"Prescriptions for {patient?.Name} (ID: {patientId}):");
                Console.WriteLine("----------------------------------------");
                
                foreach (var prescription in prescriptions)
                {
                    Console.WriteLine(prescription);
                }
            }
            else
            {
                Console.WriteLine($"No prescriptions found for patient ID: {patientId}");
            }
            Console.WriteLine();
        }

        // 2f. Get prescriptions by patient ID method
        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            if (_prescriptionMap.TryGetValue(patientId, out var prescriptions))
            {
                return prescriptions;
            }
            return new List<Prescription>();
        }

        // Main application flow
        public static void Main()
        {
            var app = new HealthSystemApp();
            
            // Seed initial data
            app.SeedData();
            
            // Build prescription mapping
            app.BuildPrescriptionMap();
            
            // Print all patients
            app.PrintAllPatients();
            
            // Print prescriptions for a specific patient (using first patient ID as example)
            var firstPatientId = app._patientRepo.GetAll().First().Id;
            app.PrintPrescriptionsForPatient(firstPatientId);
            
            // Example of getting prescriptions programmatically
            var prescriptions = app.GetPrescriptionsByPatientId(firstPatientId);
            Console.WriteLine($"Found {prescriptions.Count} prescriptions for patient {firstPatientId}");
        }
    }
}