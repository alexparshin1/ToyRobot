using ToyRobot;

namespace UnitTests;

[TestClass]
public sealed class UnitTests
{
    [TestMethod]
    public void TestConstructor()
    {
        var robot = new Robot("commands.txt");

        Assert.AreEqual((0, 0), robot.Position);
        Assert.AreEqual(Robot.Direction.NORTH, robot.Facing);
    }
    
    [TestMethod]
    public void TestPlaceCommand()
    {
        var robot = new Robot("commands.txt");
        
        robot.ExecutePlaceCommand(2, 2, Robot.Direction.NORTH);

        Assert.AreEqual((2, 2), robot.Position);
        Assert.AreEqual(Robot.Direction.NORTH, robot.Facing);
    }
    
    [TestMethod]
    public void TestLeftCommand()
    {
        var robot = new Robot("commands.txt");
        
        robot.ExecutePlaceCommand(2, 2, Robot.Direction.NORTH);
        robot.RotateLeft();

        Assert.AreEqual((2, 2), robot.Position);
        Assert.AreEqual(Robot.Direction.WEST, robot.Facing);
    }
    
    // Etc. 
}