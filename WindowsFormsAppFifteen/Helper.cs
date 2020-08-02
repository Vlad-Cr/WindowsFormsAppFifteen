using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WindowsFormsAppFifteen
{
    class Helper
    {
        const int size = 4;  // Розмір поля
        int[,] fieldOfValues; // Масив значеннь поля
        int xVoid, yVoid; // Координати пустої п'ятнашки
        Func<int, int, int, int, bool> idDFSMethod; // Метод пошуку рішення, який залежить від дискриптора

        // Зміщення відносто пустої п'ятнашки (можливі ходи)
        static int[] dx = { 0, -1, 0, 1 }; 
        static int[] dy = { 1, 0, -1, 0 };
        static int[] oppositeMove = { 2, 3, 0, 1 }; // Протележний хід
        const int infinity = 10000000; // Нескінченність
        public int LimitMovesToGetGoal { get; } // Обмеження кількості ходів для досягнення цілі
        int deepness; // Ітераціна глибина 
        int minPrevIteration;  // Найменша вартість f серед вузлів які не задовільняють глибені
        LinkedList<int> resultPath; // Результуючий список, який зберігає порядок ходів (позицій) для досягнення цілі 

        Stopwatch timer; // Службовий клас таймер для підрахунку часу пошуку рішення
        public int countOfNodesPassed { get; set; }  // Кількості пройдених вузлів під час пошуку рішення
        public int countMoves { get; set; }  // Кількості потрібних ходів для досягнення цілі 
        public string spendTime { get; set; }  // Сторака для збереження часу витраченого на пошук рішення

        public Helper() // Конструктор
        {
            // Ініціалізація
            fieldOfValues = new int[size, size];
            idDFSMethod = null;
            LimitMovesToGetGoal = 50; // Обираємо відповідний ліміт для зменшення часу пошуку рішення
            timer = new Stopwatch();
            resultPath = new LinkedList<int>();
        }

        public int GetValueOfResultPath() // Метод отримання першого значення зі списку рузультуючого шляху
        {
            if (resultPath.Count != 0) // Якщо список не пустий
            {
                return resultPath.First();
            }
            return 0;
        }

        public void RemoveValueOfResultPath() // Метод видалення першого значення зі списку рузультуючого шляху
        {
            if (resultPath.Count != 0) // Якщо список не пустий
            {
                resultPath.RemoveFirst();
            }
        }

        public bool Start(int[,] field, bool Method) // Метод початку пошуку рішення для відповідного розкладу поля 
        {
            Array.Copy(field, fieldOfValues, size * size); // Копіюємо розклад поля

            for (int i = 0; i < size; i++) // Знаходимо координати пустої п'ятнашки і запам'ятовуємо їх
            {
                for (int j = 0; j < size; j++)
                {
                    if(fieldOfValues[i, j] == 0)
                    {
                        xVoid = j;
                        yVoid = i;
                    }
                }
            }
            // Обнуляємо всі значення
            resultPath.Clear();
            countOfNodesPassed = 0;
            countMoves = 0;
            spendTime = null;
            // Відповідно до дискриптора вибираємо метод пошуку рішення, IDA* або SEA* 
            idDFSMethod = Method == false ? new Func<int, int, int, int, bool>(idaStar) : new Func<int, int, int, int, bool>(seaStar);

            timer.Restart(); // Поаток відліку часу виконання пошуку
            bool result = idDFS();  // Запуск методу пошуку рішення
            timer.Stop(); // Фіксація часу
            // Записуємо результати пошуку рішення
            countMoves = resultPath.Count;
            spendTime = timer.Elapsed.ToString();
            
            return result; // Успішність пошуку рішення
        }

        private void swap(int x1, int y1, int x2, int y2) // Метод обміну двох значень місцями в масиві поля
        {
            int value1 = fieldOfValues[y1, x1];
            int value2 = fieldOfValues[y2, x2];
            fieldOfValues[y1, x1] = value2;
            fieldOfValues[y2, x2] = value1;
        }

        private bool checkRange(int x, int y) // Метод перевірки діапазону ходу
        {
            return (x >= 0 && y >= 0 && x < 4 && y < 4) ? true : false; 
        }

        private int heuristicEvaluation() // Метод еврестичної оцінки розкладу поля за допомогою Манхетенських (Городських) квадратів
        {
            int h = 0; // Змінна для збереження значення еврестичної оцінки

            // Проходимо по полю і сумуємо відстані п'ятнашок до їх цільових позицій
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (fieldOfValues[i, j] != 0)
                    {
                        // Визначаємо координати цільової позиції відповідної п'ятншки
                        int xGoal = (fieldOfValues[i, j] - 1) % size;
                        int yGoal = (fieldOfValues[i, j] - 1) / size;
                        h += Math.Abs(xGoal - j) + Math.Abs(yGoal - i); // Обраховуємо відстань до цільової позиції та сумуємо її
                    }
                }
            }

            return h; // Повертаємо еврестичну оцінку розкладу поля
        }

        private bool idDFS() // Метод пошуку рішення який використовує IDA* або SEA* відповідно до дискрипрота
        {
            bool result = false; // Позначаємо що результат не знайдено
            deepness = heuristicEvaluation(); // Визначаємо початкову глибену, яка рівна еврестичній оцінці початкового розкладу поля

            while ((!result) && (deepness <= LimitMovesToGetGoal)) // Поки не знайдений результат і глибина не перевищує обмеження кількості ходів для досягнення цілі
            {
                minPrevIteration = infinity; // Позначаємо найменшу вартість f серед вузлів які не задовільняють глибені як нескінченність
                result = idDFSMethod(0, -1, xVoid, yVoid); // Викликаємо відповідний метод пошуку рішення
                deepness = minPrevIteration; // Позначаємо нову глибену
            };

            return result; // Успішність пошуку рішення
        }

        private bool idaStar(int g, int previousMove, int xVoid, int yVoid) // Метод пошуку рішення IDA*
        {
            countOfNodesPassed++; // Підраховуємо кількість пройдених вузлів

            int h = heuristicEvaluation(); // Проводимо еврестичну оцінку данного розкладу поля (вузла)
            if (h == 0) // Якщо еврестична оцінка рівна нулю - ціль досягнута
            {
                return true; // Звертаємо рекурсію і записуємо шлях (позиції) досягнення цілі
            }

            int f = g + h; // Визначаємо значення f данного розкладу поля (вузла)
            if (f > deepness) // Якщо f не задовільняє глибені
            {
                if (minPrevIteration > f) // Визначаємо найменшу вартість f серед вузлів які не задовільняють глибені
                {
                    minPrevIteration = f;
                }

                return false; // Зупиняємо розвертання данного вузла
            }

            for (int i = 0; i < 4; i++) // Робимо всі можливі ходи 
            {
                // Визначаємо нові координати пустої п'ятнашки
                int xVoidNew = xVoid + dx[i];
                int yVoidNew = yVoid + dy[i];

                if (checkRange(xVoidNew, yVoidNew) && oppositeMove[i] != previousMove) // Перевіряємо чи хід не виходить за межі поля і не поверне нас в в попередній розклад (вузол)
                {
                    swap(xVoid, yVoid, xVoidNew, yVoidNew); // Робимо відповідний хід, змінюючи розклад поля
                    bool result = idaStar(g + 1, i, xVoidNew, yVoidNew); // Розвертаємо рекурсію, переходимо в наступний вузол
                    swap(xVoid, yVoid, xVoidNew, yVoidNew); // Повертаємо розклад поля

                    if (result) // Якщо ціль досягнута, то записуємо хід в результуючий шлях і далі звертаємо рекурсію
                    {
                        resultPath.AddFirst(yVoidNew * size + xVoidNew);
                        return true;
                    }
                }
            }

            return false; // За данної глибени ціль не знайдена
        }

        private bool seaStar(int g, int previousMove, int xVoid, int yVoid) // Власний метод пошуку рішення SEA*
        {
            Dictionary<int, int> fMovesPriority = new Dictionary<int, int>(); // Контейнер для зберігання порядку розвертання вузлів на кожному кроці

            for (int i = 0; i < 4; i++) // Робимо всі можливі ходи з данного вузла
            {
                // Визначаємо нові координати пустої п'ятнашки
                int xVoidNew = xVoid + dx[i];
                int yVoidNew = yVoid + dy[i];

                if (checkRange(xVoidNew, yVoidNew) && oppositeMove[i] != previousMove) // Перевіряємо чи хід не виходить за межі поля і не поверне нас в в попередній розклад (вузол)
                {
                    countOfNodesPassed++; // Підраховуємо кількість пройдених вузлів
                    swap(xVoid, yVoid, xVoidNew, yVoidNew); // Робимо відповідний хід, змінюючи розклад поля
                    
                    int h = heuristicEvaluation(); // Проводимо еврестичну оцінку розкладу поля (вузла)
                    if (h == 0) // Якщо еврестична оцінка рівна нулю - ціль досягнута
                    {
                        swap(xVoid, yVoid, xVoidNew, yVoidNew); // Повертаємо розклад поля
                        resultPath.AddFirst(yVoidNew * size + xVoidNew); // Записуємо хід в результуючий шлях і звертаємо рекурсію
                        return true;
                    }

                    int f = (g + 1) + h; // Визначаємо значення f розкладу поля (вузла)
                    if (f <= deepness) // Якщо f задовільняє глибені, то запам'ятовуємо данний вузол і його значення
                    {
                        fMovesPriority.Add(i, f);
                    }
                    else // Визначаємо найменшу вартість f серед вузлів які не задовільняють глибені
                    {
                        if (minPrevIteration > f)
                        {
                            minPrevIteration = f;
                        }
                    }
                    swap(xVoid, yVoid, xVoidNew, yVoidNew); // Повертаємо розклад поля
                }
            }

            while (fMovesPriority.Count != 0) // Робимо всі ходи з данного вузла які задовільняють глибені в пріорітетному порядку
            {
                // Визначаємо дочірній вузол з найменшем f
                int fMin = fMovesPriority.Values.Min();
                int fMinMove = fMovesPriority.FirstOrDefault(x => x.Value == fMin).Key;
                // Визначаємо нові координати пустої п'ятнашки
                int xVoidNew = xVoid + dx[fMinMove];
                int yVoidNew = yVoid + dy[fMinMove];
                swap(xVoid, yVoid, xVoidNew, yVoidNew); // Робимо відповідний хід

                bool result = seaStar(g + 1, fMinMove, xVoidNew, yVoidNew); // Розвертаємо рекурсію, переходимо в наступний вузол з найменшем значенням f серед не пройдених

                swap(xVoid, yVoid, xVoidNew, yVoidNew); // Повертаємо розклад поля

                fMovesPriority.Remove(fMinMove); // Видаляємо хід, позначаєючи що данне підерево пройдене за данної глибини

                if (result) // Якщо ціль досягнута, то записуємо хід в результуючий шлях і далі звертаємо рекурсію
                {
                    resultPath.AddFirst(yVoidNew * size + xVoidNew);
                    return true;
                }
            }

            return false; // За данної глибени ціль не знайдена
        }
    }
}
