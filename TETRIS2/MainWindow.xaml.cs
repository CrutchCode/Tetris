using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ShapeFigures;

namespace TETRIS2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*
         */
        int[,] playfield = new int[,]
           {
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
           };                                   // Масив ігрового поля.
        Path pix;                                                           // Поле яке зберігає піксель падаючої фігурки.
        RectangleGeometry Pixel;                                            // Піксель.
        Line rowLine;                                                       // Рядки ігрового поля.
        Line columnLine;                                                    // Колонки ігрового поля.
        const int RETREAT = 20;                                             // Відступ.
        const int SIZECELL = 20;                                            // Розмір комірки.
        const int DELTASIZECELL = 18;                                       // Розмір пікселя.
        const int ROWGRID = 20;                                             // Кількість рядків в ігровому полі.
        const int COLUMNGRID = 10;                                          // Кількість колонок в ігровому полі.

        Figures currentFigures = new Figures(4, 0);                         // Ініціалізуємо фігуру і передаємо координати.
        DispatcherTimer timer;                                              // Таймер ігри.
        Path pix2;                                                          // Піксель для закріпленої фігурки.
        Path pix3;                                                          // Піксель для затирання фігурки.

        public MainWindow()
        {
            InitializeComponent();
            DrawGrid();                                                     // Малюємо поле тетрісу.      
        }
        /*
         * Таймер тетрісу:
         * - в ньому ми запускаємо всі необхідні методи для роботи тетрісу.
         */
        void TetrisTimer()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler((o, e) =>
            {

                ClearField();       // Очищуємо поле.
                AddFigures();       // Додаємо фігурку.
                Move();             // Рухаємо фігурку по полю.
            });
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Start();
        }

        
        void DrawGrid()
        {
        /*
         * Метод який малює ігрове поле.
         * https://docs.microsoft.com/en-us/dotnet/api/system.windows.shapes.line?view=netcore-3.1
         * https://docs.microsoft.com/en-us/dotnet/api/system.windows.shapes.rectangle?view=netcore-3.1
         * https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.rectanglegeometry?view=netcore-3.1
         * https://docs.microsoft.com/en-us/dotnet/framework/wpf/graphics-multimedia/how-to-create-a-linesegment-in-a-pathgeometry
         * https://docs.microsoft.com/ru-ru/dotnet/framework/wpf/graphics-multimedia/shapes-and-basic-drawing-in-wpf-overview
         */

            for (int i = 0; i <= COLUMNGRID; i++)
            {
                rowLine = new Line();
                rowLine.X1 = RETREAT + i * SIZECELL;
                rowLine.Y1 = RETREAT;
                rowLine.X2 = RETREAT + i * SIZECELL;
                rowLine.Y2 = 420;
                rowLine.Stroke = Brushes.Black;
                grid1.Children.Add(rowLine);
                Grid.SetColumn(rowLine, 0);
                Grid.SetRow(rowLine, 1);
            }
            for (int i = 0; i <= ROWGRID; i++)
            {
                columnLine = new Line();
                columnLine.X1 = 220;
                columnLine.Y1 = RETREAT + i * SIZECELL;
                columnLine.X2 = RETREAT;
                columnLine.Y2 = RETREAT + i * SIZECELL;
                columnLine.Stroke = Brushes.Black;
                grid1.Children.Add(columnLine);
                Grid.SetColumn(columnLine, 0);
                Grid.SetRow(columnLine, 1);
            }            
        }

        
        void DrawFigure()
        {
        /*
         * Метод який малює фігуру на ігровому полі.
         * Не самий адекватний алгоритм, але по іншому поки не знаю.
         */
            for (int y = 0; y < playfield.GetLength(0); y++)
            {
                for (int x = 0; x < playfield.GetLength(1); x++)
                {
                    if (playfield[y, x] == 1)
                    {
                        // фігурка на полі позначаються одиничками, там де одинички малюємо квадрат фігурки.
                        pix = new Path();
                        Pixel = new RectangleGeometry();
                        Pixel.Rect = new Rect((RETREAT + x * SIZECELL + 1), (RETREAT + y * SIZECELL + 1), DELTASIZECELL, DELTASIZECELL);
                        pix.Data = Pixel;
                        pix.Fill = Brushes.Crimson;

                        grid1.Children.Add(pix);
                        Grid.SetColumn(pix, 0);
                        Grid.SetRow(pix, 1);
                    }
                    else if(playfield[y, x] == 2)
                    {
                        // закріплена фігурка позначається двойки, там де двойки(моя оцінка по програмуванню, кек) малюємо квадрати закріпленої фігури.
                        pix3 = new Path();
                        Pixel = new RectangleGeometry();
                        Pixel.Rect = new Rect((RETREAT + x * SIZECELL + 1), (RETREAT + y * SIZECELL + 1), DELTASIZECELL, DELTASIZECELL);
                        pix3.Data = Pixel;
                        pix3.Fill = Brushes.Aqua;

                        grid1.Children.Add(pix3);
                        Grid.SetColumn(pix3, 0);
                        Grid.SetRow(pix3, 1);

                    }
                }
            }
        }
        
        void ClearField()
        {
        /*
         * Метод який очищує ігрове поле, коли зміщуємо фігурки затираємо попередні координати.
         */
            for (int y = 0; y < playfield.GetLength(0); y++)
            {
                for (int x = 0; x < playfield.GetLength(1); x++)
                {
                    if (playfield[y, x] == 1)
                    {
                        pix2 = new Path();
                        Pixel = new RectangleGeometry();
                        Pixel.Rect = new Rect((RETREAT + x * SIZECELL + 1), (RETREAT + y * SIZECELL + 1), DELTASIZECELL, DELTASIZECELL);
                        pix2.Data = Pixel;
                        pix2.Fill = Brushes.White;

                        grid1.Children.Add(pix2);
                        Grid.SetColumn(pix2, 0);
                        Grid.SetRow(pix2, 1);
                    }
                }
            }
        }
                
        void Move()
        {
        /*
         * Метод який рухає фігурку на ігровому полі.
         */
            AddFigures();       // додаємо фігурку на поле.
            DrawFigure();       // малюємо фігурку на полі.
            currentFigures.MovementDown();      // Визиваємо метод який переміщує фігурку на ігровому полі по кординаті - Y.
            if (CheckFigures())
            {
                /* Тут перевіряємо чи на ша фігурка опустилась на низ(чи опустилась на іншу фігуру),
                 * якщо так, то ми віднімаємо координату Y, щоб не вийти за межі масиву і не перезатерти фігурку. 
                 */
                currentFigures.Y--;
                FixFigures();       // якщо ми потрапили в цю умову то нам потрібно зафіксувати фігурку, визиваємо метод фіксації фігурки.
                RemoveLines();      // костильний метод!!! перевіряє чи заповнені рядки, якщо так то перезаписуємо масив.(детально в самому методі).
                currentFigures.NewFigures();    // Додаємо нову фігуру на поле.
                currentFigures.X = (10 - currentFigures.figures.GetLength(0)) / 2; // координата Х на яку стає нова фігурка.
                currentFigures.Y = 0;                                               // координата Y на.
            }
        }
        void RemoveLines()
        {
            /*
             * Самий костильний метод, більш костильний метод хіба повороту фігурки =)
             * Тут видаляємо рядки в яких знайшли заповнені лінії.
             */
            bool remove = true;         // Прапорець стану ліній.
            for (int i = 0; i < playfield.GetLength(0); i++)
            {
                for (int j = 0; j < playfield.GetLength(1); j++)
                {
                    if (playfield[i, j] != 2)
                    {
                        // якщо в лінія не дорівнює закріпленим фігуркам повертємо фолз.(якщо хоча б є одна двойка, то дальше перевіряти не потрібно)
                        remove = false;
                        break;
                    }
                }
                if (remove)
                {
                    // Костилі пішли: перевіряємо поле і знаходимо нашу заповнену лінію, якщо знайшли перетираємо на одинички.(такий собі маркер для малювання)
                    for (int y = 0; y < playfield.GetLength(0); y++)
                    {
                        for (int x = 0; x < playfield.GetLength(1); x++)
                        {
                            if(y == i)
                            {
                                playfield[y, x] = 1;
                            }
                        }
                    }
                    ClearField(); // Візуально очищуємо ряд в якому були знайдено заповнену лінію.
                    for (int y = i; y >= 0; y--)
                    {
                        /*
                         * Проходимо знову по полю з низу до верху від того місця де знайшли заповнений ряд.
                         */
                        for (int x = 0; x < playfield.GetLength(1); x++)
                        {
                            //Переносимо ряди зверху до низу.
                            if (playfield[y, x] == 1 && y >= 1)
                            {
                                playfield[y, x] = playfield[y - 1, x];      // з координати, що зверху зписуємо в той ряд де в нас був заповнений ряд
                                playfield[y - 1, x] = 1;                    // переносимо ряд на координату вище.
                            }
                        }
                        ClearField(); // і обовязково потрібно визивати метод очистки поля, щоб він перетирав траєкторію руху нашого заповненого ряду.
                    }
                    // цей цикл потрібний для самого верхнього ряду, щоб одинички замінити на нолі, повне очищення поля.
                    for (int y = 0; y == 0; y++)
                    {
                        for (int x = 0; x < playfield.GetLength(1); x++)
                        {
                            if (playfield[y, x] == 1)
                            {
                                playfield[y, x] = 0;
                            }
                        }
                    }

                }

                remove = true;      // скидаємо прапорец в положення тру.
            }
            DrawFigure();           // визиваємо метод малювання всього що є на полі.
        }
        bool CheckFigures()
        {
            // Ще трохи костилів, функція перевірки нашої фігурки( вихід фігурки за межі поля і нижче чи наша фігурка не наїхала на іншу фігурку )
            // Трохи тут накосясив з умовами і не розібрався де, тому костилі переніс ще в одну функцію, ахаха тру сторі...
            // Цей метод дозволя чи забороняє рухати фігурку за межі поля.
            for (int y = 0; y < currentFigures.figures.GetLength(0); y++)
            {
                for (int x = 0; x < currentFigures.figures.GetLength(1); x++)
                {
                    if (currentFigures.figures[y, x] == 1 && (currentFigures.Y + y == playfield.GetLength(0) 
                        || currentFigures.X + x < 0 || currentFigures.X + x == playfield.GetLength(1)))
                    {
                        return true;
                    }
                    else if(currentFigures.figures[y, x] == 1 && playfield[currentFigures.Y + y, currentFigures.X + x] ==2 )
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        void FixFigures()
        {
            // тут все просто, фігурка яка досягла дна чи лежить на йншій фігурі перетираємо одинички на двой, тим самим показуємо що це зафіксована фігурка
            for (int y = playfield.GetLength(0) - 1; y >= 0; y--)
            {
                for (int x = 0; x < playfield.GetLength(1); x++)
                {
                    if (playfield[y, x] == 1)
                    {
                        playfield[y, x] = 2;
                    }
                }
            }

        }
        
       void AddFigures()
       {
            // додажмо нашу фігурку на поле, по заданим координатам, при цьому додаємо всьо нашу фігурку на поле
           ClearFigures();  //це незнаю, добавив, чи треба чи нє не перевіряв, але гірше не робить наче. Візуальний метод очищення поля.
           for (int y = 0; y < currentFigures.figures.GetLength(0); y++)
           {
                for (int x = 0; x < currentFigures.figures.GetLength(1); x++)
                {
                    if (currentFigures.figures[y, x] == 1)
                    {
                        try
                        {
                            // тут в мене були помилки повязані з виходом за масив, метод СheckFigures() не завжди відпрацьовував коректно прийшлось накидати костилів
                            // Підкостиляв і працює добре, ахаха Працює - не трогай як кажуть.
                            if (currentFigures.X + x == 10)
                            {
                                currentFigures.X--;
                                playfield[currentFigures.Y + y, currentFigures.X + x] = currentFigures.figures[y, x];

                            }
                            else if(currentFigures.X + x == -1)
                            {
                                currentFigures.X++;
                                playfield[currentFigures.Y + y, currentFigures.X + x] = currentFigures.figures[y, x];
                            }
                            else if(currentFigures.Y + y == 20)
                            {
                                currentFigures.Y--;
                                playfield[currentFigures.Y + y, currentFigures.X + x] = currentFigures.figures[y, x];
                            }
                            else
                            {
                                playfield[currentFigures.Y + y, currentFigures.X + x] = currentFigures.figures[y, x];
                            }

                            
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
           }
       }
        void ClearFigures()
        {
            // Простий метод очищення фігур на полі, перетираємо на нолі.
            for (int y = 0; y < playfield.GetLength(0); y++)
            {
                for (int x = 0; x < playfield.GetLength(1); x++)
                {
                    if(playfield[y, x] == 1)
                    {
                        playfield[y, x] = 0;
                    }
                }
            }
        }        

        private void button_click(object sender, KeyEventArgs e)
        {
            // Обробник натиску кнопок, запілив на нампад
            switch(e.Key.ToString())
            {
                case "NumPad4":                    
                    currentFigures.MovementLeft(); // переміщуємо фігуру в ліво.
                    if (CheckFigures())
                    {
                        // перевіряємо чи не вийшли за поле і чи немає закріпленої фігурки.
                        // якщо є то повертаємо на попереднє місце нашу фігурку.
                        currentFigures.X++;
                    }
                    break;
                case "NumPad6":
                    currentFigures.MovementRight();
                    if (CheckFigures())
                    {
                        // аналогічні дії, що і зверху.
                        currentFigures.X--;
                    }
                    break;
                case "NumPad2":
                    // те саме тільки тут визиваємо метод мув.
                    ClearField();
                    Move();
                    break;
                case "NumPad5":
                    // а тут крутимо фігуру, мій варіант, в інтернеті там є норм фаріанти =)
                    currentFigures.Turn(); // визиваємо костилі. Детальніше в класі Фігур.
                    if (CheckFigures())
                    {
                        // якщо нашу фігурку неможливо повернути повертаємо збережену фігурку до повороту.
                        Array.Copy(currentFigures.previousfigure, currentFigures.figures, currentFigures.figures.Length);
                    }
                   break;
            }
            // це для більшої частоти кадрів, фігурка плавніше рухається по полю.
            ClearField();   // Очищуємо поле.
            AddFigures();   // Додаємо знову на поле.
            DrawFigure();   // Малюємо фігуру заново.
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        { 
            // запілив кнопку щоб запускати, типу інтерфейс мав би бути
            currentFigures.NewFigures(); // додаємо нову фігуру.
            TetrisTimer(); // запускаємо тетріс.
        }
    }
}

