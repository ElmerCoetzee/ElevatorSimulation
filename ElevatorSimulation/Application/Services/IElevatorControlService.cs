using ElevatorSimulation.Domain.Entities;
using ElevatorSimulation.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElevatorSimulation.Application.Services;

public class ElevatorControlService(int numberOfElevators, int maxCapacity, int numberOfFloors) : IElevatorControlService
{
    private readonly List<Elevator> _elevators = Enumerable.Range(1, numberOfElevators)
                                                           .Select(id => new Elevator(id, maxCapacity))
                                                           .ToList();
    private readonly List<Floor> _floors = Enumerable.Range(0, numberOfFloors)
                                                     .Select(number => new Floor(number))
                                                     .ToList();

    public void DispatchElevator(int currentFloor, int numberOfPeople, int destinationFloor)
    {
        // Validate currentFloor
        if (currentFloor < 0 || currentFloor > _floors.Count - 1)
            throw new ArgumentException($"Invalid floor number. Please enter a floor between 0 (Ground Floor) and {_floors.Count - 1}.");

        var nearestElevator = _elevators
            .Where(e => e.State == ElevatorState.Idle || e.CurrentDirection == (currentFloor > e.CurrentFloor ? Direction.Up : Direction.Down))
            .OrderBy(e => Math.Abs(e.CurrentFloor - currentFloor))
            .FirstOrDefault();

        if (nearestElevator == null)
        {
            Console.WriteLine("No available elevator for this request.");
            return;
        }

        nearestElevator.AddDestination(currentFloor);
        nearestElevator.Move();

        nearestElevator.LoadPassengers(numberOfPeople);
        nearestElevator.AddDestination(destinationFloor);
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
