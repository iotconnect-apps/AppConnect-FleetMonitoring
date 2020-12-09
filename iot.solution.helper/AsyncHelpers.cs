using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LogHandler = component.services.loghandler;

namespace component.helper
{
    public static class AsyncHelpers
    {
        private static LogHandler.Logger logger; 

        static AsyncHelpers()
        {
            logger = DependencyResolver.Current.GetService<LogHandler.Logger>();
        }

        public static void RunSync(Func<Task> task)
        {
            logger.DebugLog("Start AsyncHelpers");

            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            synch.Post(async _ =>
            {
                try
                {
                    await task();
                }
                catch (Exception e)
                {
                    var exc = ((IoTConnect.Model.IoTConnectException)e);
                    if (exc != null && exc.error != null)
                    {
                        synch.InnerException = new Exception(exc.error.FirstOrDefault().Message);
                    }
                    else
                    {
                        synch.InnerException = new Exception(e.InnerException.Message);
                    }
                    throw;
                }
                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);
            synch.BeginMessageLoop();

            SynchronizationContext.SetSynchronizationContext(oldContext);
        }

        public static T RunSync<T>(Func<Task<T>> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            T ret = default(T);
            synch.Post(async _ =>
            {
                try
                {
                    ret = await task();
                }
                catch (Exception e)
                {
                    try
                    {
                        var exc = ((IoTConnect.Model.IoTConnectException)e);
                        if (exc != null && exc.error != null)
                        {
                            synch.InnerException = new Exception(exc.error.FirstOrDefault().Message);
                        }
                        else
                        {
                            synch.InnerException = new Exception(e.InnerException.Message);
                        }
                        throw;
                    }
                    catch (Exception ex)
                    {
                        synch.InnerException = new Exception(e.Message);
                        throw;
                    }
                }
                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);
            synch.BeginMessageLoop();
            SynchronizationContext.SetSynchronizationContext(oldContext);
            return ret;
        }

        private class ExclusiveSynchronizationContext : SynchronizationContext
        {
            private bool done;
            public Exception InnerException { get; set; }
            readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);
            readonly Queue<Tuple<SendOrPostCallback, object>> items = new Queue<Tuple<SendOrPostCallback, object>>();

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("We cannot send to our same thread");
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                lock (items)
                {
                    items.Enqueue(Tuple.Create(d, state));
                }
                workItemsWaiting.Set();
            }

            public void EndMessageLoop()
            {
                Post(_ => done = true, null);
            }

            public void BeginMessageLoop()
            {
                while (!done)
                {
                    Tuple<SendOrPostCallback, object> task = null;
                    lock (items)
                    {
                        if (items.Count > 0)
                        {
                            task = items.Dequeue();
                        }
                    }
                    if (task != null)
                    {
                        task.Item1(task.Item2);
                        if (InnerException != null) // the method threw an exeption
                        {
                            throw new AggregateException(InnerException);
                        }
                    }
                    else
                    {
                        workItemsWaiting.WaitOne();
                    }
                }
            }

            public override SynchronizationContext CreateCopy()
            {
                return this;
            }
        }
    }
}
