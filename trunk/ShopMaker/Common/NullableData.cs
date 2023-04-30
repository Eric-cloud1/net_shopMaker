using System;
using System.Data;
using MakerShop.Utility;

namespace MakerShop.Common
{
    /// <summary>
    /// Utility class for handling nullable data
    /// </summary>
    public class NullableData
    {
        private NullableData() { }

        /// <summary>
        /// If the given Guid is empty returns Db null object
        /// </summary>
        /// <param name="value">The guid object to check</param>
        /// <returns>If the given Guid is empty returns db null object</returns>
        public static object DbNullify(Guid value)
        {
            if (value == Guid.Empty) return DBNull.Value;
            return value;
        }

        /// <summary>
        /// If the given string is null or empty returns db null object
        /// </summary>
        /// <param name="value">The input string to check</param>
        /// <returns>If the given string is null or empty returns db null object</returns>
        public static object DbNullify(String value)
        {
            if (string.IsNullOrEmpty(value)) return DBNull.Value;
            return value;
        }

        /// <summary>
        /// If the given DateTime value is DateTime.MinValue or DateTime.MaxValue returns the db null object
        /// </summary>
        /// <param name="value"></param>
        /// <returns>If the given DateTime value is DateTime.MinValue or DateTime.MaxValue returns the db null object</returns>
        public static object DbNullify(DateTime value)
        {
            if ((value == DateTime.MinValue) || (value == DateTime.MaxValue)) return DBNull.Value;
            return value;
        }

        /// <summary>
        /// Returns the same byte value
        /// </summary>
        /// <param name="value">input byte value</param>
        /// <returns>The same byte value</returns>
        public static object DbNullify(Byte value)
        {
            //if (value == null) return DBNull.Value;
            return value;
        }

        /// <summary>
        /// If the given input byte array is null returns the db null object
        /// </summary>
        /// <param name="value">Input value</param>
        /// <returns>If the given input byte array is null returns the db null object</returns>
        public static object DbNullify(byte[] value)
        {
            if (value == null || value.Length == 0) return DBNull.Value;
            return value;
        }

        /// <summary>
        /// Returns the same value
        /// </summary>
        /// <param name="value">Input value</param>
        /// <returns>Returns the same value</returns>
        public static object DbNullify(Int16 value)
        {
            //if (value == null) return DBNull.Value;
            return value;
        }
 
        /// <summary>
        /// If input value is 0 returns db null object
        /// </summary>
        /// <param name="value">Input value</param>
        /// <returns>If input value is 0 returns db null object</returns>
        public static object DbNullify(Int32 value)
        {
            if (value == 0) return DBNull.Value;
            return value;
        }

        /// <summary>
        /// If input value is 0 returns db null object
        /// </summary>
        /// <param name="value">Input value</param>
        /// <returns>If input value is 0 returns db null object</returns>
        public static object DbNullify(Decimal value)
        {
            if (value == 0) return DBNull.Value;
            return value;
        }

        /// <summary>
        /// If input value is 0 returns db null object
        /// </summary>
        /// <param name="value">Input value</param>
        /// <returns>If input value is 0 returns db null object</returns>
        public static object DbNullify(LSDecimal value)
        {
            if (value == 0) return DBNull.Value;
            return value;
        }

        /// <summary>
        /// Returns the same value
        /// </summary>
        /// <param name="value">Input value</param>
        /// <returns>Returns the same value</returns>
        public static object DbNullify(bool value)
        {
            //if (value == null) return DBNull.Value;
            return value;
        }

        /// <summary>
        /// If the string is empty or null null is returned otherwise the same string is returned
        /// </summary>
        /// <param name="value">Input value</param>
        /// <returns>If the string is empty or null null is returned otherwise the same string is returned</returns>
        public static string NullifyEmptyString(string value)
        {
            return (string.IsNullOrEmpty(value)) ? null : value;
        }

