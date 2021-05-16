using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Sokoban._2015
{
    
    class Grid
    {
        
        #region Parameters
        enum cellType { blank, wall, worker = 16, baggage = 17, destination = 18, workerOnDestination, baggageOnDestination };

        cellType[,] gameInfo, gameInfoOriginal;
        int width = 0;// width of the grid
        int height = 0;// height of the grid
        Point workerLocation = new Point(), workerLocationOriginal = new Point();
        int numOfDestinations = 0;
        #endregion

        #region Constuctor
        public Grid(int width, int height)
        {
            this.width = width;
            this.height = height;
            // allocate memory for the gameInfo array
            gameInfo = new cellType[width, height];
            gameInfoOriginal = new cellType[width, height];

            // initialize the gameInfo array
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    gameInfo[i, j] = cellType.blank;
                    gameInfoOriginal[i, j] = cellType.blank;
                }
        }
        #endregion

        #region Class Properties
        public int Width { get { return width; } }
        public int Height { get { return height; } }
        #endregion

        #region Class Methods
        public void Reset()
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    gameInfo[i, j] = gameInfoOriginal[i, j];
            workerLocation = new Point(workerLocationOriginal.X, workerLocationOriginal.Y);
        }
        /// <summary>
        /// Sets the cellType of the x,y location in the gamegrid
        /// </summary>
        /// <param name="x">the x-location</param>
        /// <param name="y">the y-location</param>
        /// <param name="cc">the character from the Sokoban Map</param>
        public void SetCell(int x, int y, char cc)
        {
            switch (cc)
            {
                case '#':
                    gameInfo[x, y] = cellType.wall;
                    break;
                case 'W':
                    gameInfo[x, y] = cellType.workerOnDestination;
                    workerLocation = new Point(x, y);
                    break;
                case 'w':
                    gameInfo[x, y] = cellType.worker;
                    workerLocation = new Point(x, y);
                    break;
                case 'B':
                    gameInfo[x, y] = cellType.baggageOnDestination;
                    numOfDestinations++;
                    break;
                case 'b':
                    gameInfo[x, y] = cellType.baggage;
                    break;
                case 'D':
                    gameInfo[x, y] = cellType.destination;
                    numOfDestinations++;
                    break;
                case ' ':
                    gameInfo[x, y] = cellType.blank;
                    break;
            }
            gameInfoOriginal[x, y] = gameInfo[x, y];
            workerLocationOriginal = new Point(workerLocation.X, workerLocation.Y);
        }

        /// <summary>
        /// Draw the game grid to the user window
        /// </summary>
        /// <param name="gr"> The graphics device</param>
        /// <param name="imageStrip">The  bitmap of the image pieces/components</param>
        public void Draw(Graphics gr, Bitmap imageStrip, Size CellsizeDest)
        {
            // from the image strip we can determine the cell size
            Size cellSizeSrc = new Size(imageStrip.Height, imageStrip.Height);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    // calculate the destination rectangle
                    Rectangle rectDest = new Rectangle(x * CellsizeDest.Width, y * CellsizeDest.Height, CellsizeDest.Width, CellsizeDest.Height);
                    Rectangle rectSrc;
                    switch (gameInfo[x, y])
                    {
                        case cellType.baggage:
                            rectSrc = new Rectangle((int)cellType.baggage * cellSizeSrc.Width, 0, cellSizeSrc.Width, cellSizeSrc.Height);
                            gr.DrawImage(imageStrip, rectDest, rectSrc, GraphicsUnit.Pixel);
                            break;
                        case cellType.baggageOnDestination:
                            rectSrc = new Rectangle((int)cellType.destination * cellSizeSrc.Width, 0, cellSizeSrc.Width, cellSizeSrc.Height);
                            gr.DrawImage(imageStrip, rectDest, rectSrc, GraphicsUnit.Pixel);
                            rectSrc = new Rectangle((int)cellType.baggage * cellSizeSrc.Width, 0, cellSizeSrc.Width, cellSizeSrc.Height);
                            gr.DrawImage(imageStrip, rectDest, rectSrc, GraphicsUnit.Pixel);
                            break;
                        case cellType.destination:
                            rectSrc = new Rectangle((int)cellType.destination * cellSizeSrc.Width, 0, cellSizeSrc.Width, cellSizeSrc.Height);
                            gr.DrawImage(imageStrip, rectDest, rectSrc, GraphicsUnit.Pixel);
                            break;
                        case cellType.wall:
                            rectSrc = new Rectangle(WallIndex(x, y) * cellSizeSrc.Width, 0, cellSizeSrc.Width, cellSizeSrc.Height);
                            gr.DrawImage(imageStrip, rectDest, rectSrc, GraphicsUnit.Pixel);
                            break;
                        case cellType.worker:
                            rectSrc = new Rectangle((int)cellType.worker * cellSizeSrc.Width, 0, cellSizeSrc.Width, cellSizeSrc.Height);
                            gr.DrawImage(imageStrip, rectDest, rectSrc, GraphicsUnit.Pixel);
                            break;
                        case cellType.workerOnDestination:
                            rectSrc = new Rectangle((int)cellType.destination * cellSizeSrc.Width, 0, cellSizeSrc.Width, cellSizeSrc.Height);
                            gr.DrawImage(imageStrip, rectDest, rectSrc, GraphicsUnit.Pixel);
                            rectSrc = new Rectangle((int)cellType.worker * cellSizeSrc.Width, 0, cellSizeSrc.Width, cellSizeSrc.Height);
                            gr.DrawImage(imageStrip, rectDest, rectSrc, GraphicsUnit.Pixel);
                            break;
                    }
                }

        }
        /// <summary>
        /// This method determines whether or not the x,y cell wall is bevelled on each of the four sides
        /// left = 1, bottom = 2,right = 4, top = 8
        /// </summary>
        /// <param name="x">the x-coordinate</param>
        /// <param name="y">the y-coordinate</param>
        /// <returns></returns>
        private int WallIndex(int x, int y)
        {
            int index = 0;
            // look left
            if (x == 0)
                index += 1;
            else if (gameInfo[x - 1, y] != cellType.wall)
                index += 1;
            // look down
            if (y == height - 1)
                index += 2;
            else if (gameInfo[x, y + 1] != cellType.wall)
                index += 2;
            // look right
            if (x == width - 1)
                index += 4;
            else if (gameInfo[x + 1, y] != cellType.wall)
                index += 4;
            // look up
            if (y == 0)
                index += 8;
            else if (gameInfo[x, y - 1] != cellType.wall)
                index += 8;
            //done
            return index;
        }
        public void MoveRight()
        {
            //checks to see if the worker is on a destination
            if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.workerOnDestination)
                gameInfo[workerLocation.X, workerLocation.Y] = cellType.destination;
            else
                gameInfo[workerLocation.X, workerLocation.Y] = cellType.blank;
            // trys moves the worker
            if (gameInfo[workerLocation.X + 1, workerLocation.Y] != cellType.wall)
                workerLocation.X++;
            //moves the worker if spot is blank
            if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.blank)
                gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
            // if the spot is a destination
            else if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
            // if the worker is trying to move baggage
            else if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.baggage)
                if (gameInfo[workerLocation.X + 1, workerLocation.Y] != cellType.baggage && gameInfo[workerLocation.X + 1, workerLocation.Y] != cellType.baggageOnDestination)
                {
                    int moveItems = workerLocation.X;
                    moveItems++;
                    // checks to see if the new baggage location is a wall
                    if (gameInfo[moveItems, workerLocation.Y] != cellType.wall)
                        // checks to see if the new baggage location is a destination
                        if (gameInfo[moveItems, workerLocation.Y] == cellType.destination)
                        {
                            gameInfo[moveItems, workerLocation.Y] = cellType.baggageOnDestination;
                            if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                                gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                            else
                                gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                        }
                        // not a destination or a wall
                        else
                        {
                            gameInfo[moveItems, workerLocation.Y] = cellType.baggage;
                            if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                                gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                            else
                                gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                        }
                    // 2 baggages
                    else
                    {
                        workerLocation.X--;
                        if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                        else
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                    }
                }
                    // double baggage and worker is on a destination
                else
                {
                    workerLocation.X--;
                    if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                        gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                    else
                        gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                }
            else if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.baggageOnDestination)
                if (gameInfo[workerLocation.X + 1, workerLocation.Y] != cellType.baggageOnDestination && gameInfo[workerLocation.X + 1, workerLocation.Y] != cellType.baggage)
                {
                    int moveItems = workerLocation.X;
                    moveItems++;
                    // checks to see if the new baggage location is a wall
                    if (gameInfo[moveItems, workerLocation.Y] != cellType.wall)
                        // checks to see if the new baggage location is a blank
                        if (gameInfo[moveItems, workerLocation.Y] == cellType.blank)
                        {

                            gameInfo[moveItems, workerLocation.Y] = cellType.baggage;
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                        }

                        // not a blank or a wall
                        else
                        {
                            gameInfo[moveItems, workerLocation.Y] = cellType.baggageOnDestination;
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                        }
                    // a wall
                    else
                    {
                        workerLocation.X--;
                        if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                        else
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                    }
                }
                else
                {
                    workerLocation.X--;
                    if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                        gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                    else
                        gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                }
           
        }
        public void MoveLeft()
        {
            //checks to see if the worker is on a destination
            if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.workerOnDestination)
                gameInfo[workerLocation.X, workerLocation.Y] = cellType.destination;
            else
                gameInfo[workerLocation.X, workerLocation.Y] = cellType.blank;
            // trys moves the worker
            if (gameInfo[workerLocation.X - 1, workerLocation.Y] != cellType.wall)
                workerLocation.X--;
            //moves the worker if spot is blank
            if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.blank)
                gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
            // if the spot is a destination
            else if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
            // if the worker is trying to move baggage
            else if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.baggage)
                if (gameInfo[workerLocation.X - 1, workerLocation.Y] != cellType.baggage && gameInfo[workerLocation.X - 1, workerLocation.Y] != cellType.baggageOnDestination)
                {
                    int moveItems = workerLocation.X;
                    moveItems--;
                    // checks to see if the new baggage location is a wall
                    if (gameInfo[moveItems, workerLocation.Y] != cellType.wall)
                        // checks to see if the new baggage location is a destination
                        if (gameInfo[moveItems, workerLocation.Y] == cellType.destination)
                        {
                            gameInfo[moveItems, workerLocation.Y] = cellType.baggageOnDestination;
                            if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                                gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                            else
                                gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                        }
                        // not a destination or a wall
                        else
                        {
                            gameInfo[moveItems, workerLocation.Y] = cellType.baggage;
                            if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                                gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                            else
                                gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                        }
                    // a wall
                    else
                    {
                        workerLocation.X++;
                        if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                        else
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                    }
                }
                else
                {
                    workerLocation.X++;
                    if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                        gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                    else
                        gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                }
            else if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.baggageOnDestination)
                if (gameInfo[workerLocation.X - 1, workerLocation.Y] != cellType.baggageOnDestination && gameInfo[workerLocation.X - 1, workerLocation.Y] != cellType.baggage)
                {
                    int moveItems = workerLocation.X;
                    moveItems--;
                    // checks to see if the new baggage location is a wall
                    if (gameInfo[moveItems, workerLocation.Y] != cellType.wall)
                        // checks to see if the new baggage location is a blank
                        if (gameInfo[moveItems, workerLocation.Y] == cellType.blank)
                        {

                            gameInfo[moveItems, workerLocation.Y] = cellType.baggage;
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                        }

                        // not a blank or a wall
                        else
                        {
                            gameInfo[moveItems, workerLocation.Y] = cellType.baggageOnDestination;
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                        }
                    // a wall and a worker on destination
                    else
                    {
                        workerLocation.X++;
                        if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                        else
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                    }
                }
                // double baggage and worker is on a destination
                else
                {
                    workerLocation.X++;
                    if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                        gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                    else
                    gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                }
        }
        public void MoveUp()
        {
            //checks to see if the worker is on a destination
            if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.workerOnDestination)
                gameInfo[workerLocation.X, workerLocation.Y] = cellType.destination;
            else
                gameInfo[workerLocation.X, workerLocation.Y] = cellType.blank;
            // trys moves the worker
            if (gameInfo[workerLocation.X, workerLocation.Y - 1] != cellType.wall)
                workerLocation.Y--;
            //moves the worker if spot is blank
            if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.blank)
                gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
            // if the spot is a destination
            else if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
            // if the worker is trying to move baggage
            else if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.baggage)
                if (gameInfo[workerLocation.X, workerLocation.Y - 1] != cellType.baggage && gameInfo[workerLocation.X, workerLocation.Y - 1] != cellType.baggageOnDestination)
                {
                    int moveItems = workerLocation.Y;
                    moveItems--;
                    // checks to see if the new baggage location is a wall
                    if (gameInfo[workerLocation.X, moveItems] != cellType.wall)
                        // checks to see if the new baggage location is a destination
                        if (gameInfo[workerLocation.X, moveItems] == cellType.destination)
                        {
                            gameInfo[workerLocation.X, moveItems] = cellType.baggageOnDestination;
                            if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                                gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                            else
                                gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                        }
                        // not a destination or a wall
                        else
                        {
                            gameInfo[workerLocation.X, moveItems] = cellType.baggage;
                            if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                                gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                            else
                                gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                        }
                    // a wall
                    else
                    {
                        workerLocation.Y++;
                        if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                        else
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                    }
                }
                else
                {
                    workerLocation.Y++;
                    if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                        gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                    else
                        gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                }
            else if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.baggageOnDestination)
                if (gameInfo[workerLocation.X, workerLocation.Y - 1] != cellType.baggageOnDestination && gameInfo[workerLocation.X, workerLocation.Y - 1] != cellType.baggage)
                {
                    int moveItems = workerLocation.Y;
                    moveItems--;
                    // checks to see if the new baggage location is a wall
                    if (gameInfo[workerLocation.X, moveItems] != cellType.wall)
                        // checks to see if the new baggage location is a blank
                        if (gameInfo[workerLocation.X, moveItems] == cellType.blank)
                        {

                            gameInfo[workerLocation.X, moveItems] = cellType.baggage;
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                        }

                        // not a blank or a wall
                        else
                        {
                            gameInfo[workerLocation.X, moveItems] = cellType.baggageOnDestination;
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                        }
                    // a wall and worker on the destination
                    else
                    {
                        workerLocation.Y++;
                        if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                        else
                            
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                    }
                }
                    // double baggage and worker on destination
                else
                {
                    workerLocation.Y++;
                    if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                        gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                    else
                    gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                }
        }
        public void MoveDown()
        {
            //checks to see if the worker is on a destination
            if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.workerOnDestination)
                gameInfo[workerLocation.X, workerLocation.Y] = cellType.destination;
            else
                gameInfo[workerLocation.X, workerLocation.Y] = cellType.blank;
            // trys moves the worker
            if (gameInfo[workerLocation.X, workerLocation.Y + 1] != cellType.wall)
                workerLocation.Y++;
            //moves the worker if spot is blank
            if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.blank)
                gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
            // if the spot is a destination
            else if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
            // if the worker is trying to move baggage
            else if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.baggage)
                if (gameInfo[workerLocation.X, workerLocation.Y + 1] != cellType.baggage && gameInfo[workerLocation.X, workerLocation.Y + 1] != cellType.baggageOnDestination)
                {
                    int moveItems = workerLocation.Y;
                    moveItems++;
                    // checks to see if the new baggage location is a wall
                    if (gameInfo[workerLocation.X, moveItems] != cellType.wall)
                        // checks to see if the new baggage location is a destination
                        if (gameInfo[workerLocation.X, moveItems] == cellType.destination)
                        {
                            gameInfo[workerLocation.X, moveItems] = cellType.baggageOnDestination;
                            if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                                gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                            else
                                gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                        }
                        // not a destination or a wall
                        else
                        {
                            gameInfo[workerLocation.X, moveItems] = cellType.baggage;
                            if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                                gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                            else
                                gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                        }
                    // a wall
                    else
                    {
                            workerLocation.Y--;
                            if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                                gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                            else
                                gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                    }
                }
                else
                {
                    workerLocation.Y--;
                    if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                        gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                    else
                        gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                }
            else if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.baggageOnDestination)
                if (gameInfo[workerLocation.X, workerLocation.Y + 1] != cellType.baggageOnDestination && gameInfo[workerLocation.X, workerLocation.Y + 1] != cellType.baggage)
                {
                    int moveItems = workerLocation.Y;
                    moveItems++;
                    // checks to see if the new baggage location is a wall
                    if (gameInfo[workerLocation.X, moveItems] != cellType.wall)
                        // checks to see if the new baggage location is a blank
                        if (gameInfo[workerLocation.X, moveItems] == cellType.blank)
                        {

                            gameInfo[workerLocation.X, moveItems] = cellType.baggage;
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                        }

                        // not a blank or a wall
                        else
                        {
                            gameInfo[workerLocation.X, moveItems] = cellType.baggageOnDestination;
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                        }
                    // a wall
                    else
                    {
                        workerLocation.Y--;
                        if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                        else
                            gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                    }
                }
                else
                {
                    workerLocation.Y--;
                    if (gameInfo[workerLocation.X, workerLocation.Y] == cellType.destination)
                        gameInfo[workerLocation.X, workerLocation.Y] = cellType.workerOnDestination;
                    else
                        gameInfo[workerLocation.X, workerLocation.Y] = cellType.worker;
                }
        }
        public bool Win()
        {
            bool wincheck = true;
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    if (gameInfo[i, j] == cellType.baggage)
                        wincheck = false;
            return wincheck;
        }
        #endregion
    }
}
