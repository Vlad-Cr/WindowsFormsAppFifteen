using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WindowsFormsAppFifteen
{
    class Game
    {
        const int size = 4; // Розмір поля
        public int[,] fieldOfValues { get; } // Масив значеннь поля
        static int xVoid, yVoid; // Координати пустої п'ятнашки
        static Random rand; // Службовий клас для отримання випадкових значень

        public Game() // Конструктор
        {
            // Ініціалізація
            fieldOfValues = new int[size, size];
            rand = new Random();
        }

        private int coordsToPosition(int x, int y) // Метод для переведення координат п'ятнашки в позицію на полі
        {
            return y * size + x;
        }

        private void positionToCoords(int position, out int x, out int y) // Метод для переведення позиції п'ятнашки в координати масиву
        {
            x = position % size;
            y = position / size;
        }

        public int GetPositionValue(int position) // Метод для отримання значення поля по позиції
        {
            int x, y;
            positionToCoords(position, out x, out y);
            return fieldOfValues[y, x];
        }

        public void Start() // Метод ініціалізації поля у випадковому порядку
        {
            // Створюємо список і заповнюємо його значеннями від 0 до 15
            List<int> listValues = new List<int>(); 

            for (int i = 0; i < size * size; i++)
            {
                listValues.Add(i);
            }

            // Проходимо по полю і заповнюємо його випадково вибраними значеннями зі списку
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int randIndex = rand.Next(0, listValues.Count);

                    if (listValues[randIndex] == 0) // Якщо значення рівне нулю, то запамятовуємо його координати
                    {
                        xVoid = j;
                        yVoid = i;
                    }

                    // Записуємо значення в масив і видаляємо його зі списку
                    fieldOfValues[i, j] = listValues[randIndex];
                    listValues.RemoveAt(randIndex);
                }
            }

            if(!isSolvable()) // Перевіряємо чи данне згенероване поле має рішення
            {
                Start(); // Якщо ні, заново запускаємо ініціалізацію поля у випадковому порядку
            }
        }

        private bool isSolvable() // Метод перевірки існування рішення
        {
            int[] arrayOfValues = fieldOfValues.Cast<int>().ToArray(); // Переводимо двовимірний масив в одновимірний
            int countOfInversions = 0; // Змінна для підрахунку інверсій

            // Проходимо по полю і підраховуємо кількість інверсій оминая нулюву п'ятнашку
            for (int i = 0; i < size * size; i++)
            {
                if (arrayOfValues[i] != 0)
                {
                    for (int j = i + 1; j < size * size; j++)
                    {
                        if (arrayOfValues[j] != 0)
                        {
                            if (arrayOfValues[i] > arrayOfValues[j])
                            {
                                countOfInversions++;
                            }
                        }
                    }
                }
                else // Коли знайшли нульову п'ятнашку, то додаємо її рядок починаючи з 1 до кількості інверсій
                {
                    countOfInversions += 1 + i / size;
                }
            }

            if (countOfInversions % 2 == 1) // Якщо сума інверсій непарна, то данний розклад не має рішення
            {
                return false;
            }

            return true; // Якщо сума інверсій парна, то рішення існує
        }

        public bool CheckFieldGoal() // Метод перевірки досягнення цілі
        {
            if (xVoid != size - 1 || yVoid != size - 1) // Якщо нуьова п'тнашка не стоїть в кінці, то рішення точно не знайдено
            {
                return false;
            }

            // Проходимо по полю і перевіряємо чи всі п'ятнашки стоять на своїх позиціях
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i != size - 1 || j != size - 1)
                    {
                        // Якщо хочаб одна п'ятнашка не стоїть на своєму місці, то ціль не досягнута
                        if (fieldOfValues[i, j] != coordsToPosition(j, i) + 1) 
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool Shift(int position) // Метод зсуву п'ятнашки на пусте місце якщо це можливо
        {
            int x, y;
            positionToCoords(position, out x, out y); // Визначаємо координати п'ятнашки по позиції

            if(Math.Abs(xVoid - x) + Math.Abs(yVoid - y) != 1) // Перевірка на правильність ходу
            {
                return false;
            }

            // Якщо хід правильний, міняємо місцями нулбову і данну п'ятнашки
            fieldOfValues[yVoid, xVoid] = fieldOfValues[y, x];
            fieldOfValues[y, x] = 0;
            // Запамятовуємо нові координати пустої п'ятнашки
            xVoid = x;
            yVoid = y;

            return true;
        }
    }
}