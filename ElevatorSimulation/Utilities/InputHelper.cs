namespace ElevatorSimulation.Utilities;
public static class InputHelper
{
    public static int GetValidatedInput(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            if (int.TryParse(input, out var value) && value >= min && value <= max)
                return value;

            Console.WriteLine($"Invalid input. Please enter a number between {min} and {max}.");
        }
    }
}