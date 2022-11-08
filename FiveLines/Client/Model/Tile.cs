using System;
using FiveLines.Client.Helpers;

namespace FiveLines.Client.Model
{


	public interface ITile
	{
        public const int TILE_SIZE = 30;

        Task DrawAsync(CanvasJSHelper canvas, int x, int y);
        bool isEdible();
        bool isPushable();

        bool isAir();
        bool isFlux();
        bool isUnbreakable();
        bool isPlayer();

        bool isStone();
        bool isFallingStone();
        bool isBox();
        bool isFallingBox();

        bool isKey1();
        bool isLock1();
        bool isKey2();
        bool isLock2();
    }

    public class Air : ITile
    {
        public Task DrawAsync(CanvasJSHelper canvas, int x, int y)
        {
            return Task.CompletedTask;
        }
        public bool isEdible()
        {
            return true;
        }
        public bool isPushable()
        {
            return false;
        }

        public bool isAir() => true;
        public bool isBox() => false;
        public bool isFallingBox() => false;
        public bool isFallingStone() => false;
        public bool isFlux() => false;
        public bool isKey1() => false;
        public bool isKey2() => false;
        public bool isLock1() => false;
        public bool isLock2() => false;
        public bool isPlayer() => false;
        public bool isStone() => false;
        public bool isUnbreakable() => false;
    }
    public class Box : ITile
    {        
        public async Task DrawAsync(CanvasJSHelper canvas, int x, int y)
        {
            canvas.FillStyle = "#8b4513";
            await canvas.FillRectAsync(x * ITile.TILE_SIZE, y * ITile.TILE_SIZE, ITile.TILE_SIZE, ITile.TILE_SIZE);
        }
        public bool isEdible()
        {
            return false;
        }
        public bool isPushable()
        {
            return true;
        }

        public bool isAir() => false;
        public bool isBox() => true;
        public bool isFallingBox() => false;
        public bool isFallingStone() => false;
        public bool isFlux() => false;
        public bool isKey1() => false;
        public bool isKey2() => false;
        public bool isLock1() => false;
        public bool isLock2() => false;
        public bool isPlayer() => false;
        public bool isStone() => false;
        public bool isUnbreakable() => false;
    }
    public class FallingBox : ITile
    {
        public async Task DrawAsync(CanvasJSHelper canvas, int x, int y)
        {
            canvas.FillStyle = "#8b4513";
            await canvas.FillRectAsync(x * ITile.TILE_SIZE, y * ITile.TILE_SIZE, ITile.TILE_SIZE, ITile.TILE_SIZE);
        }
        public bool isEdible()
        {
            return false;
        }
        public bool isPushable()
        {
            return false;
        }

        public bool isAir() => false;
        public bool isBox() => false;
        public bool isFallingBox() => true;
        public bool isFallingStone() => false;
        public bool isFlux() => false;
        public bool isKey1() => false;
        public bool isKey2() => false;
        public bool isLock1() => false;
        public bool isLock2() => false;
        public bool isPlayer() => false;
        public bool isStone() => false;
        public bool isUnbreakable() => false;
    }
    public class FallingStone : ITile
    {
        public async Task DrawAsync(CanvasJSHelper canvas, int x, int y)
        {
            canvas.FillStyle = "#0000cc";
            await canvas.FillRectAsync(x * ITile.TILE_SIZE, y * ITile.TILE_SIZE, ITile.TILE_SIZE, ITile.TILE_SIZE);
        }
        public bool isEdible()
        {
            return false;
        }
        public bool isPushable()
        {
            return false;
        }

        public bool isAir() => false;
        public bool isBox() => false;
        public bool isFallingBox() => false;
        public bool isFallingStone() => true;
        public bool isFlux() => false;
        public bool isKey1() => false;
        public bool isKey2() => false;
        public bool isLock1() => false;
        public bool isLock2() => false;
        public bool isPlayer() => false;
        public bool isStone() => false;
        public bool isUnbreakable() => false;
    }
    public class Flux : ITile
    {
        public async Task DrawAsync(CanvasJSHelper canvas, int x, int y)
        {
            canvas.FillStyle = "#ccffcc";
            await canvas.FillRectAsync(x * ITile.TILE_SIZE, y * ITile.TILE_SIZE, ITile.TILE_SIZE, ITile.TILE_SIZE);
        }
        public bool isEdible()
        {
            return  true;
        }
        public bool isPushable()
        {
            return false;
        }

        public bool isAir() => false;
        public bool isBox() => false;
        public bool isFallingBox() => false;
        public bool isFallingStone() => false;
        public bool isFlux() => true;
        public bool isKey1() => false;
        public bool isKey2() => false;
        public bool isLock1() => false;
        public bool isLock2() => false;
        public bool isPlayer() => false;
        public bool isStone() => false;
        public bool isUnbreakable() => false;
    }
    public class Key1 : ITile
    {
        public async Task DrawAsync(CanvasJSHelper canvas, int x, int y)
        {
            canvas.FillStyle = "#ffcc00";
            await canvas.FillRectAsync(x * ITile.TILE_SIZE, y * ITile.TILE_SIZE, ITile.TILE_SIZE, ITile.TILE_SIZE);
        }
        public bool isEdible()
        {
            return false;
        }
        public bool isPushable()
        {
            return false;
        }

