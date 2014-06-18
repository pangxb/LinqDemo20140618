using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;

namespace LinqDemo20140618
{
    class Program
    {
        static void Main(string[] args)
        {
            //查找不同的Frame Number
            Console.WriteLine("查找不同的Frame Number, todo...");
            var syncInfo = Utility.GenerateSynchDisplayInfo();
         
            syncInfo.Select(kp => kp.Value.FrameNumber).Distinct().ToList().ForEach(frameNumber =>
                Console.WriteLine("Distinct frame number found:{0}", frameNumber)
                );

            //根据查找不同的Block Time
            Console.WriteLine("查找不同的Block Time");
            syncInfo = Utility.GenerateSynchDisplayInfo();

            syncInfo.Select(kp => kp.Value.BlockTime).Distinct().ToList().ForEach(blocktime =>
            Console.WriteLine("Distinct block time found:{0}", blocktime)
            );

            syncInfo.Select(kp => kp.Value.BlockTime).Where(time => !time.Contains("2015")).Distinct().ToList().ForEach(blocktime =>
              Console.WriteLine("Distinct block time found:{0}", blocktime)
              );

            //匿名类的使用,选三个数,abc,使得a+b+c = 15
            var range = Enumerable.Range(1, 9);

            (from a in range
             from b in range
             from c in range
             where ((a < b) && (b < c) && (a + b + c) == 15)
             select new
                 {
                     A = a,
                     B = b,
                     C = c,
                 }).ToList().ForEach(ret =>
                    {
                        Console.WriteLine("Method 1st, New match found A = {0}, B = {1}, C = {2}", ret.A, ret.B, ret.C);
                    });



            (from a in range
             from b in range
             let c = (15-a-b)
             where (a < b && b < c && c<10)
             select new
             {
                 A = a,
                 B = b,
                 C = c,
             }).ToList().ForEach(ret =>
             {
                 Console.WriteLine("Method 2nd, New match found A = {0}, B = {1}, C = {2}", ret.A, ret.B, ret.C);
             });




            (from a in range
             from b in range
             let c = (15 - a - b)
             where (a < b && b < c && c < 10)
             select new
             {
                 A = a,
                 B = b,
                 C = c,
             }).All(ret =>
             {
                 Console.WriteLine("Method 3th, New match found A = {0}, B = {1}, C = {2}", ret.A, ret.B, ret.C);
                 return true;
             });


            (from a in range
             from b in range
             let c = (15 - a - b)
             where (a < b && b < c && c < 10)
             select new
             {
                 A = a,
                 B = b,
                 C = c,
             }).AsParallel().ForAll(ret => Console.WriteLine("Method 4th, New match found A = {0}, B = {1}, C = {2}", ret.A, ret.B, ret.C));

            //cast
            ArrayList al = new ArrayList()
            {
                "a","b","c","d","e"
            };
            var maxStr = al.Cast<string>().Max();// 如果包含int会怎样?

            //oftype
            object[] objArray = {1,2,3.33,"cde", new DSPFrame() };

            var maxInt = objArray.OfType<int>().FirstOrDefault();


            //Func委托,聚合函数等           

            //仅用于泛型接口和泛型委托
            //convariant 作为泛型的类型参数的基类可以被子类代替(子类->基类), 并作为输出参数(out)
            //contravariant 作为泛型的类型参数的基类可替代子类(基类->子类), 并作为输入参数(in)

            //协变
            Func<BaseClass> covariantDelegate = () =>
                {
                    return new DerivedClass();
                };            

            covariantDelegate();

            IEnumerable<DerivedClass> dcList = new List<DerivedClass>();
            IEnumerable<BaseClass> bsList = dcList;


            //不受支持
            //Func<DerivedClass> test = covariantDelegate;

            //逆变
            Action<BaseClass> baseContravariantDelegate = dc =>
                {
                    //todo
                };
            Action<DerivedClass> contravariantDelegate = baseContravariantDelegate;

            //IComparable<BaseClass>

            //不受支持
            //contravariantDelegate(new BaseClass());



            Func<BaseClass, DerivedClass> del_bc_dc = bc =>
                {
                    return new DerivedClass();
                };

          //  Func<BaseClass, BaseClass> del_bc_bc = del_bc_dc;
          //  Func<DerivedClass,DerivedClass> del_dc_dc = del_bc_dc;
           // Func<DerivedClass,BaseClass> del_dc_bc = del_bc_dc;




            int[] array = Enumerable.Range(1, 100).ToArray();

            array.AsParallel().ForAll(i =>
            {
                Console.WriteLine("AsParallel loop array {0}", i);
            });
     






            //聚合函数求和
            int sum = array.Aggregate((a, b) => a + b);

            //聚合函数算阶乘  
            Console.WriteLine("10! = {0}", Enumerable.Range(1, 10).Aggregate((a, b) => a * b));


            //聚合函数,Tuple使用,Func委托
            List<Tuple<string, string>> myList = new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("西湖","杭州"),
                new Tuple<string,string>("中国",null),
                new Tuple<string,string>("浙江","中国"),
                new Tuple<string,string>("杭州","浙江")
            };

            string myCountry = myList.Aggregate(myList[0], (child, parent) =>
                {
                    if (child.Item2 == null)
                    {
                        return child;
                    }
                    else
                    {
                        return myList.First(tp => tp.Item1 == child.Item2);
                    }
                }).Item1;

            Console.WriteLine("My Country is {0}",myCountry);


            for(int i=0;i<10;i++)
            {
                var result = Utility.CheckSomething();
                Console.WriteLine("CheckSomething, result is {0}",result);
                Thread.Sleep(1000);
            }

            //并发集合的使用
            Console.WriteLine("Concurrent Collection Sample");
            Task t1 = Task.Factory.StartNew(() =>
                {
                    while(true)
                    {
                        Utility.WriteFrame(new DSPFrame()
                        {
                            FrameIndex = Utility.GetIndex()
                        });
                        Thread.Sleep(1000);
                    }                  
                });

            Task t2 = Task.Factory.StartNew(() =>
            {
                while(true)
                {
                    Utility.WriteFrame(new DSPFrame()
                    {
                        FrameIndex = Utility.GetIndex()
                    });
                    Thread.Sleep(500);
                }
             
            });

            Task t3 = Task.Factory.StartNew(() =>
                { 
                    while(true)
                    {
                        int index = Utility.GetRemoveIndex();
                        Utility.RemovePreviousFrame(index);
                        Thread.Sleep(750);
                    }                 
                });

            Console.Read();

        }
    }



  
}
