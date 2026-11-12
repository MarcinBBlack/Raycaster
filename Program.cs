using Raycaster;

int mapWidth = 24;
int mapHeight = 24;
int screenWidth = 80;
int screenHeight = 24;
char[,] screenBuffer = new char[screenHeight, screenWidth];
int[,] map = new int[,]
{
  {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
  {1,0,0,0,0,0,2,2,2,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1},
  {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1},
  {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,3,0,0,0,3,0,0,0,1},
  {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1},
  {1,0,0,0,0,0,2,2,0,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1},
  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
  {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
  {1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
  {1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
  {1,4,0,0,0,0,5,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
  {1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
  {1,4,0,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
  {1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
  {1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
  {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
};

double rayPositionX = 15;
double rayPositionY = 20;
double cameraRotationX = 1, cameraRotationY = 0;
double planeX = 0, planeY = 0.66; //the 2d raycaster version of camera plane
double time = 0; //time of current frame
double oldTime = 0; //time of previous frame
double rayXCoordinateInCameraSpace = 0;
double rayDirectionX = 0;
double rayDirectionY = 0;
double deltaDistanceX = 0; //distance the ray has go from nearest vertical line to next (eg from line x=1 to line x=2)
double deltaDistanceY = 0; //distance the ray has go from nearest horizontal line to next (eg. from line y = 1 to line y =2 )
double distanceToNearestX = 0;
double distanceToNearestY = 0;
int rayCellCoordinateX = 0; // coordinate x of cell current player position
int rayCellCoordinateY = 0; // coordinate y of cell current player position
int stepX = 1; //determines whether ray advances in x coordinate of world map or goes in negative x direction
int stepY = 1; //determines whether ray advances in y coordinate of world map or goes in negative y direction
int hit = 0; //determines whether ray hit something or not, 0 if it hit nothing
double distanceToHit;
double rayDirectionLength = Math.Sqrt(
    cameraRotationX * cameraRotationX +
    cameraRotationY * cameraRotationY);
cameraRotationX /= rayDirectionLength;
cameraRotationY /= rayDirectionLength;
double rotationAngleInRadians = 5.0d * Math.PI / 180;
while (true)
{ 
    for (int x = 0; x < screenWidth; x++)
    {
        rayXCoordinateInCameraSpace = 2 * x / (double)screenWidth - 1; //x-coordinate of vertical line in camera space (from -1 to 1), every x in loop is ray shot 
        rayDirectionX = cameraRotationX + planeX * rayXCoordinateInCameraSpace; // x coordinate of ray, one of many rays for each vertical line to be drawn on screen
        rayDirectionY = cameraRotationY + planeY * rayXCoordinateInCameraSpace; // y coordinate of ray, one of many rays for each vertical line to be drawn on screen
        deltaDistanceX = rayDirectionX == 0 ? Double.PositiveInfinity : Math.Abs(1.0 / rayDirectionX); //Math.Sqrt(1 + (rayDirY * rayDirY) / (rayDirX * rayDirX)); -> |rayDir|/ rayDirX, |rayDir| is length of rayDir vector, but rayDir is direction vector so we set length as 1
        deltaDistanceY = rayDirectionY == 0 ? Double.PositiveInfinity : Math.Abs(1.0 / rayDirectionY); //analogically
        rayCellCoordinateX = (int)rayPositionX; //in which x coordinate of world map's cell
        rayCellCoordinateY = (int)rayPositionY; //in which y coordinate of world map's cell
        distanceToHit = 0;

        if (rayDirectionX < 0)
        {
            stepX = -1;
            distanceToNearestX = (rayPositionX - rayCellCoordinateX) * deltaDistanceX;
        }
        else
        {
            stepX = 1;
            distanceToNearestX = (rayCellCoordinateX + 1.0d - rayPositionX) * deltaDistanceX;
        }


        if (rayDirectionY < 0)
        {
            stepY = -1;
            distanceToNearestY = (rayPositionY - rayCellCoordinateY) * deltaDistanceY;
        }
        else
        {
            stepY = 1;
            distanceToNearestY = (rayCellCoordinateY + 1.0d - rayPositionY) * deltaDistanceY;
        }

        hit = map[rayCellCoordinateY, rayCellCoordinateX];

        while (hit == 0)
        {
            if (distanceToNearestX < distanceToNearestY)
            {
                rayCellCoordinateX += stepX;
                distanceToHit = distanceToNearestX;
                if (rayCellCoordinateX < 0 || rayCellCoordinateX >= mapWidth)
                {
                    break;
                }
                distanceToNearestX += deltaDistanceX;
            }

            else
            {
                rayCellCoordinateY += stepY;
                distanceToHit = distanceToNearestY;
                if (rayCellCoordinateY < 0 || rayCellCoordinateY >= mapHeight)
                {
                    break;
                }
                distanceToNearestY += deltaDistanceY;
            }
            hit = map[rayCellCoordinateY, rayCellCoordinateX];
        }
        LineDrawData lineDrawData = GetLineDrawData(screenHeight, distanceToHit);

        for (int y = 0; y < screenHeight; y++)
        {
            if (y >= lineDrawData.LineDrawStartRowIndex && y < lineDrawData.LineDrawEndRowIndex)
            {
                screenBuffer[y, x] = GetDrawCharByHitValue(hit);
            }
            else
            {
                screenBuffer[y, x] = ' ';
            }
        }


    }
    Console.Clear();
    DrawScreenBuffer(screenBuffer);

    var pressedKey = Console.ReadKey();

    if (pressedKey.Key == ConsoleKey.D)
    {
        double currentCameraRotationX = cameraRotationX;
        cameraRotationX = currentCameraRotationX * Math.Cos(rotationAngleInRadians) - cameraRotationY * Math.Sin(rotationAngleInRadians);
        double currentPlaneX = planeX;
        planeX = currentPlaneX * Math.Cos(rotationAngleInRadians) - planeY * Math.Sin(rotationAngleInRadians);

        cameraRotationY = currentCameraRotationX * Math.Sin(rotationAngleInRadians) + cameraRotationY * Math.Cos(rotationAngleInRadians);
        planeY = currentPlaneX * Math.Sin(rotationAngleInRadians) + planeY * Math.Cos(rotationAngleInRadians);
    }

    if (pressedKey.Key == ConsoleKey.A)
    {
        double currentCameraRotationX = cameraRotationX;
        cameraRotationX = currentCameraRotationX * Math.Cos(-rotationAngleInRadians) - cameraRotationY * Math.Sin(-rotationAngleInRadians);
        double currentPlaneX = planeX;
        planeX = currentPlaneX * Math.Cos(-rotationAngleInRadians) - planeY * Math.Sin(-rotationAngleInRadians);

        cameraRotationY = currentCameraRotationX * Math.Sin(-rotationAngleInRadians) + cameraRotationY * Math.Cos(-rotationAngleInRadians);
        planeY = currentPlaneX * Math.Sin(-rotationAngleInRadians) + planeY * Math.Cos(-rotationAngleInRadians);
    }
}

static char GetDrawCharByHitValue(int hitValue)
{
    return hitValue == 0 ? ' ' : '#';
}

static void DrawScreenBuffer(char[,] screenBuffer)
{
    for (int y = 0; y < screenBuffer.GetLength(0); y++)
    {
        for (int x = 0; x < screenBuffer.GetLength(1); x++)
        {
            Console.Write(screenBuffer[y, x]);
        }
        Console.WriteLine();
    }
}

static Raycaster.LineDrawData GetLineDrawData(int screenHeight, double distanceToHit)
{
    int lineHeight = (int)(screenHeight / distanceToHit);
    int unclampedStart = (screenHeight - lineHeight) / 2; //fix when lineHeight is odd number 
    int lineDrawStartRowIndex = Math.Max(0, unclampedStart);
    int lineDrawEndRowIndex = Math.Min(screenHeight, unclampedStart + lineHeight);

    return new LineDrawData(lineDrawStartRowIndex, lineDrawEndRowIndex);
}

namespace Raycaster
{
    readonly record struct LineDrawData(int LineDrawStartRowIndex, int LineDrawEndRowIndex);
}