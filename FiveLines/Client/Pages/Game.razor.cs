using FiveLines.Client.Helpers;
using FiveLines.Client.Model;
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

    private readonly Stack<Input> inputs = new();

    int playerx = 1;
    int playery = 1;
    readonly ITile[][] map = new ITile[][] {
        new ITile[] { new Unbreakable(), new Unbreakable(), new Unbreakable(), new Unbreakable(), new Unbreakable(), new Unbreakable(), new Unbreakable(), new Unbreakable()},
        new ITile[] { new Unbreakable(), new Player(),      new Air(),         new Flux(),        new Flux(),        new Unbreakable(), new Air(),         new Unbreakable()},
        new ITile[] { new Unbreakable(), new Stone(),       new Unbreakable(), new Box(),         new Flux(),        new Unbreakable(), new Air(),         new Unbreakable()},
        new ITile[] { new Unbreakable(), new Key1(),        new Stone(),       new Flux(),        new Flux(),        new Unbreakable(), new Air(),         new Unbreakable()},
        new ITile[] { new Unbreakable(), new Stone(),       new Flux(),        new Flux(),        new Flux(),        new Lock1(),       new Air(),         new Unbreakable()},
        new ITile[] { new Unbreakable(), new Unbreakable(), new Unbreakable(), new Unbreakable(), new Unbreakable(), new Unbreakable(), new Unbreakable(), new Unbreakable()}
        };
    [Inject]
    IJSRuntime? jSRuntime { get; set; }
        
    public void KeyDownEventHandler(KeyboardEventArgs args)
    {
        if (args.Key == LEFT_KEY || args.Key == "a") inputs.Push(new Left(moveHorizontal));
        else if (args.Key == UP_KEY || args.Key == "w") inputs.Push(new Up(moveVertical));
        else if (args.Key == RIGHT_KEY || args.Key == "d") inputs.Push(new Right(moveHorizontal));
        else if (args.Key == DOWN_KEY || args.Key == "s") inputs.Push(new Down(moveVertical));        
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
   
    private async Task gameLloop()
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

    private void removeLock1()
    {
        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                if (map[y][x].isLock1())
                {
                    map[y][x] = new Air();
                }
            }
        }
    }

    private void removeLock2()
    {
        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                if (map[y][x].isLock2())
                {
                    map[y][x] = new Air();
                }
            }
        }
    }

    private void moveToTile(int newx, int newy)
    {
        map[playery][playerx] = new Air();
        map[newy][newx] = new Player();
        playerx = newx;
        playery = newy;
    }

    private void moveHorizontal(int dx)
    {
        if (map[playery][playerx + dx].isEdible())
        {
            moveToTile(playerx + dx, playery);
        }
        else if (map[playery][playerx + dx].isPushable() && map[playery][playerx + dx + dx].isAir() && !map[playery + 1][playerx + dx].isAir())
        {
            map[playery][playerx + dx + dx] = map[playery][playerx + dx];
            moveToTile(playerx + dx, playery);
        }
        else if (map[playery][playerx + dx].isKey1())
        {
            removeLock1();
            moveToTile(playerx + dx, playery);
        }
        else if (map[playery][playerx + dx].isKey2())
        {
            removeLock2();
            moveToTile(playerx + dx, playery);
        }
    }

    private void moveVertical(int dy)
    {
        if (map[playery + dy][playerx].isFlux() || map[playery + dy][playerx].isAir())
        {
            moveToTile(playerx, playery + dy);
        }
        else if (map[playery + dy][playerx].isKey1())
        {
            removeLock1();
            moveToTile(playerx, playery + dy);
        }
        else if (map[playery + dy][playerx].isKey2())
        {
            removeLock2();
            moveToTile(playerx, playery + dy);
        }
    }

    private void update()
    {
        handlerInputs();
        updateMap();
    }
    private void updateMap()
    {
        for (int y = map.Length - 1; y >= 0; y--)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                updateTile(y, x);
            }
        }
    }
    private void updateTile(int y, int x)
    {
        if ((map[y][x].isStone() || map[y][x].isFallingStone()) && map[y + 1][x].isAir())
        {
            map[y + 1][x] = new FallingStone();
            map[y][x] = new Air();
        }
        else if ((map[y][x].isBox() || map[y][x].isFallingBox()) && map[y + 1][x].isAir())
        {
            map[y + 1][x] = new FallingBox();
            map[y][x] = new Air();
        }
        else if (map[y][x].isFallingStone())
        {
            map[y][x] = new Stone();
        }
        else if (map[y][x].isFallingBox())
        {
            map[y][x] = new Box();
        }
    }

    private void handlerInputs()
    {
        while (inputs.Count > 0)
        {
            Input current = inputs.Pop();
            current.Handle();
        }
    }    

    async Task draw()
    {
        CanvasJSHelper canvas = await createGraphics();
        await drawMap(canvas);
        await drawPlayer(canvas);
    }
    private async Task<CanvasJSHelper> createGraphics()
    {
        CanvasJSHelper result = CanvasJSHelper.CreateCanvasJSHelper(CANVAS_ID, jSRuntime);
        await result.ClearCanvasRectAsync();
        return result;
    }
    private async Task drawPlayer(CanvasJSHelper canvas)
    {
        canvas.FillStyle = "#ff0000";
        await canvas.FillRectAsync(playerx * TILE_SIZE, playery * TILE_SIZE, TILE_SIZE, TILE_SIZE);
    }
    private async Task drawMap(CanvasJSHelper canvas)
    {
        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                await map[y][x].DrawAsync(canvas, x, y);
             
            }
        }
    }
}

