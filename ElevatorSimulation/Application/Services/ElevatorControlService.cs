using ElevatorSimulation.Domain.Entities;

namespace ElevatorSimulation.Application.Services;

public class ElevatorControlService(int numberOfElevators, int maxCapacity, int numberOfFloors)
{
    private readonly List<Elevator> _elevators = Enumerable.Range(1, numberOfElevators)
            .Select(id => new Elevator(id, maxCapacity))
            .ToList();

    public void DispatchElevator(int currentFloor, int numberOfPeople)
    {
        var nearestElevator = _elevators
            .OrderBy(e => Math.Abs(e.CurrentFloor - currentFloor))
            .FirstOrDefault();

        if (nearestElevator == null)
        {
            Console.WriteLine("No available elevator.");
            return;
        }

        Console.WriteLine($"Calling Elevator {nearestElevator.Id} to Floor {currentFloor}...");
        nearestElevator.AddDestination(currentFloor);
        nearestElevator.Move(handleInitialBoarding: false); // Move to the caller's floor without boarding prompt

        // Board passengers and set destinations
        Console.Write($"How many passengers are boarding at Floor {currentFloor}? (Max: {nearestElevator.AvailableSpace()}): ");
        var passengersBoarding = int.Parse(Console.ReadLine() ?? "0");

        if (passengersBoarding > nearestElevator.AvailableSpace())
        {
            Console.WriteLine("Error: Cannot board more passengers than available space.");
            passengersBoarding = nearestElevator.AvailableSpace();
        }

        nearestElevator.LoadPassengers(passengersBoarding);

        for (var i = 1; i <= passengersBoarding; i++)
        {
            Console.Write($"Passenger {i}, enter your destination floor: ");
            var passengerDestination = int.Parse(Console.ReadLine() ?? "0");
            nearestElevator.AddDestination(passengerDestination);
        }

        Console.WriteLine($"Elevator {nearestElevator.Id} is heading to its destinations...");
        nearestElevator.Move(); // Handle all subsequent movements and boarding/exiting
    }

    public void DisplayElevatorStatus()
    {
        foreach (var elevator in _elevators)
        {
            Console.WriteLine($"Elevator {elevator.Id} | Floor: {elevator.CurrentFloor} | Direction: {elevator.CurrentDirection} | Passengers: {elevator.PassengerCount} | State: {elevator.State}");
        }
    }

    public string GetElevatorStatus() => string.Join(Environment.NewLine, _elevators.Select(e =>
        $"Elevator {e.Id} | Floor: {e.CurrentFloor} | Direction: {e.CurrentDirection} | Passengers: {e.PassengerCount} | State: {e.State}"));

    public IEnumerable<Elevator> GetElevators() => _elevators;
}
