namespace ElevatorSimulation.Domain.Interfaces;
public interface IElevatorControlService
{
    void DispatchElevator(int currentFloor, int numberOfPeople, int destinationFloor);
    void DisplayElevatorStatus();
}
