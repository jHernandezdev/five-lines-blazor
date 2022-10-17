using FiveLines.Client.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace FiveLines.Client.Pages;

public partial class Game
{
    const int TILE_SIZE = 30;
    const int FPS = 30;
    const long SLEEP = 1000 / FPS;
    const string LEFT_KEY = "ArrowLeft";
    const string UP_KEY = "ArrowUp";
    const string RIGHT_KEY = "ArrowRight";
    const string DOWN_KEY = "ArrowDown";
    const string CANVAS_ID = "GameCanvas";

    enum Input
    {
        UP, DOWN, LEFT, RIGHT
    }
    enum Tile
    {
        AIR = 0,
        FLUX,
        UNBREAKABLE,
        PLAYER,
        STONE, FALLING_STONE,
        BOX, FALLING_BOX,
        KEY1, LOCK1,
        KEY2, LOCK2
    };

    private readonly Stack<Input> inputs = new();

    int playerx = 1;
    int playery = 1;
    readonly Tile[][] map = new Tile[][] {
        new Tile[] { Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE},
        new Tile[] { Tile.UNBREAKABLE, Tile.PLAYER,      Tile.AIR,         Tile.FLUX,        Tile.FLUX,        Tile.UNBREAKABLE, Tile.AIR,         Tile.UNBREAKABLE},
        new Tile[] { Tile.UNBREAKABLE, Tile.STONE,       Tile.UNBREAKABLE, Tile.BOX,         Tile.FLUX,        Tile.UNBREAKABLE, Tile.AIR,         Tile.UNBREAKABLE},
        new Tile[] { Tile.UNBREAKABLE, Tile.KEY1,        Tile.STONE,       Tile.FLUX,        Tile.FLUX,        Tile.UNBREAKABLE, Tile.AIR,         Tile.UNBREAKABLE},
        new Tile[] { Tile.UNBREAKABLE, Tile.STONE,       Tile.FLUX,         Tile.FLUX,        Tile.FLUX,        Tile.LOCK1,       Tile.AIR,         Tile.UNBREAKABLE},
        new Tile[] { Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE}
        };
    [Inject]
    IJSRuntime jSRuntime { get; set; }
        
    public void KeyDownEventHandler(KeyboardEventArgs args)
    {
        if (args.Key == LEFT_KEY || args.Key == "a") inputs.Push(Input.LEFT);
        else if (args.Key == UP_KEY || args.Key == "w") inputs.Push(Input.UP);
        else if (args.Key == RIGHT_KEY || args.Key == "d") inputs.Push(Input.RIGHT);
        else if (args.Key == DOWN_KEY || args.Key == "s") inputs.Push(Input.DOWN);        
    }

    protected override async void OnAfterRender(bool firstRender)
    {
        // execute conditionally for loading data, otherwise this will load
        // every time the page refreshes
        if (firstRender)
        {
            await gameLloop();
        }
    }
   
    async Task gameLloop()
    {
        long before = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        update();
        await draw();
        long after = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        long frameTime = after - before;
        long sleep = SLEEP - frameTime;

        var timer = new Timer(new TimerCallback(_ =>
        {
            InvokeAsync(async () =>
            {
                await gameLloop();
            });
        }), null, sleep, sleep);
    }

