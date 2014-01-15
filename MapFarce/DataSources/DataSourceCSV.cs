using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Forms;
using MapFarce.DataModel;
using MapFarce.EditProperties;

namespace MapFarce.DataSources
{
    [DataSourceDescriptor("CSV")]
    public class DataSourceCSV : DataSource<DataSourceCSV, DataSourceCSV.DataTypeCSV, DataSourceCSV.DataFieldCSV>
    {
        public override string Name { get { return File == null ? "CSV data" : File.Name; } }

        public DataSourceCSV()
        {
            File = null;
            Delimiter = CsvReader.DefaultDelimiter;
            Quote = CsvReader.DefaultQuote;
            Escape = CsvReader.DefaultEscape;
            Comment = CsvReader.DefaultComment;
            HasHeaders = true;
            TrimSpaces = true;
        }

        public override bool InitializeNew()
        {
            FileDialog dialog;
            if (DataMode == Mode.Input)
                dialog = new OpenFileDialog();
            else if (DataMode == Mode.Output)
                dialog = new SaveFileDialog();
            else
                throw new Exception("Unexpected DataSource Mode: " + DataMode);

            dialog.Filter = "CSV Files|*.csv|All Files (*.*)|*.*";

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return false;

            File = new FileInfo(dialog.FileName);
            return true;
        }

        public override bool CanAddDataTypes { get { return false; } }

        protected override IList<DataTypeCSV> RetrieveDataTypes()
        {
            return new DataTypeCSV[] { new DataTypeCSV(this) };
        }

        [UIEditablePropertyAttribute(null, "The file to read from", "File")]
        public FileInfo File { get; set; }

        [UIEditablePropertyAttribute("Has headers", "Whether or not the first row of the data contains column names", "Format")]
        public bool HasHeaders { get; set; }

        [UIEditablePropertyAttribute("Delimiter char", "The character to be used to separate fields", "Format")]
        public char Delimiter { get; set; }

        [UIEditablePropertyAttribute("Quote char", "The quote character used for wrapping fields", "Format")]
        public char Quote { get; set; }

        [UIEditablePropertyAttribute("Escape char", "The character used to escape quotation characters inside a quoted field", "Format")]
        public char Escape { get; set; }

        [UIEditablePropertyAttribute("Comment char", "If used at the start of a line, indicates that this line is commented out, and should be ignored", "Format")]
        public char Comment { get; set; }

        [UIEditablePropertyAttribute("Trim spaces", "Whether or not spaces at the start & End of each field should be trimmed", "Format")]
        public bool TrimSpaces { get; set; }

        public class DataTypeCSV : DataType<DataSourceCSV, DataTypeCSV, DataFieldCSV>
        {
            public override string Name { get { return "Rows"; } }

            public DataTypeCSV(DataSourceCSV source)
                : base(source)
            {
            }

            private CsvReader CreateReader()
            {
                StreamReader sr = new StreamReader(Source.File.FullName);
                try
                {
                    reader = new CsvReader(sr, Source.HasHeaders, Source.Delimiter, Source.Quote, Source.Escape, Source.Comment, Source.TrimSpaces);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return reader;
            }

            protected override IList<DataFieldCSV> RetrieveFields()
            {
                var reader = CreateReader();
                var fields = new List<DataFieldCSV>();

                if (Source.HasHeaders)
                {
                    string[] headers = reader.GetFieldHeaders();
                    for (int i = 0; i < headers.Length; i++)
                        fields.Add(new DataFieldCSV(i, headers[i]));
                }
                else
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                        fields.Add(new DataFieldCSV(i, "Col " + i));
                }

                reader.Dispose();
                return fields;
            }

            CsvReader reader;
            public override void BeginRead()
            {
                reader = CreateReader();
            }

            public override void FinishRead()
            {
                reader.Dispose();
                reader = null;
            }

            protected override DataItem ReadNext()
            {
                if (!reader.ReadNextRecord())
                    return null;

                var fields = GetFields();
                var item = new DataItem(fields.Count);
                
                for (int i = 0; i < reader.FieldCount; i++)
                    item.AddValue(fields[i], reader[i]);

                return item;
            }

            public override bool HasMoreData()
            {
                return reader != null && !reader.EndOfStream;
            }
        }

        public class DataFieldCSV : DataField
        {
            public DataFieldCSV(int colNum, string name)
                : base(name, FieldType.String)
            {
                ColumnNumber = colNum;
            }

            public int ColumnNumber { get; private set; }

            public override int CompareTo(DataField other)
            {
                if ( other is DataFieldCSV )
                    return ColumnNumber.CompareTo((other as DataFieldCSV).ColumnNumber);
                return Name.CompareTo(other.Name);
            }
        }

        #region LumenWorks Csv Reader by Sébastien Lorion

