using System;
using System.Collections.Generic;
using System.Drawing;
using MaterialSkin.Controls;
using MaterialSkin;
using ZedGraph;

namespace tvims
{
    public partial class GrapNChar : MaterialForm
    {
        List<Form1.Random_Var> SeriesDist;//ряд распределения
        List<Form1.Random_Var> SampleSeries;//выборочный ряд распределения
        double n;//число проведенных эксперементов 
        LineItem myCurve1;//графики
        double lyambda; // интенсивность
        LineItem myCurve2;//графики
        public GrapNChar(List<Form1.Random_Var> eta, List<Form1.Random_Var> _SeriesDist, double _n, double _lyambda)
        {
            SeriesDist = new List<Form1.Random_Var>(_SeriesDist);
            SampleSeries = new List<Form1.Random_Var>(eta);
            lyambda = _lyambda;
            n = _n;
            for (int i = 0; i < SampleSeries.Count; i++)//с помощью цикла превращаем список случайных величин в ряд распределения случайно величины 
            {
                var tmp = SampleSeries[i];
                tmp.count = tmp.count / n;
                SampleSeries[i] = tmp;
            }
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
        }
        private double F(double x)
        {
            double sum = 0;
            if (x > SeriesDist[SeriesDist.Count - 1].value)
                return 1;
            for (int i = 0; i < SeriesDist.Count; i++)
            {
                if (x <= SeriesDist[i].value)
                    return sum;
                sum = sum + SeriesDist[i].count;
            }
            return -1;
        }
        private double F_(double x)
        {
            double sum = 0;
            if (x < 0)
                return 0;
            if (x > SampleSeries[SampleSeries.Count - 1].value)
                return 1;
            for (int i = 0; i < SampleSeries.Count; i++)
            {
                if (x <= SampleSeries[i].value)
                    return sum;
                sum = sum + SampleSeries[i].count;
            }
            return -1;
        }
        private double MathExpaction()//мат ожидание
        {
            double result = 0;
            for (int i = 0; i < SeriesDist.Count; i++)
            {
                result += SeriesDist[i].count * SeriesDist[i].value;
            }
            return result;
        }
        private double SampleMean()//выборочное среднее
        {
            double result = 0;
            for (int i = 0; i < SampleSeries.Count; i++)
                result += (SampleSeries[i].count * SampleSeries[i].value);
            return (result);
        }
        private double Variance()//Дисперсия 
        {
            double result = 0;
            double M = MathExpaction();
            for (int i = 0; i < SeriesDist.Count; i++)
                result += (SeriesDist[i].value - M) * (SeriesDist[i].value - M) * SeriesDist[i].count;
            return result;
        }
        private double SampleVarianse()//Выборочная дисперсия 
        {
            double result = 0;
            double M = SampleMean();
            for (int i = 0; i < SampleSeries.Count; i++)
                result += (SampleSeries[i].value - M) * (SampleSeries[i].value - M) * SampleSeries[i].count;
            return result;
        }
        private double Mediana()
        {
            if (n % 2 == 0)//для четного случая
            {
                double k1 = 0.5;
                double k2 = 0.5 + 1.0 / n;
                int v1 = 0;
                int v2 = 0;
                double sum = 0;
                for (int i = 0; i < SampleSeries.Count; i++)
                {
                    sum += SampleSeries[i].count;
                    if (sum >= k1)
                        break;
                    v1++;
                    v2++;
                }
                if (k2 > sum)
                    v2 = v1 + 1;
                return ((SampleSeries[v1].value + SampleSeries[v2].value) / 2.0);
            }
            else//для нечетного случая 
            {
                double sum = 0;
                int k = 0;
                for (int i = 0; i < SampleSeries.Count; i++)
                {
                    sum += SampleSeries[i].count;
                    if (sum >= 0.5)
                        break;
                    k++;
                }
                return SampleSeries[k].value;
            }
        }
        private double ScopeSample()//размах выборки 
        {
            return (SampleSeries[SampleSeries.Count - 1].value - SampleSeries[0].value);
        }
        private void FillTable1()
        {
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.Rows[0].Cells[0].Value = lyambda;  // мат. ожидание
            dataGridView1.Rows[0].Cells[1].Value = SampleMean(); // выборочное среднее
            dataGridView1.Rows[0].Cells[2].Value = Math.Abs(lyambda - SampleMean()); 
            dataGridView1.Rows[0].Cells[3].Value = lyambda; // Дисперсия
            dataGridView1.Rows[0].Cells[4].Value = SampleVarianse(); //Выборочная дисперсия
            dataGridView1.Rows[0].Cells[5].Value = Math.Abs(SampleVarianse() - lyambda);
            dataGridView1.Rows[0].Cells[6].Value = Mediana(); // медиана
            dataGridView1.Rows[0].Cells[7].Value = ScopeSample(); // размах выборки
        }
        private void FillTable2()
        {
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.RowCount = 3;
            dataGridView2.ColumnCount = SampleSeries.Count;
            for (int i = 0; i < SampleSeries.Count; i++)
            {
                dataGridView2.Rows[0].Cells[i].Value = SampleSeries[i].value;
                dataGridView2.Rows[1].Cells[i].Value = SeriesDist[SampleSeries[i].value].count;
                dataGridView2.Rows[2].Cells[i].Value = SampleSeries[i].count;
            }
        }
        private double MaximumDeviation(ref int k)//максимальное отклонение
        {
            double result = 0;
            for (int i = 0; i < SampleSeries.Count; i++)
            {
                double prom_result = Math.Abs(SeriesDist[SampleSeries[i].value].count - SampleSeries[i].count);
                if (prom_result > result)
                {
                    result = prom_result;
                    k = SampleSeries[i].value;
                }

            }
            return result;
        }
        private double MeasureDisc(ref int k)//вычисляем меру расхождения 
        {
            double result = 0;
            for (int x = 0; x < SeriesDist.Count; x++)
            {
                double point = x + 1 / 2.0;
                double prom_result = Math.Abs(F(point) - F_(point));
                if (prom_result > result)
                {
                    result = prom_result;
                    k = x;
                }
            }
            return result;
        }
        public void Draw()
        {
            GraphPane pane = zedGraphControl1.GraphPane;
            pane.CurveList.Clear();
            PointPairList point_list1 = new PointPairList();
            PointPairList point_list2 = new PointPairList();
            double xmin = 0;
            double xmax = (SeriesDist[SeriesDist.Count - 1].value) + 1;
            for (double x = xmin; x <= xmax; x += 0.001)
            {
                point_list1.Add(x, F(x));
                point_list2.Add(x, F_(x));
            }
            myCurve1 = pane.AddCurve("F(x)", point_list1, Color.Blue, SymbolType.None);
            myCurve2 = pane.AddCurve("F_(x)", point_list2, Color.Red, SymbolType.None);
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            FillTable1();
            FillTable2();
            int k = 0;
            double D = MeasureDisc(ref k);
            materialLabel1.Text += Convert.ToString(Math.Round(D, 6)) + " На промежутке от " + Convert.ToString(k) + " до " + Convert.ToString(k + 1);
            k = 0;
            materialLabel2.Text = Convert.ToString(Math.Round(MaximumDeviation(ref k), 6)) + " При значении " + Convert.ToString(k);
        }
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckBox1.Checked == true)
            {
                myCurve1.IsVisible = false;
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
            }
            else
            {
                myCurve1.IsVisible = true;
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
            }
        }
        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckBox2.Checked == true)
            {
                myCurve2.IsVisible = false;
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
            }
            else
            {
                myCurve2.IsVisible = true;
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
            }
        }

    }
}
