using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


//REFACTORRRR!!
//ADD CONSTS AND VARS TO THE SNAKE CLASS
//ADD SnakeSpeed to the snake class

namespace Snake_Minigame
{
    class Rect{
        public int x, y, w, h;
        public Color col;
        public int dx = 0, dy = 0;

    }
    
    class Fruit
    {
        public int x, y;
        public Bitmap im;
    }

    class Snake
    {
        public List<Rect> parts = new List<Rect>();
        public List<Color> colors = new List<Color>();
        public int currCol = 0;
        public int dx = 1, dy = 0;
        public int score = 0;

        public bool up = false;
        public bool right = false;
        public bool down = false;
        public bool left = false;

        public bool win = false;
        public bool boost = false;

        public int snakeSpeed = 20;
        public int boostSpeed = 0;
        public int snakeSize = 20;
        public int growRate = 1;
        //public int boostDuration = ;


        public void move()
        {
            if (this.parts.Count > 1)
            {
                for (int i = this.parts.Count - 1; i > 0; i--)
                {
                    this.parts[i].x = this.parts[i - 1].x;
                    this.parts[i].y = this.parts[i - 1].y;
                }
                if (this.parts.Count >= 1)
                {
                    this.parts[0].x += this.dx * (snakeSpeed + boostSpeed);
                    this.parts[0].y += this.dy * (snakeSpeed + boostSpeed);
                }
            }
        }

        public void boostSnake()
        {
            if(boost && this.parts.Count > 2)
            {   
                this.boostSpeed = 30;
                //generateFruitAtPos(this.parts[this.parts.Count - 1].x, this.parts[this.parts.Count - 1].y);
                this.parts.RemoveAt(this.parts.Count - 1);

            }
            else
            {
                boost = false;
                boostSpeed = 0;
            }
        }

        public bool isHitSelf()
        {
            for (int i = 1; i < this.parts.Count; i++)
            {
                if (this.parts[0].x >= this.parts[i].x && this.parts[0].x <= this.parts[i].x + snakeSize - 5 && this.parts[0].y >= this.parts[i].y && this.parts[0].y <= this.parts[i].y + snakeSize - 5)
                {
                    win = false;
                    return true;
                }
            }
            return false;
        }

        public bool isHitBorders(List<Rect> borders)
        {
            int tailPos = this.parts.Count - 1;
            if (this.parts[0].x <= borders[0].x || this.parts[0].x >= borders[0].x + borders[0].w - 5 || this.parts[0].y <= borders[0].y || this.parts[0].y >= borders[0].y + borders[0].h)
            {
                win = false;
                return true;
            }
            return false;
        }

