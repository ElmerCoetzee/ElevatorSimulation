using ElevatorSimulation.Application.Services;
using Xunit;

namespace ElevatorSimulation.Tests;

public class ElevatorControlServiceTests
{
    [Fact]
    public void DispatchElevator_ValidRequest_ElevatorDispatched()
    {
        // Arrange
        var service = new ElevatorControlService(2, 10, 5); // 2 elevators, max capacity 10, 5 floors (0 to 4)

        // Act
        service.DispatchElevator(currentFloor: 3, numberOfPeople: 1, destinationFloor: 4); // Floor 3 to Floor 4
        var output = service.GetElevatorStatus();

        // Assert
        Assert.Contains("Elevator 1 | Floor: 3", output); // Check if the elevator is correctly dispatched to Floor 3
    }

    [Fact]
    public void DispatchElevator_Overloaded_ThrowsException()
    {
        // Arrange
        var service = new ElevatorControlService(1, 5, 5); // 1 elevator, max capacity 5, 5 floors (0 to 4)

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => service.DispatchElevator(currentFloor: 2, numberOfPeople: 6, destinationFloor: 4)); // Exceeds max capacity
    }
}
