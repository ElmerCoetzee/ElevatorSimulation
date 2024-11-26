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
        service.DispatchElevator(currentFloor: 3, numberOfPeople: 1); // Floor 3

        // Simulate passenger destination
        service.GetElevators().First().AddDestination(4); // Add destination manually for the test
        service.GetElevators().First().Move(); // Simulate movement to destination

        var output = service.GetElevatorStatus();

        // Assert
        Assert.Contains("Elevator 1 | Floor: 4", output); // Check if the elevator reaches Floor 4
    }

    [Fact]
    public void DispatchElevator_Overloaded_ThrowsException()
    {
        // Arrange
        var service = new ElevatorControlService(1, 5, 5); // 1 elevator, max capacity 5, 5 floors (0 to 4)

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => service.DispatchElevator(currentFloor: 2, numberOfPeople: 6)); // Exceeds max capacity
    }

    [Fact]
    public void DispatchElevator_EmptyMove_NoPromptsOrErrors()
    {
        // Arrange
        var service = new ElevatorControlService(1, 10, 10);

        // Act
        service.DispatchElevator(currentFloor: 0, numberOfPeople: 0); // No people to board
        var output = service.GetElevatorStatus();

        // Assert
        Assert.Contains("Idle", output); // Ensure the elevator transitions to idle without issues
    }
}