        public bool isHitFruit(List<Fruit> generatedFruit)
        {
            for (int i = 0; i < generatedFruit.Count; i++)
            {
                if (this.parts[0].x >= generatedFruit[i].x && this.parts[0].x <= generatedFruit[i].x + generatedFruit[i].im.Width && this.parts[0].y >= generatedFruit[i].y && this.parts[0].y <= generatedFruit[i].y + generatedFruit[i].im.Height)
                {
                    this.score++;
                    generatedFruit.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public void grow()
        {
            for (int i = 0; i < this.growRate; i++)
            {
                int tailPos = this.parts.Count - 1;
                Rect snakePart = new Rect();

                if (this.parts[tailPos].dx == 0)
                {
                    snakePart.x = this.parts[tailPos].x;
                    snakePart.y = this.parts[tailPos].y - snakeSize;
                }
                else if (this.parts[tailPos].dy == 0)
                {
                    snakePart.x = this.parts[tailPos].x - snakeSize;
                    snakePart.y = this.parts[tailPos].y;

                }

                snakePart.w = snakeSize;
                snakePart.h = snakeSize;
                snakePart.col = this.colors[this.currCol];

                this.parts.Add(snakePart);
            }
        }
    }

    public partial class Form1 : Form
    {
        
        int FRUIT_SIZE = 25;
        int SNAKE_START_SIZE = 3;

        Bitmap off;
        Rect border = new Rect();
        Snake snake = new Snake();
        Snake snake2 = new Snake();
        List<Rect> borders = new List<Rect>();
        List<Fruit> fruits = new List<Fruit>();
        List<Fruit> generatedFruit = new List<Fruit>();
        List<Snake> snakes = new List<Snake>();

        int loadCt = 0;
        int ctTicks = 0;
        

        Random r = new Random();
        Timer t = new Timer();

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            Paint += Form1_Paint;
            Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            MouseDown += Form1_MouseDown;
            MouseUp += Form1_MouseUp;
            t.Tick += T_Tick;
            t.Start();
        }

        private void T_Tick(object sender, EventArgs e)
        {
            // Create font and brush.
            Font drawFont = new Font("Monospace", 16);
            SolidBrush drawBrush = new SolidBrush(Color.Gray);

            // Create point for upper-left corner of drawing.
            float x = this.ClientSize.Width / 2 - 50;
            float  y = 500;
            String drawString = "";

            for (int i = 0; i < snakes.Count; i++)
            {
                if (snakes[i].isHitBorders(borders) || snakes[i].isHitSelf())
                {
                    if (i == 1)
                    {
                        drawString = "Red Snake Won";
                    }
                    else
                    {
                        drawString = "Blue Snake Won";
                    }
                    this.CreateGraphics().DrawString(drawString, drawFont, drawBrush, x, y);
                    return;
                }

                if (winner())
                {
                    if (snake.win)
                    {
                        drawString = "Red Snake Won";
                    }
                    else if (snake2.win)
                    {
                        drawString = "Blue Snake Won";
                    }
                    this.CreateGraphics().DrawString(drawString, drawFont, drawBrush, x, y);
                    return;
                }
            }

            if(ctTicks % 30 == 0)
            {
                borders[0].w -= r.Next(50, 60);
                borders[0].h -= r.Next(50, 60);
                borders[0].x += r.Next(-40, 40);
                borders[0].y += r.Next(-40, 40);
            }

            this.Text = snake.boost + " " + snake.parts.Count;
            ctTicks = (ctTicks + 1) % 1000000;

            for (int i = 0; i < snakes.Count; i++)
            {
                snakes[i].boostSnake();
            }

            for(int i = 0; i < snakes.Count; i++)
            {
                snakes[i].move();
            }

            for (int i = 0; i < snakes.Count; i++)
            {
                if (snakes[i].left)
                {
                    snakes[i].dy = 0;
                    snakes[i].dx = -1;
                }
                else if (snakes[i].right)
                {
                    snakes[i].dy = 0;
                    snakes[i].dx = 1;
                }
                else if (snakes[i].down)
                {
                    snakes[i].dy = 1;
                    snakes[i].dx = 0;
                }
                else if (snakes[i].up)
                {
                    snakes[i].dy = -1;
                    snakes[i].dx = 0;
                }
            }

            //Zaker Droosak

            if (ctTicks % 10 == 0)
            {
                generateRandomFruit();
   
            }
            
            for(int i =  0; i<snakes.Count; i++)
            {
                if (snakes[i].isHitFruit(generatedFruit))
                {
                    snakes[i].grow();
                }
            }

            drawDoubleBuff(this.CreateGraphics());
        }

        bool winner()
        {
            for (int i = 0; i < snake2.parts.Count; i++)
            {
                if (snake.parts[0].x >= snake2.parts[i].x && snake.parts[0].x <= snake2.parts[i].x + snake2.snakeSize - 5 && snake.parts[0].y >= snake2.parts[i].y && snake.parts[0].y <= snake2.parts[i].y + snake2.snakeSize - 5)
                {
                    snake2.win = true;
                    return true;
                }
            }
            for (int i = 0; i < snake.parts.Count; i++)
            {
                if (snake2.parts[0].x >= snake.parts[i].x && snake2.parts[0].x <= snake.parts[i].x + snake.snakeSize - 5 && snake2.parts[0].y >= snake.parts[i].y && snake2.parts[0].y <= snake.parts[i].y + snake.snakeSize - 5)
                {
                    snake.win = true;
                    return true;
                }
            }
            return false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (loadCt == 0)
            {
                off = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
                border.x = (int)(this.ClientSize.Width * 0.10);
                border.y = (int)(this.ClientSize.Height * 0.10);
                border.w = (int)(this.ClientSize.Width * 0.80);
                border.h = (int)(this.ClientSize.Height * 0.80);
                borders.Add(border);

                
                Rect snakePart = new Rect();

                snake.colors.Add(Color.DarkOliveGreen);
                snake.colors.Add(Color.DarkRed);
                snakePart.x = r.Next(border.x + snake.snakeSize, border.x + border.w - snake.snakeSize);
                snakePart.y = r.Next(border.y + snake.snakeSize, border.y + border.h - snake.snakeSize);
                snakePart.w = snake.snakeSize;
                snakePart.h = snake.snakeSize;
                snakePart.col = Color.Blue;
                snake.parts.Add(snakePart);


                for(int i =  1; i < SNAKE_START_SIZE; i++)
                {
                    snakePart = new Rect();
                    snakePart.x = snake.parts[0].x - snake.snakeSize * i;
                    snakePart.y = snake.parts[0].y;
                    snakePart.w = snake.snakeSize;
                    snakePart.h = snake.snakeSize;
                    snakePart.col = Color.Blue;
                    snake.parts.Add(snakePart);

                }

                snakePart = new Rect();
                snake2.colors.Add(Color.DarkOliveGreen);
                snake2.colors.Add(Color.DarkRed);
                snakePart.x = r.Next(border.x + snake2.snakeSize, border.x + border.w - snake2.snakeSize);
                snakePart.y = r.Next(border.y + snake2.snakeSize, border.y + border.h - snake2.snakeSize);
                snakePart.w = snake2.snakeSize;
                snakePart.h = snake2.snakeSize;
                snakePart.col = Color.Red;
                snake2.parts.Add(snakePart);


                for(int i =  1; i < SNAKE_START_SIZE; i++)
                {
                    snakePart = new Rect();
                    snakePart.x = snake2.parts[0].x - snake2.snakeSize * i;
                    snakePart.y = snake2.parts[0].y;
                    snakePart.w = snake2.snakeSize;
                    snakePart.h = snake2.snakeSize;
                    snakePart.col = Color.Red;
                    snake2.parts.Add(snakePart);

                }

                for (int i = 1; i <= 2; i++)
                {
                    Fruit pnn = new Fruit();
                    string source = i + ".png";
                    Bitmap b = new Bitmap(source);
                    b.MakeTransparent(b.GetPixel(0, 0));
                    pnn.im = b;
                    fruits.Add(pnn);
                }

                snakes.Add(snake);
                snakes.Add(snake2);

                loadCt++;
            }
        }

        public void generateRandomFruit()
        {
            Fruit pnn = new Fruit();

            int x = r.Next(border.x + FRUIT_SIZE, border.x + border.w - FRUIT_SIZE);
            int y = r.Next(border.y + FRUIT_SIZE, border.y + border.h - FRUIT_SIZE);
            int choice = r.Next(0, 6);

            pnn.x = x;
            pnn.y = y;
            
            if(choice == 5)
            {
                pnn.im = fruits[1].im;
            }
            else
            {
                pnn.im = fruits[0].im;
            }
            generatedFruit.Add(pnn);
        }

        

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {   
            if(e.KeyCode == Keys.Space)
            {
                snake.boost = false;
            }
            if(e.KeyCode == Keys.Enter)
            {
                snake2.boost = false;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    if (!snake.down)
                    {
                        snake.up = true;
                        snake.down = false;
                        snake.right = false;
                        snake.left = false;
                    }
                    break;
                case Keys.S:
                    if (!snake.up)
                    {
                        snake.up = false;
                        snake.down = true;
                        snake.right = false;
                        snake.left = false;
                    }
                    break;
                case Keys.D:
                    if (!snake.left)
                    {
                        snake.up = false;
                        snake.down = false;
                        snake.right = true;
                        snake.left = false;
                    }
                    break;
                case Keys.A:
                    if (!snake.right)
                    {
                        snake.up = false;
                        snake.down = false;
                        snake.right = false;
                        snake.left = true;
                    }
                    break;
                case Keys.Space:
                    if (snake.parts.Count > 2)
                    {
                        snake.boost = true;
                    }
                    break;

                case Keys.Up:
                    if (!snake2.down)
                    {
                        snake2.up = true;
                        snake2.down = false;
                        snake2.right = false;
                        snake2.left = false;
                    }
                    break;
                case Keys.Down:
                    if (!snake2.up)
                    {
                        snake2.up = false;
                        snake2.down = true;
                        snake2.right = false;
                        snake2.left = false;
                    }
                    break;
                case Keys.Right:
                    if (!snake2.left)
                    {
                        snake2.up = false;
                        snake2.down = false;
                        snake2.right = true;
                        snake2.left = false;
                    }
                    break;
                case Keys.Left:
                    if (!snake2.right)
                    {
                        snake2.up = false;
                        snake2.down = false;
                        snake2.right = false;
                        snake2.left = true;
                    }
                    break;
                case Keys.Enter:
                    if(snake2.parts.Count > 2)
                    {
                        snake2.boost = true;
                    }
                    break;
            }
        }

        void drawScene(Graphics g)
        {
            g.Clear(Color.Black);

            Pen p = new Pen(Color.DimGray, 3);

            for (int i = 0; i < borders.Count; i++) {
                g.DrawRectangle(p, borders[i].x, borders[i].y, borders[i].w, borders[i].h);
            }

            for(int i = 0; i<snake.parts.Count; i++)
            {
                SolidBrush b = new SolidBrush(Color.Red);
                p = new Pen(Color.Gray);
                g.FillRectangle(b, snake.parts[i].x, snake.parts[i].y, snake.parts[i].w, snake.parts[i].h);
                g.DrawRectangle(p, snake.parts[i].x, snake.parts[i].y, snake.parts[i].w, snake.parts[i].h);
            }
            for(int i = 0; i<snake2.parts.Count; i++)
            {
                SolidBrush b = new SolidBrush(Color.Blue);
                p = new Pen(Color.Gray);
                g.FillRectangle(b, snake2.parts[i].x, snake2.parts[i].y, snake2.parts[i].w, snake2.parts[i].h);
                g.DrawRectangle(p, snake2.parts[i].x, snake2.parts[i].y, snake2.parts[i].w, snake2.parts[i].h);
            }

            for(int i = 0; i < generatedFruit.Count; i++)
            {
                g.DrawImage(generatedFruit[i].im, generatedFruit[i].x, generatedFruit[i].y);
            }
        }

        private void drawDoubleBuff(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            drawScene(g2);
            g.DrawImage(off, 0, 0);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            drawDoubleBuff(this.CreateGraphics());
        }

    }
}