        /// <summary>
        /// Represents a reader that provides fast, non-cached, forward-only access to CSV data.  
        /// </summary>
        public partial class CsvReader
            : IDataReader, IEnumerable<string[]>, IDisposable
        {
            /// <summary>
            /// Defines the default buffer size.
            /// </summary>
            public const int DefaultBufferSize = 0x1000;

            /// <summary>
            /// Defines the default delimiter character separating each field.
            /// </summary>
            public const char DefaultDelimiter = ',';

            /// <summary>
            /// Defines the default quote character wrapping every field.
            /// </summary>
            public const char DefaultQuote = '"';

            /// <summary>
            /// Defines the default escape character letting insert quotation characters inside a quoted field.
            /// </summary>
            public const char DefaultEscape = '"';

            /// <summary>
            /// Defines the default comment character indicating that a line is commented out.
            /// </summary>
            public const char DefaultComment = '#';

            /// <summary>
            /// Contains the field header comparer.
            /// </summary>
            private static readonly StringComparer _fieldHeaderComparer = StringComparer.CurrentCultureIgnoreCase;

            /// <summary>
            /// Contains the <see cref="T:TextReader"/> pointing to the CSV file.
            /// </summary>
            private TextReader _reader;

            /// <summary>
            /// Contains the buffer size.
            /// </summary>
            private int _bufferSize;

            /// <summary>
            /// Contains the comment character indicating that a line is commented out.
            /// </summary>
            private char _comment;

            /// <summary>
            /// Contains the escape character letting insert quotation characters inside a quoted field.
            /// </summary>
            private char _escape;

            /// <summary>
            /// Contains the delimiter character separating each field.
            /// </summary>
            private char _delimiter;

            /// <summary>
            /// Contains the quotation character wrapping every field.
            /// </summary>
            private char _quote;

            /// <summary>
            /// Indicates if spaces at the start and end of a field are trimmed.
            /// </summary>
            private bool _trimSpaces;

            /// <summary>
            /// Indicates if field names are located on the first non commented line.
            /// </summary>
            private bool _hasHeaders;

            /// <summary>
            /// Contains the default action to take when a parsing error has occured.
            /// </summary>
            private ParseErrorAction _defaultParseErrorAction;

            /// <summary>
            /// Contains the action to take when a field is missing.
            /// </summary>
            private MissingFieldAction _missingFieldAction;

            /// <summary>
            /// Indicates if the reader supports multiline.
            /// </summary>
            private bool _supportsMultiline;

            /// <summary>
            /// Indicates if the reader will skip empty lines.
            /// </summary>
            private bool _skipEmptyLines;

            /// <summary>
            /// Indicates if the class is initialized.
            /// </summary>
            private bool _initialized;

            /// <summary>
            /// Contains the field headers.
            /// </summary>
            private string[] _fieldHeaders;

            /// <summary>
            /// Contains the dictionary of field indexes by header. The key is the field name and the value is its index.
            /// </summary>
            private Dictionary<string, int> _fieldHeaderIndexes;

            /// <summary>
            /// Contains the current record index in the CSV file.
            /// A value of <see cref="M:Int32.MinValue"/> means that the reader has not been initialized yet.
            /// Otherwise, a negative value means that no record has been read yet.
            /// </summary>
            private long _currentRecordIndex;

            /// <summary>
            /// Contains the starting position of the next unread field.
            /// </summary>
            private int _nextFieldStart;

            /// <summary>
            /// Contains the index of the next unread field.
            /// </summary>
            private int _nextFieldIndex;

            /// <summary>
            /// Contains the array of the field values for the current record.
            /// A null value indicates that the field have not been parsed.
            /// </summary>
            private string[] _fields;

            /// <summary>
            /// Contains the maximum number of fields to retrieve for each record.
            /// </summary>
            private int _fieldCount;

            /// <summary>
            /// Contains the read buffer.
            /// </summary>
            private char[] _buffer;

            /// <summary>
            /// Contains the current read buffer length.
            /// </summary>
            private int _bufferLength;

            /// <summary>
            /// Indicates if the end of the reader has been reached.
            /// </summary>
            private bool _eof;

            /// <summary>
            /// Indicates if the last read operation reached an EOL character.
            /// </summary>
            private bool _eol;

            /// <summary>
            /// Indicates if the first record is in cache.
            /// This can happen when initializing a reader with no headers
            /// because one record must be read to get the field count automatically
            /// </summary>
            private bool _firstRecordInCache;

            /// <summary>
            /// Indicates if one or more field are missing for the current record.
            /// Resets after each successful record read.
            /// </summary>
            private bool _missingFieldFlag;

            /// <summary>
            /// Indicates if a parse error occured for the current record.
            /// Resets after each successful record read.
            /// </summary>
            private bool _parseErrorFlag;

            /// <summary>
            /// Initializes a new instance of the CsvReader class.
            /// </summary>
            /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
            /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
            /// <exception cref="T:ArgumentNullException">
            ///		<paramref name="reader"/> is a <see langword="null"/>.
            /// </exception>
            /// <exception cref="T:ArgumentException">
            ///		Cannot read from <paramref name="reader"/>.
            /// </exception>
            public CsvReader(TextReader reader, bool hasHeaders)
                : this(reader, hasHeaders, DefaultDelimiter, DefaultQuote, DefaultEscape, DefaultComment, true, DefaultBufferSize)
            {
            }

            /// <summary>
            /// Initializes a new instance of the CsvReader class.
            /// </summary>
            /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
            /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
            /// <param name="bufferSize">The buffer size in bytes.</param>
            /// <exception cref="T:ArgumentNullException">
            ///		<paramref name="reader"/> is a <see langword="null"/>.
            /// </exception>
            /// <exception cref="T:ArgumentException">
            ///		Cannot read from <paramref name="reader"/>.
            /// </exception>
            public CsvReader(TextReader reader, bool hasHeaders, int bufferSize)
                : this(reader, hasHeaders, DefaultDelimiter, DefaultQuote, DefaultEscape, DefaultComment, true, bufferSize)
            {
            }

            /// <summary>
            /// Initializes a new instance of the CsvReader class.
            /// </summary>
            /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
            /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
            /// <param name="delimiter">The delimiter character separating each field (default is ',').</param>
            /// <exception cref="T:ArgumentNullException">
            ///		<paramref name="reader"/> is a <see langword="null"/>.
            /// </exception>
            /// <exception cref="T:ArgumentException">
            ///		Cannot read from <paramref name="reader"/>.
            /// </exception>
            public CsvReader(TextReader reader, bool hasHeaders, char delimiter)
                : this(reader, hasHeaders, delimiter, DefaultQuote, DefaultEscape, DefaultComment, true, DefaultBufferSize)
            {
            }

            /// <summary>
            /// Initializes a new instance of the CsvReader class.
            /// </summary>
            /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
            /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
            /// <param name="delimiter">The delimiter character separating each field (default is ',').</param>
            /// <param name="bufferSize">The buffer size in bytes.</param>
            /// <exception cref="T:ArgumentNullException">
            ///		<paramref name="reader"/> is a <see langword="null"/>.
            /// </exception>
            /// <exception cref="T:ArgumentException">
            ///		Cannot read from <paramref name="reader"/>.
            /// </exception>
            public CsvReader(TextReader reader, bool hasHeaders, char delimiter, int bufferSize)
                : this(reader, hasHeaders, delimiter, DefaultQuote, DefaultEscape, DefaultComment, true, bufferSize)
            {
            }

            /// <summary>
            /// Initializes a new instance of the CsvReader class.
            /// </summary>
            /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
            /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
            /// <param name="delimiter">The delimiter character separating each field (default is ',').</param>
            /// <param name="quote">The quotation character wrapping every field (default is ''').</param>
            /// <param name="escape">
            /// The escape character letting insert quotation characters inside a quoted field (default is '\').
            /// If no escape character, set to '\0' to gain some performance.
            /// </param>
            /// <param name="comment">The comment character indicating that a line is commented out (default is '#').</param>
            /// <param name="trimSpaces"><see langword="true"/> if spaces at the start and end of a field are trimmed, otherwise, <see langword="false"/>. Default is <see langword="true"/>.</param>
            /// <exception cref="T:ArgumentNullException">
            ///		<paramref name="reader"/> is a <see langword="null"/>.
            /// </exception>
            /// <exception cref="T:ArgumentException">
            ///		Cannot read from <paramref name="reader"/>.
            /// </exception>
            public CsvReader(TextReader reader, bool hasHeaders, char delimiter, char quote, char escape, char comment, bool trimSpaces)
                : this(reader, hasHeaders, delimiter, quote, escape, comment, trimSpaces, DefaultBufferSize)
            {
            }

            /// <summary>
            /// Initializes a new instance of the CsvReader class.
            /// </summary>
            /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
            /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
            /// <param name="delimiter">The delimiter character separating each field (default is ',').</param>
            /// <param name="quote">The quotation character wrapping every field (default is ''').</param>
            /// <param name="escape">
            /// The escape character letting insert quotation characters inside a quoted field (default is '\').
            /// If no escape character, set to '\0' to gain some performance.
            /// </param>
            /// <param name="comment">The comment character indicating that a line is commented out (default is '#').</param>
            /// <param name="trimSpaces"><see langword="true"/> if spaces at the start and end of a field are trimmed, otherwise, <see langword="false"/>. Default is <see langword="true"/>.</param>
            /// <param name="bufferSize">The buffer size in bytes.</param>
            /// <exception cref="T:ArgumentNullException">
            ///		<paramref name="reader"/> is a <see langword="null"/>.
            /// </exception>
            /// <exception cref="ArgumentOutOfRangeException">
            ///		<paramref name="bufferSize"/> must be 1 or more.
            /// </exception>
            public CsvReader(TextReader reader, bool hasHeaders, char delimiter, char quote, char escape, char comment, bool trimSpaces, int bufferSize)
            {
#if DEBUG
                _allocStack = new System.Diagnostics.StackTrace();
#endif

                if (reader == null)
                    throw new ArgumentNullException("reader");

                if (bufferSize <= 0)
                    throw new ArgumentOutOfRangeException("bufferSize", bufferSize, "Buffer size must be 1 or more");

                _bufferSize = bufferSize;

                if (reader is StreamReader)
                {
                    Stream stream = ((StreamReader)reader).BaseStream;

                    if (stream.CanSeek)
                    {
                        // Handle bad implementations returning 0 or less
                        if (stream.Length > 0)
                            _bufferSize = (int)Math.Min(bufferSize, stream.Length);
                    }
                }

                _reader = reader;
                _delimiter = delimiter;
                _quote = quote;
                _escape = escape;
                _comment = comment;

                _hasHeaders = hasHeaders;
                _trimSpaces = trimSpaces;
                _supportsMultiline = true;
                _skipEmptyLines = true;

                _currentRecordIndex = -1;
                _defaultParseErrorAction = ParseErrorAction.RaiseEvent;
            }

            /// <summary>
            /// Occurs when there is an error while parsing the CSV stream.
            /// </summary>
            public event EventHandler<ParseErrorEventArgs> ParseError;

            /// <summary>
            /// Raises the <see cref="M:ParseError"/> event.
            /// </summary>
            /// <param name="e">The <see cref="ParseErrorEventArgs"/> that contains the event data.</param>
            protected virtual void OnParseError(ParseErrorEventArgs e)
            {
                EventHandler<ParseErrorEventArgs> handler = ParseError;

                if (handler != null)
                    handler(this, e);
            }

            /// <summary>
            /// Gets the comment character indicating that a line is commented out.
            /// </summary>
            /// <value>The comment character indicating that a line is commented out.</value>
            public char Comment
            {
                get
                {
                    return _comment;
                }
            }

            /// <summary>
            /// Gets the escape character letting insert quotation characters inside a quoted field.
            /// </summary>
            /// <value>The escape character letting insert quotation characters inside a quoted field.</value>
            public char Escape
            {
                get
                {
                    return _escape;
                }
            }

            /// <summary>
            /// Gets the delimiter character separating each field.
            /// </summary>
            /// <value>The delimiter character separating each field.</value>
            public char Delimiter
            {
                get
                {
                    return _delimiter;
                }
            }

            /// <summary>
            /// Gets the quotation character wrapping every field.
            /// </summary>
            /// <value>The quotation character wrapping every field.</value>
            public char Quote
            {
                get
                {
                    return _quote;
                }
            }

            /// <summary>
            /// Indicates if field names are located on the first non commented line.
            /// </summary>
            /// <value><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</value>
            public bool HasHeaders
            {
                get
                {
                    return _hasHeaders;
                }
            }

            /// <summary>
            /// Indicates if spaces at the start and end of a field are trimmed.
            /// </summary>
            /// <value><see langword="true"/> if spaces at the start and end of a field are trimmed, otherwise, <see langword="false"/>.</value>
            public bool TrimSpaces
            {
                get
                {
                    return _trimSpaces;
                }
            }

            /// <summary>
            /// Gets the buffer size.
            /// </summary>
            public int BufferSize
            {
                get
                {
                    return _bufferSize;
                }
            }

            /// <summary>
            /// Gets or sets the default action to take when a parsing error has occured.
            /// </summary>
            /// <value>The default action to take when a parsing error has occured.</value>
            public ParseErrorAction DefaultParseErrorAction
            {
                get
                {
                    return _defaultParseErrorAction;
                }
                set
                {
                    _defaultParseErrorAction = value;
                }
            }

            /// <summary>
            /// Gets or sets the action to take when a field is missing.
            /// </summary>
            /// <value>The action to take when a field is missing.</value>
            public MissingFieldAction MissingFieldAction
            {
                get
                {
                    return _missingFieldAction;
                }
                set
                {
                    _missingFieldAction = value;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating if the reader supports multiline fields.
            /// </summary>
            /// <value>A value indicating if the reader supports multiline field.</value>
            public bool SupportsMultiline
            {
                get
                {
                    return _supportsMultiline;
                }
                set
                {
                    _supportsMultiline = value;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating if the reader will skip empty lines.
            /// </summary>
            /// <value>A value indicating if the reader will skip empty lines.</value>
            public bool SkipEmptyLines
            {
                get
                {
                    return _skipEmptyLines;
                }
                set
                {
                    _skipEmptyLines = value;
                }
            }

            /// <summary>
            /// Gets the maximum number of fields to retrieve for each record.
            /// </summary>
            /// <value>The maximum number of fields to retrieve for each record.</value>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            public int FieldCount
            {
                get
                {
                    EnsureInitialize();
                    return _fieldCount;
                }
            }

            /// <summary>
            /// Gets a value that indicates whether the current stream position is at the end of the stream.
            /// </summary>
            /// <value><see langword="true"/> if the current stream position is at the end of the stream; otherwise <see langword="false"/>.</value>
            public virtual bool EndOfStream
            {
                get
                {
                    return _eof;
                }
            }

            /// <summary>
            /// Gets the field headers.
            /// </summary>
            /// <returns>The field headers or an empty array if headers are not supported.</returns>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            public string[] GetFieldHeaders()
            {
                EnsureInitialize();
                Debug.Assert(_fieldHeaders != null, "Field headers must be non null.");

                string[] fieldHeaders = new string[_fieldHeaders.Length];

                for (int i = 0; i < fieldHeaders.Length; i++)
                    fieldHeaders[i] = _fieldHeaders[i];

                return fieldHeaders;
            }

            /// <summary>
            /// Gets the current record index in the CSV file.
            /// </summary>
            /// <value>The current record index in the CSV file.</value>
            public virtual long CurrentRecordIndex
            {
                get
                {
                    return _currentRecordIndex;
                }
            }

            /// <summary>
            /// Indicates if one or more field are missing for the current record.
            /// Resets after each successful record read.
            /// </summary>
            public bool MissingFieldFlag
            {
                get { return _missingFieldFlag; }
            }

            /// <summary>
            /// Indicates if a parse error occured for the current record.
            /// Resets after each successful record read.
            /// </summary>
            public bool ParseErrorFlag
            {
                get { return _parseErrorFlag; }
            }

            /// <summary>
            /// Gets the field with the specified name and record position. <see cref="M:hasHeaders"/> must be <see langword="true"/>.
            /// </summary>
            /// <value>
            /// The field with the specified name and record position.
            /// </value>
            /// <exception cref="T:ArgumentNullException">
            ///		<paramref name="field"/> is <see langword="null"/> or an empty string.
            /// </exception>
            /// <exception cref="T:InvalidOperationException">
            ///	The CSV does not have headers (<see cref="M:HasHeaders"/> property is <see langword="false"/>).
            /// </exception>
            /// <exception cref="T:ArgumentException">
            ///		<paramref name="field"/> not found.
            /// </exception>
            /// <exception cref="T:ArgumentOutOfRangeException">
            ///		Record index must be > 0.
            /// </exception>
            /// <exception cref="T:InvalidOperationException">
            ///		Cannot move to a previous record in forward-only mode.
            /// </exception>
            /// <exception cref="T:EndOfStreamException">
            ///		Cannot read record at <paramref name="record"/>.
            ///	</exception>
            ///	<exception cref="T:MalformedCsvException">
            ///		The CSV appears to be corrupt at the current position.
            /// </exception>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            public string this[int record, string field]
            {
                get
                {
                    MoveTo(record);
                    return this[field];
                }
            }

            /// <summary>
            /// Gets the field at the specified index and record position.
            /// </summary>
            /// <value>
            /// The field at the specified index and record position.
            /// A <see langword="null"/> is returned if the field cannot be found for the record.
            /// </value>
            /// <exception cref="T:ArgumentOutOfRangeException">
            ///		<paramref name="field"/> must be included in [0, <see cref="M:FieldCount"/>[.
            /// </exception>
            /// <exception cref="T:ArgumentOutOfRangeException">
            ///		Record index must be > 0.
            /// </exception>
            /// <exception cref="T:InvalidOperationException">
            ///		Cannot move to a previous record in forward-only mode.
            /// </exception>
            /// <exception cref="T:EndOfStreamException">
            ///		Cannot read record at <paramref name="record"/>.
            /// </exception>
            /// <exception cref="T:MalformedCsvException">
            ///		The CSV appears to be corrupt at the current position.
            /// </exception>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            public string this[int record, int field]
            {
                get
                {
                    MoveTo(record);
                    return this[field];
                }
            }

            /// <summary>
            /// Gets the field with the specified name. <see cref="M:hasHeaders"/> must be <see langword="true"/>.
            /// </summary>
            /// <value>
            /// The field with the specified name.
            /// </value>
            /// <exception cref="T:ArgumentNullException">
            ///		<paramref name="field"/> is <see langword="null"/> or an empty string.
            /// </exception>
            /// <exception cref="T:InvalidOperationException">
            ///	The CSV does not have headers (<see cref="M:HasHeaders"/> property is <see langword="false"/>).
            /// </exception>
            /// <exception cref="T:ArgumentException">
            ///		<paramref name="field"/> not found.
            /// </exception>
            /// <exception cref="T:MalformedCsvException">
            ///		The CSV appears to be corrupt at the current position.
            /// </exception>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            public string this[string field]
            {
                get
                {
                    if (string.IsNullOrEmpty(field))
                        throw new ArgumentNullException("field");

                    if (!_hasHeaders)
                        throw new InvalidOperationException("The CSV does not have headers (CsvReader.HasHeaders property is false).");

                    int index = GetFieldIndex(field);

                    if (index < 0)
                        throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "'{0}' field header not found.", field), "field");

                    return this[index];
                }
            }

            /// <summary>
            /// Gets the field at the specified index.
            /// </summary>
            /// <value>The field at the specified index.</value>
            /// <exception cref="T:ArgumentOutOfRangeException">
            ///		<paramref name="field"/> must be included in [0, <see cref="M:FieldCount"/>[.
            /// </exception>
            /// <exception cref="T:InvalidOperationException">
            ///		No record read yet. Call ReadLine() first.
            /// </exception>
            /// <exception cref="T:MalformedCsvException">
            ///		The CSV appears to be corrupt at the current position.
            /// </exception>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            public virtual string this[int field]
            {
                get
                {
                    return ReadField(field, false, false);
                }
            }

            /// <summary>
            /// Ensures that the reader is initialized.
            /// </summary>
            private void EnsureInitialize()
            {
                if (!_initialized)
                    this.ReadNextRecord(true, false);

                Debug.Assert(_fieldHeaders != null);
                Debug.Assert(_fieldHeaders.Length > 0 || (_fieldHeaders.Length == 0 && _fieldHeaderIndexes == null));
            }

            /// <summary>
            /// Gets the field index for the provided header.
            /// </summary>
            /// <param name="header">The header to look for.</param>
            /// <returns>The field index for the provided header. -1 if not found.</returns>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            public int GetFieldIndex(string header)
            {
                EnsureInitialize();

                int index;

                if (_fieldHeaderIndexes != null && _fieldHeaderIndexes.TryGetValue(header, out index))
                    return index;
                else
                    return -1;
            }

            /// <summary>
            /// Copies the field array of the current record to a one-dimensional array, starting at the beginning of the target array.
            /// </summary>
            /// <param name="array"> The one-dimensional <see cref="T:Array"/> that is the destination of the fields of the current record.</param>
            /// <exception cref="T:ArgumentNullException">
            ///		<paramref name="array"/> is <see langword="null"/>.
            /// </exception>
            /// <exception cref="ArgumentException">
            ///		The number of fields in the record is greater than the available space from <paramref name="index"/> to the end of <paramref name="array"/>.
            /// </exception>
            public void CopyCurrentRecordTo(string[] array)
            {
                CopyCurrentRecordTo(array, 0);
            }

            /// <summary>
            /// Copies the field array of the current record to a one-dimensional array, starting at the beginning of the target array.
            /// </summary>
            /// <param name="array"> The one-dimensional <see cref="T:Array"/> that is the destination of the fields of the current record.</param>
            /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
            /// <exception cref="T:ArgumentNullException">
            ///		<paramref name="array"/> is <see langword="null"/>.
            /// </exception>
            /// <exception cref="T:ArgumentOutOfRangeException">
            ///		<paramref name="index"/> is les than zero or is equal to or greater than the length <paramref name="array"/>. 
            /// </exception>
            /// <exception cref="InvalidOperationException">
            ///	No current record.
            /// </exception>
            /// <exception cref="ArgumentException">
            ///		The number of fields in the record is greater than the available space from <paramref name="index"/> to the end of <paramref name="array"/>.
            /// </exception>
            public void CopyCurrentRecordTo(string[] array, int index)
            {
                if (array == null)
                    throw new ArgumentNullException("array");

                if (index < 0 || index >= array.Length)
                    throw new ArgumentOutOfRangeException("index", index, string.Empty);

                if (_currentRecordIndex < 0 || !_initialized)
                    throw new InvalidOperationException("No current record.");

                if (array.Length - index < _fieldCount)
                    throw new ArgumentException("The number of fields in the record is greater than the available space from index to the end of the destination array.", "array");

                for (int i = 0; i < _fieldCount; i++)
                {
                    if (_parseErrorFlag)
                        array[index + i] = null;
                    else
                        array[index + i] = this[i];
                }
            }

            /// <summary>
            /// Gets the current raw CSV data.
            /// </summary>
            /// <remarks>Used for exception handling purpose.</remarks>
            /// <returns>The current raw CSV data.</returns>
            public string GetCurrentRawData()
            {
                if (_buffer != null && _bufferLength > 0)
                    return new string(_buffer, 0, _bufferLength);
                else
                    return string.Empty;
            }

            /// <summary>
            /// Indicates whether the specified Unicode character is categorized as white space.
            /// </summary>
            /// <param name="c">A Unicode character.</param>
            /// <returns><see langword="true"/> if <paramref name="c"/> is white space; otherwise, <see langword="false"/>.</returns>
            private bool IsWhiteSpace(char c)
            {
                // Handle cases where the delimiter is a whitespace (e.g. tab)
                if (c == _delimiter)
                    return false;
                else
                {
                    // See char.IsLatin1(char c) in Reflector
                    if (c <= '\x00ff')
                        return (c == ' ' || c == '\t');
                    else
                        return (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.SpaceSeparator);
                }
            }

            /// <summary>
            /// Moves to the specified record index.
            /// </summary>
            /// <param name="record">The record index.</param>
            /// <exception cref="T:ArgumentOutOfRangeException">
            ///		Record index must be > 0.
            /// </exception>
            /// <exception cref="T:InvalidOperationException">
            ///		Cannot move to a previous record in forward-only mode.
            /// </exception>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            public virtual void MoveTo(long record)
            {
                if (record < 0)
                    throw new ArgumentOutOfRangeException("record", record, "Record index must be 0 or more.");

                if (record < _currentRecordIndex)
                    throw new InvalidOperationException("Cannot move to a previous record in forward-only mode.");

                // Get number of record to read

                long offset = record - _currentRecordIndex;

                if (offset > 0)
                {
                    do
                    {
                        if (!ReadNextRecord())
                            throw new EndOfStreamException(string.Format(CultureInfo.InvariantCulture, "Cannot read record at index '{0}'.", _currentRecordIndex - offset));
                    }
                    while (--offset > 0);
                }
            }

            /// <summary>
            /// Parses a new line delimiter.
            /// </summary>
            /// <param name="pos">The starting position of the parsing. Will contain the resulting end position.</param>
            /// <returns><see langword="true"/> if a new line delimiter was found; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            private bool ParseNewLine(ref int pos)
            {
                Debug.Assert(pos <= _bufferLength);

                // Check if already at the end of the buffer
                if (pos == _bufferLength)
                {
                    pos = 0;

                    if (!ReadBuffer())
                        return false;
                }

                char c = _buffer[pos];

                // Treat \r as new line only if it's not the delimiter

                if (c == '\r' && _delimiter != '\r')
                {
                    pos++;

                    // Skip following \n (if there is one)

                    if (pos < _bufferLength)
                    {
                        if (_buffer[pos] == '\n')
                            pos++;
                    }
                    else
                    {
                        if (ReadBuffer())
                        {
                            if (_buffer[0] == '\n')
                                pos = 1;
                            else
                                pos = 0;
                        }
                    }

                    if (pos >= _bufferLength)
                    {
                        ReadBuffer();
                        pos = 0;
                    }

                    return true;
                }
                else if (c == '\n')
                {
                    pos++;

                    if (pos >= _bufferLength)
                    {
                        ReadBuffer();
                        pos = 0;
                    }

                    return true;
                }

                return false;
            }

            /// <summary>
            /// Determines whether the character at the specified position is a new line delimiter.
            /// </summary>
            /// <param name="pos">The position of the character to verify.</param>
            /// <returns>
            /// 	<see langword="true"/> if the character at the specified position is a new line delimiter; otherwise, <see langword="false"/>.
            /// </returns>
            private bool IsNewLine(int pos)
            {
                Debug.Assert(pos < _bufferLength);

                char c = _buffer[pos];

                if (c == '\n')
                    return true;
                else if (c == '\r' && _delimiter != '\r')
                    return true;
                else
                    return false;
            }

            /// <summary>
            /// Fills the buffer with data from the reader.
            /// </summary>
            /// <returns><see langword="true"/> if data was successfully read; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            private bool ReadBuffer()
            {
                if (_eof)
                    return false;

                CheckDisposed();

                _bufferLength = _reader.Read(_buffer, 0, _bufferSize);

                if (_bufferLength > 0)
                    return true;
                else
                {
                    _eof = true;
                    _buffer = null;

                    return false;
                }
            }

            /// <summary>
            /// Reads the field at the specified index.
            /// Any unread fields with an inferior index will also be read as part of the required parsing.
            /// </summary>
            /// <param name="field">The field index.</param>
            /// <param name="initializing">Indicates if the reader is currently initializing.</param>
            /// <param name="discardValue">Indicates if the value(s) are discarded.</param>
            /// <returns>
            /// The field at the specified index. 
            /// A <see langword="null"/> indicates that an error occured or that the last field has been reached during initialization.
            /// </returns>
            /// <exception cref="ArgumentOutOfRangeException">
            ///		<paramref name="field"/> is out of range.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            ///		There is no current record.
            /// </exception>
            /// <exception cref="MissingFieldCsvException">
            ///		The CSV data appears to be missing a field.
            /// </exception>
            /// <exception cref="MalformedCsvException">
            ///		The CSV data appears to be malformed.
            /// </exception>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            private string ReadField(int field, bool initializing, bool discardValue)
            {
                if (!initializing)
                {
                    if (field < 0 || field >= _fieldCount)
                        throw new ArgumentOutOfRangeException("field", field, string.Format(CultureInfo.InvariantCulture, "Field index must be included in [0, FieldCount[. Specified field index was : '{0}'.", field));

                    if (_currentRecordIndex < 0)
                        throw new InvalidOperationException("No current record.");

                    // Directly return field if cached
                    if (_fields[field] != null)
                        return _fields[field];
                    else if (_missingFieldFlag)
                        return HandleMissingField(null, field, ref _nextFieldStart);
                }

                CheckDisposed();

                int index = _nextFieldIndex;

                while (index < field + 1)
                {
                    // Handle case where stated start of field is past buffer
                    // This can occur because _nextFieldStart is simply 1 + last char position of previous field
                    if (_nextFieldStart == _bufferLength)
                    {
                        _nextFieldStart = 0;

                        // Possible EOF will be handled later (see Handle_EOF1)
                        ReadBuffer();
                    }

                    string value = null;

                    if (_missingFieldFlag)
                    {
                        value = HandleMissingField(value, index, ref _nextFieldStart);
                    }
                    else if (_nextFieldStart == _bufferLength)
                    {
                        // Handle_EOF1: Handle EOF here

                        // If current field is the requested field, then the value of the field is "" as in "f1,f2,f3,(\s*)"
                        // otherwise, the CSV is malformed

                        if (index == field)
                        {
                            if (!discardValue)
                            {
                                value = string.Empty;
                                _fields[index] = value;
                            }
                        }
                        else
                        {
                            value = HandleMissingField(value, index, ref _nextFieldStart);
                        }
                    }
                    else
                    {
                        // Trim spaces at start
                        if (_trimSpaces)
                            SkipWhiteSpaces(ref _nextFieldStart);

                        if (_eof)
                            value = string.Empty;
                        else if (_buffer[_nextFieldStart] != _quote)
                        {
                            // Non-quoted field

                            int start = _nextFieldStart;
                            int pos = _nextFieldStart;

                            for (; ; )
                            {
                                while (pos < _bufferLength)
                                {
                                    char c = _buffer[pos];

                                    if (c == _delimiter)
                                    {
                                        _nextFieldStart = pos + 1;

                                        break;
                                    }
                                    else if (c == '\r' || c == '\n')
                                    {
                                        _nextFieldStart = pos;
                                        _eol = true;

                                        break;
                                    }
                                    else
                                        pos++;
                                }

                                if (pos < _bufferLength)
                                    break;
                                else
                                {
                                    if (!discardValue)
                                        value += new string(_buffer, start, pos - start);

                                    start = 0;
                                    pos = 0;
                                    _nextFieldStart = 0;

                                    if (!ReadBuffer())
                                        break;
                                }
                            }

                            if (!discardValue)
                            {
                                if (!_trimSpaces)
                                {
                                    if (!_eof && pos > start)
                                        value += new string(_buffer, start, pos - start);
                                }
                                else
                                {
                                    if (!_eof && pos > start)
                                    {
                                        // Do the trimming
                                        pos--;
                                        while (pos > -1 && IsWhiteSpace(_buffer[pos]))
                                            pos--;
                                        pos++;

                                        if (pos > 0)
                                            value += new string(_buffer, start, pos - start);
                                    }
                                    else
                                        pos = -1;

                                    // If pos <= 0, that means the trimming went past buffer start,
                                    // and the concatenated value needs to be trimmed too.
                                    if (pos <= 0)
                                    {
                                        pos = (value == null ? -1 : value.Length - 1);

                                        // Do the trimming
                                        while (pos > -1 && IsWhiteSpace(value[pos]))
                                            pos--;

                                        pos++;

                                        if (pos > 0 && pos != value.Length)
                                            value = value.Substring(0, pos);
                                    }
                                }

                                if (value == null)
                                    value = string.Empty;
                            }

                            if (_eol || _eof)
                            {
                                _eol = ParseNewLine(ref _nextFieldStart);

                                // Reaching a new line is ok as long as the parser is initializing or it is the last field
                                if (!initializing && index != _fieldCount - 1)
                                {
                                    if (value != null && value.Length == 0)
                                        value = null;

                                    value = HandleMissingField(value, index, ref _nextFieldStart);
                                }
                            }

                            if (!discardValue)
                                _fields[index] = value;
                        }
                        else
                        {
                            // Quoted field

                            // Skip quote
                            int start = _nextFieldStart + 1;
                            int pos = start;

                            bool quoted = true;
                            bool escaped = false;

                            for (; ; )
                            {
                                while (pos < _bufferLength)
                                {
                                    char c = _buffer[pos];

                                    if (escaped)
                                    {
                                        escaped = false;
                                        start = pos;
                                    }
                                    // IF current char is escape AND (escape and quote are different OR next char is a quote)
                                    else if (c == _escape && (_escape != _quote || (pos + 1 < _bufferLength && _buffer[pos + 1] == _quote) || (pos + 1 == _bufferLength && _reader.Peek() == _quote)))
                                    {
                                        if (!discardValue)
                                            value += new string(_buffer, start, pos - start);

                                        escaped = true;
                                    }
                                    else if (c == _quote)
                                    {
                                        quoted = false;
                                        break;
                                    }

                                    pos++;
                                }

                                if (!quoted)
                                    break;
                                else
                                {
                                    if (!discardValue && !escaped)
                                        value += new string(_buffer, start, pos - start);

                                    start = 0;
                                    pos = 0;
                                    _nextFieldStart = 0;

                                    if (!ReadBuffer())
                                    {
                                        HandleParseError(new MalformedCsvException(GetCurrentRawData(), _nextFieldStart, Math.Max(0, _currentRecordIndex), index), ref _nextFieldStart);
                                        return null;
                                    }
                                }
                            }

                            if (!_eof)
                            {
                                // Append remaining parsed buffer content
                                if (!discardValue && pos > start)
                                    value += new string(_buffer, start, pos - start);

                                // Skip quote
                                _nextFieldStart = pos + 1;

                                // Skip whitespaces between the quote and the delimiter/eol
                                SkipWhiteSpaces(ref _nextFieldStart);

                                // Skip delimiter
                                bool delimiterSkipped;
                                if (_nextFieldStart < _bufferLength && _buffer[_nextFieldStart] == _delimiter)
                                {
                                    _nextFieldStart++;
                                    delimiterSkipped = true;
                                }
                                else
                                {
                                    delimiterSkipped = false;
                                }

                                // Skip new line delimiter if initializing or last field
                                // (if the next field is missing, it will be caught when parsed)
                                if (!_eof && !delimiterSkipped && (initializing || index == _fieldCount - 1))
                                    _eol = ParseNewLine(ref _nextFieldStart);

                                // If no delimiter is present after the quoted field and it is not the last field, then it is a parsing error
                                if (!delimiterSkipped && !_eof && !(_eol || IsNewLine(_nextFieldStart)))
                                    HandleParseError(new MalformedCsvException(GetCurrentRawData(), _nextFieldStart, Math.Max(0, _currentRecordIndex), index), ref _nextFieldStart);
                            }

                            if (!discardValue)
                            {
                                if (value == null)
                                    value = string.Empty;

                                _fields[index] = value;
                            }
                        }
                    }

                    _nextFieldIndex = Math.Max(index + 1, _nextFieldIndex);

                    if (index == field)
                    {
                        // If initializing, return null to signify the last field has been reached

                        if (initializing)
                        {
                            if (_eol || _eof)
                                return null;
                            else
                                return string.IsNullOrEmpty(value) ? string.Empty : value;
                        }
                        else
                            return value;
                    }

                    index++;
                }

                // Getting here is bad ...
                HandleParseError(new MalformedCsvException(GetCurrentRawData(), _nextFieldStart, Math.Max(0, _currentRecordIndex), index), ref _nextFieldStart);
                return null;
            }

            /// <summary>
            /// Reads the next record.
            /// </summary>
            /// <returns><see langword="true"/> if a record has been successfully reads; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            public bool ReadNextRecord()
            {
                return ReadNextRecord(false, false);
            }

            /// <summary>
            /// Reads the next record.
            /// </summary>
            /// <param name="onlyReadHeaders">
            /// Indicates if the reader will proceed to the next record after having read headers.
            /// <see langword="true"/> if it stops after having read headers; otherwise, <see langword="false"/>.
            /// </param>
            /// <param name="skipToNextLine">
            /// Indicates if the reader will skip directly to the next line without parsing the current one. 
            /// To be used when an error occurs.
            /// </param>
            /// <returns><see langword="true"/> if a record has been successfully reads; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            protected virtual bool ReadNextRecord(bool onlyReadHeaders, bool skipToNextLine)
            {
                if (_eof)
                {
                    if (_firstRecordInCache)
                    {
                        _firstRecordInCache = false;
                        _currentRecordIndex++;

                        return true;
                    }
                    else
                        return false;
                }

                CheckDisposed();

                if (!_initialized)
                {
                    _buffer = new char[_bufferSize];

                    // will be replaced if and when headers are read
                    _fieldHeaders = new string[0];

                    if (!ReadBuffer())
                        return false;

                    if (!SkipEmptyAndCommentedLines(ref _nextFieldStart))
                        return false;

                    // Keep growing _fields array until the last field has been found
                    // and then resize it to its final correct size

                    _fieldCount = 0;
                    _fields = new string[16];

                    while (ReadField(_fieldCount, true, false) != null)
                    {
                        if (_parseErrorFlag)
                        {
                            _fieldCount = 0;
                            Array.Clear(_fields, 0, _fields.Length);
                            _parseErrorFlag = false;
                            _nextFieldIndex = 0;
                        }
                        else
                        {
                            _fieldCount++;

                            if (_fieldCount == _fields.Length)
                                Array.Resize<string>(ref _fields, (_fieldCount + 1) * 2);
                        }
                    }

                    // _fieldCount contains the last field index, but it must contains the field count,
                    // so increment by 1
                    _fieldCount++;

                    if (_fields.Length != _fieldCount)
                        Array.Resize<string>(ref _fields, _fieldCount);

                    _initialized = true;

                    // If headers are present, call ReadNextRecord again
                    if (_hasHeaders)
                    {
                        // Don't count first record as it was the headers
                        _currentRecordIndex = -1;

                        _firstRecordInCache = false;

                        _fieldHeaders = new string[_fieldCount];
                        _fieldHeaderIndexes = new Dictionary<string, int>(_fieldCount, _fieldHeaderComparer);

                        for (int i = 0; i < _fields.Length; i++)
                        {
                            _fieldHeaders[i] = _fields[i];
                            _fieldHeaderIndexes.Add(_fields[i], i);
                        }

                        // Proceed to first record
                        if (!onlyReadHeaders)
                        {
                            // Calling again ReadNextRecord() seems to be simpler, 
                            // but in fact would probably cause many subtle bugs because the derived does not expect a recursive behavior
                            // so simply do what is needed here and no more.

                            if (!SkipEmptyAndCommentedLines(ref _nextFieldStart))
                                return false;

                            Array.Clear(_fields, 0, _fields.Length);
                            _nextFieldIndex = 0;
                            _eol = false;

                            _currentRecordIndex++;
                            return true;
                        }
                    }
                    else
                    {
                        if (onlyReadHeaders)
                        {
                            _firstRecordInCache = true;
                            _currentRecordIndex = -1;
                        }
                        else
                        {
                            _firstRecordInCache = false;
                            _currentRecordIndex = 0;
                        }
                    }
                }
                else
                {
                    if (skipToNextLine)
                        SkipToNextLine(ref _nextFieldStart);
                    else if (_currentRecordIndex > -1 && !_missingFieldFlag)
                    {
                        // If not already at end of record, move there
                        if (!_eol && !_eof)
                        {
                            if (!_supportsMultiline)
                                SkipToNextLine(ref _nextFieldStart);
                            else
                            {
                                // a dirty trick to handle the case where extra fields are present
                                while (ReadField(_nextFieldIndex, true, true) != null)
                                {
                                }
                            }
                        }
                    }

                    if (!_firstRecordInCache && !SkipEmptyAndCommentedLines(ref _nextFieldStart))
                        return false;

                    if (_hasHeaders || !_firstRecordInCache)
                        _eol = false;

                    // Check to see if the first record is in cache.
                    // This can happen when initializing a reader with no headers
                    // because one record must be read to get the field count automatically
                    if (_firstRecordInCache)
                        _firstRecordInCache = false;
                    else
                    {
                        Array.Clear(_fields, 0, _fields.Length);
                        _nextFieldIndex = 0;
                    }

                    _missingFieldFlag = false;
                    _parseErrorFlag = false;
                    _currentRecordIndex++;
                }

                return true;
            }

            /// <summary>
            /// Skips empty and commented lines.
            /// If the end of the buffer is reached, its content be discarded and filled again from the reader.
            /// </summary>
            /// <param name="pos">
            /// The position in the buffer where to start parsing. 
            /// Will contains the resulting position after the operation.
            /// </param>
            /// <returns><see langword="true"/> if the end of the reader has not been reached; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            private bool SkipEmptyAndCommentedLines(ref int pos)
            {
                if (pos < _bufferLength)
                    DoSkipEmptyAndCommentedLines(ref pos);

                while (pos >= _bufferLength && !_eof)
                {
                    if (ReadBuffer())
                    {
                        pos = 0;
                        DoSkipEmptyAndCommentedLines(ref pos);
                    }
                    else
                        return false;
                }

                return !_eof;
            }

            /// <summary>
            /// <para>Worker method.</para>
            /// <para>Skips empty and commented lines.</para>
            /// </summary>
            /// <param name="pos">
            /// The position in the buffer where to start parsing. 
            /// Will contains the resulting position after the operation.
            /// </param>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            private void DoSkipEmptyAndCommentedLines(ref int pos)
            {
                while (pos < _bufferLength)
                {
                    if (_buffer[pos] == _comment)
                    {
                        pos++;
                        SkipToNextLine(ref pos);
                    }
                    else if (_skipEmptyLines && ParseNewLine(ref pos))
                        continue;
                    else
                        break;
                }
            }

            /// <summary>
            /// Skips whitespace characters.
            /// </summary>
            /// <param name="pos">The starting position of the parsing. Will contain the resulting end position.</param>
            /// <returns><see langword="true"/> if the end of the reader has not been reached; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            private bool SkipWhiteSpaces(ref int pos)
            {
                for (; ; )
                {
                    while (pos < _bufferLength && IsWhiteSpace(_buffer[pos]))
                        pos++;

                    if (pos < _bufferLength)
                        break;
                    else
                    {
                        pos = 0;

                        if (!ReadBuffer())
                            return false;
                    }
                }

                return true;
            }

            /// <summary>
            /// Skips ahead to the next NewLine character.
            /// If the end of the buffer is reached, its content be discarded and filled again from the reader.
            /// </summary>
            /// <param name="pos">
            /// The position in the buffer where to start parsing. 
            /// Will contains the resulting position after the operation.
            /// </param>
            /// <returns><see langword="true"/> if the end of the reader has not been reached; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            private bool SkipToNextLine(ref int pos)
            {
                // ((pos = 0) == 0) is a little trick to reset position inline
                while ((pos < _bufferLength || (ReadBuffer() && ((pos = 0) == 0))) && !ParseNewLine(ref pos))
                    pos++;

                return !_eof;
            }

            /// <summary>
            /// Handles a parsing error.
            /// </summary>
            /// <param name="error">The parsing error that occured.</param>
            /// <param name="pos">The current position in the buffer.</param>
            /// <exception cref="ArgumentNullException">
            ///	<paramref name="error"/> is <see langword="null"/>.
            /// </exception>
            private void HandleParseError(MalformedCsvException error, ref int pos)
            {
                if (error == null)
                    throw new ArgumentNullException("error");

                _parseErrorFlag = true;

                switch (_defaultParseErrorAction)
                {
                    case ParseErrorAction.ThrowException:
                        throw error;

                    case ParseErrorAction.RaiseEvent:
                        ParseErrorEventArgs e = new ParseErrorEventArgs(error, ParseErrorAction.ThrowException);
                        OnParseError(e);

                        switch (e.Action)
                        {
                            case ParseErrorAction.ThrowException:
                                throw e.Error;

                            case ParseErrorAction.RaiseEvent:
                                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "'{0}' is not a valid ParseErrorAction while inside a ParseError event.", e.Action), e.Error);

                            case ParseErrorAction.AdvanceToNextLine:
                                // already at EOL when fields are missing, so don't skip to next line in that case
                                if (!_missingFieldFlag && pos >= 0)
                                    SkipToNextLine(ref pos);
                                break;

                            default:
                                throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "'{0}' is not a supported ParseErrorAction.", e.Action), e.Error);
                        }
                        break;

                    case ParseErrorAction.AdvanceToNextLine:
                        // already at EOL when fields are missing, so don't skip to next line in that case
                        if (!_missingFieldFlag && pos >= 0)
                            SkipToNextLine(ref pos);
                        break;

                    default:
                        throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "'{0}' is not a supported ParseErrorAction.", _defaultParseErrorAction), error);
                }
            }

            /// <summary>
            /// Handles a missing field error.
            /// </summary>
            /// <param name="value">The partially parsed value, if available.</param>
            /// <param name="fieldIndex">The missing field index.</param>
            /// <param name="currentPosition">The current position in the raw data.</param>
            /// <returns>
            /// The resulting value according to <see cref="M:MissingFieldAction"/>.
            /// If the action is set to <see cref="T:MissingFieldAction.TreatAsParseError"/>,
            /// then the parse error will be handled according to <see cref="DefaultParseErrorAction"/>.
            /// </returns>
            private string HandleMissingField(string value, int fieldIndex, ref int currentPosition)
            {
                if (fieldIndex < 0 || fieldIndex >= _fieldCount)
                    throw new ArgumentOutOfRangeException("fieldIndex", fieldIndex, string.Format(CultureInfo.InvariantCulture, "Field index must be included in [0, FieldCount[. Specified field index was : '{0}'.", fieldIndex));

                _missingFieldFlag = true;

                for (int i = fieldIndex + 1; i < _fieldCount; i++)
                    _fields[i] = null;

                if (value != null)
                    return value;
                else
                {
                    switch (_missingFieldAction)
                    {
                        case MissingFieldAction.ParseError:
                            HandleParseError(new MissingFieldCsvException(GetCurrentRawData(), currentPosition, Math.Max(0, _currentRecordIndex), fieldIndex), ref currentPosition);
                            return value;

                        case MissingFieldAction.ReplaceByEmpty:
                            return string.Empty;

                        case MissingFieldAction.ReplaceByNull:
                            return null;

                        default:
                            throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "'{0}' is not a supported missing field action.", _missingFieldAction));
                    }
                }
            }

            /// <summary>
            /// Validates the state of the data reader.
            /// </summary>
            /// <param name="validations">The validations to accomplish.</param>
            /// <exception cref="InvalidOperationException">
            ///	No current record.
            /// </exception>
            /// <exception cref="InvalidOperationException">
            ///	This operation is invalid when the reader is closed.
            /// </exception>
            private void ValidateDataReader(DataReaderValidations validations)
            {
                if ((validations & DataReaderValidations.IsInitialized) != 0 && !_initialized)
                    throw new InvalidOperationException("No current record.");

                if ((validations & DataReaderValidations.IsNotClosed) != 0 && _isDisposed)
                    throw new InvalidOperationException("This operation is invalid when the reader is closed.");
            }

            /// <summary>
            /// Copy the value of the specified field to an array.
            /// </summary>
            /// <param name="field">The index of the field.</param>
            /// <param name="fieldOffset">The offset in the field value.</param>
            /// <param name="destinationArray">The destination array where the field value will be copied.</param>
            /// <param name="destinationOffset">The destination array offset.</param>
            /// <param name="length">The number of characters to copy from the field value.</param>
            /// <returns></returns>
            private long CopyFieldToArray(int field, long fieldOffset, Array destinationArray, int destinationOffset, int length)
            {
                EnsureInitialize();

                if (field < 0 || field >= _fieldCount)
                    throw new ArgumentOutOfRangeException("field", field, string.Format(CultureInfo.InvariantCulture, "Field index must be included in [0, FieldCount[. Specified field index was : '{0}'.", field));

                if (fieldOffset < 0 || fieldOffset >= int.MaxValue)
                    throw new ArgumentOutOfRangeException("fieldOffset");

                // Array.Copy(...) will do the remaining argument checks

                if (length == 0)
                    return 0;

                string value = this[field];

                if (value == null)
                    value = string.Empty;

                Debug.Assert(fieldOffset < int.MaxValue);

                Debug.Assert(destinationArray.GetType() == typeof(char[]) || destinationArray.GetType() == typeof(byte[]));

                if (destinationArray.GetType() == typeof(char[]))
                    Array.Copy(value.ToCharArray((int)fieldOffset, length), 0, destinationArray, destinationOffset, length);
                else
                {
                    char[] chars = value.ToCharArray((int)fieldOffset, length);
                    byte[] source = new byte[chars.Length]; ;

                    for (int i = 0; i < chars.Length; i++)
                        source[i] = Convert.ToByte(chars[i]);

                    Array.Copy(source, 0, destinationArray, destinationOffset, length);
                }

                return length;
            }

            int IDataReader.RecordsAffected
            {
                get
                {
                    // For SELECT statements, -1 must be returned.
                    return -1;
                }
            }

            bool IDataReader.IsClosed
            {
                get
                {
                    return _eof;
                }
            }

            bool IDataReader.NextResult()
            {
                ValidateDataReader(DataReaderValidations.IsNotClosed);

                return false;
            }

            void IDataReader.Close()
            {
                Dispose();
            }

            bool IDataReader.Read()
            {
                ValidateDataReader(DataReaderValidations.IsNotClosed);

                return ReadNextRecord();
            }

            int IDataReader.Depth
            {
                get
                {
                    ValidateDataReader(DataReaderValidations.IsNotClosed);

                    return 0;
                }
            }

            DataTable IDataReader.GetSchemaTable()
            {
                EnsureInitialize();
                ValidateDataReader(DataReaderValidations.IsNotClosed);

                DataTable schema = new DataTable("SchemaTable");
                schema.Locale = CultureInfo.InvariantCulture;
                schema.MinimumCapacity = _fieldCount;

                schema.Columns.Add(SchemaTableColumn.AllowDBNull, typeof(bool)).ReadOnly = true;
                schema.Columns.Add(SchemaTableColumn.BaseColumnName, typeof(string)).ReadOnly = true;
                schema.Columns.Add(SchemaTableColumn.BaseSchemaName, typeof(string)).ReadOnly = true;
                schema.Columns.Add(SchemaTableColumn.BaseTableName, typeof(string)).ReadOnly = true;
                schema.Columns.Add(SchemaTableColumn.ColumnName, typeof(string)).ReadOnly = true;
                schema.Columns.Add(SchemaTableColumn.ColumnOrdinal, typeof(int)).ReadOnly = true;
                schema.Columns.Add(SchemaTableColumn.ColumnSize, typeof(int)).ReadOnly = true;
                schema.Columns.Add(SchemaTableColumn.DataType, typeof(object)).ReadOnly = true;
                schema.Columns.Add(SchemaTableColumn.IsAliased, typeof(bool)).ReadOnly = true;
                schema.Columns.Add(SchemaTableColumn.IsExpression, typeof(bool)).ReadOnly = true;
                schema.Columns.Add(SchemaTableColumn.IsKey, typeof(bool)).ReadOnly = true;
                schema.Columns.Add(SchemaTableColumn.IsLong, typeof(bool)).ReadOnly = true;
                schema.Columns.Add(SchemaTableColumn.IsUnique, typeof(bool)).ReadOnly = true;
                schema.Columns.Add(SchemaTableColumn.NumericPrecision, typeof(short)).ReadOnly = true;
                schema.Columns.Add(SchemaTableColumn.NumericScale, typeof(short)).ReadOnly = true;
                schema.Columns.Add(SchemaTableColumn.ProviderType, typeof(int)).ReadOnly = true;

                schema.Columns.Add(SchemaTableOptionalColumn.BaseCatalogName, typeof(string)).ReadOnly = true;
                schema.Columns.Add(SchemaTableOptionalColumn.BaseServerName, typeof(string)).ReadOnly = true;
                schema.Columns.Add(SchemaTableOptionalColumn.IsAutoIncrement, typeof(bool)).ReadOnly = true;
                schema.Columns.Add(SchemaTableOptionalColumn.IsHidden, typeof(bool)).ReadOnly = true;
                schema.Columns.Add(SchemaTableOptionalColumn.IsReadOnly, typeof(bool)).ReadOnly = true;
                schema.Columns.Add(SchemaTableOptionalColumn.IsRowVersion, typeof(bool)).ReadOnly = true;

                string[] columnNames;

                if (_hasHeaders)
                    columnNames = _fieldHeaders;
                else
                {
                    columnNames = new string[_fieldCount];

                    for (int i = 0; i < _fieldCount; i++)
                        columnNames[i] = "Column" + i.ToString(CultureInfo.InvariantCulture);
                }

                // null marks columns that will change for each row
                object[] schemaRow = new object[] { 
					true,					// 00- AllowDBNull
					null,					// 01- BaseColumnName
					string.Empty,			// 02- BaseSchemaName
					string.Empty,			// 03- BaseTableName
					null,					// 04- ColumnName
					null,					// 05- ColumnOrdinal
					int.MaxValue,			// 06- ColumnSize
					typeof(string),			// 07- DataType
					false,					// 08- IsAliased
					false,					// 09- IsExpression
					false,					// 10- IsKey
					false,					// 11- IsLong
					false,					// 12- IsUnique
					DBNull.Value,			// 13- NumericPrecision
					DBNull.Value,			// 14- NumericScale
					(int) DbType.String,	// 15- ProviderType

					string.Empty,			// 16- BaseCatalogName
					string.Empty,			// 17- BaseServerName
					false,					// 18- IsAutoIncrement
					false,					// 19- IsHidden
					true,					// 20- IsReadOnly
					false					// 21- IsRowVersion
			  };

                for (int i = 0; i < columnNames.Length; i++)
                {
                    schemaRow[1] = columnNames[i]; // Base column name
                    schemaRow[4] = columnNames[i]; // Column name
                    schemaRow[5] = i; // Column ordinal

                    schema.Rows.Add(schemaRow);
                }

                return schema;
            }

            int IDataRecord.GetInt32(int i)
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

                string value = this[i];

                return Int32.Parse(value == null ? string.Empty : value, CultureInfo.CurrentCulture);
            }

            object IDataRecord.this[string name]
            {
                get
                {
                    ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                    return this[name];
                }
            }

            object IDataRecord.this[int i]
            {
                get
                {
                    ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                    return this[i];
                }
            }

            object IDataRecord.GetValue(int i)
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

                if (((IDataRecord)this).IsDBNull(i))
                    return DBNull.Value;
                else
                    return this[i];
            }

            bool IDataRecord.IsDBNull(int i)
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                return (string.IsNullOrEmpty(this[i]));
            }

            long IDataRecord.GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

                return CopyFieldToArray(i, fieldOffset, buffer, bufferoffset, length);
            }

            byte IDataRecord.GetByte(int i)
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                return Byte.Parse(this[i], CultureInfo.CurrentCulture);
            }

            Type IDataRecord.GetFieldType(int i)
            {
                EnsureInitialize();
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

                if (i < 0 || i >= _fieldCount)
                    throw new ArgumentOutOfRangeException("i", i, string.Format(CultureInfo.InvariantCulture, "Field index must be included in [0, FieldCount[. Specified field index was : '{0}'.", i));

                return typeof(string);
            }

            decimal IDataRecord.GetDecimal(int i)
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                return Decimal.Parse(this[i], CultureInfo.CurrentCulture);
            }

            int IDataRecord.GetValues(object[] values)
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

                IDataRecord record = (IDataRecord)this;

                for (int i = 0; i < _fieldCount; i++)
                    values[i] = record.GetValue(i);

                return _fieldCount;
            }

            string IDataRecord.GetName(int i)
            {
                EnsureInitialize();
                ValidateDataReader(DataReaderValidations.IsNotClosed);

                if (i < 0 || i >= _fieldCount)
                    throw new ArgumentOutOfRangeException("i", i, string.Format(CultureInfo.InvariantCulture, "Field index must be included in [0, FieldCount[. Specified field index was : '{0}'.", i));

                if (_hasHeaders)
                    return _fieldHeaders[i];
                else
                    return "Column" + i.ToString(CultureInfo.InvariantCulture);
            }

            long IDataRecord.GetInt64(int i)
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                return Int64.Parse(this[i], CultureInfo.CurrentCulture);
            }

            double IDataRecord.GetDouble(int i)
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                return Double.Parse(this[i], CultureInfo.CurrentCulture);
            }

            bool IDataRecord.GetBoolean(int i)
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

                string value = this[i];

                int result;

                if (Int32.TryParse(value, out result))
                    return (result != 0);
                else
                    return Boolean.Parse(value);
            }

            Guid IDataRecord.GetGuid(int i)
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                return new Guid(this[i]);
            }

            DateTime IDataRecord.GetDateTime(int i)
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                return DateTime.Parse(this[i], CultureInfo.CurrentCulture);
            }

            int IDataRecord.GetOrdinal(string name)
            {
                EnsureInitialize();
                ValidateDataReader(DataReaderValidations.IsNotClosed);

                int index;

                if (!_fieldHeaderIndexes.TryGetValue(name, out index))
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "'{0}' field header not found.", name), "name");

                return index;
            }

            string IDataRecord.GetDataTypeName(int i)
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                return typeof(string).FullName;
            }

            float IDataRecord.GetFloat(int i)
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                return Single.Parse(this[i], CultureInfo.CurrentCulture);
            }

            IDataReader IDataRecord.GetData(int i)
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

                if (i == 0)
                    return this;
                else
                    return null;
            }

            long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

                return CopyFieldToArray(i, fieldoffset, buffer, bufferoffset, length);
            }

            string IDataRecord.GetString(int i)
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                return this[i];
            }

            char IDataRecord.GetChar(int i)
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                return Char.Parse(this[i]);
            }

            short IDataRecord.GetInt16(int i)
            {
                ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                return Int16.Parse(this[i], CultureInfo.CurrentCulture);
            }

            /// <summary>
            /// Returns an <see cref="T:RecordEnumerator"/>  that can iterate through CSV records.
            /// </summary>
            /// <returns>An <see cref="T:RecordEnumerator"/>  that can iterate through CSV records.</returns>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            public CsvReader.RecordEnumerator GetEnumerator()
            {
                return new CsvReader.RecordEnumerator(this);
            }

            /// <summary>
            /// Returns an <see cref="T:System.Collections.Generics.IEnumerator"/>  that can iterate through CSV records.
            /// </summary>
            /// <returns>An <see cref="T:System.Collections.Generics.IEnumerator"/>  that can iterate through CSV records.</returns>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            IEnumerator<string[]> IEnumerable<string[]>.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            /// <summary>
            /// Returns an <see cref="T:System.Collections.IEnumerator"/>  that can iterate through CSV records.
            /// </summary>
            /// <returns>An <see cref="T:System.Collections.IEnumerator"/>  that can iterate through CSV records.</returns>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

