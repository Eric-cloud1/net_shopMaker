using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace MakerShop.DataClient.Api
{
    public class ZipUtility
    {

        /// <summary>
        /// Will compress the data using memory stream.
        /// </summary>
        /// <param name="bytes">data to be compressed</param>
        /// <returns>compressed data, if an error occurs while compressing will return null.</returns>
        public static byte[] Compress(byte[] bytes)
        {
            MemoryStream memory = null;
            DeflaterOutputStream stream = null;
            byte[] compressedData = null;

            try
            {
                memory = new MemoryStream();
                stream =
                    new DeflaterOutputStream(memory, new Deflater(Deflater.BEST_COMPRESSION), 131072);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                stream.Finish();
                stream.Close();

                compressedData = memory.ToArray();
            }
            catch { /* IGNORE ERROR */ }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream = null;
                }
                if (memory != null)
                {                    
                    memory.Close();
                    memory = null;
                }
            }
            return compressedData;
        }

        /// <summary>
        /// Will decompress the compressed data using memory stream.
        /// </summary>
        /// <param name="Bytes">compressed data</param>
        /// <returns>decompressed data, if an error occurs while decompressing will return null.</returns>
        public static byte[] Decompress(byte[] Bytes)
        {
            InflaterInputStream stream = null;
            MemoryStream memory = null;

            byte[] deCompressedData = null;

            try
            {
                stream =
                    new InflaterInputStream(new MemoryStream(Bytes));
                memory = new MemoryStream();
                byte[] writeData = new byte[4096];
                int size;

                while (true)
                {
                    size = stream.Read(writeData, 0, writeData.Length);
                    if (size > 0)
                    {
                        memory.Write(writeData, 0, size);
                    }
                    else break;
                }
                deCompressedData = memory.ToArray();
            }
            catch { /* IGNORE ERROR */ }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream = null;
                }
                if (memory != null)
                {                    
                    memory.Flush();
                    memory.Close();
                    memory = null;
                }
            }
            return deCompressedData;
        }

        /// <summary>
        /// Will decompress the compressed string data
        /// </summary>
        /// <param name="compressedBytes">compressed string data</param>
        /// <returns>the compressed string</returns>
        public static String DecompressToString(byte[] compressedBytes)
        {
            byte[] decompressedBytes = Decompress(compressedBytes);
            return new UTF8Encoding().GetString(decompressedBytes, 0, decompressedBytes.Length);            
        }

        /// <summary>
        /// Will compress the string data
        /// </summary>
        /// <param name="contents">contents to be compressed</param>
        /// <returns>compressed data, will return null if an error occurs while compressing.</returns>
        public static byte[] CompressStringData(string contents)
        {
            return  Compress(new UTF8Encoding().GetBytes(contents));
        }

        //public static String getDataFromZipBytes(byte[] byteFile)
        //{
        //    Stream stream = new MemoryStream(byteFile);
        //    StringBuilder fileContent = new StringBuilder();
        //    ZipInputStream zipInputStream = null;
        //    try
        //    {
        //        zipInputStream = new ZipInputStream(stream);
        //        ZipEntry firstEntry;
        //        if ((firstEntry = zipInputStream.GetNextEntry()) != null)
        //        {
        //            int size = 2048;
        //            byte[] data = new byte[2048];
        //            while (true)
        //            {
        //                size = zipInputStream.Read(data, 0, data.Length);
        //                if (size > 0)
        //                {
        //                    fileContent.Append(new ASCIIEncoding().GetString(data, 0, size));
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //TODO:Exception
        //        fileContent.AppendLine("Exception at unzip");
        //        fileContent.Append(ex.Message);
        //    }
        //    finally
        //    {
        //        if (stream != null)
        //        {
        //            stream.Flush();
        //            stream.Close();
        //            stream = null;
        //        }
        //        if (zipInputStream != null)
        //        {
        //            zipInputStream.Flush();
        //            zipInputStream.Close();
        //            zipInputStream = null;
        //        }
        //    }
        //    return fileContent.ToString();
        //}

        ///// <summary>
        ///// This method is used to zip from string contents and read back to bytes
        ///// </summary>
        ///// <param name="file"></param>
        ///// <returns></returns>        
        //public static byte[] ZipAndGetBytesFromString(string contents)
        //{
        //    String outputFilePath = AppUtility.GetAppRootPath() + "\\Data\\" + Guid.NewGuid() + ".txt";
        //    bool isFileSaved = false;
        //    try
        //    {
        //        TextWriter objTextWriter = new StreamWriter(outputFilePath, false);
        //        objTextWriter.Write(contents);
        //        objTextWriter.Close();
        //        isFileSaved = true;
        //    }
        //    catch
        //    {
        //        isFileSaved = false;
        //    }

        //    if (isFileSaved)
        //    {
        //        byte[] data = zipAndGetBytes(outputFilePath);
        //        // DELETE THE TEMPORARY TEXT FILE
        //        try
        //        {
        //            if (File.Exists(outputFilePath)) File.Delete(outputFilePath);
        //        }
        //        catch { }
        //        return data;
        //    }
        //    else return null;
        //}

    }
}
