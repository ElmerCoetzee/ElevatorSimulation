using ElevatorSimulation.Utilities;

namespace ElevatorSimulation.Domain.Entities;

public class Elevator(int id, int maxCapacity, int numberOfFloors)
{
    public int Id { get; } = id;
    public int CurrentFloor { get; private set; } = 0;
    public Direction CurrentDirection { get; private set; } = Direction.Stationary;
    public ElevatorState State { get; private set; } = ElevatorState.Idle;
    public int PassengerCount { get; private set; } = 0;
    public int MaxCapacity { get; } = maxCapacity;
    private readonly int _numberOfFloors = numberOfFloors;

    private Queue<int> DestinationQueueInternal { get; } = new();
    public IEnumerable<int> DestinationQueue => DestinationQueueInternal;

    public void AddDestination(int floor)
    {
        if (!DestinationQueueInternal.Contains(floor))
            DestinationQueueInternal.Enqueue(floor);
    }

    public void Move(bool handleInitialBoarding = true)
    {
        var isFirstStop = !handleInitialBoarding;

        while (DestinationQueueInternal.Any() || PassengerCount > 0)
        {
            SortDestinations();

            var nextFloor = DestinationQueueInternal.Any() ? DestinationQueueInternal.Peek() : CurrentFloor;
            if (CurrentFloor == nextFloor && DestinationQueueInternal.Any())
            {
                DestinationQueueInternal.Dequeue();
                continue;
            }

            if (nextFloor != CurrentFloor)
            {
                CurrentDirection = nextFloor > CurrentFloor ? Direction.Up : Direction.Down;
                State = ElevatorState.Moving;

                Console.WriteLine($"Elevator {Id} moving {CurrentDirection} from Floor {CurrentFloor} to Floor {nextFloor}.");

                while (CurrentFloor != nextFloor)
                {
                    Thread.Sleep(1000); // Simulate movement delay
                    CurrentFloor += CurrentDirection == Direction.Up ? 1 : -1;
                    Console.WriteLine($"Elevator {Id} is now on Floor {CurrentFloor}.");
                }

                Console.WriteLine($"Elevator {Id} has arrived at Floor {CurrentFloor}.");
                Console.WriteLine(new string('-', 50));
            }

            // Handle passengers exiting
            if (PassengerCount > 0)
            {
                var exitingPassengers = InputHelper.GetValidatedInput(
                    $"How many passengers are exiting at Floor {CurrentFloor}? (Max: {PassengerCount}): ",
                    0,
                    PassengerCount);

                UnloadPassengers(exitingPassengers);
            }

            // If no destinations remain but there are still passengers
            if (!DestinationQueueInternal.Any() && PassengerCount > 0)
            {
                Console.WriteLine($"There are still {PassengerCount} passengers on the elevator.");
                while (PassengerCount > 0)
                {
                    var newDestination = InputHelper.GetValidatedInput(
                        $"Please provide a new destination floor for one of the remaining {PassengerCount} passengers: ",
                        0,
                        _numberOfFloors - 1);

                    AddDestination(newDestination);

                    // Repeat until all passengers have valid destinations
                    Console.WriteLine($"Added Floor {newDestination} to the destination queue.");
                }
            }

            // Handle passengers boarding
            if (!isFirstStop && AvailableSpace() > 0)
            {
                var boardingPassengers = InputHelper.GetValidatedInput(
                    $"How many passengers are boarding at Floor {CurrentFloor}? (Max: {AvailableSpace()}): ",
                    0,
                    AvailableSpace());

                LoadPassengers(boardingPassengers);

                for (var i = 1; i <= boardingPassengers; i++)
                {
                    var newDestination = InputHelper.GetValidatedInput(
                        $"Passenger {i}, enter your destination floor: ",
                        0,
                        _numberOfFloors - 1);

                    AddDestination(newDestination);
                }
            }

            isFirstStop = false;
        }

        Console.WriteLine($"Elevator {Id} is now Idle at Floor {CurrentFloor} with {PassengerCount} passengers.");
        State = ElevatorState.Idle;
        CurrentDirection = Direction.Stationary;
    }

    public void LoadPassengers(int count)
    {
        PassengerCount += count;
        Console.WriteLine($"{count} passengers boarded Elevator {Id}. Current count: {PassengerCount}");
    }

    public void UnloadPassengers(int count)
    {
        PassengerCount -= count;
        Console.WriteLine($"{count} passengers exited Elevator {Id}. Current count: {PassengerCount}");
    }

    public int AvailableSpace() => MaxCapacity - PassengerCount;

    public void SortDestinations()
    {
        if (!DestinationQueueInternal.Any()) return;

        var currentDirectionDestinations = CurrentDirection == Direction.Up
            ? DestinationQueueInternal.Where(floor => floor >= CurrentFloor).OrderBy(floor => floor).ToList()
            : DestinationQueueInternal.Where(floor => floor <= CurrentFloor).OrderByDescending(floor => floor).ToList();

        var oppositeDirectionDestinations = CurrentDirection == Direction.Up
            ? DestinationQueueInternal.Where(floor => floor < CurrentFloor).OrderByDescending(floor => floor).ToList()
            : DestinationQueueInternal.Where(floor => floor > CurrentFloor).OrderBy(floor => floor).ToList();

        DestinationQueueInternal.Clear();
        foreach (var floor in currentDirectionDestinations.Concat(oppositeDirectionDestinations))
        {
            DestinationQueueInternal.Enqueue(floor);
        }
    }
}

public enum Direction { Stationary, Up, Down }

public enum ElevatorState { Idle, Moving, DoorsOpen }
