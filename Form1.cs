using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using WMPLib;

namespace Space_Shooter
{
    public partial class Form1 : Form
    {
        private WindowsMediaPlayer gameMedia;
        private WindowsMediaPlayer shootMedia;
        private WindowsMediaPlayer explosion;

        private PictureBox[] enemiesAmmunition;
        private int enemiesAmmunitionSpeed;

        private PictureBox[] stars;
        private int playerSpeed;

        private PictureBox[] ammunition;
        private int ammunitionSpeed;

        private PictureBox[] enemies;
        private int enemySpeed;

        private Random rnd;
        private int backgroundSpeed;

        private int score;
        private int level;
        private int difficulty;
        private bool pause;
        private bool gameIsOver;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pause = false;
            gameIsOver = false;
            score = 0;
            level = 1;
            difficulty = 9;

            // stars will move 4 pixels from top to bottom everytime
            backgroundSpeed = 4;
            playerSpeed = 4;
            ammunitionSpeed = 20;
            enemySpeed = 4;
            enemiesAmmunitionSpeed = 4;

            ammunition = new PictureBox[3];

            Image ammunitionImage = Image.FromFile(@"asserts\munition.png");

            Image Enemy1 = Image.FromFile("asserts\\E1.png");
            Image Enemy2 = Image.FromFile("asserts\\E2.png");
            Image Enemy3 = Image.FromFile("asserts\\E3.png");
            Image boss1 = Image.FromFile("asserts\\Boss1.png");
            Image boss2 = Image.FromFile("asserts\\Boss2.png");

