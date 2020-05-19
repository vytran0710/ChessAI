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
        ChessBoard chessBoard = new ChessBoard();
        Chess.Point selectedPiece = new Chess.Point();
        int selectedPlayer = -1;
        int depth = 3; //depth of the minimax
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
                        Thread T = new Thread(() => AI(depth));
                        T.Start();
                        T.Join();
                        DrawPieces(chessBoard);
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
                    Thread T = new Thread(() => AI(depth));
                    T.Start();
                    T.Join();
                    DrawPieces(chessBoard);
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

            /*
            if(chessPiece!=null && chessPiece.Player==0)
            {
                foreach(Chess.Point point in chessBoard.PieceActions(a.Column - 1, a.Row - 1))
                {
                    MessageBox.Show(point.ToString());
                }
            }
            */
        }

        private void DrawPieces(ChessBoard board)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    Button button = (Button)boardLayoutPanel.GetControlFromPosition(x + 1, y + 1);
                    button.FlatStyle = FlatStyle.Flat;
                    if (board[x, y] != null)
                    {
                        ChessPiece chessPiece = board[x, y];
                        button.Tag = chessPiece;
                        //button.Text = chessPiece.ToString().Replace("Chess.", "");
                        if (chessPiece.Player == 1) button.Image = System.Drawing.Image.FromFile(projectPath + @"\" + chessPiece.ToString().Replace("Chess.", "") + "W.png");
                        else button.Image = System.Drawing.Image.FromFile(projectPath + @"\" + chessPiece.ToString().Replace("Chess.", "") + "B.png");
                    }
                    else
                    {
                        //button.Text = "";
                        button.Image = null;
                        button.Tag = null;
                    }
                    this.coordinates.SetToolTip(button, String.Format("({0}, {1})", x, y));
                }
            }
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
            for (int x = 0; x < boardLayoutPanel.ColumnCount-1; x++)
            {
                for (int y = 0; y < boardLayoutPanel.RowCount-1; y++)
                {
                    if(board[x, y] != null)
                    {
                        if(board[x, y].Player == playerTurn)
                        {
                            foreach(Chess.Point point in board.PieceActions(x, y))
                            {
                                legalMoves.Add(new move(new Chess.Point(x, y), point));
                            }

                            //Parallel.ForEach(board.PieceActions(x, y), (point) =>
                            //{
                            //    legalMoves.Add(new move(new Chess.Point(x, y), point));
                            //});
                        }
                    }
                }
            }
            return legalMoves;
        }

        //evaluate board to calculate moves
        private int evaluateBoard(ChessBoard board)
        {
            int totalEvaluation = 0;
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    if (board[x, y] == null)
                        continue;
                    if(board[x, y].Player==1)
                    {
                        switch (board[x, y].ToString())
                        {
                            case "Chess.Pawn":
                                totalEvaluation -= 1;
                                break;
                            case "Chess.Knight":
                                totalEvaluation -= 3;
                                break;
                            case "Chess.Bishop":
                                totalEvaluation -= 3;
                                break;
                            case "Chess.Rook":
                                totalEvaluation -= 5;
                                break;
                            case "Chess.Queen":
                                totalEvaluation -= 9;
                                break;
                            case "Chess.King":
                                totalEvaluation -= 9;
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
                                totalEvaluation += 1;
                                break;
                            case "Chess.Knight":
                                totalEvaluation += 3;
                                break;
                            case "Chess.Bishop":
                                totalEvaluation += 3;
                                break;
                            case "Chess.Rook":
                                totalEvaluation += 5;
                                break;
                            case "Chess.Queen":
                                totalEvaluation += 9;
                                break;
                            case "Chess.King":
                                totalEvaluation += 9;
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
        /*
        //min player: human player
        int minVal(ChessBoard board, int depth)
        {
            if (depth == 0 || legalMoves(board, 0).Count() == 0)
            {
                return 0;
            }    
            int worst = 9999;
            int moveValue = 0;
            foreach(move m in legalMoves(board, 1))
            {
                ChessBoard tempBoard = new ChessBoard(board);
                tempBoard.ActionPiece(m.from, m.to, true);
                moveValue = evaluateBoard(tempBoard) + maxVal(tempBoard, depth - 1);
                if(moveValue < worst)
                {
                    worst = moveValue;
                }
            }
            return worst;
        }

        //max player: computer
        int maxVal(ChessBoard board, int depth)
        {
            if (depth == 0 || legalMoves(board, 0).Count() == 0)
            {
                return 0;
            }
            int best = -9999;
            int moveValue = 0;
            foreach (move m in legalMoves(board, 0))
            {
                ChessBoard tempBoard = new ChessBoard(board);
                tempBoard.ActionPiece(m.from, m.to, true);
                moveValue = evaluateBoard(tempBoard) + minVal(tempBoard, depth - 1);
                if (moveValue > best)
                {
                    best = moveValue;
                }
            }
            return best;
        }
        */

        //best move for computer
        move bestMove(ChessBoard board, int depth)
        {
            List<move> bestMoves = new List<move>();
            int max = -9999;
            int moveValue = 0;
            foreach(move m in legalMoves(board, 0))
            {
                ChessBoard tempBoard = new ChessBoard(board);
                tempBoard.ActionPiece(m.from, m.to, true);
                moveValue = Minimax(tempBoard, depth, 1, -2147483648, 2147483647) + evaluateBoard(tempBoard);
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
                    int value = evaluateBoard(tempBoard) + Minimax(board, depth - 1, 1, alpha, beta);
                    bestVal = max(bestVal, value);
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
                    int value = evaluateBoard(tempBoard) + Minimax(board, depth - 1, 0, alpha, beta);
                    bestVal = min(bestVal, value);
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
    }
}