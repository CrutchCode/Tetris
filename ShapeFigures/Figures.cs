using System;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeFigures
{
    public class Figures
    {
        /*
         *  Клас фігур, поїхали:
         */
        private int _x;         // координати фігури.
        private int _y;
        
        public int X
        {
            get { return _x; }
            set { _x = value; }
        }
        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }
        Random rand = new Random(); // створюємо обєкт рандома, для вибіру наших фігур.
        
        // Масив фігур які є в тетрісі.
        int[,,] allFigures = new int[,,]
        {
            {
                { 0, 0, 0, 0 },
                { 1, 1, 1, 1 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 0, 1, 1, 0 },
                { 0, 1, 1, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 0, 0, 1, 1 },
                { 0, 1, 1, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 0, 1, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 1, 1, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 1, 1, 0, 0 },
                { 0, 1, 1, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 0, 0, 1, 0 },
                { 0, 0, 1, 0 },
                { 0, 1, 1, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 0, 1, 0, 0 },
                { 1, 1, 1, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            },

        };

        // масив поточної фігури.
        public int[,] figures = new int[,]
        {
            {0, 0, 0, 0 },
            {0, 0, 0, 0 },
            {0, 0, 0, 0 },
            {0, 0, 0, 0 }
        };
        

        int[,] temp = new int[4, 4];                    // цей масив потрібний, коли ми крутимо фігурку.
        public int[,] previousfigure = new int[4, 4];   // зберігаємо поточну фігурку.
        public void NewFigures()
        {
            // Метод який додає згенеровану фігу, в поточку фігуру.
            int i =  rand.Next(0, 7);
            for (; i < allFigures.GetLength(0); i++)
            {
                for (int j = 0; j < allFigures.GetLength(1); j++)
                {
                    for (int k = 0; k < allFigures.GetLength(2); k++)
                    {
                        figures[j, k] = allFigures[i, j, k];
                    }

                }                
                break;
            }

        }

        public SolidColorBrush CurrentColor { get; set; } 
        public Figures(int _x, int _y)
        {
            // ініціалізуємо координати початкової фігурки.
            X = _x;
            Y = _y;
        }
        public void ColorFigure()
        {
            // не здійснені мрії, хотів щоб кожна фігурка була різного кольору, але чомусь від цього йдуть баги і відмовився від реалізації
            // фігурки кореткно малюються, але починають в будь-який момент може зависнути серед поля і буде рахуватись як зафікстована, хоча це не так.. 
            // тому поки відмовився від цього.
            Random randColor = new Random();
            int count = randColor.Next(0, 6);
            switch (count)
            {
                case 0:
                    CurrentColor = new SolidColorBrush(Colors.Red);
                    break;
                case 1:
                    CurrentColor = new SolidColorBrush(Colors.Green);
                    break;
                case 2:
                    CurrentColor = new SolidColorBrush(Colors.Yellow);
                    break;
                case 3:
                    CurrentColor = new SolidColorBrush(Colors.Blue);
                    break;
                case 4:
                    CurrentColor = new SolidColorBrush(Colors.Azure);
                    break;
                case 5:
                    CurrentColor = new SolidColorBrush(Colors.Violet);
                    break;
                case 6:
                    CurrentColor = new SolidColorBrush(Colors.Orange);
                    break;
                    //default: _color = new SolidColorBrush(Colors.Red);
            }
        }
        // методи руху фігурки на ігровому полі, просто змінюємо координати
        public void MovementRight()
        {
            _x++;
        }
        public void MovementLeft()
        {
            _x--;
        }
        public void MovementDown()
        {
            _y++;
        }
        public void Turn()
        {
            /*
             * Тут саме цікаве і костильне рішення
             * крутимо масив по кругу, а саме: колонки переводимо в рядки і так доти доки не зробимо для всього поточного масива.
             *      1, 1, 1             0, 0, 1
             *      0, 1, 0     ->      0, 1, 1
             *      0, 0, 0             0, 0, 1
             * Типу така логіка, але реалізація така собі я знаю. Крутиться за часовою стрілкою.
             */
            int k = 0;
            int f = 0;
            Array.Copy(figures, previousfigure, previousfigure.Length);
            for (int i = figures.GetLength(0) -1; i >= 0 ; i--, f++)
            {
                for (int j = 0; j < figures.GetLength(0); j++)
                {
                    switch(j)
                    {
                        case 0:
                            if (k == 0)
                            {
                                temp[k, f] = figures[i, j];
                                k++;
                            }
                            break;
                        case 1:
                            if (k == 1)
                            {
                                temp[k, f] = figures[i, j];
                                k++;
                            }
                            break;
                        case 2:
                            if (k == 2)
                            {
                                temp[k, f] = figures[i, j];
                                k++;
                            }
                            break;
                        case 3:
                            if (k == 3)
                            {
                                temp[k, f] = figures[i, j];
                                k++;
                            }
                            break;
                    }
                }
                k = 0;

            }
            Array.Copy(temp, figures, figures.Length); // Тепер з тимчасового масива в якому крутили фігурку копіююємо в поточний.
            // Фух вот і все)
            //Array.Clear(temp, 0, temp.Length);
        }
    }
}
