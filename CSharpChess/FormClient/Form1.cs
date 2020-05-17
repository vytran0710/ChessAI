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

namespace FormClient
{
    public partial class Form1 : Form
    {
        ChessBoard chessBoard = new ChessBoard();
        Chess.Point selectedPiece = new Chess.Point();
        int selectedPlayer = -1;

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
                        AI();
                    }
                    selectedPlayer = -1;
                    DrawPieces(chessBoard);
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
                    AI();
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
                        button.Text = chessPiece.ToString().Replace("Chess.", "");
                        if (chessPiece.Player == 1) button.ForeColor = Color.White;
                        else button.ForeColor = Color.Black;
                    }
                    else
                    {
                        button.Text = "";
                        button.Tag = null;
                    }
                    this.coordinates.SetToolTip(button, String.Format("({0}, {1})", x, y));
                }
            }
        }

        //AI player
        private void AI()
        {
            move temp = bestMove(chessBoard, 1);
            chessBoard.ActionPiece(temp.from, temp.to, true);
        }

        private List<move> legalMoves(ChessBoard board, int playerTurn)
        {
            List<move> legalMoves = new List<move>();
            for (int x = 0; x < boardLayoutPanel.ColumnCount-1; x++)
            {
                for (int y = 0; y < boardLayoutPanel.RowCount-1; y++)
                {
                    if(board[x, y] != null && board[x, y].Player == playerTurn)
                    {
                        if(board.getActions(x, y) != null)
                        {
                            foreach (Chess.Point point in board.getActions(x, y))
                            {
                                legalMoves.Add(new move(new Chess.Point(x, y), point));
                            }
                        }
                    }
                }
            }
            return legalMoves;
        }

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
                                totalEvaluation -= 90;
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
                                totalEvaluation += 90;
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

        //min player: human player
        int min(ChessBoard board, int depth)
        {
            int worst = 9999;
            int moveValue = 0;
            Stack<ChessBoard> sB = new Stack<ChessBoard>();
            foreach(move m in legalMoves(board, 1))
            {
                ChessBoard tempBoard = new ChessBoard(board);
                tempBoard.ActionPiece(m.from, m.to, true);
                sB.Push(tempBoard);
                if (depth != 0)
                {
                    moveValue = evaluateBoard(tempBoard) + max(tempBoard, depth - 1);
                }
                else
                {
                    sB.Pop();
                    return 0;
                }
                if(moveValue < worst)
                {
                    worst = moveValue;
                }
                sB.Pop();
            }
            return worst;
        }

        //max player: computer
        int max(ChessBoard board, int depth)
        {
            int best = -9999;
            int moveValue = 0;
            Stack<ChessBoard> sB = new Stack<ChessBoard>();
            foreach (move m in legalMoves(board, 0))
            {
                ChessBoard tempBoard = new ChessBoard(board);
                tempBoard.ActionPiece(m.from, m.to, true);
                sB.Push(tempBoard);
                if (depth != 0)
                {
                    moveValue = evaluateBoard(tempBoard) + max(tempBoard, depth - 1);
                }
                else
                {
                    sB.Pop();
                    return 0;
                }
                if (moveValue > best)
                {
                    best = moveValue;
                }
                sB.Pop();
            }
            return best;
        }

        //best move for computer
        move bestMove(ChessBoard board, int depth)
        {
            List<move> bestMoves = new List<move>();
            int max = -9999;
            int moveValue = 0;
            Stack<ChessBoard> sB = new Stack<ChessBoard>();
            foreach(move m in legalMoves(board, 0))
            {
                ChessBoard tempBoard = new ChessBoard(board);
                tempBoard.ActionPiece(m.from, m.to, true);
                sB.Push(tempBoard);
                moveValue = min(tempBoard, 2) + evaluateBoard(tempBoard);
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
                sB.Pop();
            }
            Random r = new Random();
            return bestMoves[(int)r.Next(0, bestMoves.Count()-1)];
        }
    }
}