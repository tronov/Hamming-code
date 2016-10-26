using System;

namespace Hamming_code
{
    public static class Extensions
    {
        public static string AsString(this bool bit)
        {
            return (new bool[] { bit }).AsString();
        }

        public static string AsString(this bool[] arr)
        {
            char[] cha = new char[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i]) cha[i] = '1';
                else cha[i] = '0';
            }
            return new string(cha);
        }
    }

    class Program
    {
        private const int COMMAND_LENGTH = 3;   // Разрядность команды
        private static byte ncom;               // Номер команды от 0 до 7
        private static bool[]
            com,              // Массив битов команды
            ctrl,             // Массив контрольных битов
            ham,              // Закодированное сообщение
            rcv,              // Искаженный код
            sum,              // Массив контрольных сумм
            mask,             // Корректирующая маска
            corr,             // Скорректированые код
            dmsg;             // Восстановленное сообщение


        private static byte GetCommandNumber()
        {
            Console.Write("Please, specify the instruction number from 0 to 7: ");
            string str = Console.ReadLine();
            byte num;
            if (Byte.TryParse(str, out num))
                if (num < 8) return num;
            Console.WriteLine("Incorrect choice.");
            return GetCommandNumber();
        }

        private static bool[] ADC(byte number)
        {
            bool[] result = new bool[COMMAND_LENGTH];

            for (int i = COMMAND_LENGTH - 1; i >= 0; i--)
            {
                result[i] = number % 2 > 0;
                number /= 2;
            }
            return result;
        }

        private static bool[] GetMask(bool[] a)
        {
            return new bool[] {
                !a[0] & !a[1] &  a[2],
                !a[0] &  a[1] & !a[2],
                !a[0] &  a[1] &  a[2],
                 a[0] & !a[1] & !a[2],
                 a[0] & !a[1] &  a[2],
                 a[0] &  a[1] & !a[2]
            };
        }

        static void Main(string[] args)
        {
            while (true)
            {
                ncom = GetCommandNumber();       // Получение номера команды с клавиатуры
                com = ADC(ncom);                 // Исходное сообщение                               

                // Определение контрольных битов
                ctrl = new bool[] {
                    com[0] ^ com[1],
                    com[0] ^ com[2],
                    com[1] ^ com[2]
                };

                // Построение кода Хемминга
                ham = new bool[] { ctrl[0], ctrl[1], com[0], ctrl[2], com[1], com[2] };

                // "Передача" сообщения
                rcv = (bool[])ham.Clone();
                // Искажение пятого бита
                rcv[4] = !rcv[4];

                // Вычисление контрольных битов
                sum = new bool[] {
                    rcv[0] ^ rcv[2] ^ rcv[4],
                    rcv[1] ^ rcv[2] ^ rcv[5],
                    rcv[3] ^ rcv[4] ^ rcv[5]
                };

                // Генерация корректирующей битовой маски
                mask = GetMask(sum);

                // Наложение корректирующей маски на принятое сообщение
                corr = new bool[] {
                   rcv[0] ^ mask[0],
                   rcv[1] ^ mask[1],
                   rcv[2] ^ mask[2],
                   rcv[3] ^ mask[3],
                   rcv[4] ^ mask[4],
                   rcv[5] ^ mask[5]
               };

                dmsg = new bool[] { corr[2], corr[4], corr[5] };

                Console.WriteLine("Instant message:\t  {0} {1}{2}",
                    com[0].AsString(), com[1].AsString(), com[2].AsString());
                Console.WriteLine("Control bits:\t\t{0}{1} {2}",
                    ctrl[0].AsString(), ctrl[1].AsString(), ctrl[2].AsString());
                Console.WriteLine("Hamming code:\t\t{0}", ham.AsString());
                Console.WriteLine("Transmitted code:\t{0}", rcv.AsString());
                Console.WriteLine("Checksums:\t\t{0}{1} {2}",
                    sum[0].AsString(), sum[1].AsString(), sum[2].AsString());
                Console.WriteLine("Correction mask:\t{0}", mask.AsString());
                Console.WriteLine("Adjusted code:\t\t{0}", corr.AsString());
                Console.WriteLine("Decoded message:\t  {0} {1}{2}",
                    dmsg[0].AsString(), dmsg[1].AsString(), dmsg[2].AsString());
                Console.WriteLine();
            }
        }
    }
}
