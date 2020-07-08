using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Chess;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace FormClient
{
    public partial class Form1 : Form
    {
        int[,] pawnPos = new int[,]
        {
            { 0,  0,  0,  0,  0,  0,  0,  0 },
            { 50, 50, 50, 50, 50, 50, 50, 50 },
            { 10, 10, 20, 30, 30, 20, 10, 10 },
            { 5,  5, 10, 25, 25, 10,  5,  5 },
            { 0,  0,  0, 20, 20,  0,  0,  0 },
            { 5, -5,-10,  0,  0,-10, -5,  5 },
            { 5, 10, 10,-20,-20, 10, 10,  5 },
            { 0,  0,  0,  0,  0,  0,  0,  0 },
        };
        int[,] knightPos = new int[,]
        {
            { -50,-40,-30,-30,-30,-30,-40,-50 },
            { -40,-20,  0,  0,  0,  0,-20,-40 },
            { -30,  0, 10, 15, 15, 10,  0,-30 },
            { -30,  5, 15, 20, 20, 15,  5,-30 },
            { -30,  0, 15, 20, 20, 15,  0,-30 },
            { -30,  5, 10, 15, 15, 10,  5,-30 },
            { -40,-20,  0,  5,  5,  0,-20,-40 },
            { -50,-40,-30,-30,-30,-30,-40,-50 },
        };
        int[,] bishopPos = new int[,]
        {
            { -20,-10,-10,-10,-10,-10,-10,-20 },
            { -10,  0,  0,  0,  0,  0,  0,-10 },
            { -10,  0,  5, 10, 10,  5,  0,-10 },
            { -10,  5,  5, 10, 10,  5,  5,-10 },
            { -10,  0, 10, 10, 10, 10,  0,-10 },
            { -10, 10, 10, 10, 10, 10, 10,-10 },
            { -10,  5,  0,  0,  0,  0,  5,-10 },
            { -20,-10,-10,-10,-10,-10,-10,-20 },
        };
        int[,] rookPos = new int[,]
        {
            { 0,  0,  0,  0,  0,  0,  0,  0 },
            { 5, 10, 10, 10, 10, 10, 10,  5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            { -5,  0,  0,  0,  0,  0,  0, -5 },
            {  0,  0,  0,  5,  5,  0,  0,  0 }
        };
        int[,] queenPos = new int[,]
        {
            { -20,-10,-10, -5, -5,-10,-10,-20 },
            { -10,  0,  0,  0,  0,  0,  0,-10 },
            { -10,  0,  5,  5,  5,  5,  0,-10 },
            {  -5,  0,  5,  5,  5,  5,  0, -5 },
            {   0,  0,  5,  5,  5,  5,  0, -5 },
            { -10,  5,  5,  5,  5,  5,  0,-10 },
            { -10,  0,  5,  0,  0,  0,  0,-10 },
            { -20,-10,-10, -5, -5,-10,-10,-20 }
        };
        int[,] kingPosMid = new int[,]
        {
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -30,-40,-40,-50,-50,-40,-40,-30 },
            { -20,-30,-30,-40,-40,-30,-30,-20 },
            { -10,-20,-20,-20,-20,-20,-20,-10 },
            {  20, 20,  0,  0,  0,  0, 20, 20 },
            {  20, 30, 10,  0,  0, 10, 30, 20 }
        };
        int[,] kingPosEnd = new int[,]
        {
            { -50,-40,-30,-20,-20,-30,-40,-50 },
            { -30,-20,-10,  0,  0,-10,-20,-30 },
            { -30,-10, 20, 30, 30, 20,-10,-30 },
            { -30,-10, 30, 40, 40, 30,-10,-30 },
            { -30,-10, 30, 40, 40, 30,-10,-30 },
            { -30,-10, 20, 30, 30, 20,-10,-30 },
            { -30,-30,  0,  0,  0,  0,-30,-30 },
            { -50,-30,-30,-30,-30,-30,-30,-50 }
        };

        ChessBoard chessBoard = new ChessBoard();
        Chess.Point selectedPiece = new Chess.Point();
        int selectedPlayer = -1;
        static public int depth = 3; //depth of the minimax
        string projectPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "Resources");

        public Form1()
        {
            InitializeComponent();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            for (int x = 1; x < boardLayoutPanel.ColumnCount; x++)
            {
                for (int y = 1; y < boardLayoutPanel.RowCount; y++)
                {
                    Button button = new Button();
                    button.Dock = DockStyle.Fill;
                    button.Margin = new Padding(0);
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    if ((x + y) % 2 == 1) button.BackColor = Color.Gray;
                    else button.BackColor = Color.DarkGray;
                    boardLayoutPanel.Controls.Add(button);
                    button.Click += Click_Board;
                }
            }

            DrawPieces(chessBoard);
        }

        private void Click_Board(object s, EventArgs e)
        {
            DrawPieces(chessBoard);
            if (!(s is Button)) return;
            Button button = (Button)s;
            button.FlatStyle = FlatStyle.Standard;
            TableLayoutPanelCellPosition a = boardLayoutPanel.GetPositionFromControl((Control)s);

            if (!(button.Tag is ChessPiece))
            {
                if (selectedPlayer > -1)
                {
                    if (chessBoard.ActionPiece(selectedPiece.x, selectedPiece.y, a.Column - 1, a.Row - 1))
                    {
                        DrawPieces(chessBoard);
                        this.boardLayoutPanel.Enabled = false;
                        this.difficultyToolStripMenuItem.Enabled = false;
                        StartTask(depth);
                    }
                    selectedPlayer = -1;
                }
                return;
            }

            ChessPiece chessPiece = (ChessPiece)button.Tag;
            Console.WriteLine("({2}, {3}) - {0} from team {1}", chessPiece.GetType(), chessPiece.Player, a.Column - 1, a.Row - 1);

            if (selectedPlayer > -1 && selectedPlayer != chessPiece.Player)
            {
                if(chessBoard.ActionPiece(selectedPiece.x, selectedPiece.y, a.Column - 1, a.Row - 1))
                {
                    selectedPlayer = -1;
                    DrawPieces(chessBoard);
                    this.boardLayoutPanel.Enabled = false;
                    difficultyToolStripMenuItem.Enabled = false;
                    StartTask(depth);
                }
            }
            else if (chessPiece.Player == 1)
            {
                selectedPlayer = chessPiece.Player;
                selectedPiece.x = a.Column - 1;
                selectedPiece.y = a.Row - 1;
                foreach (Chess.Point point in chessBoard.PieceActions(a.Column - 1, a.Row - 1))
                {
                    Button actionButton = (Button)boardLayoutPanel.GetControlFromPosition(point.x + 1, point.y + 1);
                    actionButton.FlatStyle = FlatStyle.Standard;
                    Console.WriteLine("~({0}, {1})", point.x, point.y);
                }
                Console.WriteLine();
            }
        }

        private void DrawPieces(ChessBoard board)
        {
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Button button = (Button)boardLayoutPanel.GetControlFromPosition(x + 1, y + 1);
                    button.FlatStyle = FlatStyle.Flat;
                    if (board[x, y] != null)
                    {
                        ChessPiece chessPiece = board[x, y];
                        button.Tag = chessPiece;
                        if (chessPiece.Player == 1) button.Image = System.Drawing.Image.FromFile(projectPath + @"\" + chessPiece.ToString().Replace("Chess.", "") + "W.png");
                        else button.Image = System.Drawing.Image.FromFile(projectPath + @"\" + chessPiece.ToString().Replace("Chess.", "") + "B.png");
                    }
                    else
                    {
                        button.Image = null;
                        button.Tag = null;
                    }
                    this.coordinates.SetToolTip(button, String.Format("({0}, {1})", x, y));
                }
            }
        }

        private async void StartTask(int depth)
        {
            await Task.Run(() => AI(depth));
            DrawPieces(chessBoard);
            this.boardLayoutPanel.Enabled = true;
            this.difficultyToolStripMenuItem.Enabled = true;
        }

        //AI player
        private void AI(int depth)
        {
            move temp = bestMove(chessBoard, depth);
            if(temp.from.x==-1)
            {
                MessageBox.Show("White wins.");
                return;
            }
            chessBoard.ActionPiece(temp.from, temp.to);
            if (legalMoves(chessBoard, 1).Count() == 0)
            {
                MessageBox.Show("Black wins.");
                return;
            }
        }

        //get a list of legal moves for all of pieces of a certain player
        private List<move> legalMoves(ChessBoard board, int playerTurn)
        {
            List<move> legalMoves = new List<move>();
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if(board[x, y] != null)
                    {
                        if(board[x, y].Player == playerTurn)
                        {
                            foreach(Chess.Point point in board.PieceActions(x, y))
                            {
                                legalMoves.Add(new move(new Chess.Point(x, y), point));
                            }
                        }
                    }
                }
            }
            return legalMoves;
        }

        //evaluate board to calculate moves
        bool checkEndgame(ChessBoard board)
        {
            int count = 0;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if(board[x, y] != null)
                    {
                        ++count;
                    }
                }
            }
            if (count <= 12)
                return true;
            else
                return false;
        }

        private int evaluateBoard(ChessBoard board)
        {
            int[,] kingPos;
            if (checkEndgame(board))
            {
                kingPos = kingPosEnd;
            }
            else
            {
                kingPos = kingPosMid;
            }
            int totalEvaluation = 0;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (board[x, y] == null)
                        continue;
                    if(board[x, y].Player==1)
                    {
                        switch (board[x, y].ToString())
                        {
                            case "Chess.Pawn":
                                totalEvaluation -= (100 + pawnPos[7 - x, 7 - y]);
                                break;
                            case "Chess.Knight":
                                totalEvaluation -= (320 + knightPos[7 - x, 7 - y]);
                                break;
                            case "Chess.Bishop":
                                totalEvaluation -= (330 + bishopPos[7 - x, 7 - y]);
                                break;
                            case "Chess.Rook":
                                totalEvaluation -= (500 + rookPos[7 - x, 7 - y]);
                                break;
                            case "Chess.Queen":
                                totalEvaluation -= (900 + queenPos[7 - x, 7 - y]);
                                break;
                            case "Chess.King":
                                totalEvaluation -= (30300 + kingPos[7 - x, 7 - y]);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (board[x, y].ToString())
                        {
                            case "Chess.Pawn":
                                totalEvaluation += (100 + pawnPos[x, y]) ;
                                break;
                            case "Chess.Knight":
                                totalEvaluation += (320 + knightPos[x, y]);
                                break;
                            case "Chess.Bishop":
                                totalEvaluation += (330 + bishopPos[x, y]);
                                break;
                            case "Chess.Rook":
                                totalEvaluation += (500 + rookPos[x, y]);
                                break;
                            case "Chess.Queen":
                                totalEvaluation += (900 + queenPos[x, y]);
                                break;
                            case "Chess.King":
                                totalEvaluation += (30300 + kingPos[x, y]);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return totalEvaluation;
        }

        //for a list of moves
        struct move
        {
            public Chess.Point from;
            public Chess.Point to;
            public move(Chess.Point from, Chess.Point to)
            {
                this.from = from;
                this.to = to;
            }
        }

        //best move for computer
        move bestMove(ChessBoard board, int depth)
        {
            List<move> bestMoves = new List<move>();
            int max = -2147483648;
            int moveValue = 0;
            foreach(move m in legalMoves(board, 0))
            {
                ChessBoard tempBoard = new ChessBoard(board);
                tempBoard.ActionPiece(m.from, m.to, true);
                moveValue = Minimax(tempBoard, depth - 1, 1, -2147483648, 2147483647) + evaluateBoard(tempBoard);
                if(moveValue == max)
                {
                    bestMoves.Add(m);
                }
                else if(moveValue > max)
                {
                    bestMoves.Clear();
                    bestMoves.Add(m);
                    max = moveValue;
                }
            }
            if (bestMoves.Count() == 0)
                return new move(new Chess.Point(-1, -1), new Chess.Point(0, 0));
            Random r = new Random();
            return bestMoves[(int)r.Next(0, bestMoves.Count()-1)];
        }

        //alpha-beta pruning
        int Minimax(ChessBoard board, int depth, int player, int alpha, int beta)
        {
            if(depth == 0)
            {
                return 0;
            }
            if (player == 0)
            {
                int bestVal = -2147483648;
                foreach (move m in legalMoves(board, 0))
                {
                    ChessBoard tempBoard = new ChessBoard(board);
                    tempBoard.ActionPiece(m.from, m.to, true);
                    bestVal = max(bestVal, evaluateBoard(tempBoard) + Minimax(tempBoard, depth - 1, 1, alpha, beta));
                    alpha = max(alpha, bestVal);
                    if (beta <= alpha)
                        break;
                }
                return bestVal;
            }
            else
            {
                int bestVal = 2147483647;
                foreach (move m in legalMoves(board, 1))
                {
                    ChessBoard tempBoard = new ChessBoard(board);
                    tempBoard.ActionPiece(m.from, m.to, true);
                    bestVal = min(bestVal, evaluateBoard(tempBoard) + Minimax(tempBoard, depth - 1, 0, alpha, beta));
                    beta = min(beta, bestVal);
                    if (beta <= alpha)
                        break;
                }
                return bestVal;
            }
        }
        int max(int a, int b)
        {
            if(a > b)
            {
                return a;
            }
            else
            {
                return b;
            }
        }
        int min(int a, int b)
        {
            if (a < b)
            {
                return a;
            }
            else
            {
                return b;
            }
        }

        private void newChessGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
            Environment.Exit(0);
        }

        private void easyDepth2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            depth = 2;
            MessageBox.Show("Difficulty changed to Easy");
        }

        private void normalDepth3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            depth = 3;
            MessageBox.Show("Difficulty changed to Normal");
        }

        private void hardDepth4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            depth = 4;
            MessageBox.Show("Difficulty changed to Hard");
        }

        private void optionalDepthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var m = new Form2();
            m.ShowDialog();
        }
    }
}