        /// <summary>
        /// If the input date is DateTime.MinValue db null object is returned otherwise the same date is returned
        /// </summary>
        /// <param name="value">Input value</param>
        /// <returns>If the input date is DateTime.MinValue db null object is returned otherwise the same date is returned</returns>
        public static object NullifyEmptyDate(DateTime value)
        {
            if (value == System.DateTime.MinValue)
            {
                return DBNull.Value;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Reads a string field at the given index from the IDataRecord data reader
        /// </summary>
        /// <param name="reader">IDataRecord data reader</param>
        /// <param name="fieldIndex">Index of the field to read</param>
        /// <returns>String value read at the given index</returns>
        public static string GetString(IDataRecord reader, int fieldIndex)
        {
            return GetString(reader, fieldIndex, string.Empty);
        }

        /// <summary>
        /// Reads a string field at the given index from the IDataRecord data reader. If the read value is null returns the default value.
        /// </summary>
        /// <param name="reader">IDataRecord data reader</param>
        /// <param name="fieldIndex">Index of the field to read</param>
        /// <param name="defaultValue">The default value to use</param>
        /// <returns>String value read at the given index or default value if the value read from reader was null</returns>
        public static string GetString(IDataRecord reader, int fieldIndex, string defaultValue)
        {
            if (!reader.IsDBNull(fieldIndex)) return reader.GetString(fieldIndex);
            return defaultValue;
        }

        /// <summary>
        /// Reads a Int32 field at the given index from the IDataRecord data reader.
        /// </summary>
        /// <param name="reader">IDataRecord data reader</param>
        /// <param name="fieldIndex">Index of the field to read</param>
        /// <returns>Int32 value read at the given index</returns>
        public static int GetInt32(IDataRecord reader, int fieldIndex)
        {
            return GetInt32(reader, fieldIndex, 0);
        }

        /// <summary>
        /// Reads a Int32 field at the given index from the IDataRecord data reader. If the read value is null returns the default value.
        /// </summary>
        /// <param name="reader">IDataRecord data reader</param>
        /// <param name="fieldIndex">Index of the field to read</param>
        /// <param name="defaultValue">The default value to use</param>
        /// <returns>Int32 value read at the given index or default value if the value read from reader was null</returns>
        public static int GetInt32(IDataRecord reader, int fieldIndex, int defaultValue)
        {
            if (!reader.IsDBNull(fieldIndex)) return reader.GetInt32(fieldIndex);
            return defaultValue;
        }

        /// <summary>
        /// Reads a DateTime field at the given index from the IDataRecord data reader. 
        /// </summary>
        /// <param name="reader">IDataRecord data reader</param>
        /// <param name="fieldIndex">Index of the field to read</param>
        /// <returns>DateTime value read at the given index</returns>
        public static DateTime GetDateTime(IDataRecord reader, int fieldIndex)
        {
            return GetDateTime(reader, fieldIndex, DateTime.MinValue);
        }

        /// <summary>
        /// Reads a DateTime field at the given index from the IDataRecord data reader. If the read value is null returns the default value.
        /// </summary>
        /// <param name="reader">IDataRecord data reader</param>
        /// <param name="fieldIndex">Index of the field to read</param>
        /// <param name="defaultValue">The default value to use</param>
        /// <returns>DateTime value read at the given index or default value if the value read from reader was null</returns>
        public static DateTime GetDateTime(IDataRecord reader, int fieldIndex, DateTime defaultValue)
        {
            if (!reader.IsDBNull(fieldIndex))
            {
                return reader.GetDateTime(fieldIndex);
            }
            return defaultValue;
        }

        /// <summary>
        /// Reads a Byte field at the given index from the IDataRecord data reader.
        /// </summary>
        /// <param name="reader">IDataRecord data reader</param>
        /// <param name="fieldIndex">Index of the field to read</param>
        /// <returns>Byte value read at the given index</returns>
        public static Byte GetByte(IDataRecord reader, int fieldIndex)
        {
            return GetByte(reader, fieldIndex, (Byte)0);
        }

        /// <summary>
        /// Reads a Byte field at the given index from the IDataRecord data reader. If the read value is null returns the default value.
        /// </summary>
        /// <param name="reader">IDataRecord data reader</param>
        /// <param name="fieldIndex">Index of the field to read</param>
        /// <param name="defaultValue">The default value to use</param>
        /// <returns>Byte value read at the given index or default value if the value read from reader was null</returns>
        public static Byte GetByte(IDataRecord reader, int fieldIndex, Byte defaultValue)
        {
            if (!reader.IsDBNull(fieldIndex)) return reader.GetByte(fieldIndex);
            return defaultValue;
        }

        /// <summary>
        /// Reads a Guid field at the given index from the IDataRecord data reader. 
        /// </summary>
        /// <param name="reader">IDataRecord data reader</param>
        /// <param name="fieldIndex">Index of the field to read</param>
        /// <returns>Guid value read at the given index</returns>
        public static Guid GetGuid(IDataRecord reader, int fieldIndex)
        {
            return GetGuid(reader, fieldIndex, Guid.Empty);
        }

        /// <summary>
        /// Reads a Guid field at the given index from the IDataRecord data reader. If the read value is null returns the default value.
        /// </summary>
        /// <param name="reader">IDataRecord data reader</param>
        /// <param name="fieldIndex">Index of the field to read</param>
        /// <param name="defaultValue">The default value to use</param>
        /// <returns>Guid value read at the given index or default value if the value read from reader was null</returns>
        public static Guid GetGuid(IDataRecord reader, int fieldIndex, Guid defaultValue)
        {
            if (!reader.IsDBNull(fieldIndex)) return reader.GetGuid(fieldIndex);
            return defaultValue;
        }

        /// <summary>
        /// Reads a Boolean field at the given index from the IDataRecord data reader.
        /// </summary>
        /// <param name="reader">IDataRecord data reader</param>
        /// <param name="fieldIndex">Index of the field to read</param>
        /// <returns>Boolean value read at the given index</returns>
        public static Boolean GetBoolean(IDataRecord reader, int fieldIndex)
        {
            return GetBoolean(reader, fieldIndex, false);
        }

        /// <summary>
        /// Reads a Boolean field at the given index from the IDataRecord data reader. If the read value is null returns the default value.
        /// </summary>
        /// <param name="reader">IDataRecord data reader</param>
        /// <param name="fieldIndex">Index of the field to read</param>
        /// <param name="defaultValue">The default value to use</param>
        /// <returns>Boolean value read at the given index or default value if the value read from reader was null</returns>
        public static Boolean GetBoolean(IDataRecord reader, int fieldIndex, Boolean defaultValue)
        {
            if (!reader.IsDBNull(fieldIndex)) return reader.GetBoolean(fieldIndex);
            return defaultValue;
        }

        /// <summary>
        /// Reads a Decimal field at the given index from the IDataRecord data reader.
        /// </summary>
        /// <param name="reader">IDataRecord data reader</param>
        /// <param name="fieldIndex">Index of the field to read</param>
        /// <returns>Decimal value read at the given index</returns>
        public static Decimal GetDecimal(IDataRecord reader, int fieldIndex)
        {
            return GetDecimal(reader, fieldIndex, (Decimal)0);
        }

        /// <summary>
        /// Reads a Decimal field at the given index from the IDataRecord data reader. If the read value is null returns the default value.
        /// </summary>
        /// <param name="reader">IDataRecord data reader</param>
        /// <param name="fieldIndex">Index of the field to read</param>
        /// <param name="defaultValue">The default value to use</param>
        /// <returns>Decimal value read at the given index or default value if the value read from reader was null</returns>
        public static Decimal GetDecimal(IDataRecord reader, int fieldIndex, Decimal defaultValue)
        {
            if (!reader.IsDBNull(fieldIndex)) return reader.GetDecimal(fieldIndex);
            return defaultValue;
        }

        /// <summary>
        /// Reads a Int16 field at the given index from the IDataRecord data reader.
        /// </summary>
        /// <param name="reader">IDataRecord data reader</param>
        /// <param name="fieldIndex">Index of the field to read</param>
        /// <returns>Int16 value read at the given index</returns>
        public static Int16 GetInt16(IDataRecord reader, int fieldIndex)
        {
            return GetInt16(reader, fieldIndex, (Int16)0);
        }

        /// <summary>
        /// Reads a Int6 field at the given index from the IDataRecord data reader. If the read value is null returns the default value.
        /// </summary>
        /// <param name="reader">IDataRecord data reader</param>
        /// <param name="fieldIndex">Index of the field to read</param>
        /// <param name="defaultValue">The default value to use</param>
        /// <returns>Int16 value read at the given index or default value if the value read from reader was null</returns>
        public static Int16 GetInt16(IDataRecord reader, int fieldIndex, Int16 defaultValue)
        {
            if (!reader.IsDBNull(fieldIndex)) return reader.GetInt16(fieldIndex);
            return defaultValue;
        }

        /// <summary>
        /// Reads a Byte array field at the given index from the IDataRecord data reader.
        /// </summary>
        /// <param name="reader">IDataRecord data reader</param>
        /// <param name="fieldIndex">Index of the field to read</param>
        /// <returns>Array of Bytes read at the given index</returns>
        public static Byte[] GetBytes(IDataRecord reader, int fieldIndex)
        {
            if (reader.IsDBNull(fieldIndex)) return null;
            int bufferSize = (int)reader.GetBytes(fieldIndex, (long)0, null, 0, 0);
            if (bufferSize == 0) return null;
            byte[] buffer = new byte[bufferSize];
            reader.GetBytes(fieldIndex, (long)0, buffer, 0, bufferSize);
            return buffer;
        }

    }
}