#if DEBUG
            /// <summary>
            /// Contains the stack when the object was allocated.
            /// </summary>
            private System.Diagnostics.StackTrace _allocStack;
#endif

            /// <summary>
            /// Contains the disposed status flag.
            /// </summary>
            private bool _isDisposed = false;

            /// <summary>
            /// Contains the locking object for multi-threading purpose.
            /// </summary>
            private readonly object _lock = new object();

            /// <summary>
            /// Occurs when the instance is disposed of.
            /// </summary>
            public event EventHandler Disposed;

            /// <summary>
            /// Gets a value indicating whether the instance has been disposed of.
            /// </summary>
            /// <value>
            /// 	<see langword="true"/> if the instance has been disposed of; otherwise, <see langword="false"/>.
            /// </value>
            [System.ComponentModel.Browsable(false)]
            public bool IsDisposed
            {
                get { return _isDisposed; }
            }

            /// <summary>
            /// Raises the <see cref="M:Disposed"/> event.
            /// </summary>
            /// <param name="e">A <see cref="T:System.EventArgs"/> that contains the event data.</param>
            protected virtual void OnDisposed(EventArgs e)
            {
                EventHandler handler = Disposed;

                if (handler != null)
                    handler(this, e);
            }

            /// <summary>
            /// Checks if the instance has been disposed of, and if it has, throws an <see cref="T:System.ComponentModel.ObjectDisposedException"/>; otherwise, does nothing.
            /// </summary>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            /// 	The instance has been disposed of.
            /// </exception>
            /// <remarks>
            /// 	Derived classes should call this method at the start of all methods and properties that should not be accessed after a call to <see cref="M:Dispose()"/>.
            /// </remarks>
            protected void CheckDisposed()
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(this.GetType().FullName);
            }

            /// <summary>
            /// Releases all resources used by the instance.
            /// </summary>
            /// <remarks>
            /// 	Calls <see cref="M:Dispose(Boolean)"/> with the disposing parameter set to <see langword="true"/> to free unmanaged and managed resources.
            /// </remarks>
            public void Dispose()
            {
                if (!_isDisposed)
                {
                    Dispose(true);
                    GC.SuppressFinalize(this);
                }
            }

            /// <summary>
            /// Releases the unmanaged resources used by this instance and optionally releases the managed resources.
            /// </summary>
            /// <param name="disposing">
            /// 	<see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.
            /// </param>
            protected virtual void Dispose(bool disposing)
            {
                // Refer to http://www.bluebytesoftware.com/blog/PermaLink,guid,88e62cdf-5919-4ac7-bc33-20c06ae539ae.aspx
                // Refer to http://www.gotdotnet.com/team/libraries/whitepapers/resourcemanagement/resourcemanagement.aspx

                // No exception should ever be thrown except in critical scenarios.
                // Unhandled exceptions during finalization will tear down the process.
                if (!_isDisposed)
                {
                    try
                    {
                        // Dispose-time code should call Dispose() on all owned objects that implement the IDisposable interface. 
                        // "owned" means objects whose lifetime is solely controlled by the container. 
                        // In cases where ownership is not as straightforward, techniques such as HandleCollector can be used.  
                        // Large managed object fields should be nulled out.

                        // Dispose-time code should also set references of all owned objects to null, after disposing them. This will allow the referenced objects to be garbage collected even if not all references to the "parent" are released. It may be a significant memory consumption win if the referenced objects are large, such as big arrays, collections, etc. 
                        if (disposing)
                        {
                            // Acquire a lock on the object while disposing.

                            if (_reader != null)
                            {
                                lock (_lock)
                                {
                                    if (_reader != null)
                                    {
                                        _reader.Dispose();

                                        _reader = null;
                                        _buffer = null;
                                        _eof = true;
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        // Ensure that the flag is set
                        _isDisposed = true;

                        // Catch any issues about firing an event on an already disposed object.
                        try
                        {
                            OnDisposed(EventArgs.Empty);
                        }
                        catch { }
                    }
                }
            }

            /// <summary>
            /// Releases unmanaged resources and performs other cleanup operations before the instance is reclaimed by garbage collection.
            /// </summary>
            ~CsvReader()
            {
#if DEBUG
                Debug.WriteLine("FinalizableObject was not disposed" + _allocStack.ToString());
#endif

                Dispose(false);
            }
        }

        /// <summary>
        /// Specifies the action to take when a parsing error has occured.
        /// </summary>
        public enum ParseErrorAction
        {
            /// <summary>
            /// Raises the <see cref="M:CsvReader.ParseError"/> event.
            /// </summary>
            RaiseEvent = 0,

            /// <summary>
            /// Tries to advance to next line.
            /// </summary>
            AdvanceToNextLine = 1,

            /// <summary>
            /// Throws an exception.
            /// </summary>
            ThrowException = 2,
        }

        /// <summary>
        /// Specifies the action to take when a field is missing.
        /// </summary>
        public enum MissingFieldAction
        {
            /// <summary>
            /// Treat as a parsing error.
            /// </summary>
            ParseError = 0,

            /// <summary>
            /// Replaces by an empty value.
            /// </summary>
            ReplaceByEmpty = 1,

            /// <summary>
            /// Replaces by a null value (<see langword="null"/>).
            /// </summary>
            ReplaceByNull = 2,
        }

        public partial class CsvReader
        {
            /// <summary>
            /// Defines the data reader validations.
            /// </summary>
            [Flags]
            private enum DataReaderValidations
            {
                /// <summary>
                /// No validation.
                /// </summary>
                None = 0,

                /// <summary>
                /// Validate that the data reader is initialized.
                /// </summary>
                IsInitialized = 1,

                /// <summary>
                /// Validate that the data reader is not closed.
                /// </summary>
                IsNotClosed = 2
            }
        }

        public partial class CsvReader
        {
            /// <summary>
            /// Supports a simple iteration over the records of a <see cref="T:CsvReader"/>.
            /// </summary>
            public struct RecordEnumerator
                : IEnumerator<string[]>, IEnumerator
            {
                /// <summary>
                /// Contains the enumerated <see cref="T:CsvReader"/>.
                /// </summary>
                private CsvReader _reader;

                /// <summary>
                /// Contains the current record.
                /// </summary>
                private string[] _current;

                /// <summary>
                /// Contains the current record index.
                /// </summary>
                private long _currentRecordIndex;

                /// <summary>
                /// Initializes a new instance of the <see cref="T:RecordEnumerator"/> class.
                /// </summary>
                /// <param name="reader">The <see cref="T:CsvReader"/> to iterate over.</param>
                /// <exception cref="T:ArgumentNullException">
                ///		<paramref name="reader"/> is a <see langword="null"/>.
                /// </exception>
                public RecordEnumerator(CsvReader reader)
                {
                    if (reader == null)
                        throw new ArgumentNullException("reader");

                    _reader = reader;
                    _current = null;

                    _currentRecordIndex = reader._currentRecordIndex;
                }

                /// <summary>
                /// Gets the current record.
                /// </summary>
                public string[] Current
                {
                    get { return _current; }
                }

                /// <summary>
                /// Advances the enumerator to the next record of the CSV.
                /// </summary>
                /// <returns><see langword="true"/> if the enumerator was successfully advanced to the next record, <see langword="false"/> if the enumerator has passed the end of the CSV.</returns>
                public bool MoveNext()
                {
                    if (_reader._currentRecordIndex != _currentRecordIndex)
                        throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

                    if (_reader.ReadNextRecord())
                    {
                        _current = new string[_reader._fieldCount];

                        _reader.CopyCurrentRecordTo(_current);
                        _currentRecordIndex = _reader._currentRecordIndex;

                        return true;
                    }
                    else
                    {
                        _current = null;
                        _currentRecordIndex = _reader._currentRecordIndex;

                        return false;
                    }
                }

                /// <summary>
                /// Sets the enumerator to its initial position, which is before the first record in the CSV.
                /// </summary>
                public void Reset()
                {
                    if (_reader._currentRecordIndex != _currentRecordIndex)
                        throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

                    _reader.MoveTo(-1);

                    _current = null;
                    _currentRecordIndex = _reader._currentRecordIndex;
                }

                /// <summary>
                /// Gets the current record.
                /// </summary>
                object IEnumerator.Current
                {
                    get
                    {
                        if (_reader._currentRecordIndex != _currentRecordIndex)
                            throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");

                        return this.Current;
                    }
                }

                /// <summary>
                /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
                /// </summary>
                public void Dispose()
                {
                    _reader = null;
                    _current = null;
                }
            }
        }

        public partial class CachedCsvReader
            : CsvReader
        {
            /// <summary>
            /// Represents a CSV record comparer.
            /// </summary>
            private class CsvRecordComparer
                : IComparer<string[]>
            {
                /// <summary>
                /// Contains the field index of the values to compare.
                /// </summary>
                private int _field;

                /// <summary>
                /// Contains the sort direction.
                /// </summary>
                private ListSortDirection _direction;

                /// <summary>
                /// Initializes a new instance of the CsvRecordComparer class.
                /// </summary>
                /// <param name="field">The field index of the values to compare.</param>
                /// <param name="direction">The sort direction.</param>
                public CsvRecordComparer(int field, ListSortDirection direction)
                {
                    if (field < 0)
                        throw new ArgumentOutOfRangeException("field", field, string.Format(CultureInfo.InvariantCulture, "Field index must be included in [0, FieldCount[. Specified field index was : '{0}'.", field));

                    _field = field;
                    _direction = direction;
                }

                public int Compare(string[] x, string[] y)
                {
                    Debug.Assert(x != null && y != null && x.Length == y.Length && _field < x.Length);

                    int result = String.Compare(x[_field], y[_field], StringComparison.CurrentCulture);

                    return (_direction == ListSortDirection.Ascending ? result : -result);
                }
            }
        }

        public partial class CachedCsvReader
            : CsvReader
        {
            /// <summary>
            /// Represents a CSV field property descriptor.
            /// </summary>
            private class CsvPropertyDescriptor
                : PropertyDescriptor
            {

                /// <summary>
                /// Contains the field index.
                /// </summary>
                private int _index;

                /// <summary>
                /// Initializes a new instance of the CsvPropertyDescriptor class.
                /// </summary>
                /// <param name="fieldName">The field name.</param>
                /// <param name="index">The field index.</param>
                public CsvPropertyDescriptor(string fieldName, int index)
                    : base(fieldName, null)
                {
                    _index = index;
                }

                /// <summary>
                /// Gets the field index.
                /// </summary>
                /// <value>The field index.</value>
                public int Index
                {
                    get { return _index; }
                }

                public override bool CanResetValue(object component)
                {
                    return false;
                }

                public override object GetValue(object component)
                {
                    return ((string[])component)[_index];
                }

                public override void ResetValue(object component)
                {
                }

                public override void SetValue(object component, object value)
                {
                }

                public override bool ShouldSerializeValue(object component)
                {
                    return false;
                }

                public override Type ComponentType
                {
                    get
                    {
                        return typeof(CachedCsvReader);
                    }
                }

                public override bool IsReadOnly
                {
                    get
                    {
                        return true;
                    }
                }

                public override Type PropertyType
                {
                    get
                    {
                        return typeof(string);
                    }
                }
            }
        }

        /// <summary>
        /// Represents a reader that provides fast, cached, dynamic access to CSV data.
        /// </summary>
        /// <remarks>The number of records is limited to <see cref="System.Int32.MaxValue"/> - 1.</remarks>
        public partial class CachedCsvReader
            : CsvReader, IListSource
        {

            /// <summary>
            /// Contains the cached records.
            /// </summary>
            private List<string[]> _records;

            /// <summary>
            /// Contains the current record index (inside the cached records array).
            /// </summary>
            private long _currentRecordIndex;

            /// <summary>
            /// Indicates if a new record is being read from the CSV stream.
            /// </summary>
            private bool _readingStream;

            /// <summary>
            /// Contains the binding list linked to this reader.
            /// </summary>
            private CsvBindingList _bindingList;

            /// <summary>
            /// Initializes a new instance of the CsvReader class.
            /// </summary>
            /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
            /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
            /// <exception cref="T:ArgumentNullException">
            ///		<paramref name="reader"/> is a <see langword="null"/>.
            /// </exception>
            /// <exception cref="T:ArgumentException">
            ///		Cannot read from <paramref name="reader"/>.
            /// </exception>
            public CachedCsvReader(TextReader reader, bool hasHeaders)
                : this(reader, hasHeaders, DefaultBufferSize)
            {
            }

            /// <summary>
            /// Initializes a new instance of the CsvReader class.
            /// </summary>
            /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
            /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
            /// <param name="bufferSize">The buffer size in bytes.</param>
            /// <exception cref="T:ArgumentNullException">
            ///		<paramref name="reader"/> is a <see langword="null"/>.
            /// </exception>
            /// <exception cref="T:ArgumentException">
            ///		Cannot read from <paramref name="reader"/>.
            /// </exception>
            public CachedCsvReader(TextReader reader, bool hasHeaders, int bufferSize)
                : this(reader, hasHeaders, DefaultDelimiter, DefaultQuote, DefaultEscape, DefaultComment, true, bufferSize)
            {
            }

            /// <summary>
            /// Initializes a new instance of the CsvReader class.
            /// </summary>
            /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
            /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
            /// <param name="delimiter">The delimiter character separating each field (default is ',').</param>
            /// <exception cref="T:ArgumentNullException">
            ///		<paramref name="reader"/> is a <see langword="null"/>.
            /// </exception>
            /// <exception cref="T:ArgumentException">
            ///		Cannot read from <paramref name="reader"/>.
            /// </exception>
            public CachedCsvReader(TextReader reader, bool hasHeaders, char delimiter)
                : this(reader, hasHeaders, delimiter, DefaultQuote, DefaultEscape, DefaultComment, true, DefaultBufferSize)
            {
            }

            /// <summary>
            /// Initializes a new instance of the CsvReader class.
            /// </summary>
            /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
            /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
            /// <param name="delimiter">The delimiter character separating each field (default is ',').</param>
            /// <param name="bufferSize">The buffer size in bytes.</param>
            /// <exception cref="T:ArgumentNullException">
            ///		<paramref name="reader"/> is a <see langword="null"/>.
            /// </exception>
            /// <exception cref="T:ArgumentException">
            ///		Cannot read from <paramref name="reader"/>.
            /// </exception>
            public CachedCsvReader(TextReader reader, bool hasHeaders, char delimiter, int bufferSize)
                : this(reader, hasHeaders, delimiter, DefaultQuote, DefaultEscape, DefaultComment, true, bufferSize)
            {
            }

            /// <summary>
            /// Initializes a new instance of the CsvReader class.
            /// </summary>
            /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
            /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
            /// <param name="delimiter">The delimiter character separating each field (default is ',').</param>
            /// <param name="quote">The quotation character wrapping every field (default is ''').</param>
            /// <param name="escape">
            /// The escape character letting insert quotation characters inside a quoted field (default is '\').
            /// If no escape character, set to '\0' to gain some performance.
            /// </param>
            /// <param name="comment">The comment character indicating that a line is commented out (default is '#').</param>
            /// <param name="trimSpaces"><see langword="true"/> if spaces at the start and end of a field are trimmed, otherwise, <see langword="false"/>. Default is <see langword="true"/>.</param>
            /// <exception cref="T:ArgumentNullException">
            ///		<paramref name="reader"/> is a <see langword="null"/>.
            /// </exception>
            /// <exception cref="T:ArgumentException">
            ///		Cannot read from <paramref name="reader"/>.
            /// </exception>
            public CachedCsvReader(TextReader reader, bool hasHeaders, char delimiter, char quote, char escape, char comment, bool trimSpaces)
                : this(reader, hasHeaders, delimiter, quote, escape, comment, trimSpaces, DefaultBufferSize)
            {
            }

            /// <summary>
            /// Initializes a new instance of the CsvReader class.
            /// </summary>
            /// <param name="reader">A <see cref="T:TextReader"/> pointing to the CSV file.</param>
            /// <param name="hasHeaders"><see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>.</param>
            /// <param name="delimiter">The delimiter character separating each field (default is ',').</param>
            /// <param name="quote">The quotation character wrapping every field (default is ''').</param>
            /// <param name="escape">
            /// The escape character letting insert quotation characters inside a quoted field (default is '\').
            /// If no escape character, set to '\0' to gain some performance.
            /// </param>
            /// <param name="comment">The comment character indicating that a line is commented out (default is '#').</param>
            /// <param name="trimSpaces"><see langword="true"/> if spaces at the start and end of a field are trimmed, otherwise, <see langword="false"/>. Default is <see langword="true"/>.</param>
            /// <param name="bufferSize">The buffer size in bytes.</param>
            /// <exception cref="T:ArgumentNullException">
            ///		<paramref name="reader"/> is a <see langword="null"/>.
            /// </exception>
            /// <exception cref="ArgumentOutOfRangeException">
            ///		<paramref name="bufferSize"/> must be 1 or more.
            /// </exception>
            public CachedCsvReader(TextReader reader, bool hasHeaders, char delimiter, char quote, char escape, char comment, bool trimSpaces, int bufferSize)
                : base(reader, hasHeaders, delimiter, quote, escape, comment, trimSpaces, bufferSize)
            {
                _records = new List<string[]>();
                _currentRecordIndex = -1;
            }

            /// <summary>
            /// Gets the current record index in the CSV file.
            /// </summary>
            /// <value>The current record index in the CSV file.</value>
            public override long CurrentRecordIndex
            {
                get
                {
                    return _currentRecordIndex;
                }
            }

            /// <summary>
            /// Gets a value that indicates whether the current stream position is at the end of the stream.
            /// </summary>
            /// <value><see langword="true"/> if the current stream position is at the end of the stream; otherwise <see langword="false"/>.</value>
            public override bool EndOfStream
            {
                get
                {
                    if (_currentRecordIndex < base.CurrentRecordIndex)
                        return false;
                    else
                        return base.EndOfStream;
                }
            }

            /// <summary>
            /// Gets the field at the specified index.
            /// </summary>
            /// <value>The field at the specified index.</value>
            /// <exception cref="T:ArgumentOutOfRangeException">
            ///		<paramref name="field"/> must be included in [0, <see cref="M:FieldCount"/>[.
            /// </exception>
            /// <exception cref="T:InvalidOperationException">
            ///		No record read yet. Call ReadLine() first.
            /// </exception>
            /// <exception cref="MissingFieldCsvException">
            ///		The CSV data appears to be missing a field.
            /// </exception>
            /// <exception cref="T:MalformedCsvException">
            ///		The CSV appears to be corrupt at the current position.
            /// </exception>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///		The instance has been disposed of.
            /// </exception>
            public override String this[int field]
            {
                get
                {
                    if (_readingStream)
                        return base[field];
                    else if (_currentRecordIndex > -1)
                    {
                        if (field > -1 && field < this.FieldCount)
                            return _records[(int)_currentRecordIndex][field];
                        else
                            throw new ArgumentOutOfRangeException("field", field, string.Format(CultureInfo.InvariantCulture, "Field index must be included in [0, FieldCount[. Specified field index was : '{0}'.", field));
                    }
                    else
                        throw new InvalidOperationException("No current record.");
                }
            }

            /// <summary>
            /// Reads the CSV stream from the current position to the end of the stream.
            /// </summary>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            public virtual void ReadToEnd()
            {
                _currentRecordIndex = base.CurrentRecordIndex;

                while (ReadNextRecord()) ;
            }

            /// <summary>
            /// Reads the next record.
            /// </summary>
            /// <param name="onlyReadHeaders">
            /// Indicates if the reader will proceed to the next record after having read headers.
            /// <see langword="true"/> if it stops after having read headers; otherwise, <see langword="false"/>.
            /// </param>
            /// <param name="skipToNextLine">
            /// Indicates if the reader will skip directly to the next line without parsing the current one. 
            /// To be used when an error occurs.
            /// </param>
            /// <returns><see langword="true"/> if a record has been successfully reads; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///	The instance has been disposed of.
            /// </exception>
            protected override bool ReadNextRecord(bool onlyReadHeaders, bool skipToNextLine)
            {
                if (_currentRecordIndex < base.CurrentRecordIndex)
                {
                    _currentRecordIndex++;
                    return true;
                }
                else
                {
                    _readingStream = true;

                    try
                    {
                        bool canRead = base.ReadNextRecord(onlyReadHeaders, skipToNextLine);

                        if (canRead)
                        {
                            string[] record = new string[this.FieldCount];

                            if (base.CurrentRecordIndex > -1)
                            {
                                CopyCurrentRecordTo(record);
                                _records.Add(record);
                            }
                            else
                            {
                                MoveTo(0);
                                CopyCurrentRecordTo(record);
                                MoveTo(-1);
                            }

                            if (!onlyReadHeaders)
                                _currentRecordIndex++;
                        }
                        else
                        {
                            // No more records to read, so set array size to only what is needed
                            _records.Capacity = _records.Count;
                        }

                        return canRead;
                    }
                    finally
                    {
                        _readingStream = false;
                    }
                }
            }

            /// <summary>
            /// Moves before the first record.
            /// </summary>
            public void MoveToStart()
            {
                _currentRecordIndex = -1;
            }

            /// <summary>
            /// Moves to the last record read so far.
            /// </summary>
            public void MoveToLastCachedRecord()
            {
                _currentRecordIndex = base.CurrentRecordIndex;
            }

            /// <summary>
            /// Moves to the specified record index.
            /// </summary>
            /// <param name="record">The record index.</param>
            /// <exception cref="T:ArgumentOutOfRangeException">
            ///		Record index must be > 0.
            /// </exception>
            /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
            ///		The instance has been disposed of.
            /// </exception>
            public override void MoveTo(long record)
            {
                if (record < -1)
                    throw new ArgumentOutOfRangeException("record", record, "Record index must be 0 or more.");

                if (record <= base.CurrentRecordIndex)
                    _currentRecordIndex = record;
                else
                {
                    _currentRecordIndex = base.CurrentRecordIndex;

                    long offset = record - _currentRecordIndex;

                    // read to the last record before the one we want
                    while (offset-- > 0 && ReadNextRecord()) ;
                }
            }

            bool IListSource.ContainsListCollection
            {
                get { return false; }
            }

            System.Collections.IList IListSource.GetList()
            {
                if (_bindingList == null)
                    _bindingList = new CsvBindingList(this);

                return _bindingList;
            }
        }

        public partial class CachedCsvReader
            : CsvReader
        {
            /// <summary>
            /// Represents a binding list wrapper for a CSV reader.
            /// </summary>
            private class CsvBindingList
                : IBindingList, ITypedList, IList<string[]>, IList
            {
                /// <summary>
                /// Contains the linked CSV reader.
                /// </summary>
                private CachedCsvReader _csv;

                /// <summary>
                /// Contains the cached record count.
                /// </summary>
                private int _count;

                /// <summary>
                /// Contains the cached property descriptors.
                /// </summary>
                private PropertyDescriptorCollection _properties;

                /// <summary>
                /// Contains the current sort property.
                /// </summary>
                private CsvPropertyDescriptor _sort;

                /// <summary>
                /// Contains the current sort direction.
                /// </summary>
                private ListSortDirection _direction;

                /// <summary>
                /// Initializes a new instance of the CsvBindingList class.
                /// </summary>
                /// <param name="csv"></param>
                public CsvBindingList(CachedCsvReader csv)
                {
                    _csv = csv;
                    _count = -1;
                    _direction = ListSortDirection.Ascending;
                }

                public void AddIndex(PropertyDescriptor property)
                {
                }

                public bool AllowNew
                {
                    get
                    {
                        return false;
                    }
                }

                public void ApplySort(PropertyDescriptor property, System.ComponentModel.ListSortDirection direction)
                {
                    _sort = (CsvPropertyDescriptor)property;
                    _direction = direction;

                    _csv.ReadToEnd();

                    _csv._records.Sort(new CsvRecordComparer(_sort.Index, _direction));
                }

                public PropertyDescriptor SortProperty
                {
                    get
                    {
                        return _sort;
                    }
                }

                public int Find(PropertyDescriptor property, object key)
                {
                    int fieldIndex = ((CsvPropertyDescriptor)property).Index;
                    string value = (string)key;

                    int recordIndex = 0;
                    int count = this.Count;

                    while (recordIndex < count && _csv[recordIndex, fieldIndex] != value)
                        recordIndex++;

                    if (recordIndex == count)
                        return -1;
                    else
                        return recordIndex;
                }

                public bool SupportsSorting
                {
                    get
                    {
                        return true;
                    }
                }

                public bool IsSorted
                {
                    get
                    {
                        return _sort != null;
                    }
                }

                public bool AllowRemove
                {
                    get
                    {
                        return false;
                    }
                }

                public bool SupportsSearching
                {
                    get
                    {
                        return true;
                    }
                }

                public System.ComponentModel.ListSortDirection SortDirection
                {
                    get
                    {
                        return _direction;
                    }
                }

                public event System.ComponentModel.ListChangedEventHandler ListChanged
                {
                    add { }
                    remove { }
                }

                public bool SupportsChangeNotification
                {
                    get
                    {
                        return false;
                    }
                }

                public void RemoveSort()
                {
                    _sort = null;
                    _direction = ListSortDirection.Ascending;
                }

                public object AddNew()
                {
                    throw new NotSupportedException();
                }

                public bool AllowEdit
                {
                    get
                    {
                        return false;
                    }
                }

                public void RemoveIndex(PropertyDescriptor property)
                {
                }

                public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
                {
                    if (_properties == null)
                    {
                        PropertyDescriptor[] properties = new PropertyDescriptor[_csv.FieldCount];

                        for (int i = 0; i < properties.Length; i++)
                            properties[i] = new CsvPropertyDescriptor(((System.Data.IDataReader)_csv).GetName(i), i);

                        _properties = new PropertyDescriptorCollection(properties);
                    }

                    return _properties;
                }

                public string GetListName(PropertyDescriptor[] listAccessors)
                {
                    return string.Empty;
                }

                public int IndexOf(string[] item)
                {
                    throw new NotSupportedException();
                }

                public void Insert(int index, string[] item)
                {
                    throw new NotSupportedException();
                }

                public void RemoveAt(int index)
                {
                    throw new NotSupportedException();
                }

                public string[] this[int index]
                {
                    get
                    {
                        _csv.MoveTo(index);
                        return _csv._records[index];
                    }
                    set
                    {
                        throw new NotSupportedException();
                    }
                }

                public void Add(string[] item)
                {
                    throw new NotSupportedException();
                }

                public void Clear()
                {
                    throw new NotSupportedException();
                }

                public bool Contains(string[] item)
                {
                    throw new NotSupportedException();
                }

                public void CopyTo(string[][] array, int arrayIndex)
                {
                    _csv.MoveToStart();

                    while (_csv.ReadNextRecord())
                        _csv.CopyCurrentRecordTo(array[arrayIndex++]);
                }

                public int Count
                {
                    get
                    {
                        if (_count < 0)
                        {
                            _csv.ReadToEnd();
                            _count = (int)_csv.CurrentRecordIndex + 1;
                        }

                        return _count;
                    }
                }

                public bool IsReadOnly
                {
                    get { return true; }
                }

                public bool Remove(string[] item)
                {
                    throw new NotSupportedException();
                }

                public IEnumerator<string[]> GetEnumerator()
                {
                    return _csv.GetEnumerator();
                }

                public int Add(object value)
                {
                    throw new NotSupportedException();
                }

                public bool Contains(object value)
                {
                    throw new NotSupportedException();
                }

                public int IndexOf(object value)
                {
                    throw new NotSupportedException();
                }

                public void Insert(int index, object value)
                {
                    throw new NotSupportedException();
                }

                public bool IsFixedSize
                {
                    get { return true; }
                }

                public void Remove(object value)
                {
                    throw new NotSupportedException();
                }

                object IList.this[int index]
                {
                    get
                    {
                        return this[index];
                    }
                    set
                    {
                        throw new NotSupportedException();
                    }
                }

                public void CopyTo(Array array, int index)
                {
                    _csv.MoveToStart();

                    while (_csv.ReadNextRecord())
                        _csv.CopyCurrentRecordTo((string[])array.GetValue(index++));
                }

                public bool IsSynchronized
                {
                    get { return false; }
                }

                public object SyncRoot
                {
                    get { return null; }
                }

                System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }
            }
        }

        /// <summary>
        /// Represents the exception that is thrown when a there is a missing field in a record of the CSV file.
        /// </summary>
        /// <remarks>
        /// MissingFieldException would have been a better name, but there is already a <see cref="T:System.MissingFieldException"/>.
        /// </remarks>
        [Serializable()]
        public class MissingFieldCsvException
            : MalformedCsvException
        {

            /// <summary>
            /// Initializes a new instance of the MissingFieldCsvException class.
            /// </summary>
            public MissingFieldCsvException()
                : base()
            {
            }

            /// <summary>
            /// Initializes a new instance of the MissingFieldCsvException class.
            /// </summary>
            /// <param name="message">The message that describes the error.</param>
            public MissingFieldCsvException(string message)
                : base(message)
            {
            }

            /// <summary>
            /// Initializes a new instance of the MissingFieldCsvException class.
            /// </summary>
            /// <param name="message">The message that describes the error.</param>
            /// <param name="innerException">The exception that is the cause of the current exception.</param>
            public MissingFieldCsvException(string message, Exception innerException)
                : base(message, innerException)
            {
            }

            /// <summary>
            /// Initializes a new instance of the MissingFieldCsvException class.
            /// </summary>
            /// <param name="rawData">The raw data when the error occured.</param>
            /// <param name="currentPosition">The current position in the raw data.</param>
            /// <param name="currentRecordIndex">The current record index.</param>
            /// <param name="currentFieldIndex">The current field index.</param>
            public MissingFieldCsvException(string rawData, int currentPosition, long currentRecordIndex, int currentFieldIndex)
                : base(rawData, currentPosition, currentRecordIndex, currentFieldIndex)
            {
            }

            /// <summary>
            /// Initializes a new instance of the MissingFieldCsvException class.
            /// </summary>
            /// <param name="rawData">The raw data when the error occured.</param>
            /// <param name="currentPosition">The current position in the raw data.</param>
            /// <param name="currentRecordIndex">The current record index.</param>
            /// <param name="currentFieldIndex">The current field index.</param>
            /// <param name="innerException">The exception that is the cause of the current exception.</param>
            public MissingFieldCsvException(string rawData, int currentPosition, long currentRecordIndex, int currentFieldIndex, Exception innerException)
                : base(rawData, currentPosition, currentRecordIndex, currentFieldIndex, innerException)
            {
            }

            /// <summary>
            /// Initializes a new instance of the MissingFieldCsvException class with serialized data.
            /// </summary>
            /// <param name="info">The <see cref="T:SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
            /// <param name="context">The <see cref="T:StreamingContext"/> that contains contextual information about the source or destination.</param>
            protected MissingFieldCsvException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
        }

        /// <summary>
        /// Represents the exception that is thrown when a CSV file is malformed.
        /// </summary>
        [Serializable()]
        public class MalformedCsvException
            : Exception
        {
            /// <summary>
            /// Contains the message that describes the error.
            /// </summary>
            private string _message;

            /// <summary>
            /// Contains the raw data when the error occured.
            /// </summary>
            private string _rawData;

            /// <summary>
            /// Contains the current field index.
            /// </summary>
            private int _currentFieldIndex;

            /// <summary>
            /// Contains the current record index.
            /// </summary>
            private long _currentRecordIndex;

            /// <summary>
            /// Contains the current position in the raw data.
            /// </summary>
            private int _currentPosition;

            /// <summary>
            /// Initializes a new instance of the MalformedCsvException class.
            /// </summary>
            public MalformedCsvException()
                : this(null, null)
            {
            }

            /// <summary>
            /// Initializes a new instance of the MalformedCsvException class.
            /// </summary>
            /// <param name="message">The message that describes the error.</param>
            public MalformedCsvException(string message)
                : this(message, null)
            {
            }

            /// <summary>
            /// Initializes a new instance of the MalformedCsvException class.
            /// </summary>
            /// <param name="message">The message that describes the error.</param>
            /// <param name="innerException">The exception that is the cause of the current exception.</param>
            public MalformedCsvException(string message, Exception innerException)
                : base(String.Empty, innerException)
            {
                _message = (message == null ? string.Empty : message);

                _rawData = string.Empty;
                _currentPosition = -1;
                _currentRecordIndex = -1;
                _currentFieldIndex = -1;
            }

            /// <summary>
            /// Initializes a new instance of the MalformedCsvException class.
            /// </summary>
            /// <param name="rawData">The raw data when the error occured.</param>
            /// <param name="currentPosition">The current position in the raw data.</param>
            /// <param name="currentRecordIndex">The current record index.</param>
            /// <param name="currentFieldIndex">The current field index.</param>
            public MalformedCsvException(string rawData, int currentPosition, long currentRecordIndex, int currentFieldIndex)
                : this(rawData, currentPosition, currentRecordIndex, currentFieldIndex, null)
            {
            }

            /// <summary>
            /// Initializes a new instance of the MalformedCsvException class.
            /// </summary>
            /// <param name="rawData">The raw data when the error occured.</param>
            /// <param name="currentPosition">The current position in the raw data.</param>
            /// <param name="currentRecordIndex">The current record index.</param>
            /// <param name="currentFieldIndex">The current field index.</param>
            /// <param name="innerException">The exception that is the cause of the current exception.</param>
            public MalformedCsvException(string rawData, int currentPosition, long currentRecordIndex, int currentFieldIndex, Exception innerException)
                : base(String.Empty, innerException)
            {
                _rawData = (rawData == null ? string.Empty : rawData);
                _currentPosition = currentPosition;
                _currentRecordIndex = currentRecordIndex;
                _currentFieldIndex = currentFieldIndex;

                _message = String.Format(CultureInfo.InvariantCulture, "The CSV appears to be corrupt near record '{0}' field '{1} at position '{2}'. Current raw data : '{3}'.", _currentRecordIndex, _currentFieldIndex, _currentPosition, _rawData);
            }

            /// <summary>
            /// Initializes a new instance of the MalformedCsvException class with serialized data.
            /// </summary>
            /// <param name="info">The <see cref="T:SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
            /// <param name="context">The <see cref="T:StreamingContext"/> that contains contextual information about the source or destination.</param>
            protected MalformedCsvException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                _message = info.GetString("MyMessage");

                _rawData = info.GetString("RawData");
                _currentPosition = info.GetInt32("CurrentPosition");
                _currentRecordIndex = info.GetInt64("CurrentRecordIndex");
                _currentFieldIndex = info.GetInt32("CurrentFieldIndex");
            }

            /// <summary>
            /// Gets the raw data when the error occured.
            /// </summary>
            /// <value>The raw data when the error occured.</value>
            public string RawData
            {
                get { return _rawData; }
            }

            /// <summary>
            /// Gets the current position in the raw data.
            /// </summary>
            /// <value>The current position in the raw data.</value>
            public int CurrentPosition
            {
                get { return _currentPosition; }
            }

            /// <summary>
            /// Gets the current record index.
            /// </summary>
            /// <value>The current record index.</value>
            public long CurrentRecordIndex
            {
                get { return _currentRecordIndex; }
            }

            /// <summary>
            /// Gets the current field index.
            /// </summary>
            /// <value>The current record index.</value>
            public int CurrentFieldIndex
            {
                get { return _currentFieldIndex; }
            }

            /// <summary>
            /// Gets a message that describes the current exception.
            /// </summary>
            /// <value>A message that describes the current exception.</value>
            public override string Message
            {
                get { return _message; }
            }

            /// <summary>
            /// When overridden in a derived class, sets the <see cref="T:SerializationInfo"/> with information about the exception.
            /// </summary>
            /// <param name="info">The <see cref="T:SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
            /// <param name="context">The <see cref="T:StreamingContext"/> that contains contextual information about the source or destination.</param>
            public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            {
                base.GetObjectData(info, context);

                info.AddValue("MyMessage", _message);

                info.AddValue("RawData", _rawData);
                info.AddValue("CurrentPosition", _currentPosition);
                info.AddValue("CurrentRecordIndex", _currentRecordIndex);
                info.AddValue("CurrentFieldIndex", _currentFieldIndex);
            }
        }

        /// <summary>
        /// Provides data for the <see cref="M:CsvReader.ParseError"/> event.
        /// </summary>
        public class ParseErrorEventArgs
            : EventArgs
        {
            /// <summary>
            /// Contains the error that occured.
            /// </summary>
            private MalformedCsvException _error;

            /// <summary>
            /// Contains the action to take.
            /// </summary>
            private ParseErrorAction _action;

            /// <summary>
            /// Initializes a new instance of the ParseErrorEventArgs class.
            /// </summary>
            /// <param name="error">The error that occured.</param>
            /// <param name="defaultAction">The default action to take.</param>
            public ParseErrorEventArgs(MalformedCsvException error, ParseErrorAction defaultAction)
                : base()
            {
                _error = error;
                _action = defaultAction;
            }

            /// <summary>
            /// Gets the error that occured.
            /// </summary>
            /// <value>The error that occured.</value>
            public MalformedCsvException Error
            {
                get { return _error; }
            }

            /// <summary>
            /// Gets or sets the action to take.
            /// </summary>
            /// <value>The action to take.</value>
            public ParseErrorAction Action
            {
                get { return _action; }
                set { _action = value; }
            }
        }

        #endregion

    }
}
