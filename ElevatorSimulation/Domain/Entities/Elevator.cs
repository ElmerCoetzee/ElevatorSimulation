using ElevatorSimulation.Utilities;

namespace ElevatorSimulation.Domain.Entities;

public class Elevator(int id, int maxCapacity, int numberOfFloors)
{
    public int Id { get; } = id;
    public int CurrentFloor { get; private set; } = 0; // Start at ground floor.
    public Direction CurrentDirection { get; private set; } = Direction.Stationary;
    public ElevatorState State { get; private set; } = ElevatorState.Idle;
    public int PassengerCount { get; private set; } = 0;
    public int MaxCapacity { get; } = maxCapacity;
    private readonly int _numberOfFloors = numberOfFloors;

    private Queue<int> DestinationQueueInternal { get; } = new();
    public IEnumerable<int> DestinationQueue => DestinationQueueInternal;

    public void AddDestination(int floor)
    {
        if (floor < 0 || floor >= _numberOfFloors)
        {
            Console.WriteLine($"Invalid floor number. Please choose a floor between 0 and {_numberOfFloors - 1}.");
            return;
        }

        if (!DestinationQueueInternal.Contains(floor))
            DestinationQueueInternal.Enqueue(floor);
    }

    public void Move(bool handleInitialBoarding = true)
    {
        var isFirstStop = !handleInitialBoarding;

        while (DestinationQueueInternal.Any())
        {
            SortDestinations();

            var nextFloor = DestinationQueueInternal.Peek();
            if (CurrentFloor == nextFloor)
            {
                DestinationQueueInternal.Dequeue();
                continue;
            }

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
            DestinationQueueInternal.Dequeue();

            // Handle passengers exiting
            if (PassengerCount > 0)
            {
                var exitingPassengers = InputHelper.GetValidatedInput(
                    $"How many passengers are exiting at Floor {CurrentFloor}? (Max: {PassengerCount}): ",
                    0,
                    PassengerCount);

                UnloadPassengers(exitingPassengers);
            }

            // Skip boarding prompt during the first stop if already handled
            if (isFirstStop)
            {
                isFirstStop = false;
                continue;
            }

            // Handle passengers boarding
            if (AvailableSpace() > 0)
            {
                var boardingPassengers = InputHelper.GetValidatedInput(
                    $"How many passengers are boarding at Floor {CurrentFloor}? (Max: {AvailableSpace()}): ",
                    0,
                    AvailableSpace());

                LoadPassengers(boardingPassengers);

                // Add destinations for new passengers
                for (var i = 1; i <= boardingPassengers; i++)
                {
                    var newDestination = InputHelper.GetValidatedInput(
                        $"Passenger {i}, enter your destination floor: ",
                        0,
                        _numberOfFloors - 1);

                    AddDestination(newDestination);
                }
            }
        }

        Console.WriteLine($"Elevator {Id} is now Idle at Floor {CurrentFloor} with {PassengerCount} passengers.");
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

    public void SortDestinations()
    {
        if (!DestinationQueueInternal.Any()) return;

        // Split destinations into two groups: current direction and opposite direction
        var currentDirectionDestinations = CurrentDirection == Direction.Up
            ? DestinationQueueInternal.Where(floor => floor >= CurrentFloor).OrderBy(floor => floor).ToList()
            : DestinationQueueInternal.Where(floor => floor <= CurrentFloor).OrderByDescending(floor => floor).ToList();

        var oppositeDirectionDestinations = CurrentDirection == Direction.Up
            ? DestinationQueueInternal.Where(floor => floor < CurrentFloor).OrderByDescending(floor => floor).ToList()
            : DestinationQueueInternal.Where(floor => floor > CurrentFloor).OrderBy(floor => floor).ToList();

        // Clear and repopulate the queue
        DestinationQueueInternal.Clear();
        foreach (var floor in currentDirectionDestinations.Concat(oppositeDirectionDestinations))
        {
            DestinationQueueInternal.Enqueue(floor);
        }
    }
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