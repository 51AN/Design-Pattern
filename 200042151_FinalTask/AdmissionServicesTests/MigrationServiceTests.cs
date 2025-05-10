using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using _200042151_FinalTask;

namespace AdmissionServicesTests
{
    public class AdmissionServicesUnitTests
    {
        private List<Department> departments;
        private List<Student> students;
        private MigrationSystem migrationSystem;

        [SetUp]
        public void Setup()
        {

            departments = new List<Department>
            {
                new Department("CSE", 1),
                new Department("EEE", 1),
                new Department("ME", 1)
            };

            students = new List<Student>
            {
                new Student("S1", 1, new List<string>{ "CSE", "EEE", "ME" }),
                new Student("S2", 2, new List<string>{ "ME", "EEE" }),
                new Student("S3", 3, new List<string>{ "CSE", "EEE" })
            };


            migrationSystem = new MigrationSystem(
                departments,
                students,
                new ConcreteMigrationStrategy()
            );
        }

        [Test]
        public void RunMigration_WithAllYesAnswers_ShouldLockEveryoneImmediately()
        {
            // Simulate user typing "y" to every prompt (accept and lock)
            // We need enough "y\n" lines for every Console.ReadLine call your code makes.
            var simulatedInput = string.Join(
                "\n",
                Enumerable.Repeat("y", 20)  // 20 is just a safe upper bound
            ) + "\n";

            // Redirect Console.In
            Console.SetIn(new StringReader(simulatedInput));

            // Optionally, capture Console.Out so the test log isn't polluted:
            var outWriter = new StringWriter();
            Console.SetOut(outWriter);

            // Act
            migrationSystem.RunMigration(totalCalls: 1);

            // Assert: every student should end up Locked on their first assignment
            Assert.That(students.All(s => s.Status == MigrationStatus.Locked),
                        "Expected all students to have status Locked");
        }

        [Test]
        public void RunMigration_WithNoAnswers_ShouldDeclineAllOffers()
        {
            // Simulate user typing "n" to every prompt (decline and never lock)
            var simulatedInput = string.Join(
                "\n",
                Enumerable.Repeat("n", 20)
            ) + "\n";

            Console.SetIn(new StringReader(simulatedInput));
            Console.SetOut(TextWriter.Null);

            migrationSystem.RunMigration(totalCalls: 1);

            // Every student who got offered should have Status == Declined
            Assert.That(students.Where(s => s.CurrentDepartment != null)
                                .All(s => s.Status == MigrationStatus.Declined));
        }

        [Test]
        public void RunMigration_S1DeclinesOffer_StatusShouldBeDeclined()
        {
            var inputSequence = new List<string>
            {
                "n",              // S1 declines CSE
                "y", "y",         // S2 accepts ME, locks
                "y", "y"          // S3 accepts CSE, locks
            };

            Console.SetIn(new StringReader(string.Join("\n", inputSequence)));
            Console.SetOut(TextWriter.Null); // Optional: suppress output

            migrationSystem.RunMigration(totalCalls: 1);

            var s1 = students.First(s => s.Name == "S1");
            Assert.That(s1.Status, Is.EqualTo(MigrationStatus.Declined), "S1 should have declined the offer.");
            Assert.That(s1.CurrentDepartment, Is.Null, "S1 should not be assigned to any department.");
        }
    }
}
