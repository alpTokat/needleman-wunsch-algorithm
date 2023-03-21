using System.Diagnostics;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;

namespace DnaApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {



            Stopwatch sw = new Stopwatch();
            sw.Start();
            DataGridViewRow row = new DataGridViewRow();



            string[] sequences = new string[2];

            string dirPath = Directory.GetCurrentDirectory();
            string txtPath1 = dirPath + "\\seq1.txt";
            string txtPath2 = dirPath + "\\seq2.txt";

            string seq1 = fileRead(txtPath1);
            string seq2 = fileRead(txtPath2);

            int seq1Length = Convert.ToInt32(seq1[0].ToString());
            int seq2Length = Convert.ToInt32(seq2[0].ToString());

            seq1 = seq1.Remove(0, 1); // seq1 remove the length side
            seq2 = seq2.Remove(0, 1); // saat kaç oldu




            string[,] matrix = makeMatrix(seq1, seq2, seq1Length, seq2Length);


            matrix = fiilMatrix(matrix);

            showMatrix(matrix, dataTable1);

            List<string> indexesList = findPath(matrix);

            drawThePath(indexesList, dataTable1);


            sequences = createdSequence(indexesList, matrix).Split(",");


            int score = calculateScore(sequences[0], sequences[1]);

            sw.Stop();


            label1.Text = sw.Elapsed.Milliseconds.ToString();
            label2.Text = sequences[0].ToString();
            label3.Text = sequences[1].ToString();
            label4.Text = score.ToString();

