using ElevatorSimulation.Application.Services;
using Xunit;

namespace ElevatorSimulation.Tests.Application.Services;

public class ElevatorControlServiceTests
{
    [Fact]
    public void DispatchElevator_ValidRequest_ElevatorDispatched()
    {
        // Arrange
        var service = new ElevatorControlService(1, 10, 5);
        var currentFloor = 0;
        var numberOfPeople = 2;
        var passengerDestinations = new List<int> { 3, 4 };

        // Act
        service.DispatchElevator(currentFloor, numberOfPeople, passengerDestinations);

        // Assert
        var output = service.GetElevatorStatus();
        Assert.Contains("Idle", output); // Ensure the elevator returns to idle
    }

    [Fact]
    public void DispatchElevator_ExceedsCapacity_ThrowsException()
    {
        // Arrange
        var service = new ElevatorControlService(1, 5, 5);
        var currentFloor = 2;
        var numberOfPeople = 6;
        var passengerDestinations = new List<int> { 3, 4 };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => service.DispatchElevator(currentFloor, numberOfPeople, passengerDestinations));
    }

    [Fact]
    public void DispatchElevator_NoPassengers_ElevatorIdle()
    {
        // Arrange
        var service = new ElevatorControlService(1, 10, 5);
        var currentFloor = 0;
        var numberOfPeople = 0;
        var passengerDestinations = new List<int>();

        // Act
        service.DispatchElevator(currentFloor, numberOfPeople, passengerDestinations);

        // Assert
        var output = service.GetElevatorStatus();
        Assert.Contains("Idle", output); // Ensure the elevator transitions to idle without passengers
    }
}