    private void remove(Tile tile)
    {
        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                if (map[y][x] == tile)
                {
                    map[y][x] = Tile.AIR;
                }
            }
        }
    }

    private void moveToTile(int newx, int newy)
    {
        map[playery][playerx] = Tile.AIR;
        map[newy][newx] = Tile.PLAYER;
        playerx = newx;
        playery = newy;
    }

    private void moveHorizontal(int dx)
    {
        if (map[playery][playerx + dx] == Tile.FLUX
          || map[playery][playerx + dx] == Tile.AIR)
        {
            moveToTile(playerx + dx, playery);
        }
        else if ((map[playery][playerx + dx] == Tile.STONE
          || map[playery][playerx + dx] == Tile.BOX)
          && map[playery][playerx + dx + dx] == Tile.AIR
          && map[playery + 1][playerx + dx] != Tile.AIR)
        {
            map[playery][playerx + dx + dx] = map[playery][playerx + dx];
            moveToTile(playerx + dx, playery);
        }
        else if (map[playery][playerx + dx] == Tile.KEY1)
        {
            remove(Tile.LOCK1);
            moveToTile(playerx + dx, playery);
        }
        else if (map[playery][playerx + dx] == Tile.KEY2)
        {
            remove(Tile.LOCK2);
            moveToTile(playerx + dx, playery);
        }
    }

    private void moveVertical(int dy)
    {
        if (map[playery + dy][playerx] == Tile.FLUX
          || map[playery + dy][playerx] == Tile.AIR)
        {
            moveToTile(playerx, playery + dy);
        }
        else if (map[playery + dy][playerx] == Tile.KEY1)
        {
            remove(Tile.LOCK1);
            moveToTile(playerx, playery + dy);
        }
        else if (map[playery + dy][playerx] == Tile.KEY2)
        {
            remove(Tile.LOCK2);
            moveToTile(playerx, playery + dy);
        }
    }

    private void update()
    {
        while (inputs.Count > 0)
        {
            Input current = inputs.Pop();
            if (current == Input.LEFT)
                moveHorizontal(-1);
            else if (current == Input.RIGHT)
                moveHorizontal(1);
            else if (current == Input.UP)
                moveVertical(-1);
            else if (current == Input.DOWN)
                moveVertical(1);
        }

        for (int y = map.Length - 1; y >= 0; y--)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                if ((map[y][x] == Tile.STONE || map[y][x] == Tile.FALLING_STONE)
                  && map[y + 1][x] == Tile.AIR)
                {
                    map[y + 1][x] = Tile.FALLING_STONE;
                    map[y][x] = Tile.AIR;
                }
                else if ((map[y][x] == Tile.BOX || map[y][x] == Tile.FALLING_BOX)
                  && map[y + 1][x] == Tile.AIR)
                {
                    map[y + 1][x] = Tile.FALLING_BOX;
                    map[y][x] = Tile.AIR;
                }
                else if (map[y][x] == Tile.FALLING_STONE)
                {
                    map[y][x] = Tile.STONE;
                }
                else if (map[y][x] == Tile.FALLING_BOX)
                {
                    map[y][x] = Tile.BOX;
                }
            }
        }
    }

    async Task draw()
    {
        CanvasJSHelper canvas = CanvasJSHelper.CreateCanvasJSHelper(CANVAS_ID, jSRuntime);
        await canvas.ClearCanvasRectAsync();

        // draw map
        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                if (map[y][x] == Tile.FLUX)
                    canvas.FillStyle = "#ccffcc";
                else if (map[y][x] == Tile.UNBREAKABLE)
                    canvas.FillStyle = "#999999";
                else if (map[y][x] == Tile.STONE || map[y][x] == Tile.FALLING_STONE)
                    canvas.FillStyle = "#0000cc";
                else if (map[y][x] == Tile.BOX || map[y][x] == Tile.FALLING_BOX)
                    canvas.FillStyle = "#8b4513";
                else if (map[y][x] == Tile.KEY1 || map[y][x] == Tile.LOCK1)
                    canvas.FillStyle = "#ffcc00";
                else if (map[y][x] == Tile.KEY2 || map[y][x] == Tile.LOCK2)
                    canvas.FillStyle = "#00ccff";

                if (map[y][x] != Tile.AIR && map[y][x] != Tile.PLAYER)
                    await canvas.FillRectAsync(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
            }
        }

        // Draw player
        canvas.FillStyle = "#ff0000";
        await canvas.FillRectAsync(playerx * TILE_SIZE, playery * TILE_SIZE, TILE_SIZE, TILE_SIZE);
    }
}

