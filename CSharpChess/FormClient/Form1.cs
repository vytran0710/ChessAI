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
                chessBoard.ActionPiece(selectedPiece.x, selectedPiece.y, a.Column - 1, a.Row - 1);
                selectedPlayer = -1;
                AI();
                DrawPieces(chessBoard);
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

        //AI player
        private void AI()
        {
            moveRandomly(chessBoard);
        }

        private List<move> legalMoves(ChessBoard board)
        {
            List<move> legalMoves = new List<move>();
            for (int x = 0; x < boardLayoutPanel.ColumnCount-1; x++)
            {
                for (int y = 0; y < boardLayoutPanel.RowCount-1; y++)
                {
                    if(board[x, y] != null && board[x, y].Player == 0)
                    {
                        if(chessBoard.getActions(x, y) != null)
                        {
                            foreach (Chess.Point point in chessBoard.getActions(x, y))
                            {
                                legalMoves.Add(new move(new Chess.Point(x, y), point));
                            }
                        }
                    }
                }
            }
            return legalMoves;
        }

        private void moveRandomly(ChessBoard board)
        {
            Random r = new Random();
            int i = r.Next(0, legalMoves(board).Count() - 1);
            chessBoard.ActionPiece(legalMoves(board)[i].from, legalMoves(board)[i].to);
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
                                totalEvaluation += 10;
                                break;
                            case "Chess.Knight":
                                totalEvaluation += 30;
                                break;
                            case "Chess.Bishop":
                                totalEvaluation += 30;
                                break;
                            case "Chess.Rook":
                                totalEvaluation += 50;
                                break;
                            case "Chess.Queen":
                                totalEvaluation += 90;
                                break;
                            case "Chess.King":
                                totalEvaluation += 900;
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
                                totalEvaluation -= 10;
                                break;
                            case "Chess.Knight":
                                totalEvaluation -= 30;
                                break;
                            case "Chess.Bishop":
                                totalEvaluation -= 30;
                                break;
                            case "Chess.Rook":
                                totalEvaluation -= 50;
                                break;
                            case "Chess.Queen":
                                totalEvaluation -= 90;
                                break;
                            case "Chess.King":
                                totalEvaluation -= 900;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return totalEvaluation;
        }

        //nodes
        struct node
        {
            public ChessBoard board;
            public ChessBoard prevBoard;
            public node (ChessBoard board, ChessBoard prevBoard)
            {
                this.board = board;
                this.prevBoard = prevBoard;
            }
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
    }
}
