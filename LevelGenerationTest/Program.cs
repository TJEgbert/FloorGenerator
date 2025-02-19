using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Tracing;
using System.Reflection.Metadata.Ecma335;
using System.Xml.XPath;

class TestMain
{
    static void Main(string[] args)
    {
        TestMain testMain = new TestMain();
        FloorGenerator floor = new FloorGenerator(15, 4, 3);
        Room[] rooms = floor.Generate();

        Console.WriteLine("Floor generated");

        foreach (Room room in rooms)
        {
           Console.WriteLine(room.ToString());
        }

        Console.ReadLine();
    }
}

/// <summary>
/// A class used to create a room that can be used in FloorGenerator class
/// </summary>
class Room
{
    // Holds the index of the connecting room to the north
    public int north { get; set; } = -1;
    // Holds the index of the connecting room to the south
    public int south { get; set; } = -1;
    // Holds the index of the connecting room to the west
    public int west { get; set; } = -1;
    // Holds the index of the connecting room to the east
    public int east { get; set; } = -1;
    // The index of the room itself
    public int roomIndex { get; set; } = -1;
    // Holds if the room is a treasure room or not
    public bool treasure { get; set; } = false;
    // Holds if the room is the starting of the floor
    public bool start { get; set; } = false;
    //  Holds if the room is the goal of the floor
    public bool goal { get; set; } = false;

    /// <summary>
    /// Constructor of the Room class 
    /// </summary>
    /// <param name="index">The index of where the room is located in the array</param>
    public Room(int index)
    {
        roomIndex = index;
    }

    /// <summary>
    /// Get the indexes of the connecting rooms
    /// </summary>
    /// <returns>A list of ints of the connected rooms</returns>
    public List<int> ConnectedRooms()
    {
        List<int> rooms = new List<int>();

        if (north >= 0)
        {
            rooms.Add(north);
        }
        if (south >= 0)
        {
            rooms.Add(south);
        }
        if (west >= 0)
        {
            rooms.Add(west);
        }
        if (east >= 0) 
        {
            rooms.Add(east);
        }
        return rooms;
    }

    /// <summary>
    /// Gets a list of open directions on a room
    /// </summary>
    /// <returns>A list of strings of the open directions</returns>
    public List<string> OpenDirections()
    {
        List<string> returnList = new List<string>();
        if (north < 0)
        {
            returnList.Add("north");
        }
        if (south < 0)
        {
            returnList.Add("south");
        }
        if (west < 0)
        {
            returnList.Add("west");
        }
        if (east < 0)
        {
            returnList.Add("east");
        }
        return returnList;
    }

    /// <summary>
    /// Gets the number of open direction based on the OpenDirections() results
    /// </summary>
    /// <returns>An int if the total number of open directions</returns>
    public int NumberOfOpenDirections()
    {
        return OpenDirections().Count;
    }

    /// <summary>
    /// Overide the ToString() to print information about a room
    /// </summary>
    /// <returns>A string formatted room information</returns>
    public override string ToString()
    {
        string returnString = string.Empty;
        returnString += "Room number is " + roomIndex + "\n";

        if (start)
        {
            returnString += "This is the starting point\n";
        }
        if (goal)
        {
            returnString += "This is the end point\n";
        }
        if (treasure)
        {
            returnString += "This is a treasure room\n";
        }

        returnString += "The rooms attached to this room are \n";
        if (north >= 0) { returnString += "north Room number: " + north + "\n";}
        if (south >= 0) { returnString += "south Room number: " + south + "\n"; }
        if (west >= 0) { returnString += "west Room number: " + west + "\n"; }
        if (east >= 0) { returnString += "east Room number: " + east + "\n"; }
        returnString += "\n";

        return returnString;
    }
}

/// <summary>
/// Randomly generates a level of connected rooms 
/// </summary>
class FloorGenerator
{
    // Holds the number of rooms to be created
    private int roomsCount { get; set; }
    // Holds the number of rooms between the start and goal
    private int roomsBetween { get; set; }
    // Holds the number of treasure rooms on a floor
    private int numberOfTreasures {  get; set; }

    /// <summary>
    /// Constructor of the LevelGenerator class
    /// </summary>
    /// <param name="roomCount">The number of rooms the user wants on the floor</param>
    /// <param name="roomsBetween">The number of rooms the user wants between the start and goal</param>
    /// <param name="numberOfTreasures">The number of treaure the user wants on the floor</param>
    public FloorGenerator(int roomCount, int roomsBetween, int numberOfTreasures)
    {
        this.roomsCount = roomCount;
        this.roomsBetween = roomsBetween;
        this.numberOfTreasures = numberOfTreasures;
    }

