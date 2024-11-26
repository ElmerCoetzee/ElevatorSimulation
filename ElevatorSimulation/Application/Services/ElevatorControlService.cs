using ElevatorSimulation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElevatorSimulation.Application.Services;

public class ElevatorControlService(int numberOfElevators, int maxCapacity, int numberOfFloors)
{
    private readonly List<Elevator> _elevators = Enumerable.Range(1, numberOfElevators)
            .Select(id => new Elevator(id, maxCapacity))
            .ToList();

    public void DispatchElevator(int currentFloor, int numberOfPeople, int destinationFloor)
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
        nearestElevator.Move();

        nearestElevator.LoadPassengers(numberOfPeople);

        Console.WriteLine($"Elevator {nearestElevator.Id} heading to Floor {destinationFloor}.");
        nearestElevator.AddDestination(destinationFloor);
        nearestElevator.Move();

        Console.Write($"How many passengers are exiting at Floor {destinationFloor}? (Max: {nearestElevator.PassengerCount}): ");
        var exitingPassengers = int.Parse(Console.ReadLine() ?? "0");
        nearestElevator.UnloadPassengers(exitingPassengers);

        Console.WriteLine($"Elevator {nearestElevator.Id} is now Idle at Floor {nearestElevator.CurrentFloor} with {nearestElevator.PassengerCount} passengers.");
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
}
