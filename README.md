# 🐍 Snake Game en Windows Forms con Sonido y Puntuación

![image](https://github.com/user-attachments/assets/3e6c47b5-6d74-45d5-9b17-26131f298034)

![image](https://github.com/user-attachments/assets/eebd4d13-91ed-4ade-95e6-744e10892458)

## 🎯 Crear el Proyecto

1. Abre **Visual Studio 2022**.
2. Crea un nuevo proyecto del tipo **Aplicación Windows Forms** (C#).
3. Ponle un nombre, por ejemplo: `SnakeGame`.
4. En el formulario principal (`Form1.cs`), **borra cualquier control que venga por defecto**.

---

## ✅ Requisitos antes de ejecutar

- Crea un proyecto **Windows Forms App (.NET Framework o .NET 6/7)**.
- Agrega los siguientes archivos al proyecto:
  - `eat.wav`: Sonido cuando la serpiente come comida.
  - `gameover.wav`: Sonido al perder.
  - `highscore.txt`: Puedes crearlo vacío o dejar que el programa lo genere automáticamente.

🔁 Asegúrate de que estos archivos `.wav` y `.txt` estén en el mismo directorio que el `.exe`, o bien:

> Haz clic derecho en los archivos → **Propiedades** → `Copiar al directorio de salida` → `Copiar si es más nuevo`.

✅ Solución recomendada en Visual Studio
Para que estos archivos se copien automáticamente al directorio de salida cada vez que se compila , haz lo siguiente:

🧭 Paso a paso:
En el Explorador de soluciones , busca los archivos eat.wav, gameover.wavy highscore.txt.

Haz clic derecho sobre cada uno → selecciona Propiedades .

En la propiedad "Copiar al directorio de salida" , selecciona:

![image](https://github.com/user-attachments/assets/a121e310-6859-4989-8238-9410a7bd2049)

Guarda todo y vuelve a compilar.

---

🎵 Cómo crear los archivos .wav y .txt necesarios para Snake Game
Para que el juego Snake funcione correctamente, necesitas los siguientes archivos:

🎧 1. Archivos de sonido.wav
Puedes crear o descargar sonidos fácilmente.

🟥 Opción A: Crear archivos .wav simples (Windows)
Abra la aplicación Grabadora de voz en Windows.

Graba un sonido corto, como un “click” o un "pip".

Guarda el archivo como eat.wav.

Graba otro sonido tipo “error” o algo más fuerte para el Game Over .

Guarda ese archivo como gameover.wav.

💡Asegúrate de que los archivos estén en formato .wav. Si no lo están, puedes cambiar la extensión manualmente usando el Bloc de notas o las opciones de renombrado.

🟩 Opción B: Descargar archivos .wav listos
Puedes descargar sonidos desde estos sitios gratuitos:

https://freesound.org (requiere cuenta gratuita)

https://soundbible.com

https://mixkit.co/free-sound-effects/

Nombres sugeridos:

eat.wav→ sonido corto tipo "blip" o "chomp"

gameover.wav → sonido tipo "fallo", "error" o "explosión suave"

📁 Coloca ambos archivos en la carpeta del proyecto (junto al archivo .exe o en bin\Debug).

📝 2. Archivo highscore.txt
Este archivo guarda la puntuación más alta obtenida.

Crear manualmente:
Abre el Explorador de archivos.

Haz clic derecho > Nuevo > Documento de texto.

Nómbralo exactamente como: highscore.txt.

Ábrelo y escribe el número 0.
----/-----
TXT
0
----/----
Guarda y colócalo en la carpeta del proyecto (junto al ejecutable), o deja que el programa lo genere automáticamente la primera vez que juegues.

🛠️ Asegúrate de que Visual Studio copie los archivos a la carpeta de salida
Haga clic derecho sobre el archivo en el Explorador de soluciones.

Selecciona Propiedades.

En la opción Copiar al directorio de salida, elige:
Copiar si es más reciente( Copiar si es más nuevo ).

---

archivos necesarios para tu juego Snake.

🔗 Descargar serpiente_assets.zip

Contenido del ZIP:

eat.wav → Sonido corto tipo “pip” al comer.

gameover.wav → Sonido grave descendente al perder.

highscore.txt → Archivo de puntuación con valor inicial “0”.

Coloca estos archivos en la carpeta de tu ejecutable (por ejemplo: bin\Debug\net6.0-windows) y funcionarán correctamente.

---

## 🧩 Código completo de `Form1.cs`

```csharp
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
        private Timer gameTimer = new Timer();
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
            this.Text = "Snake Game - Fosforescente 🐍";
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
                snake.Add(snake[snake.Count - 1]); // crecer
                score += 10;
                PlayEatSound();
                SpawnFood();
            }

            if (IsCollision())
            {
                PlayGameOverSound();

                if (score > highScore)
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

            // colisión con el cuerpo
            for (int i = 1; i < snake.Count; i++)
            {
                if (snake[i] == head)
                    return true;
            }

            // colisión con paredes
            if (head.X < 0 || head.Y < 0 || head.X >= this.ClientSize.Width / cellSize || head.Y >= this.ClientSize.Height / cellSize)
                return true;

            return false;
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Dibujar la serpiente
            foreach (Point p in snake)
            {
                g.FillRectangle(Brushes.Lime, new Rectangle(p.X * cellSize, p.Y * cellSize, cellSize, cellSize));
                g.DrawRectangle(Pens.Black, new Rectangle(p.X * cellSize, p.Y * cellSize, cellSize, cellSize));
            }

            // Dibujar comida
            g.FillEllipse(Brushes.Red, new Rectangle(food.X * cellSize, food.Y * cellSize, cellSize, cellSize));

            // Dibujar puntajes
            using (Font font = new Font("Arial", 14, FontStyle.Bold))
            {
                g.DrawString($"Puntaje: {score}  |  Récord: {highScore}", font, Brushes.White, 10, 10);
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
    }
}

📦 ¿Qué incluye este código?
✅ Comida roja ( Brushes.Red)

)✅ Sonido al comer ( eat.wav)

)✅ Sonido al perder ( gameover.wav)

)✅ Serpiente verde fosforescente ( Brushes.Lime)

✅ Puntuación actual visible en pantalla punto

✅ Guardado de la mejor puntuación enhighscore.txt

✅ Reinicio automático del juego tras perder

🧪 Resultado Final
La serpiente se mueve automáticamente.

Puedes controlarla con las teclas de flechas .

Al chocar consigo misma o con las paredes, se termina el juego.

La serpiente es de color verde fosforescente .

La comida es de color rojo .