    /// <summary>
    /// Generates a floor based on the properties of the class
    /// </summary>
    /// <returns>An array of connected Rooms objects</returns>
    public Room[] Generate()
    {
        // Creates the return array
        Room[] floor = new Room[roomsCount];
        // Sets up and Random object
        Random rand = new Random();
        // Number rooms marked as a treasure
        int treasureAdded = 0;

        // Loops through the length of the floor array and creates room
        for(int i = 0; i < floor.Length; i++)
        {
            if (i == 0)
            {
                // Mark the room as the start
                floor[i] = CreateStart(i);
            }
            else if (i == floor.Length - 1)
            {
                // Marks the room as goal
                floor[i] = CreateGoal(i);
            }
            else
            {
                // Creates treasure rooms until treasureAdded is equal to numberOfTreasures
                if (treasureAdded != numberOfTreasures)
                {
                    floor[i] = CreateTreasureRoom(i);
                    treasureAdded++;
                }
                else
                {
                    // Generates a normal room
                    floor[i] = CreateRoom(i);
                }
            }
        }
        // Creates a list from the floor array
        List<Room> neededToBeConnected = floor.ToList();

        // Creates a list to add rooms the be connected to
        List<Room> connectAbleRooms = new List<Room>();

        // Adds the start room
        connectAbleRooms.Add(floor[0]);
        neededToBeConnected.Remove(floor[0]); 

        // Loops through connecting rooms to each other until counter equal to roomsBetween
        int counter = 0;
        while (counter < roomsBetween)
        {
            // Gets a random room from the neededToBeConnected thats not the goal room
            int connectingRoomIndex = rand.Next(0, neededToBeConnected.Count -1);
            // Checks to see if the room was already connected
            while (connectAbleRooms.Contains(neededToBeConnected[connectingRoomIndex]))
            {
                // Gets another room room
                connectingRoomIndex = rand.Next(0, neededToBeConnected.Count -1);
            }
            // Gets the last room that was connected
            Room lastConnectedRoom = connectAbleRooms.LastOrDefault();
            // Connects the current and last room together
            ConnectRooms(lastConnectedRoom, neededToBeConnected[connectingRoomIndex]);
            // Updates the list being used to keep track
            connectAbleRooms.Add(neededToBeConnected[connectingRoomIndex]);
            neededToBeConnected.Remove(neededToBeConnected[connectingRoomIndex]);
            counter++;
        }
        // Connects the goal room to the last connected room
        Room goalConnected = connectAbleRooms.LastOrDefault();
        ConnectRooms(goalConnected, floor[floor.Length - 1]);
        neededToBeConnected.Remove(floor[floor.Length - 1]);

        // Connects the rest of the floors to any room except the goal
        while (neededToBeConnected.Count > 0)
        {
            Room nextRoom = neededToBeConnected.First();
            int connectingRoomIndex = rand.Next(0, connectAbleRooms.Count - 1);
            Room connectedRoom = connectAbleRooms[connectingRoomIndex];
            // Tries to connect room
            while (!ConnectRooms(nextRoom, connectedRoom))
            {
                // Gets a new random room
                connectingRoomIndex = rand.Next(0, connectAbleRooms.Count - 1);
                connectedRoom = connectAbleRooms[connectingRoomIndex];
            }
            // Updates the lists
            connectAbleRooms.Add(nextRoom);
            neededToBeConnected.Remove(nextRoom);
        }

        return floor;
    }

    /// <summary>
    /// Creates a Room object and sets start to true
    /// </summary>
    /// <param name="index">The room index in the array</param>
    /// <returns>Room object with start set to true</returns>
    private Room CreateStart(int index)
    {
        Room newRoom = new Room(index);
        newRoom.start = true;
        return newRoom;
    }

    /// <summary>
    /// Creates a Room object and sets goal to true
    /// </summary>
    /// <param name="index">The room index in the array</param>
    /// <returns>Room object with start set to goal</returns>
    private Room CreateGoal(int index)
    {
        Room newRoom = new Room(index);
        newRoom.goal = true;
        return newRoom;
    }

    /// <summary>
    /// Creates a Room object
    /// </summary>
    /// <param name="index">The room index in the array</param>
    /// <returns>Room object</returns>
    private Room CreateRoom(int index)
    {
        Room newRoom = new Room(index);
        return newRoom;
    }

    /// <summary>
    /// Creates a Room object and sets treasure to true
    /// </summary>
    /// <param name="index">The room index in the array</param>
    /// <returns>Room object with start set to treasure</returns>
    private Room CreateTreasureRoom(int index)
    {
        Room newRoom = new Room(index);
        newRoom.treasure = true;
        return newRoom;
    }

    /// <summary>
    /// Connects the two passed in Room Objects
    /// </summary>
    /// <param name="mainRoom">The Room object to be connected</param>
    /// <param name="connectingRoom">The Room object to connect to</param>
    /// <returns>True if the connection was successful or false if not sucessfull</returns>
    private bool ConnectRooms(Room mainRoom, Room connectingRoom)
    {
        bool returnBool = false;
        // Get the open directions for the rooms
        List<string> mainRoomOpenDirections = mainRoom.OpenDirections();
        List<string> connectingRoomDirections = connectingRoom.OpenDirections();
        // Checks to see connectingRoom has any open direction
        if (connectingRoomDirections.Count != 0)
        {

            // Get a random direction from the mainRoomOpenDirections
            Random random = new Random();
            int directionIndex = random.Next(0, mainRoomOpenDirections.Count);
            string direction = mainRoomOpenDirections[directionIndex];
            
            // Connects the rooms if possible and sets returnBool to true
            if(direction == "north" && connectingRoomDirections.Contains("south"))
            {
                returnBool = true;
                mainRoom.north = connectingRoom.roomIndex;
                connectingRoom.south = mainRoom.roomIndex;
            }
            else if (direction == "south" && connectingRoomDirections.Contains("north"))
            {
                returnBool = true;
                mainRoom.south = connectingRoom.roomIndex;
                connectingRoom.north = mainRoom.roomIndex;
            }
            if (direction == "east" && connectingRoomDirections.Contains("west"))
            {
                returnBool = true;
                mainRoom.east = connectingRoom.roomIndex;
                connectingRoom.west = mainRoom.roomIndex;
            }
            if (direction == "west" && connectingRoomDirections.Contains("east"))
            {
                returnBool = true;
                mainRoom.west = connectingRoom.roomIndex;
                connectingRoom.east = mainRoom.roomIndex;
            }
           

        }

        return returnBool;
    }

}
