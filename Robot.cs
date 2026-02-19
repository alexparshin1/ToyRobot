using System.Text.RegularExpressions;

namespace ToyRobot;

/// <summary>
/// Toy robot.
/// </summary>
/// <remarks>
/// Some methods and props are public to allow unit testing.
/// </remarks>
public class Robot
{
    public enum Direction { NORTH, EAST, SOUTH, WEST }
    private enum Command { PLACE, MOVE, LEFT, RIGHT, REPORT }
    
    const int TabletopSize = 5;

    private bool isBatchMode = false;
    private TextReader _reader;
    private bool _isPlaced;
    public Direction Facing { get; set; } = Direction.NORTH;
    public (int x, int y) Position { get; set; } = (0, 0);

    private static readonly Regex _commandParser = new (@"^(Move|Left|Right|Report)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex _placeParser = new (@"^Place (\d+),(\d+),(North|East|South|West)$", RegexOptions.IgnoreCase|RegexOptions.Compiled);
    
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="inputFileName">Input file name.</param>
    /// <exception cref="InvalidOperationException">Thrown when the robot can't open the file.</exception>
    public Robot(string inputFileName)
    {
        if (inputFileName != "stdin" && !File.Exists(inputFileName)) throw new InvalidOperationException($"The file {inputFileName} does not exist.");
        _reader = inputFileName == "stdin" ? Console.In : new StreamReader(inputFileName);
        isBatchMode = inputFileName != "stdin";
    }

    /// <summary>
    /// Execute the robot.
    /// </summary>
    public void Execute()
    {
        if (!isBatchMode) Console.WriteLine("ToyRobot is reporting for duty. Waiting for the PLACE command:");

        while (ReadAndExecuteALine(_reader)) {}
        
        _reader.Close();

        if (!isBatchMode)
        {
            Console.WriteLine("Bye.");
        }
        else
        {
            _reader.Dispose();
        }
    }

    /// <summary>
    /// Read and execute a single command line.
    /// </summary>
    /// <param name="reader">Input file or console reader.</param>
    /// <returns>True if reading should continue, or false if robot should stop reading commands.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private bool ReadAndExecuteALine(TextReader reader)
    {
        try
        {
            var line = reader.ReadLine();
            
            // If the line is null, EOF was reached.
            // If the line is empty or ~'exit', exit.
            if (string.IsNullOrWhiteSpace(line) || line.ToUpperInvariant() == "EXIT") return false; // end of input

            // Is it a command (excluding PLACE)?
            var match = _commandParser.Match(line);
            if (match.Success)
            {
                if (!_isPlaced)
                {
                    Console.Error.WriteLine($"PLACE me on the table, first.");
                    return true; // Continue reading lines.
                }

                if (!Enum.TryParse(match.Groups[1].Value, out Command command))
                {
                    // We should never get here.
                    throw new InvalidOperationException($"Unknown command: {match.Groups[1].Value}");
                }
                switch (command)
                {
                    case Command.LEFT: RotateLeft(); break;
                    case Command.RIGHT: RotateRight(); break;
                    case Command.MOVE: Move(); break;
                    case Command.REPORT: Report(); break;
                }
                return true;
            }

            // Is it a PLACE command?
            var matchPlace = _placeParser.Match(line);
            if (matchPlace.Success)
            {
                if (!Enum.TryParse(matchPlace.Groups[3].Value.ToUpperInvariant(), out Direction facing))
                {
                    // We should never get here.
                    throw new InvalidOperationException($"Unknown direction: {match.Groups[3].Value}");
                }

                ExecutePlaceCommand(int.Parse(matchPlace.Groups[1].Value), int.Parse(matchPlace.Groups[2].Value), facing);
                _isPlaced = true;
                return true;
            }

            Console.Error.WriteLine($"Invalid command or command format: {line}");
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"Can't read line: {exception.Message}");
            return false; // Stop reading lines.
        }
        return true; // Continue reading lines.
    }

    /// <summary>
    /// Report the robot position and facing.
    /// </summary>
    public void Report()
    {
        Console.WriteLine($"{Position.x},{Position.y},{Facing}");
    }

    /// <summary>
    /// Validate the position is on the tabletop.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool ValidPosition(int x, int y) => x >= 0 && x < TabletopSize && y >= 0 && y < TabletopSize;
    
    /// <summary>
    /// Move the robot one position in the current direction.
    /// </summary>
    public void Move()
    {
        var newPosition = (Position.x, Position.y);
        switch (Facing)
        {
            case Direction.NORTH: newPosition.y++; break;
            case Direction.WEST: newPosition.x--; break;
            case Direction.SOUTH: newPosition.y--; break;
            case Direction.EAST: newPosition.x--; break;
        }

        if (!ValidPosition(newPosition.x, newPosition.y))
        {
            Console.Error.WriteLine($"Oops! Can't do it or I'd fall off the tabletop!");
        }
        
        Position = newPosition;
    }

    /// <summary>
    /// Rotate the robot 90 degrees left.
    /// </summary>
    public void RotateLeft()
    {
        switch (Facing)
        {
            case Direction.NORTH: Facing = Direction.WEST; break;
            case Direction.WEST: Facing = Direction.SOUTH; break;
            case Direction.SOUTH: Facing = Direction.EAST; break;
            case Direction.EAST: Facing = Direction.NORTH; break;
        }
    }

    /// <summary>
    /// Rotate the robot 90 degrees right.
    /// </summary>
    public void RotateRight()
    {
        switch (Facing)
        {
            case Direction.NORTH: Facing = Direction.EAST; break;
            case Direction.EAST: Facing = Direction.SOUTH; break;
            case Direction.SOUTH: Facing = Direction.WEST; break;
            case Direction.WEST: Facing = Direction.NORTH; break;
        }
    }

    /// <summary>
    /// Execute the PLACE command.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="facing"></param>
    public void ExecutePlaceCommand(int x, int y, Direction facing)
    {
        if (!ValidPosition(x, y))
        {
            Console.Error.WriteLine($"Oops! Can't do it or I'd fall off the tabletop!");
        }
        Position = (x, y);
        Facing = facing;
    }
}