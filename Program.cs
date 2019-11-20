using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Test
{

    class Example {

        public int Index {get; private set;} 
        public string In {get; set;}
        public string Out {get; set; }

        public Example(int index, string inStr, string outStr) {
            Index = index;
            In = inStr;
            Out = outStr;
        }
        public Example(int index) {
            Index = index;
        }
    }

     class Program
    {
        public static Regex _inputFileRegex = new Regex(@"^input(?<index>\d+).txt$", RegexOptions.Compiled);
        public static Regex _outputFileRegex = new Regex(@"^output(?<index>\d+).txt$", RegexOptions.Compiled);

        static void Main(string[] args)
        {
            Console.Error.WriteLine("=========== Lancement du test =================");

            // Récupération des exemples
            Example[] examples = ReadExamples();

            // Mémorisation des writers standards de la console
            TextWriter stdOut = Console.Out;
            TextWriter stdErr = Console.Error;

            TeeTextWriter outConsole = null;
            TeeTextWriter errConsole = null;

            foreach (var ex in examples) 
            {
                Console.Error.WriteLine($"**** Test {ex.Index} : début ****");

                try {
                    // Remplacement de console.Out
                    outConsole = new TeeTextWriter(stdOut, "OUT : ");
                    errConsole = new TeeTextWriter(stdErr, "TRC : ");
                    Console.SetOut(outConsole);
                    Console.SetError(errConsole);

                    // Remplacement de Console.In
                    Console.SetIn(new StringReader(ex.In));

                    // Appel de la fonction à tester. Note : par reflexion car méthode non 'public' :-( 
                    Type typeToTest = Assembly.GetExecutingAssembly().GetType("CSharpContestProject.Program");
                    MethodInfo dynMethod = typeToTest.GetMethod("Main",  BindingFlags.NonPublic | BindingFlags.Static);
                    dynMethod.Invoke(null, new object[] { new string[] {} });

                } catch(Exception e) {
                    stdErr.WriteLine(e.ToString());

                } finally {
                    Console.SetOut(stdOut);
                    Console.SetError(stdErr);
                }

                // Vérification du résultat
                string obtainedOutput = PurgeCariageReturn(outConsole?.OutputText);
                string expectedOutput = PurgeCariageReturn(ex.Out);
                if (obtainedOutput == expectedOutput) {
                    Console.Error.WriteLine($"**** Test {ex.Index} : ok ****");
                } else {
                    Console.Error.WriteLine($"!!!! Test {ex.Index} : KO !!!!");
                    Console.Error.WriteLine($"Attendu : ");
                    Console.Error.WriteLine($"{expectedOutput}");
                    Console.Error.WriteLine($"**** Test {ex.Index} : fin ****");
                }
            }
        }

        // replace the carriage returns (\n, \r or \r\n) by just a \n.
        public static string PurgeCariageReturn(string s) {
            if (s == null) {
                return null;
            }

            string ret = s.Replace("\r\n", "\n");
            ret = ret.Replace("\r", "\n");
            if (ret.EndsWith("\n")) {
                ret = ret.Substring(0, ret.Length - 1);
            }
            return ret;
        }

        public static Example[] ReadExamples() {
            string[] lstZips = Directory.GetFiles("exemples")
                                        .Where(f => f.ToLowerInvariant().EndsWith(".zip"))
                                        .OrderByDescending(f => File.GetLastWriteTimeUtc(f))
                                        .ToArray();

            string zipFile = lstZips.First(); // Plante une exception si pas de zip trouvé : ça nous va bien

            Console.Error.WriteLine($"Lecture des exemples dans le fichier {zipFile}");

            var map = new Dictionary<int, Example>();

            ZipArchive archive = ZipFile.OpenRead(zipFile);
            foreach(ZipArchiveEntry entry in archive.Entries) {
                Match matchInput = _inputFileRegex.Match(entry.Name);
                Match matchOutput = _outputFileRegex.Match(entry.Name);

                if (matchInput.Success) {
                    int val = int.Parse(matchInput.Groups["index"].Value);
                    Example exemple;
                    if (! map.TryGetValue(val, out exemple)) {
                        exemple = new Example(val);
                        map.Add(val, exemple);
                    }
                    var strReader = new StreamReader(entry.Open(), Encoding.Default);
                    exemple.In = strReader.ReadToEnd();
                }
                if (matchOutput.Success) {
                    int val = int.Parse(matchOutput.Groups["index"].Value);
                    Example exemple;
                    if (! map.TryGetValue(val, out exemple)) {
                        exemple = new Example(val);
                        map.Add(val, exemple);
                    }
                    var strReader = new StreamReader(entry.Open(), Encoding.Default);
                    exemple.Out = strReader.ReadToEnd();
                }
            }

            Example[] ret = map.OrderBy(kv => kv.Key).Select(kv => kv.Value).ToArray();
            Console.Error.WriteLine($"Trouvé {ret.Length} exemples.");
            return ret;
        }
    }
}
