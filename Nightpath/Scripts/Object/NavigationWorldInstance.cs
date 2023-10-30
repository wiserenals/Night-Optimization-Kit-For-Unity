public class NavigationWorldInstance
{
    public NavigationTile[,] Tiles;
    public bool[,] Wall;
    public int obstacleBuildTime;
    
    public NavigationWorldInstance(int sizeX, int sizeY, bool[,] wall = null)
    {
        Tiles = new NavigationTile[sizeX, sizeY];
        Wall = wall ?? new bool[sizeX, sizeY];
    }
}
