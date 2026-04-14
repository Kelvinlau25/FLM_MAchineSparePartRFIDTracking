using System.Data;

namespace Library.Database
{
    /// <summary>
    /// Passing Data between Library and Interface
    /// </summary>
    public class DTO
    {
        private DataTable _table1;
        private bool _error1;
        private string _errorString1;
        private string _str1;
        private int _int1;

        public DataTable Table
        {
            get { return _table1; }
            set { _table1 = value; }
        }

        /// <summary>
        /// Set Error = true if hit error
        /// </summary>
        public bool Error
        {
            get { return _error1; }
            set { _error1 = value; }
        }

        /// <summary>
        /// Pass Error Message
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorString1; }
            set { _errorString1 = value; }
        }

        public string Str
        {
            get { return _str1; }
            set { _str1 = value; }
        }

        public int Int
        {
            get { return _int1; }
            set { _int1 = value; }
        }
    }
}
