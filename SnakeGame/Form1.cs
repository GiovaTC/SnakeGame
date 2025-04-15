using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using System.IO;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer();
        private List<Point> snake = new List<Point>();
        private Point food = Point.Empty;
        private string direction = "right";
        private int cellSize = 20;
        private int score = 0;
        private int highScore = 0;
        private string highScoreFile = "highscore.txt";

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Width = 800;
            this.Height = 600;
            this.Text = "Snake Game - Fosforecente 🐍";
            this.BackColor = Color.Black;

            this.KeyDown += new KeyEventHandler(OnKeyDown);
            this.Paint += new PaintEventHandler(OnPaint);

            InitGame();
        }

        private void InitGame()
        {
            LoadHighScore();
            score = 0;

            snake.Clear();
            snake.Add(new Point(5, 5));
            snake.Add(new Point(4, 5));
            snake.Add(new Point(3, 5));
            direction = "right";

            SpawnFood();

            gameTimer.Interval = 100;
            gameTimer.Tick += new EventHandler(Update);
            gameTimer.Start();
        }

        private void Update(object sender, EventArgs e)
        {
            MoveSnake();

            if (snake[0] == food)
            {
                snake.Add(snake[snake.Count - 1]);// crecer
                score += 10;
                PlayEatSound();
                SpawnFood();
            }

            if(IsCollision())
            {
                PlayGameOverSound();

                if(score > highScore)
                {
                    highScore = score;
                    SaveHighScore();
                }

                gameTimer.Stop();
                MessageBox.Show($"Game Over!\nPuntaje: {score}\nMejor Puntaje: {highScore}", "Fin del Juego");
                InitGame();
            }

            Invalidate();
        }

        private void MoveSnake()
        {
            for (int i = snake.Count - 1; i > 0; i--)
                snake[i] = snake[i - 1];

            Point head = snake[0]; 
            
            switch (direction)
            {
                case "up": head.Y -= 1; break;
                case "down": head.Y += 1; break;
                case "left": head.X -= 1; break;
                case "right": head.X += 1; break;
            }

            snake[0] = head;
        }

        private void SpawnFood()
        {
            Random rnd = new Random();
            int maxX = this.ClientSize.Width / cellSize;
            int maxY = this.ClientSize.Height / cellSize;
            Point newFood;

            do
            {
                newFood = new Point(rnd.Next(0, maxX), rnd.Next(0, maxY));
            }
            while (snake.Contains(newFood));

            food = newFood;
        }

        private bool IsCollision()
        {
            Point head = snake[0];

            // colision con el cuerpo
            for (int i = 1; i < snake.Count;  i++)
            {
                if (snake[i] == head)
                    return true;
            }

            // colision con paredes
            if (head.X < 0 || head.Y < 0 || head.X >= this.ClientSize.Width / cellSize || head.Y >= this.ClientSize.Height / cellSize)
                return true;

            return false;
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            //Dibujar la serpiente
            foreach (Point p in snake)
            {
                g.FillRectangle(Brushes.Lime, new Rectangle(p.X * cellSize, p.Y * cellSize, cellSize, cellSize));
                g.DrawRectangle(Pens.Black, new Rectangle(p.X * cellSize, p.Y * cellSize, cellSize, cellSize));
            }

            //Dibujar la comida
            g.FillEllipse(Brushes.Red, new Rectangle(food.X * cellSize, food.Y * cellSize, cellSize, cellSize));

            //Dibujar puntajes
            using (Font font = new Font("Arial", 14, FontStyle.Bold))
            {
                g.DrawString($"Puntaje: {score} | Récord: {highScore}", font, Brushes.White, 10, 10);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (direction != "down") direction = "up";
                    break;
                case Keys.Down:
                    if (direction != "up") direction = "down";
                    break;
                case Keys.Left:
                    if (direction != "right") direction = "left";
                    break;
                case Keys.Right:
                    if (direction != "left") direction = "right";
                    break;
            }
        }

        private void PlayEatSound()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("eat.wav");
                player.Play();
            }
            catch { }
        }

        private void PlayGameOverSound()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("gameover.wav");
                player.Play();
            }
            catch { }
        }

        private void LoadHighScore()
        {
            if (File.Exists(highScoreFile))
            {
                string content = File.ReadAllText(highScoreFile);
                int.TryParse(content, out highScore);
            }
        }

        private void SaveHighScore()
        {
            File.WriteAllText(highScoreFile, highScore.ToString());
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
