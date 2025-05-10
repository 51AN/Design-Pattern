using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalTaskTest
{
    internal class WorkingAron
    {
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

            public bool HasVacancy => AllottedStudents.Count < Capacity;

            public void AssignStudent(Student student)
            {
                if (HasVacancy)
                    AllottedStudents.Add(student);
            }

            public void RemoveStudent(Student student)
            {
                AllottedStudents.Remove(student);
            }
        }

        public enum MigrationStatus
        {
            NotOffered,
            Offered,
            Accepted,
            Declined,
            Locked
        }

        public class Student
        {
            public string Name { get; }
            public int Rank { get; }
            public List<string> Preferences { get; }
            public MigrationStatus Status { get; set; } = MigrationStatus.NotOffered;
            public string? CurrentDepartment { get; set; }

            public bool MigrationLocked => Status == MigrationStatus.Locked;

            public Student(string name, int rank, List<string> preferences)
            {
                Name = name;
                Rank = rank;
                Preferences = preferences;
            }

            public bool Prefers(string departmentName, string currentDept)
            {
                return Preferences.IndexOf(departmentName) < Preferences.IndexOf(currentDept);
            }

            public string? NextPreferredDepartment(List<Department> departments)
            {
                return Preferences
                    .Where(dept => departments.First(d => d.Name == dept).HasVacancy)
                    .FirstOrDefault();
            }
        }


        public class MigrationSystem
        {
            private List<Department> _departments;
            private List<Student> _students;
            private int _callNumber = 1;

            public MigrationSystem(List<Department> departments, List<Student> students)
            {
                _departments = departments.OrderBy(d => d.Name).ToList();
                _students = students.OrderBy(s => s.Rank).ToList();
            }

            public void RunMigration(int totalCalls)
            {
                for (int call = 0; call < totalCalls; call++)
                {
                    Console.WriteLine($"\n---Migration Call {_callNumber}---");

                    var callAssignments = new Dictionary<string, List<Student>>();
                    foreach (var dept in _departments)
                        callAssignments[dept.Name] = new List<Student>();

                    // First: migrate accepted students to better options if unlocked
                    var migratedStudents = new List<(Student, Department, Department)>(); // student, fromDept, toDept
                    foreach (var student in _students
                                 .Where(s => s.Status == MigrationStatus.Accepted && !s.MigrationLocked))
                    {
                        foreach (var deptName in student.Preferences)
                        {
                            if (deptName == student.CurrentDepartment) break;

                            var targetDept = _departments.First(d => d.Name == deptName);
                            if (targetDept.HasVacancy)
                            {
                                var currentDept = _departments.First(d => d.Name == student.CurrentDepartment);
                                migratedStudents.Add((student, currentDept, targetDept));
                                break;
                            }
                        }
                    }

                    foreach (var (student, fromDept, toDept) in migratedStudents)
                    {
                        fromDept.RemoveStudent(student);
                        toDept.AssignStudent(student);
                        student.CurrentDepartment = toDept.Name;

                        Console.Write($"For {student.Name}, you have been migrated to {toDept.Name}, do you wish to lock (y/n)?: ");
                        var lockInput = Console.ReadLine()?.Trim().ToLower();
                        if (lockInput == "y")
                            student.Status = MigrationStatus.Locked;
                    }


                    // Second: assign unassigned students
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
                                student.Status = MigrationStatus.Offered;
                                callAssignments[deptName].Add(student);
                                break;
                            }
                        }
                    }

                    // Output department assignments for this call
                    foreach (var dept in _departments)
                    {
                        var studentsInDept = dept.AllottedStudents.Select(s => s.Name).ToList();
                        Console.WriteLine($"{dept.Name}: {(studentsInDept.Any() ? string.Join(", ", studentsInDept) : "")}");
                    }


                    HandleStudentDecisions();
                    _callNumber++;
                }

                // After all calls
                PrintFinalAllocations();
            }

            private void PrintFinalAllocations()
            {
                Console.WriteLine("\n--- Final Allocation ---");
                foreach (var dept in _departments)
                {
                    Console.WriteLine($"{dept.Name}: {string.Join(", ", dept.AllottedStudents.Select(s => s.Name))}");
                }
            }


            private void HandleStudentDecisions()
            {
                foreach (var student in _students)
                {
                    if (student.Status == MigrationStatus.Offered)
                    {
                        Console.Write($"For {student.Name}, do you accept the offer: {student.CurrentDepartment} (y/n)?: ");
                        var input = Console.ReadLine()?.Trim().ToLower();
                        if (input == "y")
                        {
                            student.Status = MigrationStatus.Accepted;
                            Console.Write($"Do you wish to lock {student.CurrentDepartment}? (y/n)?: ");
                            var lockInput = Console.ReadLine()?.Trim().ToLower();
                            if (lockInput == "y")
                                student.Status = MigrationStatus.Locked;
                        }
                        else
                        {
                            student.Status = MigrationStatus.Declined;
                            _departments.First(d => d.Name == student.CurrentDepartment).RemoveStudent(student);
                            student.CurrentDepartment = null;
                        }
                    }
                    else if (student.Status == MigrationStatus.Accepted && !student.MigrationLocked)
                    {
                        Console.Write($"For {student.Name}, do you wish to lock {student.CurrentDepartment}? (y/n)?: ");
                        var lockInput = Console.ReadLine()?.Trim().ToLower();
                        if (lockInput == "y")
                            student.Status = MigrationStatus.Locked;
                    }
                }
            }
        }


    }
}
