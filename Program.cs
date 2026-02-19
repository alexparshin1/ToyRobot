namespace ToyRobot;

internal class Program
{
    static void Main(string[] args)
    {
        string inputFile = "stdin";
        if (args.Length > 0)
        {
            inputFile = args[0];
        }

        if (string.IsNullOrWhiteSpace(inputFile))
        {
            Console.WriteLine("No input file specified.");
        }

        try
        {
            var robot = new Robot(inputFile);
            robot.Execute();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }
}