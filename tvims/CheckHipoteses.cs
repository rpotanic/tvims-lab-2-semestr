using System;
using System.Collections.Generic;
using MaterialSkin.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using MaterialSkin;

namespace tvims
{
    public partial class CheckHipoteses : MaterialForm

    {
        double alfa;
        int k;
        double n;
        List<Form1.Random_Var> SeriesDist;
        List<Form1.Random_Var> SampleSeries;
        double R0;
        double FR0;
        int[] intervals;
        public CheckHipoteses(double _n, List<Form1.Random_Var> _SeriesDist, List<Form1.Random_Var> _SampleSeries)
        {
            SeriesDist = _SeriesDist;
            SampleSeries = _SampleSeries;
            n = _n;
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (materialSingleLineTextField1.Text == "")
            {
                k = 0;
            }
            else {
                k = int.Parse(materialSingleLineTextField1.Text);
            }
            dataGridView1.RowCount = k;
            dataGridView1.ColumnCount = 3;
            for (int i = 0; i < k; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = i + 1;
            }
        }
        private int GetNj(int i, int j)
        {

            int value = 0;
            for (int k = 0; k < SampleSeries.Count; k++)
            {
                //int tmp = Convert.ToInt32(data.Rows[0].Cells[k].Value);
                if (i <= SampleSeries[k].value && SampleSeries[k].value < j)
                    value += (int)SampleSeries[k].count;
            }
            return value;
        }
        private double Calculate_R0()
        {
            double result = 0;
            for (int i = 0; i < k; i++)
            {
                int i1 = Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value);
                int i2 = Convert.ToInt32(dataGridView1.Rows[i].Cells[2].Value);
                int nj = GetNj(i1, i2);
                double qj = Convert.ToDouble(dataGridView2.Rows[1].Cells[i].Value);
                result = result + (nj - n * qj) * (nj - n * qj) / (n * qj);
            }
            return result;
        }
        private double f(double x)
        {
            var chart = new Chart();
            double r = k - 1;
            return (Math.Pow(2, -r / 2.0) * 1 / (chart.DataManipulator.Statistics.GammaFunction(r / 2)) * Math.Pow(x, r / 2.0 - 1) * Math.Pow(Math.E, -x / 2.0));
        }
        private double F()
        {
            return (1 - Integrate());
        }

        private double Integrate()
        {
            double result = 0;
            for (int i = 2; i <= 1005; i++)
            {
                result += (f(R0 * (i - 1) / 1000.0) + f(R0 * i / 1000.0)) * R0 / (2 * 1000.0);
            }
            return result;
        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            materialLabel5.Text = "R0=";
            materialLabel6.Text = "F_(R0)=";
            materialLabel7.Text = "Гипотеза:";
            alfa = Convert.ToDouble(materialSingleLineTextField2.Text);
            dataGridView1.RowHeadersVisible = false;
            dataGridView2.RowCount = 2;
            dataGridView2.ColumnCount = k;
            for (int i = 0; i < k; i++)
            {
                dataGridView2.Rows[0].Cells[i].Value = "q" + Convert.ToInt32(i + 1);
                double value = 0;
                for (int j = Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value); j < Convert.ToInt32(dataGridView1.Rows[i].Cells[2].Value); j++)
                {
                    value += SeriesDist[j].count;
                }
                dataGridView2.Rows[1].Cells[i].Value = value;
            }
            R0 = Calculate_R0();
            materialLabel5.Text += R0;
            FR0 = F();
            materialLabel6.Text += FR0;
            if (FR0 >= alfa)
                materialLabel7.Text += " принята";
            else
                materialLabel7.Text += " не принята";
        }
    }
}