            Console.ReadLine();


        }


        public static string fileRead(string path)
        {
            StreamReader sr = new StreamReader(path);
            string seq = "";
            string line = sr.ReadLine();


            while (line != null)
            {
                seq += line;
                line = sr.ReadLine();

            }
            return seq;
        }


        public static string[,] makeMatrix(string seq1, string seq2, int seq1Length, int seq2Length)
        {
            string[,] matrix = new string[seq1Length + 2, seq2Length + 2];

            matrix[1, 1] = "0";
            for (int x = 0; x < seq1Length; x++)
            {
                matrix[0, x + 2] = seq1[x].ToString();
                matrix[x + 2, 0] = seq2[x].ToString();
            }
            return matrix;
        }


        public static string[,] fiilMatrix(string[,] matrix)
        {
            int? max = null;
            for (int j = 1; j < matrix.GetLength(0); j++)
            {
                for (int i = 2; i < matrix.GetLength(1); i++) // 0,1   ,   0,2   -1 , 0,1
                {
                    max = null;
                    // çapraz kontrol :   matrix[j - 1, i - 1];
                    // sol kontrol :      matrix[j, i - 1];
                    // üst kontrol :      matrix[j - 1, i];
                    if (j - 1 != 0 && i - 1 != 0)
                    {
                        if (max == null || max < Convert.ToInt32(matrix[j - 1, i - 1]))
                        {
                            max = Convert.ToInt32(matrix[j - 1, i - 1]);
                        }
                        if (matrix[j, 0] == matrix[0, i])
                        {
                            max += 1;
                        }
                        else
                        {
                            max -= 1;
                        }
                    }
                    if (i - 1 != 0)
                    {
                        if (max == null || max < Convert.ToInt32(matrix[j, i - 1]) - 2)    // i artan deðer olduðu için yatayda sutün deðiþen olacaðý için i yi virgülden sonra verildi.
                        {
                            max = Convert.ToInt32(matrix[j, i - 1]) - 2;
                        }
                    }
                    if (j - 1 != 0)
                    {
                        if (max == null || max < Convert.ToInt32(matrix[j - 1, i]) - 2)
                        {
                            max = Convert.ToInt32(matrix[j - 1, i]) - 2;
                        }
                    }
                    matrix[j, i] = max.ToString();
                }
                for (int i = 2; i < matrix.GetLength(1); i++) // 1,0    ,    2,0   -1 , 1,0
                {
                    max = null;
                    // çapraz kontrol : matrix[i-1,j-1]
                    // sol kontrol    : matrix[i, j - 1];
                    // üst kontrol    : matrix[i-1,j];
                    if (i - 1 != 0 && j - 1 != 0)
                    {
                        if (max == null || max < Convert.ToInt32(matrix[i - 1, j - 1]))
                        {
                            max = Convert.ToInt32(matrix[i - 1, j - 1]);
                        }
                        if (matrix[i, 0] == matrix[0, j])
                        {
                            max += 1;
                        }
                        else
                        {
                            max -= 1;
                        }
                    }
                    if (j - 1 != 0)
                    {
                        if (max == null || max < Convert.ToInt32(matrix[i, j - 1]) - 2) // dikeyde satýr önemli olduðu için virgülden öncede satýrý temsil ettiði için i virgülden önce verildi.
                        {
                            max = Convert.ToInt32(matrix[i, j - 1]) - 2;
                        }
                    }
                    if (i - 1 != 0)
                    {
                        if (max == null || max < Convert.ToInt32(matrix[i - 1, j]) - 2)
                        {
                            max = Convert.ToInt32(matrix[i - 1, j]) - 2;
                        }
                    }
                    matrix[i, j] = max.ToString();

                }
            }
            return matrix;
        }

        public static void showMatrix(string[,] matrix, DataGridView dataGrid)
        {
            dataGrid.ColumnCount = matrix.GetLength(0);

            dataGrid.RowCount = matrix.GetLength(1);
            for (int j = 0; j < matrix.GetLength(0); j++)
            {
                for (int i = 0; i < matrix.GetLength(1); i++)
                {
                    dataGrid.Rows[j].Cells[i].Value = matrix[j, i];
                }
            }
        }


        public static List<string> findPath(string[,] matrix)
        {
            int kontrolRowIndex = matrix.GetLength(0) - 1;
            int kontrolColumnIndex = matrix.GetLength(1) - 1;

            int tempRowIndex = kontrolRowIndex; ;
            int tempColumnIndex = kontrolColumnIndex;

            int maxValue;

            List<string> indexesList = new List<string>();
            indexesList.Add(kontrolRowIndex + "" + kontrolColumnIndex);
            while (kontrolRowIndex != 1 && kontrolColumnIndex != 1)
            {
                if (matrix[kontrolRowIndex, 0] == matrix[0, kontrolColumnIndex])
                {
                    tempRowIndex -= 1;
                    tempColumnIndex -= 1;
                }
                else
                {
                    maxValue = Convert.ToInt32(matrix[kontrolRowIndex, kontrolColumnIndex - 1]);
                    tempColumnIndex = kontrolColumnIndex - 1;
                    if (maxValue < Convert.ToInt32(matrix[kontrolRowIndex - 1, kontrolColumnIndex - 1]))
                    {
                        maxValue = Convert.ToInt32(matrix[kontrolRowIndex - 1, kontrolColumnIndex - 1]);
                        tempRowIndex = kontrolRowIndex - 1;
                        tempColumnIndex = kontrolColumnIndex - 1;
                    }
                    if (maxValue < Convert.ToInt32(matrix[kontrolRowIndex - 1, kontrolColumnIndex]))
                    {
                        maxValue = Convert.ToInt32(matrix[kontrolRowIndex - 1, kontrolColumnIndex]);
                        tempRowIndex = kontrolRowIndex - 1;
                    }
                }
                    kontrolRowIndex = tempRowIndex;
                    kontrolColumnIndex = tempColumnIndex;
                    indexesList.Add(kontrolRowIndex + "" + kontrolColumnIndex);
                
            }
            return indexesList;
        }


        public static int calculateScore(string seq1, string seq2)
        {
            int score = 0;
            for (int i = 0; i < seq1.Length; i++)
            {
                if (seq1[i] == '-' || seq2[i] == '-')
                {
                    score += -2;
                }
                else if (seq1[i] == seq2[i])
                {
                    score += 1;
                }
                else
                {
                    score -= 1;
                }
            }
            return score;
            
        }




        public static string createdSequence(List<string> List, string[,] matrix)
        {
            string result = "";
            string value = "", seq1 = "", seq2 = "";
            int rowIndex, columnIndex, rowIndexOnceki = 0, columnIndexOncesi = 0, i = 0;
            List.Reverse();
            while (value != List[List.Count - 1])
            {
                value = List[i];
                rowIndex = Convert.ToInt32(value[0].ToString());
                columnIndex = Convert.ToInt32(value[1].ToString());

                if (matrix[rowIndex, 0] != null && rowIndexOnceki != rowIndex)
                {
                    seq1 += matrix[rowIndex, 0];
                }
                if (matrix[0, columnIndex] != null && columnIndex != columnIndexOncesi)
                {
                    seq2 += matrix[0, columnIndex];
                }
                if (matrix[rowIndex, 0] == null || rowIndex == rowIndexOnceki)
                {
                    seq1 += "-";
                }
                if (matrix[0, columnIndex] == null || columnIndex == columnIndexOncesi)
                {
                    seq2 += "-";
                }
                rowIndexOnceki = rowIndex;
                columnIndexOncesi = columnIndex;
                i++;
            }
            result = seq1 + "," + seq2;

            return result;
        }





        public static void drawThePath(List<string> List, DataGridView dataGrid)
        {
            string value= "";
            int rowIndex, columnIndex;
            for (int i = 0; i < List.Count ; i++)
            {
                value = List[i];
                rowIndex = Convert.ToInt32(value[0].ToString());
                columnIndex = Convert.ToInt32(value[1].ToString());

                dataGrid.Rows[rowIndex].Cells[columnIndex].Style.BackColor = Color.Aqua;

            }
            


        }


    }
}