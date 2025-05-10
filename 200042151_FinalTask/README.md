# Project Structure and Design Pattern Usage

## Project Overview

This C# solution is organized into two main folders:

1. **200042151_FinalTask/**
   - Contains core implementation files for migration and allocation logic.
2. **AdmissionServicesTests/**
   - Contains NUnit test cases for validating the logic in the main project.

---

## Folder: 200042151_FinalTask

### 1. Allocation.cs
- **Contents:**
  - `AllocationService` class implementing the Singleton pattern.
  - Central coordination logic for room assignment using a configurable strategy.

- **Design Pattern:**
  - **Singleton Pattern:**
    - Ensures a single instance of `AllocationService` throughout the application.
    - Accessed via the static `Instance` property.
  - **Strategy Pattern (Used via Distribution):**
    - Delegates room allocation logic to an injected strategy.

### 2. Distribution.cs
- **Contents:**
  - Entities: `Room`, `Center`
  - Interface: `IDistributionStrategy`
  - Concrete strategy: `ThresholdBasedDistribution`

- **Design Pattern:**
  - **Strategy Pattern:**
    - Defines how rooms are selected and activated dynamically based on thresholds and center constraints.

### 3. Migration.cs
- **Contents:**
  - Entities: `Student`, `Department`
  - Interface: `IMigrationStrategy`
  - Core service: `MigrationSystem`
  - Concrete strategy: `ConcreteMigrationStrategy`

- **Design Pattern:**
  - **Strategy Pattern:**
    - Enables different migration strategies (e.g., preference-based migration) without changing the core logic in `MigrationSystem`.

- **Conceptual Design Pattern:**
  - **State Pattern (Candidate):**
    - The student decision-making process (offered, accepted, declined, locked) could be modeled using the state pattern for cleaner transitions and encapsulated behavior.

### 4. Program.cs
- **Contents:**
  - Entry point for the application.
  - Initializes data, sets strategies, and triggers service logic for allocation and migration.

---

## Folder: AdmissionServicesTests

### 1. AllocationServiceTests.cs
- **Purpose:**
  - Unit tests for room allocation using NUnit.

- **Test Focus:**
  - Verifies singleton enforcement.
  - Ensures room allocation logic based on capacity and thresholds.
  - Validates behavior when all rooms are full.

### 2. MigrationServiceTests.cs
- **Purpose:**
  - Unit tests for department migration logic.

- **Test Focus:**
  - Confirms that declined departments are not re-offered.
  - Ensures correct offer, accept, and lock behaviors are maintained.

---

## Summary of Design Patterns Used

| Pattern           | Location                    | Purpose                                                           |
|------------------|-----------------------------|-------------------------------------------------------------------|
| Strategy          | `Allocation.cs`, `Migration.cs` | Allows plugging in interchangeable logic for allocation/migration |
| Singleton         | `Allocation.cs`             | Controls access to a single allocation service instance           |
| State             | `Migration.cs`              | Potential for modeling student decision states                    |

---

## Conclusion

The project structure clearly separates responsibilities between allocation, migration, and test logic. Design patterns are applied to promote modularity, flexibility, and testability across both services.

