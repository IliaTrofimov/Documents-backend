using Documents.Models.Entities;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Documents.Services
{
    public class QueueInfo
    {
        public int TotalTries { get; set; }
        public int TotalSent { get; set; }
        public int Enqueued  { get; set; }
        public bool IsRunning { get; set; }
    }

    public class MailingQueue
    {
        private readonly ConcurrentQueue<Action> queue;
        private readonly Mailing mailingClient;
        private readonly Thread workingThread;
        private bool isRunning = false;
        private readonly AutoResetEvent _signal = new AutoResetEvent(true);


        public bool IsRunning
        { 
            get => isRunning; 
            set 
            {
                if (value)
                    workingThread.Start();
                else
                    workingThread.Interrupt();
                isRunning = value;
                Info.IsRunning = value;
            } 
        }

        public QueueInfo Info { get; set; } = new QueueInfo();


        public MailingQueue(Mailing mailing)
        {
            mailingClient = mailing;
            queue = new ConcurrentQueue<Action>();
            workingThread = new Thread(ExecutionCycle);
        }


        public void Switch()
        {
            IsRunning = !IsRunning;
        }


        public bool SignatoryNotification(Sign sign)
        {
            if (!isRunning) 
                return false;
            Info.TotalTries++;
            queue.Enqueue(async () => { await mailingClient.SignatoryNotification(sign); Info.TotalSent++; });
            _signal.Set();
            return true;
        }


        private void ExecutionCycle()
        {
            while (isRunning)
            {
                _signal.WaitOne();
                while (queue.TryDequeue(out Action func))
                    func();
            }
        }
    }
}