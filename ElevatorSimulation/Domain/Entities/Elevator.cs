using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ElevatorSimulation.Domain.Entities;

public class Elevator(int id, int maxCapacity)
{
    public int Id { get; } = id;
    public int CurrentFloor { get; private set; } = 0; // Start at ground floor.
    public Direction CurrentDirection { get; private set; } = Direction.Stationary;
    public ElevatorState State { get; private set; } = ElevatorState.Idle;
    public int PassengerCount { get; private set; } = 0;
    public int MaxCapacity { get; } = maxCapacity;

    private Queue<int> DestinationQueueInternal { get; } = new();
    public IEnumerable<int> DestinationQueue => DestinationQueueInternal;

    public void AddDestination(int floor)
    {
        if (!DestinationQueueInternal.Contains(floor))
            DestinationQueueInternal.Enqueue(floor);
    }

    public void Move()
    {
        if (!DestinationQueueInternal.Any())
        {
            State = ElevatorState.Idle;
            CurrentDirection = Direction.Stationary;
            return;
        }

        var nextFloor = DestinationQueueInternal.Peek();

        if (CurrentFloor == nextFloor)
        {
            Console.WriteLine($"Elevator {Id} is already at Floor {CurrentFloor}.");
            DestinationQueueInternal.Dequeue();
            State = ElevatorState.Idle;
            CurrentDirection = Direction.Stationary;
            return;
        }

        CurrentDirection = nextFloor > CurrentFloor ? Direction.Up : Direction.Down;
        State = ElevatorState.Moving;

        Console.WriteLine($"Elevator {Id} moving {CurrentDirection} from Floor {CurrentFloor} to Floor {nextFloor}.");

        while (CurrentFloor != nextFloor)
        {
            Thread.Sleep(1000); // Simulate movement delay.
            CurrentFloor += CurrentDirection == Direction.Up ? 1 : -1;
            Console.WriteLine($"Elevator {Id} is now on Floor {CurrentFloor}.");
        }

        Console.WriteLine($"Elevator {Id} has arrived at Floor {CurrentFloor}.");
        DestinationQueueInternal.Dequeue();
        State = ElevatorState.Idle;
        CurrentDirection = Direction.Stationary;
    }

    public void LoadPassengers(int count)
    {
        if (PassengerCount + count > MaxCapacity)
            throw new InvalidOperationException("Exceeding maximum capacity!");

        PassengerCount += count;
        Console.WriteLine($"{count} passengers boarded Elevator {Id}. Current count: {PassengerCount}");
    }

    public void UnloadPassengers(int count)
    {
        if (count > PassengerCount)
            throw new InvalidOperationException("Cannot unload more passengers than present.");

        PassengerCount -= count;
        Console.WriteLine($"{count} passengers exited Elevator {Id}. Current count: {PassengerCount}");
    }

    public int AvailableSpace() => MaxCapacity - PassengerCount;
}

public enum Direction
{
    Stationary,
    Up,
    Down
}

public enum ElevatorState
{
    Idle,
    Moving,
    DoorsOpen
}