using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ShortestPath
{
    class Program
    {

        // Функция для преобразования десятичного числа в массив последовательности бит этого двоичного числа
        static int[] DecimalToBinaryMassive(int nInDecimal, int bytesAmount)
        {
            int[] binarySequence = new int[bytesAmount];
            string binaryCode = Convert.ToString(nInDecimal, 2);
            while (binaryCode.Length < bytesAmount)
                binaryCode = "0" + binaryCode;
            for (uint i = 0; i < bytesAmount; i++)
                binarySequence[i] = binaryCode[(int)i] - 48;

            return binarySequence;
        }

        // Функция для слияния двух массивов
        static int[] MassiveMerge(int[] m1, int[] m2)
        {
            int[] resultMassive = new int[m1.Length + m2.Length];
            for (uint i = 0; i < m1.Length + m2.Length; i++)
                if (i < m1.Length)
                    resultMassive[i] = m1[i];
                else
                    resultMassive[i] = m2[i - m1.Length];

            return resultMassive;
        }

        // Функция добавления ребра графа в BDD форму
        static void AddEdgeToBDD(int[] seq, ref BDDNode root, ref BDDManager a)
        {
            var x = root;
            for (uint i = 0; i < seq.Length; i++)
            {
                if (seq[i] == 0)
                {
                    if (x.Low == a.Zero && i + 1 < seq.Length)
                    {
                        x.Low = new BDDNode(x.Index + 1, a.Zero, a.Zero);
                        x = x.Low;
                    }
                    else if (x.Low == a.One && i + 1 == seq.Length)
                    {
                        Console.WriteLine("Ты уже отметил здесь единицу!");
                        break;
                    }
                    else if (x.Low == a.Zero && i + 1 == seq.Length)
                    {
                        x.Low = a.One;
                    }
                    else
                        x = x.Low;
                }
                if (seq[i] == 1)
                {
                    if (x.High == a.Zero && i + 1 < seq.Length)
                    {
                        x.High = new BDDNode(x.Index + 1, a.Zero, a.Zero);
                        x = x.High;
                    }
                    else if (x.High == a.One && i + 1 == seq.Length)
                    {
                        Console.WriteLine("Ты уже отметил здесь единицу!");
                        break;
                    }
                    else if (x.High == a.Zero && i + 1 == seq.Length)
                    {
                        x.High = a.One;
                    }
                    else
                        x = x.High;
                }
            }
        }

        // Получить новый корень
        static BDDNode GetNewRoot(int[] sequence, ref BDDNode root)
        {
            BDDNode x;
            if (sequence[0] == 1)
                x = root.High;
            else
                x = root.Low;

            for (int i = 1; i < sequence.Length; i++)
            {
                if (sequence[i] == 1)
                    x = x.High;
                if (sequence[i] == 0)
                    x = x.Low;
            }
            return x;
        }

        // Функция для создания BDD по матрице весов
        static void AdjMatToBDD(int[,] adjMat, int bytesForNode, int bytesForWeight
                              , ref BDDNode root, ref BDDManager BDD)
        {
            int[] sequence;

            for (int i = 0; i < adjMat.GetLength(0); i++)
            {
                for (int j = 0; j < adjMat.GetLength(0); j++)
                {
                    if (adjMat[i, j] != 0 && i != j)
                    {
                        sequence = MassiveMerge(DecimalToBinaryMassive(i, bytesForNode)
                                              , DecimalToBinaryMassive(j, bytesForNode));
                        sequence = MassiveMerge(sequence, DecimalToBinaryMassive(adjMat[i, j], bytesForWeight));

                        AddEdgeToBDD(sequence, ref root, ref BDD);
                    }
                }
            }
        }

        // DFS и обновление delta по необходимости
        static void DFSAndDeltaUpdate(ref BDDNode root, ref BDDManager BDD, ref string sequence
                        , int[] newRootSeq, int bytesForNode, int bytesForWeight, ref uint[] delta, ref uint[] pred)
        {
            if (root.Value != false && root.Value != true)
            {
                sequence += "0";
                var x = root.Low;
                DFSAndDeltaUpdate(ref x, ref BDD, ref sequence, newRootSeq, bytesForNode, bytesForWeight
                                , ref delta, ref pred);
                sequence = sequence.Remove(sequence.Length - 1, 1);
                sequence += "1";
                x = root.High;
                DFSAndDeltaUpdate(ref x, ref BDD, ref sequence, newRootSeq, bytesForNode, bytesForWeight
                                , ref delta, ref pred);
                sequence = sequence.Remove(sequence.Length - 1, 1);
            }
            if (root.Value == true)
                DeltaUpdate(ref delta, sequence, newRootSeq, bytesForNode, bytesForWeight, ref pred);
        }

        // Обновление информации в векторе дельта, где храняться известные пути до вершин
        static void DeltaUpdate(ref uint[] delta, string sequence, int[] newRootseq
                               , int bytesForNode, int bytesForWeight, ref uint[] pred)
        {
            uint edgeBetweenV_U = 0, vInDecimal = 0, uInDecimal = 0;

            for (int i = 0; i < bytesForWeight; i++)
            {
                if (sequence[sequence.Length - 1 - i] == '1')
                    edgeBetweenV_U += (uint)Math.Pow(2, i);
            }

            for (int i = 0; i < bytesForNode; i++)
            {
                if (sequence[sequence.Length - i - 1 - bytesForWeight] == '1')
                    uInDecimal += (uint)Math.Pow(2, i);
                if (newRootseq[bytesForNode - 1 - i] == 1)
                    vInDecimal += (uint)Math.Pow(2, i);
            }
            if (delta[uInDecimal] > delta[vInDecimal] + edgeBetweenV_U)
            {
                delta[uInDecimal] = delta[vInDecimal] + edgeBetweenV_U;
                pred[uInDecimal] = vInDecimal;
            }
        }

        static string Reverse(string text)
        {
            char[] cArray = text.ToCharArray();
            string reverse = String.Empty;
            for (int i = cArray.Length - 1; i > -1; i--)
            {
                reverse += cArray[i];
            }
            return reverse;
        }

        static void Main(string[] args)
        {
            const uint INF = 4000000000;
            int[,] adjMat;
            string sequence = "";
            int v_length_BDD, weight_length_BDD, wei_i_j;
            uint v_number = 0, source, sink, min, max_weight = 0;

            string path = @"", fileName;
            Console.Write("Укажите дирректорию, где хранится файл (без пробелов в именах): ");
            path = Console.ReadLine();
            if (path.Split(' ').Length > 1)
            {
                Console.WriteLine("Некорректно введена дирректория.");
                Console.ReadLine();
                return;
            }
            Console.Write("Укажите имя файла: ");
            fileName = Console.ReadLine();
            if (fileName.Split(' ').Length > 1)
            {
                Console.WriteLine("Некорректно введено имя.");
                Console.ReadLine();
                return;
            }
            Directory.CreateDirectory(path);
            fileName = Path.Combine(path, fileName);



            if (!File.Exists(fileName))
            {
                Console.WriteLine("К сожалению, файл не найден. Проверьте всё и попробуйте снова");
                Console.ReadLine();
                return;
            }

            FileStream graph = new FileStream(fileName, FileMode.Open);


            StreamReader reader = new StreamReader(graph);

            string s = "";


            while (s != "#")
            {
                s = reader.ReadLine();
                v_number += 1;
            }
            v_number -= 1;

            adjMat = new int[v_number, v_number];
            int i, j;
            string part;

            while (true)
            {
                part = "";
                int n = 0;
                s = reader.ReadLine();
                if (s == null)
                    break;

                while (s[n] != ' ')
                {
                    part += s[n];
                    n += 1;
                }
                i = Convert.ToInt32(part, 10) - 1;
                n += 1;
                part = "";

                while (s[n] != ' ')
                {
                    part += s[n];
                    n += 1;
                }
                j = Convert.ToInt32(part, 10) - 1;
                n += 1;
                part = "";

                while (n < s.Length)
                {
                    part += s[n];
                    n += 1;
                }
                wei_i_j = Convert.ToInt32(part, 10);

                if (wei_i_j > max_weight)
                    max_weight = (uint)wei_i_j;

                adjMat[i, j] = wei_i_j;
            }
            reader.Close();


            Console.Write("Введите номер вершины, от которой будет идти поиск расстояния (индексация с 0): ");
            uint.TryParse(Console.ReadLine(), out source);
            if (source < 0 || source >= v_number)
            {
                Console.WriteLine("Введены некорректные данные. Попробуйте заново");
                Console.ReadLine();
                return;
            }
            Console.Write("Введите номер вершины, до которой ищем расстояние: ");
            uint.TryParse(Console.ReadLine(), out sink);
            if (sink < 0 || sink >= v_number)
            {
                Console.WriteLine("Введены некорректные данные. Попробуйте заново");
                Console.ReadLine();
                return;
            }


            uint[] delta = new uint[v_number];
            bool[] visited = new bool[v_number];
            uint[] pred = new uint[v_number];

            v_length_BDD = (int)Math.Ceiling(Math.Log(v_number, 2));
            weight_length_BDD = (int)Math.Ceiling(Math.Log(max_weight + 1, 2));
            int BDD_height = 2 * v_length_BDD + weight_length_BDD;
            int[] minIndexToBinary = new int[v_length_BDD];

            var BDD_repr = new BDDManager(BDD_height);
            var root = new BDDNode(0, BDD_repr.Zero, BDD_repr.Zero);

            AdjMatToBDD(adjMat, v_length_BDD, weight_length_BDD, ref root, ref BDD_repr);

            int min_index = (int)source;

            for (uint n = 0; n < v_number; n++)
                if (n != source)
                    delta[n] = INF;

            for (int n = 0; n < v_number; n++)
            {
                min = INF;
                for (int l = 0; l < v_number; l++)
                {
                    if (delta[l] < min && visited[l] == false)
                    {
                        min = delta[l];
                        min_index = l;
                    }
                }

                if (min_index == sink)
                {
                    string way = "";
                    Console.Write("Поздравляю, путь найден. Он равен: " + delta[sink]
                                    + ", выглядит следующим образом: ");
                    way += sink;
                    while (pred[sink] != source)
                    {
                        way += pred[sink];
                        sink = pred[sink];
                    }
                    way += source;
                    way = Reverse(way);

                    fileName = Path.Combine(path, "result.tgf");

                    if (File.Exists(fileName))
                        File.Delete(fileName);

                    string infoToTgf = "";



                    for (int l = 0; l < way.Length; l++)
                    {
                        if (l != way.Length - 1)
                        {
                            Console.Write(way[l] + "-");
                            infoToTgf += way[l] + " " + way[l + 1] + '\n';
                        }
                        else
                            Console.Write(way[l]);
                    }

                    Byte[] info = new UTF8Encoding(true).GetBytes(infoToTgf);
                    using (FileStream fs = File.Create(fileName))
                        fs.Write(info, 0, info.Length);



                    Console.WriteLine();
                    Console.WriteLine("В дирректории с графом создан новый файл, показывающий путь");

                    Console.ReadLine();
                    return;
                }

                visited[min_index] = true;
                minIndexToBinary = DecimalToBinaryMassive(min_index, v_length_BDD);

                var newRoot = GetNewRoot(minIndexToBinary, ref root);
                DFSAndDeltaUpdate(ref newRoot, ref BDD_repr, ref sequence, minIndexToBinary
                                , v_length_BDD, weight_length_BDD, ref delta, ref pred);

            }
            Console.WriteLine("К сожалению, в графе нет пути между указанными вершинами");
            Console.ReadLine();
        }
    }
}
