namespace ElevatorSimulation.Utilities;

public static class InputHelper
{
    public static int GetValidatedInput(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out var value) && value >= min && value <= max)
                return value;

            Console.WriteLine($"Invalid input. Please enter a number between {min} and {max}.");
        }
    }
}