        public bool isAir() => false;
        public bool isBox() => false;
        public bool isFallingBox() => false;
        public bool isFallingStone() => false;
        public bool isFlux() => false;
        public bool isKey1() => true;
        public bool isKey2() => false;
        public bool isLock1() => false;
        public bool isLock2() => false;
        public bool isPlayer() => false;
        public bool isStone() => false;
        public bool isUnbreakable() => false;
    }
    public class Key2 : ITile
    {
        public async Task DrawAsync(CanvasJSHelper canvas, int x, int y)
        {
            canvas.FillStyle = "#00ccff";
            await canvas.FillRectAsync(x * ITile.TILE_SIZE, y * ITile.TILE_SIZE, ITile.TILE_SIZE, ITile.TILE_SIZE);
        }
        public bool isEdible()
        {
            return false;
        }
        public bool isPushable()
        {
            return false;
        }

        public bool isAir() => false;
        public bool isBox() => false;
        public bool isFallingBox() => false;
        public bool isFallingStone() => false;
        public bool isFlux() => false;
        public bool isKey1() => false;
        public bool isKey2() => true;
        public bool isLock1() => false;
        public bool isLock2() => false;
        public bool isPlayer() => false;
        public bool isStone() => false;
        public bool isUnbreakable() => false;
    }
    public class Lock1 : ITile
    {
        public async Task DrawAsync(CanvasJSHelper canvas, int x, int y)
        {
            canvas.FillStyle = "#ffcc00";
            await canvas.FillRectAsync(x * ITile.TILE_SIZE, y * ITile.TILE_SIZE, ITile.TILE_SIZE, ITile.TILE_SIZE);
        }
        public bool isEdible()
        {
            return false;
        }
        public bool isPushable()
        {
            return false;
        }

        public bool isAir() => false;
        public bool isBox() => false;
        public bool isFallingBox() => false;
        public bool isFallingStone() => false;
        public bool isFlux() => false;
        public bool isKey1() => false;
        public bool isKey2() => false;
        public bool isLock1() => true;
        public bool isLock2() => false;
        public bool isPlayer() => false;
        public bool isStone() => false;
        public bool isUnbreakable() => false;
    }
    public class Lock2 : ITile
    {
        public async Task DrawAsync(CanvasJSHelper canvas, int x, int y)
        {
            canvas.FillStyle = "#00ccff";
            await canvas.FillRectAsync(x * ITile.TILE_SIZE, y * ITile.TILE_SIZE, ITile.TILE_SIZE, ITile.TILE_SIZE);
        }
        public bool isEdible()
        {
            return false;
        }
        public bool isPushable()
        {
            return false;
        }

        public bool isAir() => false;
        public bool isBox() => false;
        public bool isFallingBox() => false;
        public bool isFallingStone() => false;
        public bool isFlux() => false;
        public bool isKey1() => false;
        public bool isKey2() => false;
        public bool isLock1() => false;
        public bool isLock2() => true;
        public bool isPlayer() => false;
        public bool isStone() => false;
        public bool isUnbreakable() => false;
    }
    public class Player : ITile
    {
        public Task DrawAsync(CanvasJSHelper canvas, int x, int y)
        {
            return Task.CompletedTask;
        }
        public bool isEdible()
        {
            return false;
        }
        public bool isPushable()
        {
            return false;
        }

        public bool isAir() => false;
        public bool isBox() => false;
        public bool isFallingBox() => false;
        public bool isFallingStone() => false;
        public bool isFlux() => false;
        public bool isKey1() => false;
        public bool isKey2() => false;
        public bool isLock1() => false;
        public bool isLock2() => false;
        public bool isPlayer() => true;
        public bool isStone() => false;
        public bool isUnbreakable() => false;
    }
    public class Stone : ITile
    {
        public async Task DrawAsync(CanvasJSHelper canvas, int x, int y)
        {
            canvas.FillStyle = "#0000cc";
            await canvas.FillRectAsync(x * ITile.TILE_SIZE, y * ITile.TILE_SIZE, ITile.TILE_SIZE, ITile.TILE_SIZE);
        }
        public bool isEdible()
        {
            return false;
        }
        public bool isPushable()
        {
            return true;
        }

        public bool isAir() => false;
        public bool isBox() => false;
        public bool isFallingBox() => false;
        public bool isFallingStone() => false;
        public bool isFlux() => false;
        public bool isKey1() => false;
        public bool isKey2() => false;
        public bool isLock1() => false;
        public bool isLock2() => false;
        public bool isPlayer() => false;
        public bool isStone() => true;
        public bool isUnbreakable() => false;
    }
    public class Unbreakable : ITile
    {
        public async Task DrawAsync(CanvasJSHelper canvas, int x, int y)
        {
            canvas.FillStyle = "#999999";
            await canvas.FillRectAsync(x * ITile.TILE_SIZE, y * ITile.TILE_SIZE, ITile.TILE_SIZE, ITile.TILE_SIZE);
        }
        public bool isEdible()
        {
            return false;
        }
        public bool isPushable()
        {
            return false;
        }

        public bool isAir() => false;
        public bool isBox() => false;
        public bool isFallingBox() => false;
        public bool isFallingStone() => false;
        public bool isFlux() => false;
        public bool isKey1() => false;
        public bool isKey2() => false;
        public bool isLock1() => false;
        public bool isLock2() => false;
        public bool isPlayer() => false;
        public bool isStone() => false;
        public bool isUnbreakable() => true;
    }
}

