using OpenNos.Core.Networking.Communication.Scs.Communication.Messages;
using OpenNos.Core.Networking.Communication.Scs.Communication.Protocols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenNos.Core
{
    public class WireProtocol : IScsWireProtocol, IDisposable
    {
        #region Members

        /// <summary>
        /// Maximum length of a message.
        /// </summary>
        private const short MAX_MESSAGE_LENGTH = 4096;

        private IDictionary<string, DateTime> connectionHistory;

        private bool disposed;

        /// <summary>
        /// This MemoryStream object is used to collect receiving bytes to build messages.
        /// </summary>
        private MemoryStream receiveMemoryStream;

        #endregion

        #region Instantiation

        public WireProtocol()
        {
            receiveMemoryStream = new MemoryStream();
            connectionHistory = new Dictionary<string, DateTime>();
        }

        #endregion

        #region Methods

        public IEnumerable<IScsMessage> CreateMessages(byte[] receivedBytes)
        {
            // Write all received bytes to the _receiveMemoryStream
            receiveMemoryStream.Write(receivedBytes, 0, receivedBytes.Length);

            // Create a list to collect messages
            List<IScsMessage> messages = new List<IScsMessage>();

            // Read all available messages and add to messages collection
            while (ReadSingleMessage(messages))
            {
            }

            // Return message list
            return messages;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
                disposed = true;
            }
        }

        public byte[] GetBytes(IScsMessage message)
        {
            // Serialize the message to a byte array
            var textMessage = message as ScsTextMessage;
            byte[] bytes = textMessage != null ?
                Encoding.Default.GetBytes(textMessage.Text) :
                ((ScsRawDataMessage)message).MessageData;

            return bytes;
        }

        public void Reset()
        {
            if (receiveMemoryStream.Length > 0)
            {
                receiveMemoryStream = new MemoryStream();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                receiveMemoryStream.Dispose();
            }
        }

        /// <summary>
        /// Reads a byte array with specified length.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <param name="length">Length of the byte array to read</param>
        /// <returns>Read byte array</returns>
        /// <exception cref="EndOfStreamException">
        /// Throws EndOfStreamException if can not read from stream.
        /// </exception>
        private static byte[] ReadByteArray(Stream stream, short length)
        {
            byte[] buffer = new byte[length];

            var read = stream.Read(buffer, 0, length);
            if (read <= 0)
            {
                throw new EndOfStreamException("Can not read from stream! Input stream is closed.");
            }

            return buffer;
        }

        /// <summary>
        /// This method tries to read a single message and add to the messages collection.
        /// </summary>
        /// <param name="messages">Messages collection to collect messages</param>
        /// <returns>
        /// Returns a boolean value indicates that if there is a need to re-call this method.
        /// </returns>
        /// <exception cref="CommunicationException">
        /// Throws CommunicationException if message is bigger than maximum allowed message length.
        /// </exception>
        private bool ReadSingleMessage(ICollection<IScsMessage> messages)
        {
            // Go to the beginning of the stream
            receiveMemoryStream.Position = 0;

            // check if message length is 0
            if (receiveMemoryStream.Length == 0)
            {
                return false;
            }

            // get length of frame
            var frameLength = (short)receiveMemoryStream.Length;

            // Read length of the message
            if (frameLength > MAX_MESSAGE_LENGTH)
            {
                throw new Exception("Message is too big (" + frameLength + " bytes). Max allowed length is " + MAX_MESSAGE_LENGTH + " bytes.");
            }

            // Read bytes of serialized message and deserialize it
            byte[] serializedMessageBytes = ReadByteArray(receiveMemoryStream, frameLength);
            messages.Add(new ScsRawDataMessage(serializedMessageBytes));

            // Read remaining bytes to an array
            if (receiveMemoryStream.Length > frameLength)
            {
                byte[] remainingBytes = ReadByteArray(receiveMemoryStream, (short)(receiveMemoryStream.Length - frameLength));

                // Re-create the receive memory stream and write remaining bytes
                receiveMemoryStream = new MemoryStream();
                receiveMemoryStream.Write(remainingBytes, 0, remainingBytes.Length);
            }
            else
            {
                // nothing left, just recreate
                receiveMemoryStream = new MemoryStream();
            }

            // Return true to re-call this method to try to read next message
            return receiveMemoryStream.Length > 0;
        }

        #endregion
    }
}