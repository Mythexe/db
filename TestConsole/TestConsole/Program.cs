using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestConsole
{
    internal class Program
    {
        static void Main()//string[] args)
        {

            //float a = 0.03f;
            //float b = 0.0309257182638f;


            //double x = 2.012345678901234567E-03;

            List<Result> Values = new();

            Values.Add(new("Double", 3.14, "V"));
            Values.Add(new("Text", "Gut"));
            Values.Add(new("Messreihe", new double[] { 3.13, 12.0 },"A"));



            string test = string.Join(ResultHelper.RESULT_SPLITTER, Values);

            string[] arr = test.Split(ResultHelper.RESULT_SPLITTER[0]);
            Values.Clear();
            foreach (string s in arr) { 
                Values.Add(Result.FromString(s));
            }


            return;

            //var a = GetConstants(typeof(cooooo));
            //DoStuff();
            //string path = Directory.GetCurrentDirectory();


            //doooo a = new doooo();
            //SQLiteAccessor.Access<doooo> argh = new(path + "\\DBTest.db", "TestTable");
            //var are = argh.LoadDatasfromFile();
            //argh.SaveDatatoFile(a, "FirstName");//, "Textfield", "BlobField", "RealFiel", "NumericField");
        }

        public static class ResultHelper
        {
            public const string RESULT_SPLITTER = ">";
            static public string ResultToJson(Result result)
            {
                string s = JsonConvert.SerializeObject(new TPResult(result));
                return s;
            }
            static internal Result JsonToResult(string jsonString)
            {
                TPResult tPResult = JsonConvert.DeserializeObject<TPResult>(jsonString);
                dynamic val;
                string[] strings;
                try
                {
                    switch (tPResult.T)
                    {
                        case "Int32[]":
                            strings = Regex.Replace(tPResult.V.ToString().Replace('[', '{').Replace(']', '}'), "[{}\n\r]", string.Empty).Split(',');
                            val = Array.ConvertAll(strings, int.Parse);
                            break;
                        case "System.Double[]":
                            strings = Regex.Replace(tPResult.V.ToString().Replace('[', '{').Replace(']', '}'), "[{}\n\r]", string.Empty).Split(',');
                            val = Array.ConvertAll(strings, DoubleParseInvariant);
                            break;
                        case "System.String[]":
                            val = Regex.Replace(tPResult.V.rReplace('[', '{').Replace(']', '}'), "[{}\n\r]", string.Empty).Split(',');
                            break;
                        default:
                            val = Convert.ChangeType(tPResult.V, Type.GetType(tPResult.T));
                            break;
                    }
                    return new(tPResult.N, val, tPResult.U);
                }
                catch
                {
                    return new(tPResult.N, null, tPResult.U);
                }
            }

            static private double DoubleParseInvariant(string s)
            {
                return double.Parse(s, NumberFormatInfo.InvariantInfo);
            }
        }




        internal class TPResult
        {
            public string T { get; private set; }
            public string N { get; private set; }
            public dynamic V { get; private set; }
            public string U { get; private set; }

            private Result Res { get; set; }

            private Type type;

            public TPResult(string name, dynamic value, string unit = "")
            {
                type = value.GetType();
                T = value.GetType().FullName;
                N = name;
                V = value;
                U = unit;
                Res = new(name, value, unit);
            }
            public TPResult(Result res)
            {
                type = res.Value.GetType();
                T = res.Value.GetType().FullName;
                N = res.Name;
                V = res.Value;
                U = res.Unit;
                Res = res;
            }
            [JsonConstructor]
            public TPResult(string T, string N, dynamic V, string U = "")
            {
                this.type = Type.GetType(T);
                this.T = Type.GetType(T).FullName;
                this.N = N;
                this.V = V;
                this.U = U;
                Res = new(N, V, U);
            }

        }
        public class Result
        {
            public string Name { get; private set; }
            public dynamic Value { get; private set; }
            public string Unit { get; private set; }
            public Result(string name, dynamic value, string unit = "")
            {
                Name = name;
                Value = value;
                Unit = unit;
            }
            public override string ToString()
            {
                return ResultHelper.ResultToJson(this);
            }
            public static Result FromString(string json)
            {
                return ResultHelper.JsonToResult(json);
            }
        }





        //public class Result
        //{
        //    public string Name { get; private set; }
        //    public string Value { get; private set; }
        //    public string Unit { get; private set; }
        //    public Result(string name, string value, string unit = "")
        //    {
        //        Name = name;
        //        Value = value;
        //        Unit = unit;
        //    }

        //}














        static List<FieldInfo> GetConstants(Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public |
                 BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
        }



        public static void DoStuff()
        {
            Console.WriteLine(new StackFrame(0, true).GetMethod().Name);
        }
    }

    public class Doooo
    {
        //public int IntFiled = 1;
        private string firstName = "abc";

        public string FirstName { get => firstName; set => firstName = value; }
        //public object BlobField = null;
        //public double RealFiel = 3.14;
        //public decimal NumericField = 1234.5678M;



    }



   
    public class Cooooo
    {
        public const int RUN = 0;
        public const int FAILED = 1;
        public const int SUCCEEDED = 2;
        public const int FAILED_EXCEPTION = 3;
        public const int SUCCEEDED_EXCEPTION = 4;
        
        public int t = RUN;

        public Cooooo() { }

        public void Foo()
        {
            t++;
            if(t == SUCCEEDED_EXCEPTION) { t = RUN; }
        }
    }
}
