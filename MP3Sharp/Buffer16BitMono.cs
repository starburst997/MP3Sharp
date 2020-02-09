// /***************************************************************************
//  * SampleBuffer.cs
//  * Copyright (c) 2015 the authors.
//  * 
//  * All rights reserved. This program and the accompanying materials
//  * are made available under the terms of the GNU Lesser General Public License
//  * (LGPL) version 3 which accompanies this distribution, and is available at
//  * https://www.gnu.org/licenses/lgpl-3.0.en.html
//  *
//  * This library is distributed in the hope that it will be useful,
//  * but WITHOUT ANY WARRANTY; without even the implied warranty of
//  * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  * Lesser General Public License for more details.
//  *
//  ***************************************************************************/
using System;

namespace MP3Sharp.Decoding
{
    /// <summary>
    ///     The SampleBuffer class implements an output buffer
    ///     that provides storage for a fixed size block of samples.
    /// </summary>
    internal class Buffer16BitMono : ABuffer
    {
        private readonly byte[] buffer;
        int position;

        /// <summary>
        ///     Constructor
        /// </summary>
        public Buffer16BitMono()
        {
            buffer = new byte[OBUFFERSIZE];
            position = 0;
        }

        public virtual int ChannelCount
        {
            get { return 1; }
        }

        public virtual int SampleFrequency
        {
            get { return -1; }
        }

        public virtual int BufferLength
        {
            get { return position; }
        }

        public override int Read(byte[] bufferOut, int offset, int count)
        {
            if (bufferOut == null)
            {
                throw new ArgumentNullException("bufferOut");
            }
            if ((count + offset) > bufferOut.Length)
            {
                throw new ArgumentException("The sum of offset and count is larger than the buffer length");
            }
            int remaining = BytesLeft;
            int copySize;
            if (count > remaining)
            {
                copySize = remaining;
            }
            else
            {
                // Copy an even number of sample frames
                int remainder = count % 2;
                copySize = count - remainder;
            }

            Array.Copy(buffer, m_Offset, bufferOut, offset, copySize);

            m_Offset += copySize;
            return copySize;
        }

        /// <summary>
        ///     Takes a 16 Bit PCM sample.
        /// </summary>
        public override void Append(int channel, short valueRenamed)
        {
            buffer[position] = (byte)(valueRenamed & 0xff);
            buffer[position + 1] = (byte)(valueRenamed >> 8);
            position += 2; ;
        }

        public override void AppendSamples(int channel, float[] samples)
        {
            if (samples == null)
            {
                // samples is required.
                throw new ArgumentNullException("samples");
            }
            if (samples.Length < 32)
            {
                throw new ArgumentException("samples must have 32 values");
            }
            // Always, 32 samples are appended

            for (int i = 0; i < 32; i++)
            {
                float fs = samples[i];
                if (fs > 32767.0f) // can this happen?
                    fs = 32767.0f;
                else if (fs < -32767.0f)
                    fs = -32767.0f;

                int sample = (int)fs;
                buffer[position] = (byte)(sample & 0xff);
                buffer[position + 1] = (byte)(sample >> 8);
                position += 2;
            }
        }

        /// <summary>
        ///     Write the samples to the file (Random Acces).
        /// </summary>
        public override void WriteBuffer()
        {
            m_Offset = 0;
            m_End = position;
        }

        public override void Close()
        {
        }

        /// <summary>
        ///     *
        /// </summary>
        public override void ClearBuffer()
        {
            position = 0;
        }

        /// <summary>
        ///     *
        /// </summary>
        public override void SetStopFlag()
        {

        }
    }
}