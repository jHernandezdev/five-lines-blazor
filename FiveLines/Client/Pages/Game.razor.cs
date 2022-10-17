using System;
using System.Net.NetworkInformation;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using Blazor.Extensions.Canvas;
using Microsoft.AspNetCore.Components.Web;
using System.Reflection.Metadata;

namespace FiveLines.Client.Pages;

public partial class Game
{
    const int TILE_SIZE = 30;
    const int FPS = 30;
    const int SLEEP = 1000 / FPS;
    const string LEFT_KEY = "ArrowLeft";
    const string UP_KEY = "ArrowUp";
    const string RIGHT_KEY = "ArrowRight";
    const string DOWN_KEY = "ArrowDown";

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

    private readonly Stack<Input> inputs = new Stack<Input>();

    private void KeyDownEventHandler(KeyboardEventArgs args)
    {
        switch (args.Key)
        {
            case LEFT_KEY:
                inputs.Push(Input.LEFT);
                break;
            case RIGHT_KEY:
                inputs.Push(Input.RIGHT);
                break;
            case UP_KEY:
                inputs.Push(Input.UP);
                break;
            case DOWN_KEY:
                inputs.Push(Input.DOWN);
                break;
            default:
                break;
        }
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



    int playerx = 1;
    int playery = 1;
    Tile[][] map = new Tile[][] {
        new Tile[] { Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE},
        new Tile[] { Tile.UNBREAKABLE, Tile.PLAYER,      Tile.AIR,         Tile.FLUX,        Tile.FLUX,        Tile.UNBREAKABLE, Tile.AIR,         Tile.UNBREAKABLE},
        new Tile[] { Tile.UNBREAKABLE, Tile.STONE,       Tile.UNBREAKABLE, Tile.BOX,         Tile.FLUX,        Tile.UNBREAKABLE, Tile.AIR,         Tile.UNBREAKABLE},
        new Tile[] { Tile.UNBREAKABLE, Tile.KEY1,        Tile.STONE,       Tile.FLUX,        Tile.FLUX,        Tile.UNBREAKABLE, Tile.AIR,         Tile.UNBREAKABLE},
        new Tile[] { Tile.UNBREAKABLE, Tile.STONE,       Tile.FLUX,         Tile.FLUX,        Tile.FLUX,        Tile.LOCK1,       Tile.AIR,         Tile.UNBREAKABLE},
        new Tile[] { Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE, Tile.UNBREAKABLE}
        };

    private Canvas2DContext _context;
    protected BECanvasComponent _canvasReference;

    async Task gameLloop()
    {
        DateTime before = DateTime.Now;
        update();
        await draw();
        DateTime after = DateTime.Now;
        var frameTime = after - before;
        int sleep = SLEEP - frameTime.Milliseconds;

        var timer = new Timer(new TimerCallback(_ =>
        {
            InvokeAsync(async () =>
            {
                await gameLloop();
            });
        }), null, 100, 100);
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
        this._context = await this._canvasReference.CreateCanvas2DAsync();
        for (int y = 0; y < map.Length; y++)
        {
            for (int x = 0; x < map[y].Length; x++)
            {
                if (map[y][x] == Tile.FLUX)
                    await this._context.SetFillStyleAsync("#ccffcc");
                else if (map[y][x] == Tile.UNBREAKABLE)
                    await this._context.SetFillStyleAsync("#999999");
                else if (map[y][x] == Tile.STONE || map[y][x] == Tile.FALLING_STONE)
                    await this._context.SetFillStyleAsync("#0000cc");
                else if (map[y][x] == Tile.BOX || map[y][x] == Tile.FALLING_BOX)
                    await this._context.SetFillStyleAsync("#8b4513");
                else if (map[y][x] == Tile.KEY1 || map[y][x] == Tile.LOCK1)
                    await this._context.SetFillStyleAsync("#ffcc00");
                else if (map[y][x] == Tile.KEY2 || map[y][x] == Tile.LOCK2)
                    await this._context.SetFillStyleAsync("#00ccff");

                if (map[y][x] != Tile.AIR && map[y][x] != Tile.PLAYER)
                    await this._context.FillRectAsync(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
            }
        }

        // Draw player
        await this._context.SetFillStyleAsync("#ff0000");
        await this._context.FillRectAsync(playerx * TILE_SIZE, playery * TILE_SIZE, TILE_SIZE, TILE_SIZE);
    }
}