            enemies = new PictureBox[10];

            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i] = new PictureBox
                {
                    Size = new Size(40, 40),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BorderStyle = BorderStyle.None,
                    Visible = false
                };
                this.Controls.Add(enemies[i]);
                enemies[i].Location = new Point((i + 1) * 50, -50);
            }

            enemies[0].Image = boss1;
            enemies[1].Image = Enemy2;
            enemies[2].Image = Enemy3;
            enemies[3].Image = Enemy3;
            enemies[4].Image = Enemy1;
            enemies[5].Image = Enemy3;
            enemies[6].Image = Enemy2;
            enemies[7].Image = Enemy1;
            enemies[8].Image = Enemy2;
            enemies[9].Image = boss2;

            for (int i = 0; i < ammunition.Length; i++)
            {
                ammunition[i] = new PictureBox
                {
                    Size = new Size(8, 8),
                    Image = ammunitionImage,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BorderStyle = BorderStyle.None
                };
                this.Controls.Add(ammunition[i]);
            }

            // Create VMP
            gameMedia = new WindowsMediaPlayer();
            shootMedia = new WindowsMediaPlayer();
            explosion = new WindowsMediaPlayer();

            // Load all songs
            gameMedia.URL = "songs\\GameSong.mp3";
            shootMedia.URL = "songs\\shoot.mp3";
            explosion.URL = "songs\\boom.mp3";

            // Setup Songs Settings
            gameMedia.settings.setMode("loop", true);
            gameMedia.settings.volume = 5;
            shootMedia.settings.volume = 1;
            explosion.settings.volume = 6;

            stars = new PictureBox[20];
            rnd = new Random();

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i] = new PictureBox
                {
                    BorderStyle = BorderStyle.None,
                    Location = new Point(rnd.Next(20, 580), rnd.Next(-10, 400))
                };
                if (i % 2 == 1)
                {
                    stars[i].Size = new Size(2, 2);
                    stars[1].BackColor = Color.Wheat;
                }
                else
                {
                    stars[i].Size = new Size(3, 3);
                    stars[i].BackColor = Color.DarkGray;
                }

                this.Controls.Add(stars[i]);
            }

            enemiesAmmunition = new PictureBox[10];

            for (int i = 0; i < enemiesAmmunition.Length; i++)
            {
                enemiesAmmunition[i] = new PictureBox
                {
                    Size = new Size(2, 25),
                    Visible = false,
                    BackColor = Color.Yellow
                };

                // This random number generator will determine which enemies get to shoot ammunition at the player as we cant have every single enemy shooting at one
                int x = rnd.Next(0, 10);
                enemiesAmmunition[i].Location = new Point(enemies[x].Location.X, enemies[x].Location.Y - 20);
                this.Controls.Add(enemiesAmmunition[i]);
            }

            gameMedia.controls.play();
        }

        private void MoveBgTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < stars.Length / 2; i++)
            {
                stars[i].Top += backgroundSpeed;

                if (stars[i].Top >= this.Height)
                {
                    stars[i].Top = -stars[i].Height;
                }
            }

            for (int i = stars.Length / 2; i < stars.Length; i++)
            {
                stars[i].Top += backgroundSpeed - 2;

                if (stars[i].Top >= this.Height)
                {
                    stars[i].Top = -stars[i].Height;
                }
            }
        }

        // This algorithm just checks if our player (pictureBox) image is within the playing area of the pixels
        // You can use right property when using pictureBoxes
        private void DownMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Top < 400)
            {
                Player.Top += playerSpeed;
            }
        }

        private void UpMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Top > 10)
            {
                Player.Top -= playerSpeed;
            }
        }

        private void RightMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Right < 580)
            {
                Player.Left += playerSpeed;
            }
        }

        private void LeftMoveTimer_Tick(object sender, EventArgs e)
        {
            if (Player.Left > 10)
            {
                Player.Left -= playerSpeed;
            }
        }

        // This is when key is pressed
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!pause)
            {
                if (e.KeyCode == Keys.Right)
                {
                    RightMoveTimer.Start();
                }

                if (e.KeyCode == Keys.Left)
                {
                    LeftMoveTimer.Start();
                }

                if (e.KeyCode == Keys.Down)
                {
                    DownMoveTimer.Start();
                }

                if (e.KeyCode == Keys.Up)
                {
                    UpMoveTimer.Start();
                }
            }
        }

        // This is when key is released
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            RightMoveTimer.Stop();
            LeftMoveTimer.Stop();
            UpMoveTimer.Stop();
            DownMoveTimer.Stop();

            if (e.KeyCode == Keys.Space)
            {
                if (!gameIsOver)
                {
                    if (pause)
                    {
                        StartTimers();
                        gameOverLabel.Visible = false;
                        gameMedia.controls.play();
                        pause = false;
                    }
                    else
                    {
                        gameOverLabel.Location = new Point(this.Width / 2 - 120, 150);
                        gameOverLabel.Text = "PAUSED";
                        gameOverLabel.Visible = true;
                        gameMedia.controls.pause();
                        StopTimers();
                        pause = true;
                    }
                }
            }
        }

        private void MoveAmmunitionTimer_Tick(object sender, EventArgs e)
        {
            shootMedia.controls.play();
            for (int i = 0; i < ammunition.Length; i++)
            {
                if (ammunition[i].Top > 0)
                {
                    ammunition[i].Visible = true;
                    ammunition[i].Top -= ammunitionSpeed;

                    Collision();
                    CollisionWithEnemiesAmmunication();
                }
                else
                {
                    ammunition[i].Visible = false;
                    ammunition[i].Location = new Point(Player.Location.X + 20, Player.Location.Y - i * 30);
                }
            }
        }

        private void MoveEnemiesTimer_Tick(object sender, EventArgs e)
        {
            MoveEnemies(enemies, enemySpeed);
        }

        private void MoveEnemies(PictureBox[] array, int Speed)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i].Visible = true;
                array[i].Top += Speed;

                if (array[i].Top > this.Height)
                {
                    array[i].Location = new Point((i + 1) * 50, -100);
                }
            }
        }

        private void Collision()
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (ammunition[0].Bounds.IntersectsWith(enemies[i].Bounds) ||
                    ammunition[1].Bounds.IntersectsWith(enemies[i].Bounds) ||
                    ammunition[2].Bounds.IntersectsWith(enemies[i].Bounds))
                {
                    explosion.controls.play();

                    score += 1;
                    scoreLbl.Text = (score < 10) ? "0" + score.ToString() : score.ToString();

                    if (score % 30 == 0)
                    {
                        level += 1;
                        levelLbl.Text = (level < 10) ? "0" + level.ToString() : level.ToString();

                        if (enemySpeed <= 10 && enemiesAmmunitionSpeed <= 10 && difficulty >= 0)
                        {
                            difficulty--;
                            enemySpeed++;
                            enemiesAmmunitionSpeed++;
                        }

                        if (level == 10)
                        {
                            GameOver("Nice One");
                        }
                    }

                    enemies[i].Location = new Point((i + 1) * 50, -100);
                }

                if (Player.Bounds.IntersectsWith(enemies[i].Bounds))
                {
                    explosion.settings.volume = 30;
                    explosion.controls.play();
                    Player.Visible = false;
                    GameOver("");
                }
            }
        }

        private void GameOver(String str)
        {
            gameOverLabel.Text = str;
            gameOverLabel.Location = new Point(160, 120);
            gameOverLabel.Visible = true;
            replayButton.Visible = true;
            exitButton.Visible = true;

            gameMedia.controls.stop();
            StopTimers();
        }

        private void StopTimers()
        {
            MoveBgTimer.Stop();
            MoveEnemiesTimer.Stop();
            MoveAmmunitionTimer.Stop();
            EnemiesAmmunitionTimer.Stop();
            LeftMoveTimer.Stop();
            RightMoveTimer.Stop();
            UpMoveTimer.Stop();
            DownMoveTimer.Stop();
        }

        private void StartTimers()
        {
            int milliseconds = 2000;
            Thread.Sleep(milliseconds);

            LeftMoveTimer.Start();
            RightMoveTimer.Start();
            UpMoveTimer.Start();
            DownMoveTimer.Start();

            MoveBgTimer.Start();
            MoveEnemiesTimer.Start();
            MoveAmmunitionTimer.Start();
            EnemiesAmmunitionTimer.Start();
        }

        private void EnemiesAmmunitionTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < (enemiesAmmunition.Length - difficulty); i++)
            {
                if (enemiesAmmunition[i].Top < this.Height)
                {
                    enemiesAmmunition[i].Visible = true;
                    enemiesAmmunition[i].Top += enemiesAmmunitionSpeed;
                }
                else
                {
                    enemiesAmmunition[i].Visible = false;
                    int x = rnd.Next(0, 10);
                    enemiesAmmunition[i].Location = new Point(enemies[x].Location.X + 20, enemies[x].Location.Y + 30);
                }
            }
        }

        private void CollisionWithEnemiesAmmunication()
        {
            for (int i = 0; i < enemiesAmmunition.Length; i++)
            {
                if (enemiesAmmunition[i].Bounds.IntersectsWith(Player.Bounds))
                {
                    enemiesAmmunition[i].Visible = false;
                    explosion.settings.volume = 30;
                    explosion.controls.play();
                    Player.Visible = false;
                    GameOver("Game Over");
                }
            }
        }

        private void replayButton_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            InitializeComponent();
            Form1_Load(e, e);
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(1);
        }
    }
}