using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Reflection;

namespace Sokoban._2015
{
    public partial class Form1 : Form
    {
        // set up the various game parameters

        Size cellSize = new Size();
        Size gridSize = new Size();
        Grid gameGrid = null;
        Bitmap images = new Bitmap(typeof(Form1), "SokobanImages.png");
        ArrayList gameGrids = new ArrayList();
        int gridnum = 1, NumReset = 0;
        public Form1()
        {
            InitializeComponent();

            // set the cell size of the graphic
            cellSize = new Size(images.Height, images.Height);
            // used to automaticly open the file
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Sokoban._2015.sokoban_maps2.sbn";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                ReadFile(reader);
                Setup_Grid(gridnum);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Sokoban (c) JH Hiebert, 2015", "About...");
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // open a sokoban map file and read the contents
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamReader fileInput = new StreamReader(openFileDialog1.FileName);
                ReadFile(fileInput);

                // close the file
                fileInput.Close();

                // setup the window form to be the right size
                Setup_Grid(gridnum);
            }
        }
        private void ReadFile(StreamReader fileInput)
        {
            char[] lineSplitter = new char[] { ',', ' ' }; // to split lines with spaces and commas

            // read in the file data
            while (!fileInput.EndOfStream)
            {
                // read in one line
                string lineOfText = fileInput.ReadLine();
                if (lineOfText[0] == '-')//ignore a line beginning with a dash
                    continue;

                string[] data = lineOfText.Split(lineSplitter, StringSplitOptions.RemoveEmptyEntries);
                gridSize = new Size(Convert.ToInt16(data[0]), Convert.ToInt16(data[1]));
                gameGrid = new Grid(gridSize.Width, gridSize.Height);

                for (int row = 0; row < gridSize.Height; row++)
                {
                    lineOfText = fileInput.ReadLine();
                    // parse the entire line, one charater at a time
                    for (int x = 0; x < lineOfText.Length; x++)
                        gameGrid.SetCell(x, row, lineOfText[x]);
                }
                gameGrids.Add(gameGrid);
            }
        }
        private void Setup_Grid(int gridnum)
        {
            gameGrid = (Grid)gameGrids[gridnum];
            gameGrid.Reset();
            this.ClientSize = new Size(gameGrid.Width * cellSize.Width, gameGrid.Height * cellSize.Height + menuStrip1.Height +statusStrip1.Height);
            //changes the size of the cells look in Draw(cellsizeDest and cellsizeSrc
            if (ClientSize.Height > 700 + menuStrip1.Height)
            {
                cellSize.Height = 700 / gameGrid.Height;
                cellSize.Width = cellSize.Height;
                this.ClientSize = new Size(gameGrid.Width * cellSize.Width, gameGrid.Height * cellSize.Height + menuStrip1.Height + statusStrip1.Height);

            }
            this.CenterToScreen();
            toolStripStatusLabel1.Text = ("Map #" + gridnum + " resets:" + NumReset);
            this.Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(0, menuStrip1.Height);
            if (gameGrid != null)
                gameGrid.Draw(e.Graphics, images, cellSize);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // changes the map being played
            if (e.KeyCode == Keys.PageUp && gridnum < gameGrids.Count -1)
                Setup_Grid(++gridnum);
            if (e.KeyCode == Keys.PageDown && gridnum > 0)
                Setup_Grid(--gridnum);
            if (e.KeyCode == Keys.Right)
                gameGrid.MoveRight();
            if (e.KeyCode == Keys.Left)
                gameGrid.MoveLeft();
            if (e.KeyCode == Keys.Up)
                gameGrid.MoveUp();
            if (e.KeyCode == Keys.Down)
                gameGrid.MoveDown();
            if (e.KeyCode == Keys.R)
                Reset();
            // update graphics
            this.Invalidate();
            if (gameGrid.Win() && gridnum != 0)
            {
                MessageBox.Show("You win!", "Winner");
                Setup_Grid(++gridnum);
            }
        }
        /// <summary>
        /// reset the maps
        /// </summary>
        private void Reset()
        {
            NumReset++;
            toolStripStatusLabel1.Text = ("Map #" + gridnum + " resets:" + NumReset);

            gameGrid.Reset();
            this.ClientSize = new Size(gameGrid.Width * cellSize.Width, gameGrid.Height * cellSize.Height + menuStrip1.Height + statusStrip1.Height);
            this.CenterToScreen();
            this.Invalidate();
        }
    }
}
