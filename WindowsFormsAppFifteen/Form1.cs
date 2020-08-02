using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsAppFifteen
{
    public partial class Form1 : Form
    {
        Helper Helper; // Об'єкт Helper, який виконує пошук найкоротшого шляху до перемоги
        DateTime time; // Об'єкт DateTime, який використовується для таймера
        Game Game; // Об'єкт Game, який відповідає за всю логіку гри
        int countOfMoves; // Змінна, яка підраховує кілкості ходів гравця
        bool HelperIsRun; // Флаг який показує чи використовуєть помічник
        enum method { ida, sea }; // Перелік методів, які може використовувати помічник для пошуку рішення

        public Form1() // Конструктор форми
        { 
            // Ініціалізація всі елементів форми
            InitializeComponent(); 
            Helper = new Helper();
            time = new DateTime();
            Game = new Game();
        }

        private void Load_Form(object sender, EventArgs e) // Метод форми, який викликається при запуску додатка
        {
            EnabledButtons(false); // Відключаємо доступ до кнопок, допоки не почнеться гра
        }
        
        private void button_Click_StartGame(object sender, EventArgs e) // Метод початку гри, який викликається при нажатті на відповідну кнопку
        {
            PaintField(Color.Silver); // Фарбуємо поле в срібний колір
            HelperIsRun = false; // Помічник не використовується
            countOfMoves = 0;  // Обнулення значеня кількості ходів
            time = DateTime.MinValue;  // Обнулення значення часу
            Game.Start(); // Ініціалізація поля у випадковому порядку
            label1.Text = "Ходи: " + countOfMoves; // Оновлення кількісті ходів у формі
            label2.Text = "Час: " + time.ToString("HH:mm:ss"); // Оновлення часу у формі 
            EnabledButtons(true); // Дозвіл на доступ до кнопок
            RefreshField(); // Оновлення форми; відображення змін
            timer1.Start(); // Запуск таймера
        }

        private void button_Click_StartHelperIDAstar(object sender, EventArgs e) // Запуск помічника з методом IDA* при натисканні на відповідну конопку
        {
            timer1.Stop(); // Зупинка відліку часу
            MessageBox.Show("Зачекайте будь ласка! Помічник шукає рішення.\nЦе може зайняти деякий час.");
            if (Helper.Start(Game.fieldOfValues, Convert.ToBoolean(method.ida))) // Пошук рішення за допомогою IDA*, та вивід результатів
            {
                MessageBox.Show("IDA* Moves: " + Helper.countMoves + " Time: " + Helper.spendTime + " Nodes passed: " + Helper.countOfNodesPassed);
                HelperIsRun = true;
                button(Helper.GetValueOfResultPath()).BackColor = Color.DarkOrange;
            }
            else
            {
                MessageBox.Show("Помічник IDA* не зміг знайти рішення =(");
            }
            timer1.Start();  // Продовження відліку часу
        }

        private void button_Click_StartHelperSEAstar(object sender, EventArgs e) // Запуск помічника з методом SEA* при натисканні на відповідну конопку
        {
            timer1.Stop(); // Зупинка відліку часу
            MessageBox.Show("Зачекайте будь ласка! Помічник шукає рішення.\nЦе може зайняти деякий час.");
            if (Helper.Start(Game.fieldOfValues, Convert.ToBoolean(method.sea))) // Пошук рішення за допомогою SEA*, та вивід результатів
            {
                MessageBox.Show("SEA* Moves: " + Helper.countMoves + " Time: " + Helper.spendTime + " Nodes passed: " + Helper.countOfNodesPassed);
                HelperIsRun = true;
                button(Helper.GetValueOfResultPath()).BackColor = Color.DarkOrange;
            }
            else
            {
                MessageBox.Show("Помічник SEA* не зміг знайти рішення =(");
            }
            timer1.Start(); // Продовження відліку часу
        }

        private void button_Click(object sender, EventArgs e) // Метод який відповідає за нажаття на п'ятнашку і її зсув
        {
            int position = Convert.ToInt32(((Button)sender).Tag); // Визначаємо позицію нажатої п'ятнашки
            
            if(Game.Shift(position)) // Зсуваємо п'ятнашку на пусте місце якщо хді є коректним
            {
                if (HelperIsRun) // Перевіряємо чи допомагає помічник
                {
                    if (position == Helper.GetValueOfResultPath()) // Якщо помічник допомагає перевіряємо чи була нажата п'ятнашка на яку він вказує 
                    {
                        // Продовження підказок помічника
                        button(Helper.GetValueOfResultPath()).BackColor = Color.Silver;
                        Helper.RemoveValueOfResultPath();
                        button(Helper.GetValueOfResultPath()).BackColor = Color.DarkOrange;
                    }
                    else
                    {
                        PaintField(Color.Silver);
                        HelperIsRun = false; // Помічник перестає допомагати
                    }
                }

                label1.Text = "Ходи: " + ++countOfMoves; // Підрахунок кількості ходів
                RefreshField(); // Оновлення форми; відображення змін, якщо був зроблений хід
            }
            
            if (Game.CheckFieldGoal()) // Перевірка на досягнення цілі
            {
                PaintField(Color.Silver);
                timer1.Stop(); // Зупинка відліку часу
                MessageBox.Show("Перемога!!!!!\nВаш результат:\nКількість ходів - " + countOfMoves + "\nЧас - " + time.ToString("HH:mm:ss"));
                EnabledButtons(false); // Відключаємо доступ до кнопок
            }
        }

        private Button button(int position) // Метод доступ до всіх кнопок на полі по їх позиції
        {
            switch (position)
            {
                case 0: return button0;
                case 1: return button1;
                case 2: return button2;
                case 3: return button3;
                case 4: return button4;
                case 5: return button5;
                case 6: return button6;
                case 7: return button7;
                case 8: return button8;
                case 9: return button9;
                case 10: return button10;
                case 11: return button11;
                case 12: return button12;
                case 13: return button13;
                case 14: return button14;
                case 15: return button15;
                case 16: return button16;
                case 17: return button17;
                default: return null;
                    
            }
        }

        private void RefreshField() // Метод оновлення форми; відображення змін
        {
            for (int position = 0; position < 16; position++) // Проходимо по всіх п'ятнашках і відображаємо їх значення
            {
                button(position).Text = Convert.ToString(Game.GetPositionValue(position));
                button(position).Visible = (Game.GetPositionValue(position) > 0);  // П'ятнашку з нульовим значенням робимо невидимою
            }
        }

        private void EnabledButtons(bool enabled) // Метод Вкл/Викл доступу до кнопок
        {
            for (int position = 0; position < 18; position++)
            {
                button(position).Enabled = enabled;
            }
        }

        private void PaintField(Color color) // Метод фарбування поля
        {
            for (int position = 0; position < 16; position++)
            {
                button(position).BackColor = color;
            }
        }

        private void timer1_Tick(object sender, EventArgs e) // Метод таймера який рахує час продовження гри і відображає його
        {
            time = time.AddSeconds(1);
            label2.Text = "Час: " + time.ToString("HH:mm:ss");
        }
    }
}