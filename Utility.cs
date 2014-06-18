using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;

namespace LinqDemo20140618
{
    public static class Utility
    {
        static Random r;

        static ConcurrentDictionary<int, DSPFrame> signalFrames;

        static Utility()
        {
            r = new Random((int)DateTime.Now.Ticks);
            signalFrames = new ConcurrentDictionary<int, DSPFrame>();
        }

        internal static IDictionary<int, SynchronizationDisplayInfo> GenerateSynchDisplayInfo()
        {
            IDictionary<int, SynchronizationDisplayInfo> dicInfo = new ConcurrentDictionary<int, SynchronizationDisplayInfo>();

            string blockTime = DateTime.Now.ToString();
            int frameNumber = r.Next(1111, 2222);

            foreach (int id in Enumerable.Range(r.Next(100, 1000), r.Next(10, 20)).Select(g => g * g))
            {
                dicInfo[id] = new SynchronizationDisplayInfo()
                {
                    BlockTime = blockTime,
                    DeviceId = id,
                    FrameNumber = frameNumber
                };
            }

            dicInfo[int.MinValue] = new SynchronizationDisplayInfo()
            {
                BlockTime = blockTime.Replace("2014", "2015"),
                FrameNumber = frameNumber,
                DeviceId = int.MinValue
            };

            dicInfo[int.MaxValue] = new SynchronizationDisplayInfo()
            {
                BlockTime = blockTime,
                FrameNumber = frameNumber + 1,
                DeviceId = int.MaxValue
            };

            return dicInfo;
        }

        internal static int GetIndex()
        {
            return r.Next(1, 100000);
        }


        internal static int GetRemoveIndex()
        {
            var orderedKeys = signalFrames.Keys.OrderBy(key => key);
            if(orderedKeys.Count() >= 2)
            {
                return orderedKeys.Skip(orderedKeys.Count() / 2).First(); //Take(1).....
            }
            else
            {
                return -1;
            }
        }


        internal static void WriteFrame(DSPFrame frame)
        {
            signalFrames[frame.FrameIndex] = frame;
            Console.WriteLine("Write frame at {0},by thread {1}", frame.FrameIndex, Thread.CurrentThread.ManagedThreadId);
        }

        internal static void RemovePreviousFrame(int frameIndex)
        {
            if (frameIndex == -1) return;

            signalFrames.Keys.Where(key => key <= frameIndex).ToList().ForEach(index =>
                {
                    DSPFrame frame;
                    if (signalFrames.TryRemove(index, out frame))
                    {
                        Console.WriteLine("Remove frame at {0}", frame.FrameIndex);
                    }
                    else
                    {
                        Console.WriteLine("Failed to remove signal frame");
                    }                    
                }
                );
        }

        internal static void RemoveHalfSizeFrames()
        {
            int halfSize = signalFrames.Count / 2;
            signalFrames.Keys.OrderBy(key => key).Take(halfSize).ToList().ForEach(index =>
                {
                    DSPFrame frame;
                    if (signalFrames.TryRemove(index, out frame))
                    {
                        Console.WriteLine("Remove frame at {0}", frame.FrameIndex);
                    }
                    else
                    {
                        Console.WriteLine("Failed to remove signal frame");
                    }          
                }
                );
        }

        internal static bool CheckSomething(out string message)
        {
            bool checkResult = r.Next() % 2 == 0;
            message = string.Empty;

            if (checkResult)
            {
                message = "Invalid!";
            }
            else
            {
                message = "Valid!";
            }

            return checkResult;
        }

        internal static Tuple<bool,string> CheckSomething()
        {
            bool checkResult = r.Next() % 2 == 0;
            string message = string.Empty;

            if (checkResult)
            {
                message = "Invalid!";
            }
            else 
            {
                message = "Valid!";
            }

            return new Tuple<bool, string>(checkResult, message);
        }

      
    }

    public class SynchronizationDisplayInfo
    {
        public int DeviceId { get; set; }
        public int FrameNumber { get; set; }
        public string BlockTime { get; set; }
    }

    public class DSPFrame
    {
        public int FrameIndex { get; set; }
        public double[][] FrameData { get; set; }
    }

    public class BaseClass
    {

    }

    public class DerivedClass : BaseClass
    {

    }
}
