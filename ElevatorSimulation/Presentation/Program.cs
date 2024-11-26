using ElevatorSimulation.Application.Services;

class Program
{
    static void Main()
    {
        var elevatorService = new ElevatorControlService(numberOfElevators: 3, maxCapacity: 10, numberOfFloors: 11);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Elevator Simulation");
            Console.WriteLine("1. Call Elevator");
            Console.WriteLine("2. View Elevator Status");
            Console.WriteLine("3. Exit");
            Console.Write("Select an option: ");

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    try
                    {
                        Console.Write("Enter Current Floor (0 for Ground Floor): ");
                        var currentFloor = int.Parse(Console.ReadLine() ?? "0");

                        Console.Write("Enter Number of People: ");
                        var numberOfPeople = int.Parse(Console.ReadLine() ?? "0");

                        Console.Write("Enter Destination Floor: ");
                        var destinationFloor = int.Parse(Console.ReadLine() ?? "0");

                        elevatorService.DispatchElevator(currentFloor, numberOfPeople, destinationFloor);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }

                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    break;

                case "2":
                    elevatorService.DisplayElevatorStatus();
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    break;

                case "3":
                    return;

                default:
                    Console.WriteLine("Invalid option. Try again.");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }
    }
}