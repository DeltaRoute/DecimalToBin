using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;

namespace DecimalToBin
{
    internal class Program
    {

        const int bits = 7;
        const uint mask = 128;
        static bool owerflow_in = false;
        static bool owerflow_out = false;
        static bool parity = false; //четность двоичного числа
        static bool signum = false; // результат интерпретируется как беззнаковый по условию
        static bool[] DecToBin(int value)
        {
            bool[] result = new bool[bits];
            int i;
            uint tmp_value = (uint)value % mask;//ограничение числа при переполнении разрядной сетки
            owerflow_in |= (tmp_value != value);//если после обрезания числа не равны, значит было переполнение разрядной сетки
            for (i = 1; i <= bits; i++)
            {
                result[bits-i] = (tmp_value % 2) == 1;
                tmp_value = (tmp_value / 2);
            }
            return result;
        }
        static int BinToDec(bool[] value)//восстанавливаем десятичное число по его двоичной форме
        {
            int result = 0;
            int i,step = 1;
            for (i = bits-1; i >= 0; i--)
            {
                result += value[i] ? step : 0;
                step = step * 2;
            }
            return result;
        }
        static bool[] Inv(bool[] value)//предполагается, что сюда будут отправлться уже двоичные числа
        {
            bool[] result = new bool[bits];
            int i;
            for (i = 0; i < bits; i++)
                result[i] = !value[i];
            return result;
        }
        static bool[] Sub(bool[] a, bool[] b)
        {
            bool[] result = new bool[bits];

            b = Inv(b);
            int i;
            bool capacitor = true;//чекайте схему полусумматора с переносом бит
            bool quartSum;
            for (i = bits-1; i >= 0; i--)//старший бит будет вычислен вне цикла
            {
                quartSum = (!a[i] & b[i]) | (a[i] & !b[i]);
                result[i] = (quartSum & !capacitor) | (!quartSum & capacitor);
                capacitor = (quartSum & capacitor) | (a[i] & b[i]);
            }
            parity = result[0];
            for (i = bits - 1; i > 0; i--)
                parity = (parity & !result[i]) | (!parity & result[i]);
            owerflow_out = capacitor;
            return result;
        }
        static string Print(bool[] value)
        {
            string result = "";
            for (int i = 0; i < bits; i++)
                result += value[i] ? "1" : "0";
            return result;
        }
        static void Main(string[] args)
        {
            int first_value, second_value;
            bool[] bin_first_value, bin_second_value;
            string tmp_first, tmp_second;
            Console.Write("Введите первое число:");
            tmp_first =  Console.ReadLine();
            Console.Write("Введите второе число:");
            tmp_second = Console.ReadLine();
            //проверка чисел на корректность
            if (Int32.TryParse(tmp_first, out first_value) && Int32.TryParse(tmp_second, out second_value))
            {
                bin_first_value = DecToBin(first_value);
                bin_second_value = DecToBin(second_value);
                bool[] c = Sub(bin_first_value, bin_second_value);
                Console.Write(@"{0} {1} {2} {3} {4} {5}",
                                Print(c),BinToDec(c),signum ? 1 : 0,parity ? 0 : 1,
                                owerflow_out ? 1 : 0 ,owerflow_in ? 1 : 0);
            }
            Console.Read();//если запускаете без отладки, то можно убрать эту строку
        }
    }
}
