using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Utilities;

namespace ElevatorSimulation.Application.Services;

public class ElevatorControlService(int numberOfElevators, int maxCapacity, int numberOfFloors)
{
    private readonly List<Elevator> _elevators = Enumerable.Range(1, numberOfElevators)
            .Select(id => new Elevator(id, maxCapacity, numberOfFloors))
            .ToList();
    private readonly int _numberOfFloors = numberOfFloors;

    public void InteractiveDispatchElevator()
    {
        var currentFloor = InputHelper.GetValidatedInput("Enter Current Floor (0 for Ground Floor): ", 0, _numberOfFloors - 1);
        var numberOfPeople = InputHelper.GetValidatedInput("Enter Number of People: ", 1, int.MaxValue);

        var passengerDestinations = new List<int>();
        for (var i = 1; i <= numberOfPeople; i++)
        {
            var destination = InputHelper.GetValidatedInput($"Passenger {i}, enter your destination floor: ", 0, _numberOfFloors - 1);
            passengerDestinations.Add(destination);
        }

        DispatchElevator(currentFloor, numberOfPeople, passengerDestinations);
    }

    public void DispatchElevator(int currentFloor, int numberOfPeople, List<int> passengerDestinations)
    {
        var nearestElevator = GetBestElevator(currentFloor) ?? throw new InvalidOperationException("No available elevator.");


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

    public void DisplayElevatorStatus()
    {
        foreach (var elevator in _elevators)
        {
            Console.WriteLine($"Elevator {elevator.Id} | Floor: {elevator.CurrentFloor} | Direction: {elevator.CurrentDirection} | Passengers: {elevator.PassengerCount}");
        }
    }

    public string GetElevatorStatus() => string.Join(Environment.NewLine, _elevators.Select(e =>
        $"Elevator {e.Id} | Floor: {e.CurrentFloor} | Direction: {e.CurrentDirection} | Passengers: {e.PassengerCount}"));

    private Elevator? GetBestElevator(int currentFloor) => _elevators.OrderBy(e => Math.Abs(e.CurrentFloor - currentFloor))
                                                                     .ThenBy(e => e.PassengerCount) // Prefer elevators with fewer passengers
                                                                     .FirstOrDefault();

}
