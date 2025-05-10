using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _200042151_FinalTask
{

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Singleton Pattern for Allocation
    /////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class AllocationService
    {
        // Singleton instance of AllocationService to ensure a single point of control
        private static AllocationService? _instance;

        // Public accessor to get the singleton instance
        public static AllocationService Instance => _instance ??= new AllocationService();

        private IDistributionStrategy _distributionStrategy;

        private List<Center> _centers;

        private AllocationService()
        {
            _centers = new List<Center>();
        }

        // Set the distribution strategy
        public void SetDistributionStrategy(IDistributionStrategy strategy)
        {
            _distributionStrategy = strategy;
        }

        // Add a new center to the pool of available centers
        public void AddCenter(Center center)
        {
            _centers.Add(center);
        }

        // Allocate a room to a student
        public void ApplyStudent(string studentId)
        {
            // Use the strategy to get an appropriate room
            Room room = _distributionStrategy.AllocateRoom(_centers);
            room.AssignSeat();

            Console.WriteLine($"Student {studentId} assigned to Room {room.Id} in {room.Location}");
        }
    }

}
