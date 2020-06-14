using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;
using System.Diagnostics;
using System.Windows.Forms;


namespace CourseWorkDO
{
    public partial class Main : Form
    {
        // Получим панель для рисования
        OpenFileDialog ofd = new OpenFileDialog();
        DialogResult dresult;
        GraphPane pane;
        int n = 0;
        int[] arrayA = new int[2];
        int[] arrayB = new int[2];
        int[][] arrayP = new int[2][];
        int[][] arrayT = new int[2][];
        int[][] M = new int[2][];
        int F = 0;
        double degreeA = 0.0;
        double degreeB = 0.0;
        int k = 0;
        int r = 0;
        double q = 0.0;
        double f = 0.0;
        int varNull = 0;
        long time;

        public Main()
        {

            InitializeComponent();
            DrawTime(zedGraphControl1);
            DrawRate(zedGraphControl2);
            DrawRateAnt(zedGraphControl3);
            DrawTimeGrDy(zedGraphControl4);
            textBoxWriteData.Text = "";
            if (radioDataFile.Checked == true)
            {
                groupBoxReadFile.Enabled = true;
                textBoxWriteData.ReadOnly = true;
                groupBoxRandChoose.Enabled = false;
                //groupBoxWriteData.Text = "Дані з файлу";
            }
            
        }
        public void DrawTime(ZedGraphControl graph)
        {
            GraphPane myPane = graph.GraphPane;
            myPane.Title.Text = "Залежність часу виконання від кількості робочих місць";
            myPane.XAxis.Title.Text = "Кількість робочих місць";
            myPane.YAxis.Title.Text = "Час";
            PointPairList list1 = new PointPairList();
            PointPairList list2 = new PointPairList();
            PointPairList list3 = new PointPairList();
            int i = 5;
            while(i < 101) 
            {
                generateRandTask(i);
                GreedyAlg(i, arrayA, arrayB, arrayP, arrayT);
                list1.Add(i, time);
                DynamicAlg(i, arrayA, arrayB, arrayP, arrayT);
                list2.Add(i, time);
                AntAlg(i, arrayA, arrayB, arrayP, arrayT, 0.6, 0.6, 10, 10, 0.8, 0.1);
                list3.Add(i, time);
                i += 5;
            }

            LineItem myCurve = myPane.AddCurve("Greedy",
                  list1, Color.Red, SymbolType.Diamond);
            LineItem myCurve2 = myPane.AddCurve("Dynamic",
                  list2, Color.Blue, SymbolType.Circle);
            LineItem myCurve3 = myPane.AddCurve("Ant",
                  list3, Color.Green, SymbolType.Star);
            graph.AxisChange();
        }
        public void DrawTimeGrDy(ZedGraphControl graph)
        {
            GraphPane myPane = graph.GraphPane;
            myPane.Title.Text = "Залежність часу виконання від кількості робочих місць";
            myPane.XAxis.Title.Text = "Кількість робочих місць";
            myPane.YAxis.Title.Text = "Час";
            PointPairList list1 = new PointPairList();
            PointPairList list2 = new PointPairList();
            int i = 5;
            while (i < 501)
            {
                generateRandTask(i);
                GreedyAlg(i, arrayA, arrayB, arrayP, arrayT);
                list1.Add(i, time);
                DynamicAlg(i, arrayA, arrayB, arrayP, arrayT);
                list2.Add(i, time);
                i += 5;
            }
            LineItem myCurve = myPane.AddCurve("Greedy",
                  list1, Color.Red, SymbolType.Diamond);
            LineItem myCurve2 = myPane.AddCurve("Dynamic",
                  list2, Color.Blue, SymbolType.Circle);
            graph.AxisChange();
        }
        public void DrawRate(ZedGraphControl graph)
        {
            GraphPane myPane = graph.GraphPane;
            myPane.Title.Text = "Досягнення оптимуму";
            myPane.XAxis.Title.Text = "Кількість рабочих місць";
            myPane.YAxis.Title.Text = "Відсоток досягнення оптимуму";
            PointPairList list1 = new PointPairList();
            PointPairList list2 = new PointPairList();
            PointPairList list3 = new PointPairList();
            int i = 5;
            while (i < 101)
            {
                generateRandTask(i);

                DynamicAlg(i, arrayA, arrayB, arrayP, arrayT);
                double fOpt = F;
                list2.Add(i, fOpt/F);
                GreedyAlg(i, arrayA, arrayB, arrayP, arrayT);
                list1.Add(i, fOpt / F);
                double minF = 99999999999999;
                for (int j = 1; j < 5; j++)
                {
                    AntAlg(i, arrayA, arrayB, arrayP, arrayT, 0.6, 0.6, 10, 10, 0.8, 0.1);
                    if (minF > F)
                        minF = F;
                }
                list3.Add(i, fOpt / minF);
                i += 5;
            }

            LineItem myCurve = myPane.AddCurve("Greedy",
                  list1, Color.Red, SymbolType.Diamond);
            LineItem myCurve2 = myPane.AddCurve("Dynamic",
                  list2, Color.Blue, SymbolType.Circle);
            LineItem myCurve3 = myPane.AddCurve("Ant",
               list3, Color.Green, SymbolType.Star);
            graph.AxisChange();
        }
        public void DrawRateAnt(ZedGraphControl graph)
        {
            GraphPane myPane = graph.GraphPane;
            myPane.Title.Text = "Досягнення оптимуму";
            myPane.XAxis.Title.Text = "Кількість ітерацій";
            myPane.YAxis.Title.Text = "Відсоток досягнення оптимуму";
            PointPairList list1 = new PointPairList();
            PointPairList list2 = new PointPairList();
            int i = 10;
            while (i < 501)
            {
                generateRandTask(10);
                DynamicAlg(10, arrayA, arrayB, arrayP, arrayT);
                double fOpt = F;
                list2.Add(i, fOpt / F);
                AntAlg(10, arrayA, arrayB, arrayP, arrayT, 0.6, 0.6, 20, i, 0.8, 0.1);
                list1.Add(i, fOpt / F);
                i += 20;
            }
            LineItem myCurve = myPane.AddCurve("Ant",
                  list1, Color.Red, SymbolType.Diamond);
            LineItem myCurve2 = myPane.AddCurve("Optimum",
                  list2, Color.Blue, SymbolType.Circle);
            graph.AxisChange();
        }
        
        
        private void buttonFind_Click(object sender, EventArgs e)
        {
            if (radioDataFile.Checked == true)
            {
                if (dresult == DialogResult.OK)
                {
                    try
                    {
                        String file = ofd.FileName;
                        String text = File.ReadAllText(file);
                        string[] arr = File.ReadAllLines(file);
                        string stringData = String.Join(Environment.NewLine, arr);
                        defineArrays(stringData);
                        checkedAlgorithm();
                        showAlgAnswer();
                        if (checkBoxSaveFile.Checked == true)
                        {
                            dataFileOutput();
                        }
                    }
                    catch
                    {
                        textBoxAnswer.Text = "Неправильний формат даних";
                    }                }
                else
                {
                    textBoxAnswer.Text = "Оберіть файл";
                }
            }
            else if (radioDataWrite.Checked == true) 
            {
                String stringData = textBoxWriteData.Text;
                if (stringData == "")
                {
                    textBoxAnswer.Text = "Введіть дані";
                }
                else
                {
                    try
                    {
                        defineArrays(stringData);
                        checkedAlgorithm();
                        showAlgAnswer();
                        if (checkBoxSaveFile.Checked == true)
                        {
                            dataFileOutput();
                        }
                    }catch 
                    {
                        textBoxAnswer.Text = "Неправильний формат даних";
                    }
                    
                }
            }
            else if (radioRandom.Checked == true) 
            {
                String stringData = textBoxWriteData.Text;
                if (stringData == "")
                {
                    textBoxAnswer.Text = "Згенеруйте дані";
                }
                else
                {
                    try
                    {
                        defineArrays(stringData);
                        checkedAlgorithm();
                        showAlgAnswer();
                        if (checkBoxSaveFile.Checked == true)
                        {
                            dataFileOutput();
                        }
                    }
                    catch
                    {
                        textBoxAnswer.Text = "Неправильний формат даних";
                    }
                    
                }


            }

        }
        public void dataFileOutput()
        {
            string fileName = System.IO.Path.Combine(Environment.CurrentDirectory, "output.txt");
            try
            {
                using (StreamWriter sw = new StreamWriter(fileName, true, System.Text.Encoding.Default))
                {
                    if (radioAlgGreedy.Checked == true)
                    {
                        sw.WriteLine("Жадібний алгоритм: ");
                        
                    }
                    else if (radioAlgDynamic.Checked == true)
                    {
                        sw.WriteLine("Алгоритм динамічного програмування: ");
                    }
                    else if (radioAlgAnt.Checked == true)
                    {
                        sw.WriteLine("Алгоритм мурашиних колоній: ");
                    }
                    sw.WriteLine("Кількість робочих місць (n): " + n);
                    sw.WriteLine("Вектор A: " + arrayA[0] + " " + arrayA[1]);
                    sw.WriteLine("Вектор В: " + arrayB[0] + " " + arrayB[1]);
                    sw.WriteLine("Масив Р: ");
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            sw.Write(arrayP[i][j].ToString() + " ");
                        }
                        sw.Write("\n");
                    }
                    sw.WriteLine("Масив T: ");
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < n - 1; j++)
                        {
                            sw.Write(arrayT[i][j].ToString() + " ");
                        }
                        sw.Write("\n");
                    }
                    sw.WriteLine("Степінь стадності: " + degreeA);
                    sw.WriteLine("Степінь жадібності: " + degreeB);
                    sw.WriteLine("Кількість мурах: " + k);
                    sw.WriteLine("Кількість ітерацій: " + r);
                    sw.WriteLine("Стала випаровування: " + q);
                    sw.WriteLine("Початковий рівень феромону: " + f);
                    sw.WriteLine("\nРезультати виконання алгоритму: ");
                    sw.WriteLine("Матриця М: ");
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            sw.Write(M[i][j] + " "); 
                        }
                        sw.Write("\n");
                    }
                    sw.WriteLine("Загальний витрачений час (F): " + F);
                    sw.WriteLine("-------------------------------------");
                }
            }
            catch (Exception e)
            {
                textBoxAnswer.Text = e.Message;
            }
        }
        public void showAlgAnswer()
        {
            textBoxAnswer.Text += "M: " + Environment.NewLine;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    textBoxAnswer.Text += M[i][j].ToString() + " ";

                }
                textBoxAnswer.Text += Environment.NewLine;
            }
            textBoxAnswer.Text += "F: "+F;
        }
        public void checkedAlgorithm()
        {
            if (radioAlgGreedy.Checked == true)
            {
                GreedyAlg(n, arrayA, arrayB, arrayP, arrayT);
                textBoxAnswer.Text = "Жадібний алгоритм: " + Environment.NewLine;
            }
            else if (radioAlgDynamic.Checked == true)
            {
                DynamicAlg(n, arrayA, arrayB, arrayP, arrayT);
                textBoxAnswer.Text = "Алгоритм динамічного програмування: " + Environment.NewLine;
            }
            else if (radioAlgAnt.Checked == true)
            {
                AntAlg(n, arrayA, arrayB, arrayP, arrayT, degreeA, degreeB, k, r, q, f);
                textBoxAnswer.Text = "Алгоритм мурашиних колоній: " + Environment.NewLine;
            }
        }
        public void defineArrays(String stringData)
        {
            char patternEnter = '\n';
            char patternSpace = ' ';
            String[] data = stringData.Split(patternEnter);
            n = Int32.Parse(data[0]);

            for (int i = 0; i < 2; i++)
            {
                arrayP[i] = new int[n];
                arrayT[i] = new int[n - 1];
            }
            //read and define arrayA
            for (int i = 0; i < 2; i++)
            {
                String[] temp = data[1].Split(patternSpace);
                arrayA[i] = int.Parse(temp[i]);
            }
            //read and define arrayB
            for (int i = 0; i < 2; i++)
            {
                String[] temp = data[2].Split(patternSpace);
                arrayB[i] = int.Parse(temp[i]);
            }
            //read and define arrayP
            for (int i = 0; i <= 1; i++)
                for (int j = 0; j < n; j++)
                {
                    String[] temp = data[i + 3].Split(patternSpace);
                    arrayP[i][j] = int.Parse(temp[j]);

                }
            //read and define arrayT
            for (int i = 0; i <= 1; i++)
                for (int j = 0; j < n - 1; j++)
                {
                    String[] temp = data[i + 5].Split(patternSpace);
                    arrayT[i][j] = int.Parse(temp[j]);

                }

            //if (radioAlgAnt.Checked == true)
            //{
                degreeA = Convert.ToDouble(data[7]);
                degreeB = Convert.ToDouble(data[8]);
                k = int.Parse(data[9]);
                r = int.Parse(data[10]);
                q = Convert.ToDouble(data[11]);
                f = Convert.ToDouble(data[12]);
            //}

    }
        public void showFileData()
        {
            textBoxWriteData.Text = /*"n: " + */n + Environment.NewLine;
            textBoxWriteData.Text += /*"ArrayA: " +*/ arrayA[0] + " " + arrayA[1] + Environment.NewLine;
            textBoxWriteData.Text += /*"ArrayB: " +*/ arrayB[0] + " " + arrayB[1] + Environment.NewLine;
            //textBoxWriteData.Text += /*"ArrayP: " +*/ Environment.NewLine;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    textBoxWriteData.Text += arrayP[i][j].ToString() + " ";

                }
                textBoxWriteData.Text += Environment.NewLine;
            }
            //textBoxWriteData.Text += /*"ArrayT: " +*/ Environment.NewLine;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    textBoxWriteData.Text += arrayT[i][j].ToString() + " ";

                }
                textBoxWriteData.Text += Environment.NewLine;
            }
            textBoxWriteData.Text += degreeA + Environment.NewLine;
            textBoxWriteData.Text += degreeB + Environment.NewLine;
            textBoxWriteData.Text += k + Environment.NewLine;
            textBoxWriteData.Text += r + Environment.NewLine;
            textBoxWriteData.Text += q + Environment.NewLine;
            textBoxWriteData.Text += f + Environment.NewLine;
        }
        public void showEnterDataAnswer()
        {
            textBoxAnswer.Text = "n: " + n + Environment.NewLine;
            textBoxAnswer.Text += "ArrayA: " + arrayA[0] + " " + arrayA[1] + Environment.NewLine;
            textBoxAnswer.Text += "ArrayB: " + arrayB[0] + " " + arrayB[1] + Environment.NewLine;
            textBoxAnswer.Text += "ArrayP: " + Environment.NewLine;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    textBoxAnswer.Text += arrayP[i][j].ToString() + " ";

                }
                textBoxAnswer.Text += Environment.NewLine;
            }
            textBoxAnswer.Text += "ArrayT: " + Environment.NewLine;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < n - 1; j++)
                {
                    textBoxAnswer.Text += arrayT[i][j].ToString() + " ";

                }
                textBoxAnswer.Text += Environment.NewLine;
            }
        }
        private void radioDataFile_CheckedChanged(object sender, EventArgs e)
        {
            textBoxWriteData.Text = "";
            //groupBoxWriteData.Enabled = true;
            groupBoxReadFile.Enabled = true;
            textBoxWriteData.ReadOnly = true;
            //groupBoxWriteData.Text = "Дані з файлу";
            groupBoxRandChoose.Enabled = false;
            if (dresult == DialogResult.OK)
            {
                readFileData();
                showFileData();
            }
            //readFileData();
            //showFileData();
        }

        private void radioDataWrite_CheckedChanged(object sender, EventArgs e)
        {
            textBoxWriteData.Text = "";
            //groupBoxWriteData.Enabled = true;
            groupBoxReadFile.Enabled = false;
            textBoxWriteData.ReadOnly = false;
            //groupBoxWriteData.Text = "Введення даних вручну";
            groupBoxRandChoose.Enabled = false;
        }
        private void radioRandom_CheckedChanged(object sender, EventArgs e)
        {
            textBoxWriteData.Text = "";
            //groupBoxWriteData.Enabled = true;
            groupBoxReadFile.Enabled = false;
            textBoxWriteData.ReadOnly = true;
            //groupBoxWriteData.Text = "Випадкові значення";
            groupBoxRandChoose.Enabled = true;
        }
        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            ofd.Filter = "(*.TXT;*.RTF)|*.TXT;*.RTF;";
            dresult = ofd.ShowDialog();

            if (dresult == DialogResult.OK)
            {
                labelFileName.Text = "Назва файлу: " + ofd.SafeFileName;
                readFileData();
                showFileData();
            }
        }
        public void readFileData()
        {
            String file = ofd.FileName;
            string[] arr = File.ReadAllLines(file);
            string stringData = String.Join(Environment.NewLine, arr);
            defineArrays(stringData);
        }
        
        public void DynamicAlg(int n, int[] A, int[] B, int[][] P, int[][] T)
        {
            var sw = new Stopwatch();
            sw.Start();
            int[][] f = new int[2][];
            int[][] X = new int[2][];
            for (int i = 0; i < 2; i++)
            {
                M[i] = new int[n];
                f[i] = new int[n];
                X[i] = new int[n];
            }
            for (int i = 0; i < 2; i++)
            {
                for (int pointer2 = 0; pointer2 < n; pointer2++)
                    M[i][pointer2] = 0;
            }

            f[0][0] = A[0] + P[0][0];
            f[1][0] = A[1] + P[1][0];
            int j = 0;

            while (j < n - 1)
            {
                j += 1;
                f[0][j] = Math.Min((f[0][j - 1] + P[0][j]), (f[1][j - 1] + P[0][j] + T[1][j - 1]));
                if ((f[0][j - 1] + P[0][j]) < (f[1][j - 1] + P[0][j] + T[1][j - 1]))
                {
                    X[0][j] = 0;
                }
                else
                {
                    X[0][j] = 1;
                }
                f[1][j] = Math.Min((f[1][j - 1] + P[1][j]), (f[0][j - 1] + P[1][j] + T[0][j - 1]));
                if ((f[1][j - 1] + P[1][j]) < (f[0][j - 1] + P[1][j] + T[0][j - 1]))
                {
                    X[1][j] = 1;
                }
                else
                {
                    X[1][j] = 0;
                }
            }
            F = Math.Min((f[0][j] + B[0]), (f[1][j] + B[1])); //n-1
            int Xlast;
            if ((f[0][j] + B[0]) < (f[1][j] + B[1]))
            {
                Xlast = 0;
                M[0][j] = 1;
            }
            else
            {
                Xlast = 1;
                M[1][j] = 1;
            }
            while (j > 0)
            {
                if (X[Xlast][j] == 0)
                {
                    M[0][j - 1] = 1;
                    Xlast = 0;
                }
                else
                {
                    M[1][j - 1] = 1;
                    Xlast = 1;
                }
                j -= 1;
            }
            sw.Stop();
            time = (sw.ElapsedTicks);
        }
        public void GreedyAlg(int n, int[] A, int[] B, int[][] P, int[][] T)
        {
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 2; i++)
            {
                M[i] = new int[n];
            }
            F = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int pointer2 = 0; pointer2 < n; pointer2++)
                    M[i][pointer2] = 0;
            }
            if ((A[0] + P[0][0]) > (A[1] + P[1][0]))
            {
                M[1][0] = 1;
                F += A[1] + P[1][0];
            }
            else
            {
                M[0][0] = 1;
                F += A[0] + P[0][0];
            }
            int j = 1;
            while (j < n-1)
            {
                if (M[0][j - 1] == 1)
                {
                    if ((P[0][j]) > (T[0][j - 1] + P[1][j]))
                    {
                        M[1][j] = 1;
                        F += T[0][j - 1] + P[1][j];
                    }
                    else
                    {
                        M[0][j] = 1;
                        F += P[0][j];
                    }
                }
                else
                {
                    if ((T[1][j - 1] + P[0][j]) > (P[1][j]))
                    {
                        M[1][j] = 1;
                        F += P[1][j];
                    }
                    else
                    {
                        M[0][j] = 1;
                        F += T[1][j - 1] + P[0][j];
                    }
                }
                j += 1;
            }
            if (M[0][j - 1] == 1)
            {
                if ((B[0] + P[0][j]) > (T[0][j - 1] + B[1] + P[1][j]))
                {
                    M[1][j] = 1;
                    F += T[0][j - 1] + B[1] + P[1][j];
                }
                else
                {
                    M[0][j] = 1;
                    F += B[0] + P[0][j];
                }
            }
            else
            {
                if ((T[1][j - 1] + B[0] + P[0][j]) > (B[1] + P[1][j]))
                {
                    M[1][j] = 1;
                    F += B[1] + P[1][j];
                }
                else
                {
                    M[0][j] = 1;
                    F += T[1][j - 1] + B[0] + P[0][j];
                }
            }
            sw.Stop();
            time = (sw.ElapsedTicks);
        }

        private void labelRandTime_Click(object sender, EventArgs e)
        {
            toolTipTime.SetToolTip(labelRandTime, "Час постановки деталі на конвеєр\nЧас зняття деталі з конвеєру\nЧас виготовлення деталі на робочому місці\nЧас переходу деталі на інший конвеєр");
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void panelAntRand_Paint(object sender, PaintEventArgs e)
        {

        }
        public void AntAlg(int n, int[] A, int[] B, int[][] P, int[][] T, double degreeA, double degreeB, int k, int r, double q, double f)
        {
            var sw = new Stopwatch();
            sw.Start();
            int[][] minM = new int[2][];
            for (int i = 0; i < 2; i++)
            {
                minM[i] = new int[n];
            }
            int[][] minMtemp = new int[2][];
            for (int i = 0; i < 2; i++)
            {
                minMtemp[i] = new int[n];
            }
            int z = n * 2 + 2; //num of vertices
            int minFinalLen = 0;
            for (int counter1 = 0; counter1 < r; counter1++)
            {
                double[][] arrayF = new double[z][];
                int[][] arrayY = new int[z][];
                double[][] arrayD = new double[z][];
                for (int i = 0; i < z; i++)
                {
                    arrayF[i] = new double[z];
                    arrayY[i] = new int[z];
                    arrayD[i] = new double[z];
                }
                //fill 
                for (int i = 0; i < z; i++)
                {
                    for (int j = 0; j < z; j++)
                    {
                        arrayY[i][j] = 0;
                        arrayD[i][j] = varNull;
                    }
                }
                arrayD[0][1] = arrayA[0] + arrayP[0][0];
                arrayD[0][2] = arrayA[1] + arrayP[1][0];
                arrayD[z - 3][z - 1] = arrayB[0];
                arrayD[z - 2][z - 1] = arrayB[1];
                for (int i = 0; i < z; i++)
                {
                    for (int j = 0; j < z; j++)
                    {
                        if (j > i)
                        {
                            if (((j - i == 1) && ((j + 1) % 2 != 1)) || j - i == 2 || ((j - i == 3) && ((j + 1) % 2 != 0)))
                            {
                                arrayF[i][j] = f;
                                arrayY[i][j] = 1;

                            }
                        }
                    }
                }
                for (int i = 0; i < z; i++)
                {
                    for (int j = 0; j < z; j++)
                    {
                        if (arrayY[i][j] == 1)
                        {
                            if (i > 0 && j < z - 1)
                            {
                                if ((i + 1) % 2 == 0 && (j - i == 3) && (i + 1 < n * 2))
                                {
                                    arrayD[i][j] = arrayT[0][((i + 1) / 2) - 1] + arrayP[1][((j + 1) / 2) - 1];
                                }
                                else if ((i + 1) % 2 == 1 && j - i == 1 && i + 1 < n * 2)
                                {
                                    arrayD[i][j] = arrayT[1][((i + 1) / 2) - 1] + arrayP[0][((j + 1) / 2) - 1];
                                }
                                if ((i + 1) % 2 == 0 && (j + 1) % 2 == 0 && (i + 1 < n * 2))
                                {
                                    arrayD[i][j] = arrayP[0][((j + 1) / 2) - 1];
                                }
                                else if ((i + 1) % 2 == 1 && (j + 1) % 2 == 1 && (i + 1 < n * 2))
                                {
                                    arrayD[i][j] = arrayP[1][((j + 1) / 2) - 1];
                                }
                            }

                        }
                    }
                }
                //------------------------------------------
                double[][] tempNumTo = new double[2][];
                for (int i = 0; i < 2; i++)
                {
                    tempNumTo[i] = new double[3];
                }
                //------------------------------------------
                int numOfAntPath = n + 1;
                int numOfPathCounter = 0;
                double[][] arrayFromToPath = new double[numOfAntPath][];
                for (int i = 0; i < numOfAntPath; i++)
                {
                    arrayFromToPath[i] = new double[2];
                }
                //------------------------------------------
                double[][] arrayFromToFinal = new double[numOfAntPath][];
                for (int i = 0; i < numOfAntPath; i++)
                {
                    arrayFromToFinal[i] = new double[2];
                }

                Random rndDouble = new Random();
                int varFrstScnd = 1;
                int counter = 0;
                DynamicAlg(n, arrayA, arrayB, arrayP, arrayT); 

                for (int counter2 = 0; counter2 < k; counter2++)
                {
                    counter = 0;
                    numOfPathCounter = 0;
                    while (counter != z - 1)
                    {
                        int numOfPath = 0;
                        for (int j = 0; j < z; j++)
                        {
                            if (arrayY[counter][j] == 1)
                            {
                                if (varFrstScnd == 1)
                                {
                                    tempNumTo[0][0] = arrayD[counter][j];
                                    tempNumTo[0][1] = counter;
                                    tempNumTo[0][2] = j;
                                    varFrstScnd++;
                                    numOfPath++;
                                }
                                else /*if (varFrstScnd == 2)*/
                                {
                                    tempNumTo[1][0] = arrayD[counter][j];
                                    tempNumTo[1][1] = counter;
                                    tempNumTo[1][2] = j;
                                    varFrstScnd = 1;
                                    numOfPath++;
                                }
                            }
                        }
                        arrayFromToPath[numOfPathCounter][0] = counter; 
                        if (numOfPath == 2)
                        {
                            double probabilityTo1 = probabilityFunc(tempNumTo, 0, arrayF, z, arrayY);
                            double probabilityTo2 = probabilityFunc(tempNumTo, 1, arrayF, z, arrayY);
                            double randVar = rndDouble.NextDouble();
                            if (randVar <= probabilityTo1)
                            {
                                counter = Convert.ToInt32(tempNumTo[0][2]);
                            }
                            else if (randVar > probabilityTo1)
                            {
                                counter = Convert.ToInt32(tempNumTo[1][2]);
                            }
                        }
                        else if (numOfPath == 1)
                        {
                            counter = z - 1;
                        }
                        arrayFromToPath[numOfPathCounter][1] = counter;
                        numOfPathCounter++;
                    }
                    int lengthPath = 0;
                    for (int i = 0; i < numOfAntPath; i++)
                    {
                        lengthPath += Convert.ToInt32(arrayD[Convert.ToInt32(arrayFromToPath[i][0])][Convert.ToInt32(arrayFromToPath[i][1])]);
                    }
                    for (int i = 0; i < z; i++)
                    {
                        for (int j = 0; j < z; j++)
                        {
                            if (arrayY[i][j] == 1)
                            {
                                bool isBelongToPath = false;
                                for (int cs = 0; cs < numOfAntPath; cs++)
                                {
                                    if (i == Convert.ToInt32(arrayFromToPath[cs][0]) && j == Convert.ToInt32(arrayFromToPath[cs][1]))
                                    {
                                        isBelongToPath = true;
                                    }
                                }
                                if (isBelongToPath == true)
                                {
                                    double divideTemp = Convert.ToDouble(F) / lengthPath;
                                    double tempArrFel = arrayF[i][j];
                                    double addTempForArrF = (1.0 - q) * tempArrFel;
                                    double tempFer = addTempForArrF + divideTemp ;
                                    arrayF[i][j] = tempFer;
                                }
                                else if (isBelongToPath == false)
                                {
                                    double tempFer = (1.0 - q) * arrayF[i][j];
                                    arrayF[i][j] = tempFer;
                                }
                            }
                        }
                    }
                    for (int i = 0; i < numOfAntPath; i++)
                    {
                        arrayFromToPath[i][0] = 0;
                        arrayFromToPath[i][1] = 0;
                    }
                    numOfPathCounter = 0;
                    counter = 0;
                    while (counter != z - 1)
                    {
                        int numOfPath = 0;
                        for (int j = 0; j < z; j++)
                        {
                            if (arrayY[counter][j] == 1)
                            {
                                if (varFrstScnd == 1)
                                {
                                    tempNumTo[0][0] = arrayF[counter][j];
                                    tempNumTo[0][1] = counter;
                                    tempNumTo[0][2] = j;
                                    varFrstScnd++;
                                    numOfPath++;
                                }
                                else /*if (varFrstScnd == 2)*/
                                {
                                    tempNumTo[1][0] = arrayF[counter][j];
                                    tempNumTo[1][1] = counter;
                                    tempNumTo[1][2] = j;
                                    varFrstScnd = 1;
                                    numOfPath++;
                                }
                            }
                        }
                        arrayFromToFinal[numOfPathCounter][0] = counter;
                        if (numOfPath == 2)
                        {
                            if (tempNumTo[0][0] > tempNumTo[1][0])
                            {
                                counter = Convert.ToInt32(tempNumTo[0][2]);
                            }
                            else if (tempNumTo[1][0] > tempNumTo[0][0])
                            {
                                counter = Convert.ToInt32(tempNumTo[1][2]);
                            }
                        }
                        else if (numOfPath == 1)
                        {
                            counter = z - 1;
                        }
                        arrayFromToFinal[numOfPathCounter][1] = counter;
                        numOfPathCounter++;
                    }
                }
                int tempfinalLength = 0;
                for (int i = 0; i < numOfAntPath; i++)
                {
                    tempfinalLength += Convert.ToInt32(arrayD[Convert.ToInt32(arrayFromToFinal[i][0])][Convert.ToInt32(arrayFromToFinal[i][1])]);
                }
                int pathCounter = 0;
                for (int path = 1; path < numOfAntPath; path++)
                {
                    if ((arrayFromToFinal[path][0]+1) % 2 == 0)
                    {
                        minMtemp[0][pathCounter] = 1;
                        minMtemp[1][pathCounter] = 0;
                    }
                    else if ((arrayFromToFinal[path][0]+1) % 2 == 1)
                    {
                        minMtemp[0][pathCounter] = 0;
                        minMtemp[1][pathCounter] = 1;
                    }
                    pathCounter++;
                }
                if (minFinalLen == 0)
                {
                    minFinalLen = tempfinalLength;
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            minM[i][j] = minMtemp[i][j];
                        }
                    }
                }
                else if (tempfinalLength < minFinalLen)
                {
                    minFinalLen = tempfinalLength;
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            minM[i][j] = minMtemp[i][j];
                        }
                    }
                }
            }
            F = minFinalLen;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    M[i][j] = minM[i][j];
                }
            }
            sw.Stop();
            time = (sw.ElapsedTicks);
        }
        public double probabilityFunc(double[][] tempNumTo, int numTo, double[][] arrayF, int z, int[][] arrayY)
        {
            double[][] arrayFcopy = new double[z][];
            for (int i = 0; i < z; i++)
            {
                arrayFcopy[i] = new double[z];
            }
            //fill 
            for (int i = 0; i < z; i++)
            {
                for (int j = 0; j < z; j++)
                {
                    arrayFcopy[i][j] = arrayF[i][j];
                }
            }
            double upVar = Math.Pow(arrayFcopy[Convert.ToInt32
                (tempNumTo[numTo][1])][Convert.ToInt32
                (tempNumTo[numTo][2])],degreeA) * Math.Pow((1/ tempNumTo[numTo][0]),degreeB);
            double downVar = (Math.Pow(arrayFcopy[Convert.ToInt32
                (tempNumTo[0][1])][Convert.ToInt32
                (tempNumTo[0][2])], degreeA) * Math.Pow((1 / tempNumTo[0][0]), degreeB)) + (Math.Pow(arrayFcopy[Convert.ToInt32
                (tempNumTo[1][1])][Convert.ToInt32
                (tempNumTo[1][2])], degreeA) * Math.Pow((1 / tempNumTo[1][0]), degreeB));
            double var = upVar / downVar;

            return var;
        }
        private void buttonRandGen_Click(object sender, EventArgs e)
        {
            generateRandData();
        }
        public void generateRandTask(int n)
        {
            Random rnd = new Random();
            if (Convert.ToInt32(numRandTimeMin.Value) < Convert.ToInt32(numRandTimeMax.Value) &&
                Convert.ToDouble(numRandDegreeAMin.Value) < Convert.ToDouble(numRandDegreeAMax.Value) &&
                Convert.ToDouble(numRandDegreeBMin.Value) < Convert.ToDouble(numRandDegreeBMax.Value) &&
                Convert.ToInt32(numRandKMin.Value) < Convert.ToInt32(numRandKMax.Value) &&
                Convert.ToInt32(numRandRMin.Value) < Convert.ToInt32(numRandRMax.Value) &&
                Convert.ToDouble(numRandQMin.Value) < Convert.ToDouble(numRandQMax.Value) &&
                Convert.ToDouble(numRandFMin.Value) < Convert.ToDouble(numRandFMax.Value))
            {
                for (int i = 0; i < 2; i++)
                {
                    arrayP[i] = new int[n];
                    arrayT[i] = new int[n - 1];
                }
                for (int i = 0; i <= 1; i++)
                {
                    arrayA[i] = rnd.Next(Convert.ToInt32(numRandTimeMin.Value), Convert.ToInt32(numRandTimeMax.Value) + 1);
                    arrayB[i] = rnd.Next(Convert.ToInt32(numRandTimeMin.Value), Convert.ToInt32(numRandTimeMax.Value) + 1);
                }
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        arrayP[i][j] = rnd.Next(Convert.ToInt32(numRandTimeMin.Value), Convert.ToInt32(numRandTimeMax.Value) + 1);
                    }
                }
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < n - 1; j++)
                    {
                        arrayT[i][j] = rnd.Next(Convert.ToInt32(numRandTimeMin.Value), Convert.ToInt32(numRandTimeMax.Value) + 1);

                    }
                }
                degreeA = rnd.NextDouble() * (Convert.ToDouble(numRandDegreeAMax.Value) - Convert.ToDouble(numRandDegreeAMin.Value)) + Convert.ToDouble(numRandDegreeAMin.Value);
                degreeB = rnd.NextDouble() * (Convert.ToDouble(numRandDegreeBMax.Value) - Convert.ToDouble(numRandDegreeBMin.Value)) + Convert.ToDouble(numRandDegreeBMin.Value);
                k = rnd.Next(Convert.ToInt32(numRandKMin.Value), Convert.ToInt32(numRandKMax.Value) + 1);
                r = rnd.Next(Convert.ToInt32(numRandRMin.Value), Convert.ToInt32(numRandRMax.Value) + 1);
                q = rnd.NextDouble() * (Convert.ToDouble(numRandQMax.Value) - Convert.ToDouble(numRandQMin.Value)) + Convert.ToDouble(numRandQMin.Value);
                f = rnd.NextDouble() * (Convert.ToDouble(numRandFMax.Value) - Convert.ToDouble(numRandFMin.Value)) + Convert.ToDouble(numRandFMin.Value);
                //showFileData();
            }
            else
            {
                textBoxAnswer.Text = "Значення *Від* має бути меншим від значення *До*";
            }
        }
        public void generateRandData()
        {
            Random rnd = new Random();
            if(Convert.ToInt32(numRandNMin.Value)<Convert.ToInt32(numRandNMax.Value) &&
                Convert.ToInt32(numRandTimeMin.Value)< Convert.ToInt32(numRandTimeMax.Value) &&
                Convert.ToDouble(numRandDegreeAMin.Value) < Convert.ToDouble(numRandDegreeAMax.Value) &&
                Convert.ToDouble(numRandDegreeBMin.Value) < Convert.ToDouble(numRandDegreeBMax.Value) &&
                Convert.ToInt32(numRandKMin.Value) < Convert.ToInt32(numRandKMax.Value) &&
                Convert.ToInt32(numRandRMin.Value) < Convert.ToInt32(numRandRMax.Value) &&
                Convert.ToDouble(numRandQMin.Value) < Convert.ToDouble(numRandQMax.Value) &&
                Convert.ToDouble(numRandFMin.Value) < Convert.ToDouble(numRandFMax.Value))
            {
                n = rnd.Next(Convert.ToInt32(numRandNMin.Value), Convert.ToInt32(numRandNMax.Value) + 1);
                for (int i = 0; i < 2; i++)
                {
                    arrayP[i] = new int[n];
                    arrayT[i] = new int[n - 1];
                }
                for (int i = 0; i <= 1; i++)
                {
                    arrayA[i] = rnd.Next(Convert.ToInt32(numRandTimeMin.Value), Convert.ToInt32(numRandTimeMax.Value) + 1);
                    arrayB[i] = rnd.Next(Convert.ToInt32(numRandTimeMin.Value), Convert.ToInt32(numRandTimeMax.Value) + 1);
                }
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        arrayP[i][j] = rnd.Next(Convert.ToInt32(numRandTimeMin.Value), Convert.ToInt32(numRandTimeMax.Value) + 1);
                    }
                }
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < n - 1; j++)
                    {
                        arrayT[i][j] = rnd.Next(Convert.ToInt32(numRandTimeMin.Value), Convert.ToInt32(numRandTimeMax.Value) + 1);

                    }
                }
                degreeA = rnd.NextDouble() * (Convert.ToDouble(numRandDegreeAMax.Value) - Convert.ToDouble(numRandDegreeAMin.Value)) + Convert.ToDouble(numRandDegreeAMin.Value);
                degreeB = rnd.NextDouble() * (Convert.ToDouble(numRandDegreeBMax.Value) - Convert.ToDouble(numRandDegreeBMin.Value)) + Convert.ToDouble(numRandDegreeBMin.Value);
                k = rnd.Next(Convert.ToInt32(numRandKMin.Value), Convert.ToInt32(numRandKMax.Value) + 1);
                r = rnd.Next(Convert.ToInt32(numRandRMin.Value), Convert.ToInt32(numRandRMax.Value) + 1);
                q = rnd.NextDouble() * (Convert.ToDouble(numRandQMax.Value) - Convert.ToDouble(numRandQMin.Value)) + Convert.ToDouble(numRandQMin.Value);
                f = rnd.NextDouble() * (Convert.ToDouble(numRandFMax.Value) - Convert.ToDouble(numRandFMin.Value)) + Convert.ToDouble(numRandFMin.Value);
                showFileData();
            }else
            {
                textBoxAnswer.Text = "Значення *Від* має бути меншим від значення *До*";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Help form = new Help();
            form.Show();
        }

        private void zedGraphControl1_Load(object sender, EventArgs e)
        {
            
        }

        private void numRandDegreeAMax_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
