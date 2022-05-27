using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLife
{
    public partial class Form1
    {
        const int mapSize = 30;
        const int cellSize = 30;

        int[,] currentState = new int[mapSize, mapSize];
        int[,] nextState = new int[mapSize, mapSize];

        Button[,] cells = new Button[mapSize, mapSize];

        bool isPlaying = false;

        Timer mainTimer;

        int offset = 25;

        void SetFormSize()
        {
            this.Width = (mapSize + 1) * cellSize;
            this.Height = (mapSize + 1) * cellSize + 40;
        }

        public void Init()
        {
            isPlaying = false;
            mainTimer = new Timer
            {
                Interval = 100
            };
            mainTimer.Tick += new EventHandler(UpdateStates);
            currentState = InitMap();
            nextState = InitMap();
            InitCells();
        }

        void ClearGame()
        {
            isPlaying = false;
            mainTimer = new Timer
            {
                Interval = 100
            };
            mainTimer.Tick += new EventHandler(UpdateStates);
            currentState = InitMap();
            nextState = InitMap();
            ResetCells();
        }

        void ResetCells()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    cells[i, j].BackColor = Color.White;
                }
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Игра «Жизнь» происходит на клеточном поле, которое, традиционно, называется «вселенная»." + "\n"+
                "Каждая клетка может быть живой или мёртвой." +"\n"+
                "Поколения сменяются синхронно по простым правилам:" +"\n"+
                "1) в пустой (мёртвой) клетке, рядом с которой ровно три живые клетки, зарождается жизнь;" + "\n" +
                "2) если у живой клетки есть две или три живые соседки, то эта клетка продолжает жить; " + "\n" +
                "3) в противном случае (если соседей меньше двух или больше трёх) клетка умирает («от одиночества» или «от перенаселённости»).");
        }
        public int count = 1;
        public Label label = new Label();

        public Timer timer = new Timer();

        private void timer_Tick(object sender, EventArgs e)
        {
            count++;
            label.Text = count.ToString();
        }

        void BuildMenu()
        {
            var menu = new MenuStrip();

            var label2 = new Label
            {
                Text = "Поколение",
                Location = new Point(950, 450),
                Font = new Font("",24,FontStyle.Bold),
                Size = new Size(200,150)
                
            };

            label.Text = "0";
            label.Size = new Size(200, 150);
            label.Font = new Font("", 24, FontStyle.Bold);
            label.Location = new Point(1025, 500);

            Controls.Add(label);
            Controls.Add(label2);
            timer = new Timer
            {
                Interval = 100
            };

            timer.Tick += timer_Tick;


            var restart = new ToolStripMenuItem("Начать заново");
            restart.Click += new EventHandler(Restart);

            var play = new ToolStripMenuItem("Начать");
            play.Click += new EventHandler(Play);

            var stop = new ToolStripMenuItem("Стоп");
            stop.Click += new EventHandler(Stop);

            var resume = new ToolStripMenuItem("Продолжить");
            resume.Click += new EventHandler(Resume);

            var button = new Button();
            Controls.Add(button);

            button.Name = "Правила";
            button.Text = "Правила";
            button.Location = new Point(950, 100);
            button.Size = new Size(200, 100);
            button.Font = new Font("", 18, FontStyle.Bold);
            button.Click += button_Click;

            menu.Items.Add(play);
            menu.Items.Add(restart);
            menu.Items.Add(stop);
            menu.Items.Add(resume);

            this.WindowState = FormWindowState.Maximized;


            this.Controls.Add(menu);
        }

        private void Resume(object sender, EventArgs e)
        {
            mainTimer.Start();
            timer.Start();
        }

        private void Stop(object sender, EventArgs e)
        {
            mainTimer.Stop();
            timer.Stop();
        }

        private void Restart(object sender, EventArgs e)
        {
            mainTimer.Stop();
            ClearGame();
            count = 0;
            label.Text = "0";
            timer.Stop();
        }

        private void Play(object sender, EventArgs e)
        {
            if (!isPlaying)
            {
                isPlaying = true;
                mainTimer.Start();
                timer.Start();
            }
        }

        private void UpdateStates(object sender, EventArgs e)
        {
            CalculateNextState();
            DisplayMap();
            
            if (CheckGenerationDead())
            {
                mainTimer.Stop();
                timer.Stop();
                MessageBox.Show("Все клетки погибли"+"\n"+"\n"+"Последнее поколение:" + "\n" +
                    count.ToString());
            }
        }

        bool CheckGenerationDead()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (currentState[i, j] == 1)
                        return false;
                }
            }
            return true;
        }

        void CalculateNextState()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    var countNeighboors = CountNeighboors(i, j);
                    if (currentState[i, j] == 0 && countNeighboors == 3)
                    {
                        nextState[i, j] = 1;
                    }
                    else if (currentState[i, j] == 1 && (countNeighboors < 2 && countNeighboors > 3))
                    {
                        nextState[i, j] = 0;
                    }
                    else if (currentState[i, j] == 1 && (countNeighboors >= 2 && countNeighboors <= 3))
                    {
                        nextState[i, j] = 1;
                    }
                    else
                    {
                        nextState[i, j] = 0;
                    }
                }
            }
            currentState = nextState;
            nextState = InitMap();
        }

        void DisplayMap()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (currentState[i, j] == 1)
                        cells[i, j].BackColor = Color.Black;
                    else cells[i, j].BackColor = Color.White;
                }
            }
        }

        int CountNeighboors(int i, int j)
        {
            var count = 0;
            for (int k = i - 1; k <= i + 1; k++)
            {
                for (int l = j - 1; l <= j + 1; l++)
                {
                    if (!IsInsideMap(k, l))
                        continue;
                    if (k == i && l == j)
                        continue;
                    if (currentState[k, l] == 1)
                        count++;
                }
            }
            return count;
        }

        bool IsInsideMap(int i, int j)
        {
            if (i < 0 || i >= mapSize || j < 0 || j >= mapSize)
            {
                return false;
            }
            return true;
        }

        int[,] InitMap()
        {
            int[,] arr = new int[mapSize, mapSize];
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    arr[i, j] = 0;
                }
            }
            return arr;
        }

        void InitCells()
        {
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    Button button = new Button
                    {
                        Size = new Size(cellSize, cellSize),
                        BackColor = Color.White,
                        Location = new Point(j * cellSize, (i * cellSize) + offset)
                    };
                    button.Click += new EventHandler(OnCellClick);
                    this.Controls.Add(button);
                    cells[i, j] = button;
                }
            }
        }

        private void OnCellClick(object sender, EventArgs e)
        {
            var pressedButton = sender as Button;
            if (!isPlaying)
            {
                var i = (pressedButton.Location.Y - offset) / cellSize;
                var j = pressedButton.Location.X / cellSize;

                if (currentState[i, j] == 0)
                {
                    currentState[i, j] = 1;
                    cells[i, j].BackColor = Color.Black;
                }
                else
                {
                    currentState[i, j] = 0;
                    cells[i, j].BackColor = Color.White;
                }
            }
        }
    }
}
