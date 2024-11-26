namespace ElevatorSimulation.Domain.Entities;

public class Floor(int floorNumber)
{
    public int FloorNumber { get; } = floorNumber;
    public int WaitingPassengers { get; private set; } = 0;

    public void AddPassengers(int count) => WaitingPassengers += count;

    public void RemovePassengers(int count)
    {
        if (count > WaitingPassengers)
            throw new InvalidOperationException("Cannot remove more passengers than waiting.");

        WaitingPassengers -= count;
    }
}
