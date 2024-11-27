using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Utilities;

namespace ElevatorSimulation.Application.Services;

public class ElevatorControlService(int numberOfElevators, int maxCapacity, int numberOfFloors)
{
    private readonly List<Elevator> _elevators = Enumerable.Range(1, numberOfElevators)
        .Select(id => new Elevator(id, maxCapacity, numberOfFloors))
        .ToList();

    public void DispatchElevator()
    {
        var currentFloor = InputHelper.GetValidatedInput("Enter Current Floor (0 for Ground Floor): ", 0, numberOfFloors - 1);
        var numberOfPeople = InputHelper.GetValidatedInput("Enter Number of People: ", 1, int.MaxValue);
        var passengerDestinations = GatherPassengerDestinations(numberOfPeople);

        var nearestElevator = _elevators
            .OrderBy(e => Math.Abs(e.CurrentFloor - currentFloor))
            .FirstOrDefault() ?? throw new InvalidOperationException("No available elevator.");

        Console.WriteLine($"Calling Elevator {nearestElevator.Id} to Floor {currentFloor}...");
        nearestElevator.AddDestination(currentFloor);
        nearestElevator.Move(false);

        nearestElevator.LoadPassengers(numberOfPeople);

        foreach (var destination in passengerDestinations)
        {
            nearestElevator.AddDestination(destination);
        }

        nearestElevator.Move();
    }

    private List<int> GatherPassengerDestinations(int numberOfPeople)
    {
        var destinations = new List<int>();
        for (var i = 1; i <= numberOfPeople; i++)
        {
            destinations.Add(InputHelper.GetValidatedInput($"Passenger {i}, enter your destination floor: ", 0, numberOfFloors - 1));
        }

        return destinations;
    }

    public void DisplayElevatorStatus()
    {
        foreach (var elevator in _elevators)
        {
            Console.WriteLine($"Elevator {elevator.Id} | Floor: {elevator.CurrentFloor} | Direction: {elevator.CurrentDirection} | Passengers: {elevator.PassengerCount}");
        }
    }

    public string GetElevatorStatus() => string.Join(Environment.NewLine, _elevators.Select(e =>
                                                  $"Elevator {e.Id} | Floor: {e.CurrentFloor} | Direction: {e.CurrentDirection} | Passengers: {e.PassengerCount}"));
}
