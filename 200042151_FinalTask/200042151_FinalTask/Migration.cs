using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _200042151_FinalTask
{
    // Department Entity
    public class Department
    {
        public string Name { get; }
        public int Capacity { get; }
        public List<Student> AllottedStudents { get; } = new();

        public Department(string name, int capacity)
        {
            Name = name;
            Capacity = capacity;
        }

        // Property that returns whether the department has vacancy
        public bool HasVacancy => AllottedStudents.Count < Capacity;

        // Assign a student to the department if there's space
        public void AssignStudent(Student student)
        {
            if (HasVacancy)
                AllottedStudents.Add(student);
        }

        // Remove a student from the department
        public void RemoveStudent(Student student)
        {
            AllottedStudents.Remove(student);
        }
    }

    // Enum representing various statuses for students in migration process
    // This represents the State Design Pattern, as a student's status can change over time based on their interactions.
    public enum MigrationStatus
    {
        NotOffered,
        Offered,
        Accepted,
        Declined,
        Locked
    }

    // Student Entity
    public class Student
    {
        public string Name { get; }
        public int Rank { get; }
        public List<string> Preferences { get; }
        public MigrationStatus Status { get; set; } = MigrationStatus.NotOffered;
        public string? CurrentDepartment { get; set; }

        // Property to check if the student's migration is locked (based on their status)
        public bool MigrationLocked => Status == MigrationStatus.Locked;

        public Student(string name, int rank, List<string> preferences)
        {
            Name = name;
            Rank = rank;
            Preferences = preferences;
        }

        // Check if a student prefers a given department over their current department
        public bool Prefers(string departmentName, string currentDept)
        {
            return Preferences.IndexOf(departmentName) < Preferences.IndexOf(currentDept);
        }

        // Get the next department that the student prefers and has a vacancy
        public string? NextPreferredDepartment(List<Department> departments)
        {
            return Preferences
                .Where(dept => departments.First(d => d.Name == dept).HasVacancy)
                .FirstOrDefault();
        }
    }


    // Strategy Pattern


    // IMigrationStrategy is an interface for different migration strategies
    // This allows different strategies to be used for migration based on business rules.
    public interface IMigrationStrategy
    {
        // Method to get migration candidates based on the strategy
        List<(Student student, Department fromDept, Department toDept)> GetMigrationCandidates(
            List<Student> students,
            List<Department> departments
        );
    }

    // Concrete migration strategy implementation
    public class ConcreteMigrationStrategy : IMigrationStrategy
    {
        public List<(Student student, Department fromDept, Department toDept)> GetMigrationCandidates(
            List<Student> students,
            List<Department> departments)
        {
            var migratedStudents = new List<(Student, Department, Department)>();

            // Track temp vacancies, initial vacancies before migration
            var departmentVacancyMap = departments.ToDictionary(
                d => d.Name,
                d => d.Capacity - d.AllottedStudents.Count
            );

            // Sort by student rank to preserve priority
            foreach (var student in students
                         .Where(s => s.Status == MigrationStatus.Accepted && !s.MigrationLocked)
                         .OrderBy(s => s.Rank))
            {
                foreach (var deptName in student.Preferences)
                {
                    if (deptName == student.CurrentDepartment)
                        break;

                    if (departmentVacancyMap[deptName] > 0)
                    {
                        var targetDept = departments.First(d => d.Name == deptName);
                        var currentDept = departments.First(d => d.Name == student.CurrentDepartment);

                        migratedStudents.Add((student, currentDept, targetDept));
                        departmentVacancyMap[deptName]--; // Temporarily reduce vacancy
                        departmentVacancyMap[currentDept.Name]++; // Free up previous spot
                        break;
                    }
                }
            }

            return migratedStudents;
        }
    }


    // The MigrationSystem class controls the entire process of student migration
    // It incorporates both the Strategy and State patterns by handling different migration strategies and student status changes.
    public class MigrationSystem
    {
        private List<Department> _departments;
        private List<Student> _students;
        private int _callNumber = 1;
        private IMigrationStrategy _migrationStrategy;

        public MigrationSystem(List<Department> departments, List<Student> students, IMigrationStrategy migrationStrategy)
        {
            _departments = departments.OrderBy(d => d.Name).ToList();  // Sort departments alphabetically
            _students = students.OrderBy(s => s.Rank).ToList();        // Sort students based on rank
            _migrationStrategy = migrationStrategy;  // Assign the migration strategy
        }

        // Method to run the migration process for a set number of calls
        public void RunMigration(int totalCalls)
        {
            for (int call = 0; call < totalCalls; call++)
            {
                Console.WriteLine($"\n---Migration Call {_callNumber}---");

                var callAssignments = new Dictionary<string, List<Student>>();
                foreach (var dept in _departments)
                    callAssignments[dept.Name] = new List<Student>();

                // First: Migrate accepted students to better options (if not locked)
                var migratedStudents = _migrationStrategy.GetMigrationCandidates(_students, _departments);

                foreach (var (student, fromDept, toDept) in migratedStudents)
                {
                    // Move the student from their current department to the new department
                    fromDept.RemoveStudent(student);
                    toDept.AssignStudent(student);
                    student.CurrentDepartment = toDept.Name;

                    // Ask the student if they want to lock the department
                    Console.Write($"For {student.Name}, you have been migrated to {toDept.Name}, do you wish to lock (y/n)?: ");
                    var lockInput = Console.ReadLine()?.Trim().ToLower();
                    if (lockInput == "y")
                        student.Status = MigrationStatus.Locked;  // Change the student's status to "Locked"
                }

                // Second: Assign unassigned students to available departments
                foreach (var student in _students
                             .Where(s => s.Status == MigrationStatus.NotOffered))
                {
                    foreach (var deptName in student.Preferences)
                    {
                        var dept = _departments.First(d => d.Name == deptName);
                        if (dept.HasVacancy)
                        {
                            dept.AssignStudent(student);
                            student.CurrentDepartment = deptName;
                            student.Status = MigrationStatus.Offered;  // Mark the student as "Offered"
                            callAssignments[deptName].Add(student);
                            break;  // Stop after assigning one department
                        }
                    }
                }

                // Output the department assignments after this call
                foreach (var dept in _departments)
                {
                    var studentsInDept = dept.AllottedStudents.Select(s => s.Name).ToList();
                    Console.WriteLine($"{dept.Name}: {(studentsInDept.Any() ? string.Join(", ", studentsInDept) : "")}");
                }

                // Handle student decisions (whether they accept or decline offers)
                HandleStudentDecisions();
                _callNumber++;
            }

            // Print the final allocations after all migration calls
            PrintFinalAllocations();
        }

        // Handles student decisions (accepting, declining, or locking departments)
        private void HandleStudentDecisions()
        {
            foreach (var student in _students)
            {
                if (student.Status == MigrationStatus.Offered)
                {
                    // Prompt the student to accept or decline the offer
                    Console.Write($"For {student.Name}, do you accept the offer: {student.CurrentDepartment} (y/n)?: ");
                    var input = Console.ReadLine()?.Trim().ToLower();
                    if (input == "y")
                    {
                        student.Status = MigrationStatus.Accepted;  // Mark the offer as accepted
                        Console.Write($"Do you wish to lock {student.CurrentDepartment}? (y/n)?: ");
                        var lockInput = Console.ReadLine()?.Trim().ToLower();
                        if (lockInput == "y")
                            student.Status = MigrationStatus.Locked;  // Lock the department if desired
                    }
                    else
                    {
                        student.Status = MigrationStatus.Declined;  // Mark the offer as declined
                        _departments.First(d => d.Name == student.CurrentDepartment).RemoveStudent(student);
                        student.CurrentDepartment = null;  // Remove the student from the current department
                    }
                }
                else if (student.Status == MigrationStatus.Accepted && !student.MigrationLocked)
                {
                    // If the student has accepted and not locked, ask if they want to lock
                    Console.Write($"For {student.Name}, do you wish to lock {student.CurrentDepartment}? (y/n)?: ");
                    var lockInput = Console.ReadLine()?.Trim().ToLower();
                    if (lockInput == "y")
                        student.Status = MigrationStatus.Locked;  // Lock the student's current department
                }
            }
        }

        // Output the final allocations after all migration calls
        private void PrintFinalAllocations()
        {
            Console.WriteLine("\n--- Final Allocation ---");
            foreach (var dept in _departments)
            {
                Console.WriteLine($"{dept.Name}: {string.Join(", ", dept.AllottedStudents.Select(s => s.Name))}");
            }
        }
    }

}
