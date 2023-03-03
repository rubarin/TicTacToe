using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TicTacToe
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<Player, ImageSource> imageSources = new()
        {
            { Player.X, new BitmapImage(new Uri("/Elements/X15.png", UriKind.Relative)) },
            { Player.O, new BitmapImage(new Uri("/Elements/O15.png", UriKind.Relative)) },
        };

        private readonly Image[,] imageControls = new Image[3, 3];
        private readonly GameState gameState = new GameState();
        private int winsCount = 0;
        private int xWins = 0;
        private int oWins = 0;
        public MainWindow()
        {
            InitializeComponent();
            SetupGameGrid();
            gameState.MoveMade += OnMoveMade;
            gameState.GameEnded += OnGameEnded;
            gameState.GameRestarted += OnGameRestarted;
        }

        private void CountWins(Player winner)
        {
            if (winner == Player.X)
            {
                xWins++;
            }
            else if (winner == Player.O)
            {
                oWins++;
            }

            WinsCounter.Text = $"{xWins} : {oWins}";
        }
        private void SetupGameGrid()
        {
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    Image imageControl = new Image();
                    GameGrid.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }
        }

        private void OnMoveMade(int r, int c)
        {
            Player player = gameState.GameGrid[r, c];
            imageControls[r, c].Source = imageSources[player];
            PlayerImage.Source = imageSources[gameState.CurrentPlayer];

        }

        private async void OnGameEnded(GameResult gameResult)
        {
            await Task.Delay(1000);

            if (gameResult.Winner == Player.None)
            {
                TransitionToEndScreen("It's a tie!", null);
            }
            else
            {
                CountWins(gameResult.Winner);
                TransitionToEndScreen("Winner:", imageSources[gameResult.Winner]);
            }
        }

        private void TransitionToEndScreen(string text, ImageSource winnerImage)
        {
            TurnPanel.Visibility = Visibility.Hidden;
            GameCanvas.Visibility = Visibility.Hidden;
            ResultText.Text = text;
            WinnerImage.Source = winnerImage;
            EndScreen.Visibility = Visibility.Visible;
        }

        private void TransitionToGameScreen()
        {
            EndScreen.Visibility = Visibility.Hidden;
            TurnPanel.Visibility = Visibility.Visible;
            GameCanvas.Visibility = Visibility.Visible;
        }

        private void OnGameRestarted()
        {
            for (int r = 0; r < 3;  r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    imageControls[r, c].Source = null;
                }
            }

 
            PlayerImage.Source = imageSources[gameState.CurrentPlayer];
            TransitionToGameScreen();
        }

        private void GameGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            double squareSize = GameGrid.Width / 3;
            Point clickPosition = e.GetPosition(GameGrid);
            int row = (int)(clickPosition.Y / squareSize);
            int column = (int)(clickPosition.X / squareSize);
            gameState.MakeMove(row, column);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            gameState.Reset();
        }
    }
}
