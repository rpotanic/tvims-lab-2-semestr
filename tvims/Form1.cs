using System;
using System.Collections.Generic;
using MaterialSkin.Controls;
using MaterialSkin;

namespace tvims
{
    public partial class Form1 : MaterialForm
    {
        //Лаба 1
        double u; //зн-е случайной величины
        double lyambda; //интенсивность
        double time; //время
        double countExp; //количество экспериментов;
        List<Random_Var> eta;//случайная величина 
        List<Random_Var> SeriesDist;//ряд распределения

        public Form1()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
           // materialSkinManager.ColorScheme = new ColorScheme(Primary.Amber100, Primary.Amber100,Primary.Amber100, Accent.Amber100,TextShade.WHITE);
        }

        public struct Random_Var
        {
            public int value;
            public double count;
            public Random_Var(int _value = 0, double _count = 0)
            {
                value = _value;//значение которое принимает случайная величина 
                count = _count;//сколько раз встретилась случайная величина 
            }
        }

        private List<Random_Var> Simplify(List<Random_Var> list, int j)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].value == j)
                {
                    var tmp = list[i];
                    tmp.count++;
                    list[i] = tmp;
                    return list;
                }
            }
            Random_Var kers = new Random_Var(j, 1);
            list.Add(kers);
            return list;

        }

        private int Factorial(int number) {
            int res = 1;
            for (int i = number; i > 1; i--)
                res *= i;
            return res;
        }

        private double CheckSum(double sum, double pj)
        {
            double tmp1_sum = sum;
            double tmp2_sum = sum;
            tmp1_sum = Math.Truncate(1000000 * tmp1_sum) / 1000000;
            tmp2_sum = tmp2_sum + pj;
            tmp2_sum = Math.Truncate(1000000 * tmp2_sum) / 1000000;
            if (tmp1_sum == tmp2_sum)
                return 0;
            return tmp2_sum;
        }

        private void modelRandom(object sender, EventArgs e)
        {
            Random rand = new Random(2);
            countExp = int.Parse(materialSingleLineTextField3.Text);
            lyambda = double.Parse(materialSingleLineTextField1.Text);
            time = double.Parse(materialSingleLineTextField2.Text);
            double p0 = Math.Exp(-lyambda * time);
            eta = new List<Random_Var>();//случайная величина
            SeriesDist = new List<Random_Var>();

            for (int i = 0; i < countExp; i++) {                 //главный цикл, который отвечает за кол-во экспериментов
                int j = 1;//счетчик определяющий к какому интервалу принадлежит случайная величина
                double sum = p0;
                double pj_1 = p0;
                u = rand.NextDouble();
                while (true)//цикл отвечающий за определение случайной величины
                {
                    if (u < sum)
                    {
                        eta = Simplify(eta, j - 1);
                        break;
                    }
                    double pj = (lyambda * time / j) * pj_1; ;//распределение по пуассону 
                    pj_1 = pj;
                    sum = CheckSum(sum, pj);
                    j++;
                    if (sum == 0)
                    {
                        eta = Simplify(eta, j-1);
                        break;
                    }
                }
            }
            //double S = 0;//сумма

            Random_Var tmp0 = new Random_Var(0, p0);
            SeriesDist.Add(tmp0);
            for (int i = 1; i < 50; i++)
            {
                double pi = (lyambda * time / i) * SeriesDist[i - 1].count; /*(Math.Exp(-lambda*t)*Math.Pow(lambda * t, i)) / Factorial(i);*/
                pi = Math.Truncate(1000000000 * pi) / 1000000000;
                Random_Var tmp = new Random_Var(i, pi);
                SeriesDist.Add(tmp);
            }

            eta.Sort((x, y) => x.value.CompareTo(y.value));//клевая штука
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowCount = 3;
            dataGridView1.ColumnCount = eta.Count;
            for (int i = 0; i < eta.Count; i++){
                dataGridView1.Rows[0].Cells[i].Value = eta[i].value;
                dataGridView1.Rows[1].Cells[i].Value = eta[i].count;
                dataGridView1.Rows[2].Cells[i].Value = (eta[i].count + "/" + countExp);
            }
        }

        private void materialFlatButton2_Click(object sender, EventArgs e)
        {
            GrapNChar form2 = new GrapNChar(eta, SeriesDist, countExp);
            form2.Draw();
            form2.Show();
        }

        private void materialFlatButton3_Click(object sender, EventArgs e)
        {
            CheckHipoteses form3 = new CheckHipoteses(countExp, SeriesDist, eta);
            form3.Show();
        }
    }
}
