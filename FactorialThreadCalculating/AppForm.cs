﻿using System;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;

namespace FactorialThreadCalculating
{
    public partial class AppForm : Form
    {
        public AppForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Метод, инкрементирующий прогресс-бар по мере выполнения задачи.
        /// </summary>
        public void SetToProgress(int inp)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetToProgress(inp)));
            }
            else
            {
                progress.Value = inp;

            }
        }

        /// <summary>
        /// Добавляет отчет о выполнении задачи с именем файла результата
        /// </summary>
        public void AddNewResult(string inp)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AddNewResult(inp)));
            }
            else
            {
                listBox.Items.Add(inp);
            }
        }

        Fact fact = null;
        private void scroll(object sender, EventArgs e)
        {
            if (fact != null)
            {
                fact.StopCalculation();
            }

            progress.Value = 0;
            int input = trackBar.Value;
            progress.Maximum = input;

            fact = new Fact(input, SetToProgress, AddNewResult);
            fact.StartCalculation();
        }
    }

    public class Fact
    {
        public BigInteger Result { get; private set; } = 1;

        // Инкрементация прогресс бара и запись резов в листбокс
        public Action<int> SetToProgress { get; private set; }
        public Action<string> AddNewResult { get; private set; }
        //Число факториал которого надо вычислть
        public int Input { get; private set; }
        // Флаг работы вычисления
        public bool ThreadJob { get; private set; } = true;

        public Thread ThreadFrac;

        public void SegmentFractalCalculate()
        {
            for (int i = 1; i <= Input; i++)
            {
                if (ThreadJob == false)
                {
                    break;
                }

                Result *= i;
                SetToProgress.Invoke(i);
            }

            //Вывод результата если поток не был прерван
            if (ThreadJob) {
                DateTime dateTime = DateTime.Now;

                string res = string.Format("result{0}.{1}-{2}-{3}.txt", Input, dateTime.Hour, dateTime.Minute, dateTime.Second);
                AddNewResult.Invoke(res);
                StreamWriter sw = new StreamWriter(res);
                sw.Write(Result);
                sw.Close();
            }
        }

        public void StopCalculation()
        {
            ThreadJob = false;
        }

        public void StartCalculation()
        {
            ThreadFrac.Start();
        }

        public Fact(int input, Action<int> SetToProgress, Action<string> AddNewResult)
        {
            this.Input = input;

            this.SetToProgress = SetToProgress;
            this.AddNewResult = AddNewResult;

            ThreadFrac = new Thread(new ThreadStart(this.SegmentFractalCalculate));
        }
    }
}
