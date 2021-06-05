using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

namespace BrowscapNet
{
    /// <summary>
    /// A parser for an ini file. Provide an IIniHandler implementation.
    /// </summary>
    public class IniParser
    {
        private IIniHandler handler;

        public IniParser(IIniHandler handler)
        {
            this.handler = handler;
        }

        /// <summary>
        /// Parse the specified ini file stream.
        /// </summary>
        /// <param name="stream">The ini file stream.</param>
        public void Parse(Stream stream)
        {
            using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
            {
                long lineNumber = 0;
                while (!sr.EndOfStream)
                {
                    bool cancelParsing = false;
                    lineNumber++;
                    string line = sr.ReadLine();
                    if (line.Length == 0)
                    {
                        continue;
                    }

                    if (line[0] == ';')
                    {
                        // comment

                    }
                    else if (line[0] == '[')
                    {
                        // section
                        string name = line.Substring(1, line.IndexOf(']') - 1);
                        handler.StartSection(name, lineNumber, out cancelParsing);
                    }
                    else
                    {
                        // key/value
                        int eqi = line.IndexOf('=');
                        if (eqi != -1)
                        {
                            var key = line.Substring(0, eqi);
                            var value = line.Substring(eqi + 1);
                            handler.KeyValue(key, value, lineNumber, out cancelParsing);
                        }
                    }

                    if (cancelParsing)
                    {
                        return;
                    }
                }

                handler.EndFile(lineNumber);
            }
        }

        /// <summary>
        /// Parse the specified ini file stream.
        /// </summary>
        /// <param name="stream">The ini file stream.</param>
        public async Task ParseAsync(Stream stream)
        {
            var pipeReader = PipeReader.Create(stream, new StreamPipeReaderOptions(bufferSize: 1024 * 1024));
            Task reading = ReadPipeAsync(pipeReader);
            await reading;
        }

        private async Task ReadPipeAsync(PipeReader reader)
        {
            bool cancelParsing = false;
            int lineNumber = 0;
            while (true)
            {
                ReadResult result = await reader.ReadAsync();
                ReadOnlySequence<byte> buffer = result.Buffer;

                while (TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
                {
                    lineNumber++;

                    if (!line.IsEmpty)
                    {
                        ProcessLine(in line, lineNumber, out cancelParsing);
                    }
                }

                reader.AdvanceTo(buffer.Start, buffer.End);

                if (result.IsCompleted || cancelParsing)
                {
                    break;
                }
            }

            await reader.CompleteAsync();
        }

        private bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
        {
            SequencePosition? position = buffer.PositionOf((byte)'\n');

            if (position == null)
            {
                line = default;
                return false;
            }

            line = buffer.Slice(0, position.Value);
            var hasR = !line.IsEmpty && line.Slice(line.GetPosition(line.Length - 1)).FirstSpan[0] == (byte)'\r';
            if (hasR)
            {
                // remove trailing \r
                line = line.Slice(0, line.GetPosition(line.Length - 1));
            }

            buffer = buffer.Slice(buffer.GetPosition(1, position.Value));

            return true;
        }

        private void ProcessLine(in ReadOnlySequence<byte> line, int lineNumber, out bool cancelParsing)
        {
            cancelParsing = false;
            SequenceReader<byte> reader = new SequenceReader<byte>(line);
            byte firstByte = line.First.Span[0];
            if (firstByte == ';')
            {
                // comment
            }
            else if (firstByte == '[')
            {
                // section
                reader.Advance(1);
                if (reader.TryReadTo(out ReadOnlySpan<byte> spanSection, (byte)']'))
                {
                    string name = Encoding.UTF8.GetString(spanSection);
                    handler.StartSection(name, lineNumber, out cancelParsing);
                }
            }
            else
            {
                // key/value
                if (reader.TryReadTo(out ReadOnlySpan<byte> seqKey, (byte)'='))
                {
                    string key = Encoding.UTF8.GetString(seqKey);
                    var seqValue = reader.UnreadSpan;
                    string value = Encoding.UTF8.GetString(seqValue);
                    handler.KeyValue(key, value, lineNumber, out cancelParsing);
                }
            }

        }
    }

    /// <summary>
    /// Ini handler.
    /// </summary>
    public interface IIniHandler
    {
        /// <summary>
        /// Called when IniParser encounters a new section.
        /// </summary>
        /// <param name="section">The name of the section.</param>
        /// <param name="lineNumber">The line number of the section.</param>
        /// <param name="cancelParsing">If set to <c>true</c>, cancel the parsing operation.</param>
        void StartSection(string section, long lineNumber, out bool cancelParsing);

        /// <summary>
        /// Called when IniParser encounters each key value pair.
        /// </summary>
        /// <param name="key">Key name.</param>
        /// <param name="value">Value.</param>
        /// <param name="lineNumber">The line number of the key value pair.</param>
        /// <param name="cancelParsing">If set to <c>true</c>, cancel the parsing operation.</param>
        void KeyValue(string key, string value, long lineNumber, out bool cancelParsing);

        /// <summary>
        /// Called when IniParser encounters the end of the file.
        /// </summary>
        /// <param name="lineNumber">The line number of the last line.</param>
        void EndFile(long lineNumber);
    }
}
