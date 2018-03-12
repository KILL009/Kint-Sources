using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenNos.Core.Networking.Communication.Scs.Threading
{
    /// <summary>
    /// This class is used to process items sequentially in a multithreaded manner.
    /// </summary>
    /// <typeparam name="TItem">Type of item to process</typeparam>
    public class SequentialItemProcessor<TItem>
    {
        #region Members

        /// <summary>
        /// The method delegate that is called to actually process items.
        /// </summary>
        private readonly Action<TItem> processMethod;

        /// <summary>
        /// Item queue. Used to process items sequentially.
        /// </summary>
        private readonly Queue<TItem> queue;

        /// <summary>
        /// An object to synchronize threads.
        /// </summary>
        private readonly object syncObj = new object();

        /// <summary>
        /// A reference to the current Task that is processing an item in ProcessItem method.
        /// </summary>
        private Task currentProcessTask;

        /// <summary>
        /// Indicates state of the item processing.
        /// </summary>
        private bool isProcessing;

        /// <summary>
        /// A boolean value to control running of SequentialItemProcessor.
        /// </summary>
        private bool isRunning;

        #endregion

        #region Instantiation

        /// <summary>
        /// Creates a new SequentialItemProcessor object.
        /// </summary>
        /// <param name="processMethod">
        /// The method delegate that is called to actually process items
        /// </param>
        public SequentialItemProcessor(Action<TItem> processMethod)
        {
            this.processMethod = processMethod;
            queue = new Queue<TItem>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds an item to queue to process the item.
        /// </summary>
        /// <param name="item">Item to add to the queue</param>
        public void EnqueueMessage(TItem item)
        {
            // Add the item to the queue and start a new Task if needed
            lock (syncObj)
            {
                if (!isRunning)
                {
                    return;
                }

                queue.Enqueue(item);

                if (!isProcessing)
                {
                    currentProcessTask = Task.Factory.StartNew(ProcessItem);
                }
            }
        }

        /// <summary>
        /// Starts processing of items.
        /// </summary>
        public void Start() => isRunning = true;

        /// <summary>
        /// Stops processing of items and waits stopping of current item.
        /// </summary>
        public void Stop()
        {
            isRunning = false;

            // Clear all incoming messages
            lock (syncObj)
                queue.Clear();

            // Check if is there a message that is being processed now
            if (!isProcessing)
            {
                return;
            }

            // Wait current processing task to finish
            try
            {
                currentProcessTask.Wait();
            }
            catch
            {
            }
        }

        /// <summary>
        /// This method runs on a new seperated Task (thread) to process items on the queue.
        /// </summary>
        private void ProcessItem()
        {
            // Try to get an item from queue to process it.
            TItem itemToProcess;
            lock (syncObj)
            {
                if (!isRunning || isProcessing)
                {
                    return;
                }

                if (queue.Count <= 0)
                {
                    return;
                }

                isProcessing = true;
                itemToProcess = queue.Dequeue();
            }

            // Process the item (by calling the _processMethod delegate)
            processMethod(itemToProcess);

            // Process next item if available
            lock (syncObj)
            {
                isProcessing = false;
                if (!isRunning || queue.Count <= 0)
                {
                    return;
                }

                // Start a new task
                currentProcessTask = Task.Factory.StartNew(ProcessItem);
            }
        }

        #endregion
    }
}