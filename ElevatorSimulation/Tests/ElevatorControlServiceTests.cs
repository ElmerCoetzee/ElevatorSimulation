using ElevatorSimulation.Application.Services;
using Xunit;

namespace ElevatorSimulation.Tests;

public class ElevatorControlServiceTests
{
    [Fact]
    public void DispatchElevator_ValidRequest_ElevatorDispatched()
    {
        var service = new ElevatorControlService(1, 10, 5);

        // Simulate a request to dispatch an elevator
        service.DispatchElevator();

        // Verify elevator status
        var output = service.GetElevatorStatus();
        Assert.Contains("Idle", output); // Ensure the elevator returns to idle
    }
}
