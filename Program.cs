using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace wordnet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" welcome in WordNet ");
            Console.WriteLine("__________________________________________________________________________");
        Continue_Program:
            Console.WriteLine("\n[1] Sample test cases\n[2] Complete test cases\n[3] Exit\n");
            Console.Write("Please enter your choice : ");
            char Choice = (char)Console.ReadLine()[0]; if (Choice == '1')
            {
                Console.WriteLine("\nwhich case you wanna run\n");
                Console.Write("Please pick up number between 1 to 6: ");
                char case_num = (char)Console.ReadLine()[0];
                if (case_num <= '6')
                {
                    if (case_num == '1' || case_num == '3' || case_num == '4')
                    {
                        string FileInfo = Read_Test_File("1synsets " + case_num + ".txt", "2hypernyms " + case_num + ".txt", "3RelationsQueries " + case_num + ".txt", "4OutcastQueries " + case_num + ".txt");
                    }
                    else
                    {
                        string FileInfo = Read_Test_File("1synsets " + case_num + ".txt", "2hypernyms " + case_num + ".txt", "3RelationsQueries " + case_num + ".txt", "");
                    }
                    goto Continue_Program;
                }
                else
                {
                    Console.Write("Please pick a right number\n");
                }
                goto Continue_Program;
            }
            else if (Choice == '2')
            {
                Console.WriteLine("\nwhich case you wanna run\n");
                Console.Write("Please pick up number between 1 to 7: "); char case_num = (char)Console.ReadLine()[0];
                if (case_num <= '6')
                {
                    string FileInfo = Read_Test_File("1synsets_case " + case_num + ".txt", "2hypernyms_case " + case_num + ".txt", "3RelationsQueries_case " + case_num + ".txt", "4OutcastQueries_case " + case_num + ".txt");
                    goto Continue_Program;
                }
                else if (case_num <= '7')
                {
                    string FileInfo = Read_Test_File("1synsets_case " + case_num + ".txt", "2hypernyms_case " + case_num + ".txt", "3RelationsQueries_case " + case_num + ".txt", "");
                    goto Continue_Program;
                }
                else
                {
                    Console.Write("Please pick a right number\n");
                }
                goto Continue_Program;
            }
            else if (Choice == '3')
            {
                goto EndProgram;
            }
            else
            {
                Console.WriteLine("Invalid choice, Please try agian\n");
                goto Continue_Program;
            }
        EndProgram:
            Console.WriteLine("__________________________________________________________________________");
        }

        public static int mdis = 100000;
        public static Dictionary<int, List<string>> vertices = new Dictionary<int, List<string>>();
        public static int size = 0;
        public static Dictionary<string, List<int>> Id = new Dictionary<string, List<int>>();
        public static Dictionary<int, List<int>> parents = new Dictionary<int, List<int>>();
        public static List<int> distance1;
        public static List<int> distance2;
        public static List<bool> local1;
        public static List<bool> local2; 
        public static List<bool> global = new List<bool>();
        public static List<int> disGlobal = new List<int>();
        public static Stopwatch stopwatch = new Stopwatch();
        public static Stopwatch s_outcast = new Stopwatch();
        static List<int> ids1 = new List<int>();
        static List<int> ids2 = new List<int>();
        static string Read_Test_File(string FileName1, string FileName2, string FileName3, string FileName4)
        {
            StreamReader synsets = new StreamReader(FileName1);
            string synset = string.Empty;
            int myRoot;
            while ((synset = synsets.ReadLine()) != null)
            {
                size++;
                string[] split = synset.Split(',');
                string[] nouns = split[1].Split(' ');
                List<string> Verlist = new List<string>();
                foreach (string noun in nouns)
                {
                    if (!Id.ContainsKey(noun))
                    {
                        Id.Add(noun, new List<int>());
                    }
                    Id[noun].Add(int.Parse(split[0]));
                    Verlist.Add(noun);
                }
                vertices.Add(int.Parse(split[0]), Verlist);
                disGlobal.Add(-1);
                global.Add(false);
            }
            StreamReader hypernyms = new StreamReader(FileName2);
            string hypernym = string.Empty;
            while ((hypernym = hypernyms.ReadLine()) != null)
            {
                string[] split = hypernym.Split(',');
                List<int> parent = new List<int>();
                 if (split.Length == 1)
                 {
                     myRoot = Int32.Parse(split[0]);
                     parents.Add(Int32.Parse(split[0]), null);
                 }
                 else
                 {
                     for (int i = 1; i < split.Length; i++)
                     {
                         parent.Add(Int32.Parse(split[i]));
                         if (i + 1 == split.Length)
                         {
                             parents.Add(Int32.Parse(split[0]), parent);
                         }
                     }
                 }
                
            }
            FileStream relationoutput = new FileStream(@"D:\relation.txt", FileMode.OpenOrCreate);
            StreamWriter RelationOutPut = new StreamWriter(relationoutput);
           // stopwatch.Start();
            //int count = 0;
            StreamReader RelationsQueries = new StreamReader(FileName3);
            string relation = string.Empty;
            while ((relation = RelationsQueries.ReadLine()) != null)
            {
                List<int> ids1;
                List<int> ids2;
                
                string[] singleline = relation.Split(',');
                if (singleline.Length == 1)
                {
                    continue;
                }
                else
                {
                    
                    ids1 = toId(singleline[0]);
                    ids2 = toId(singleline[1]);
                    int min = int.MaxValue;
                    int par = 0;
                    foreach (int x in ids1)
                    {
                        foreach (int y in ids2)
                        {
                            local1 = new List<bool>();
                            local2 = new List<bool>();
                            local1.AddRange(global);
                            local2.AddRange(global);
                            distance1 = new List<int>();
                            distance2 = new List<int>();
                            distance1.AddRange(disGlobal);
                            distance2.AddRange(disGlobal);
                            ////Tuple<int, int> q  = get_PD(x, y, distance1, distance2, local1, local2);
                            //if (q.Item2 < min)
                            //{
                            //    min = q.Item2;
                            //    par = q.Item1;
                            //}
                           RelationOutPut.Write(min + "," + string.Format("{0}", string.Join(" ", toNoun(par))));
                            
                        }
                        RelationOutPut.WriteLine();
                        
                    }
                }
                distance1 = new List<int>();
                distance2 = new List<int>();
                local1 = new List<bool>();
                local2 = new List<bool>();
            }
            // stopwatch.Stop();
            // Console.WriteLine("time is : " +stopwatch.Elapsed);
            //s_outcast.Start();
            FileStream Outcast = new FileStream(@"D:\outcast.txt", FileMode.OpenOrCreate);
            StreamWriter outcast = new StreamWriter(Outcast);
            StreamReader OutcastQueries = new StreamReader(FileName4);
            string Qutcast = string.Empty;
            while ((Qutcast = OutcastQueries.ReadLine()) != null)
            {
                string[] SingleLine = Qutcast.Split(',');
                if (SingleLine.Length == 1)
                {
                    continue;
                }
                List<string> words = new List<string>(); //list of pairs of the the test file each node of the pair in line 
                for (int i = 0; i < SingleLine.Length; i++)
                    words.Add(SingleLine[i]);

            }
            //s_outcast.Stop();
            // Console.WriteLine("time is : " +s_outcast.Elapsed);
            return "";
            
        }

        // All Functions
        public static List<string> toNoun(int node)
        {
            return vertices[node];
        }
        public static List<int> toId(string node)
        {
            return Id[node];
        }
        
    }
}
    

