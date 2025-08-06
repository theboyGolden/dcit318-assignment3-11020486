using System;
using System.Collections.Generic;
using System.IO;

namespace SchoolGradingSystem
{
    // 4a. Student class
    public class Student
    {
        public int Id { get; }
        public string FullName { get; }
        public int Score { get; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            return Score switch
            {
                >= 80 and <= 100 => "A",
                >= 70 and < 80 => "B",
                >= 60 and < 70 => "C",
                >= 50 and < 60 => "D",
                < 50 => "F",
                _ => throw new InvalidScoreFormatException($"Invalid score value: {Score}")
            };
        }
    }

    // 4b. Custom exception for invalid score format
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    // 4c. Custom exception for missing fields
    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    // 4d. Student result processor class
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();

            using (var reader = new StreamReader(inputFilePath))
            {
                int lineNumber = 0;
                while (!reader.EndOfStream)
                {
                    lineNumber++;
                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    try
                    {
                        string[] fields = line.Split(',');
                        if (fields.Length != 3)
                        {
                            throw new MissingFieldException(
                                $"Line {lineNumber}: Expected 3 fields but found {fields.Length}");
                        }

                        if (!int.TryParse(fields[0].Trim(), out int id))
                        {
                            throw new InvalidScoreFormatException(
                                $"Line {lineNumber}: Invalid ID format '{fields[0]}'");
                        }

                        string fullName = fields[1].Trim();
                        if (string.IsNullOrWhiteSpace(fullName))
                        {
                            throw new MissingFieldException(
                                $"Line {lineNumber}: Missing student name");
                        }

                        if (!int.TryParse(fields[2].Trim(), out int score))
                        {
                            throw new InvalidScoreFormatException(
                                $"Line {lineNumber}: Invalid score format '{fields[2]}'");
                        }

                        students.Add(new Student(id, fullName, score));
                    }
                    catch (Exception ex) when (
                        ex is MissingFieldException || 
                        ex is InvalidScoreFormatException)
                    {
                        Console.WriteLine($"Skipping line {lineNumber}: {ex.Message}");
                    }
                }
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (var writer = new StreamWriter(outputFilePath))
            {
                writer.WriteLine("STUDENT GRADE REPORT");
                writer.WriteLine("====================");
                writer.WriteLine();

                foreach (var student in students)
                {
                    writer.WriteLine(
                        $"{student.FullName} (ID: {student.Id}): " +
                        $"Score = {student.Score}, Grade = {student.GetGrade()}");
                }

                writer.WriteLine();
                writer.WriteLine($"Total students processed: {students.Count}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            const string inputFile = "students.txt";
            const string outputFile = "grade_report.txt";

            var processor = new StudentResultProcessor();

            try
            {
                Console.WriteLine("Starting grade processing...");
                
                // Read and validate student data
                var students = processor.ReadStudentsFromFile(inputFile);
                
                // Generate report
                processor.WriteReportToFile(students, outputFile);
                
                Console.WriteLine($"Successfully processed {students.Count} students.");
                Console.WriteLine($"Report saved to: {outputFile}");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Error: Input file '{inputFile}' not found.");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}