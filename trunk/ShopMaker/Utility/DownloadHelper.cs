using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using MakerShop.DigitalDelivery;

namespace MakerShop.Utility
{
    /// <summary>
    /// Helper class to support resumable downloads
    /// </summary>
    public static class DownloadHelper
    {
        #region "Constants used for HTTP communication"

        // The boundary is used in multipart/byteranges responses
        // to separate each ranges content. It should be as unique
        // as possible to avoid confusion with binary content.
        private const string MULTIPART_BOUNDARY = "<q1w2e3r4t5y6u7i8o9p0>";
        private const string MULTIPART_CONTENTTYPE = "multipart/byteranges; boundary=" + MULTIPART_BOUNDARY;

        private const string HTTP_HEADER_ACCEPT_RANGES = "Accept-Ranges";
        private const string HTTP_HEADER_ACCEPT_RANGES_BYTES = "bytes";
        private const string HTTP_HEADER_CONTENT_TYPE = "Content-Type";
        private const string HTTP_HEADER_CONTENT_RANGE = "Content-Range";
        private const string HTTP_HEADER_CONTENT_LENGTH = "Content-Length";
        private const string HTTP_HEADER_CONTENT_DISPOSITION = "Content-Disposition";
        private const string HTTP_HEADER_ENTITY_TAG = "ETag";
        private const string HTTP_HEADER_LAST_MODIFIED = "Last-Modified";
        private const string HTTP_HEADER_RANGE = "Range";
        private const string HTTP_HEADER_IF_RANGE = "If-Range";
        private const string HTTP_HEADER_IF_MATCH = "If-Match";
        private const string HTTP_HEADER_IF_NONE_MATCH = "If-None-Match";
        private const string HTTP_HEADER_IF_MODIFIED_SINCE = "If-Modified-Since";
        private const string HTTP_HEADER_IF_UNMODIFIED_SINCE = "If-Unmodified-Since";
        private const string HTTP_HEADER_UNLESS_MODIFIED_SINCE = "Unless-Modified-Since";

        private const string HTTP_METHOD_GET = "GET";
        private const string HTTP_METHOD_HEAD = "HEAD";

        private const string DEFAULT_CONTENT_TYPE = "application/octet-stream";
        private const string DEFAULT_ENTITY_TAG = "00abcd5fga11d90:0000";

        #endregion

        /// <summary>
        /// Sends the string file data to http client
        /// </summary>
        /// <param name="fileData">The data to send</param>
        /// <param name="fileName">Name of the file to send to the client</param>
        public static void SendFileDataToClient(string fileData, string fileName)
        {
            if (!string.IsNullOrEmpty(fileData))
            {
                HttpResponse Response = HttpContext.Current.Response;
                Response.Clear();                
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
                Response.ContentType = DEFAULT_CONTENT_TYPE;
                Response.AddHeader(HTTP_HEADER_CONTENT_LENGTH, fileData.Length.ToString());
                Response.AddHeader(HTTP_HEADER_CONTENT_DISPOSITION, "attachment; filename=" + fileName);
                System.Text.Encoding enc = System.Text.Encoding.UTF8;
                byte[] fileBytes = enc.GetBytes(fileData);
                Response.OutputStream.Write(fileBytes, 0, fileBytes.Length);
                Response.Flush();
                Response.Close();
            }
        }

        /// <summary>
        /// Sends file data for the given digital good to the http client.
        /// </summary>
        /// <param name="context">Http context to get the request details and the response object for sending the data</param>
        /// <param name="digitalGood">The digital good of which to send the data</param>
        public static void SendFileDataToClient(HttpContext context, DigitalGood digitalGood)
        {
            if (digitalGood == null) throw new ArgumentNullException("digitalGood");
            SendFileDataToClient(context, digitalGood.AbsoluteFilePath, digitalGood.FileName);
        }

        /// <summary>
        /// Sends file data for the file to the http client.
        /// </summary>
        /// <param name="context">Http context to get the request details and the response object for sending the data</param>
        /// <param name="filePath">The absolute path to the file to send to the client</param>
        /// <param name="saveAsName">The file name to identify with the file data when sending to the client</param>
        public static void SendFileDataToClient(HttpContext context, string filePath, string saveAsName)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException("filePath");

            HttpResponse Response = context.Response;
            HttpRequest Request = context.Request;

            Response.Clear();
            Response.AddHeader("Cache-Control", "private");
            Response.AddHeader("Pragma", "cache");
            Response.AddHeader("Expires", "0");

            FileInfo fi = new FileInfo(filePath);

            RequestStatus status = ValidateRequest(context, fi);
            if (status.IsValid)
            {
                RangeInfo rangeInfo = status.RangeInfo;
                //proceed to data output
                SetupHeaders(context, rangeInfo, fi, saveAsName);

                if (Request.HttpMethod.Equals(HTTP_METHOD_HEAD))
                {
                    //Only the HEAD was requested, so we can quit the Response right here...             
                    Response.Close();
                    return;
                }

                // Flush the HEAD information to the client...
                Response.Flush();
                int chunkSize = 10240;
                using (FileStream fs = fi.OpenRead())
                {
                    bool downloadBroken = false;
                    foreach (Range range in rangeInfo.Ranges)
                    {
                        //Move the stream to the desired start position...
                        fs.Seek(range.begin, System.IO.SeekOrigin.Begin);

                        // Calculate the total amount of bytes for this range
                        Int32 bytesToRead = Convert.ToInt32(range.end - range.begin) + 1;

                        if (rangeInfo.IsMultiPartRequest)
                        {
                            // If this is a multipart response, we must add 
                            // certain headers before streaming the content:
                            // The multipart boundary
                            Response.Output.WriteLine("--" + MULTIPART_BOUNDARY);
                            //The mime type of this part of the content 
                            Response.Output.WriteLine(HTTP_HEADER_CONTENT_TYPE + ": " + DEFAULT_CONTENT_TYPE);
                            // The actual range
                            Response.Output.WriteLine(HTTP_HEADER_CONTENT_RANGE + ": bytes " + range.begin.ToString() + "-"
                                + range.end.ToString() + "/" + fi.Length.ToString());

                            //Indicating the end of the intermediate headers
                            Response.Output.WriteLine();
                        }

                        // Now stream the range to the client...
                        byte[] buffer;
                        int bytesRead;
                        while (bytesToRead > 0)
                        {
                            if (Response.IsClientConnected)
                            {
                                buffer = new byte[chunkSize];
                                bytesRead = fs.Read(buffer, 0, Math.Min(chunkSize, bytesToRead));
                                Response.OutputStream.Write(buffer, 0, bytesRead);
                                Response.Flush();
                                bytesToRead -= bytesRead;
                            }
                            else
                            {
                                bytesToRead = -1;
                                downloadBroken = true;
                                break;
                            }
                        }

                        // In Multipart responses, mark the end of the part 
                        if (rangeInfo.IsMultiPartRequest) Response.Output.WriteLine();

                        // No need to proceed to the next part if the 
                        // client was disconnected
                        if (downloadBroken) break;
                    }

                    if (!downloadBroken)
                    {
                        if (rangeInfo.IsMultiPartRequest)
                        {
                            // In multipart responses, close the response once more with 
                            // the boundary and line breaks
                            Response.Output.WriteLine("--" + MULTIPART_BOUNDARY + "--");
                            Response.Output.WriteLine();
                        }
                    }

                    fs.Close();
                }

                Response.Close();
            }
            else
            {
                Response.Close();
                return;
            }

        }

        private static RequestStatus ValidateRequest(HttpContext context, FileInfo fi)
        {
            HttpResponse Response = context.Response;
            HttpRequest Request = context.Request;

            RequestStatus status = new RequestStatus();
            status.IsValid = false;
            status.RangeInfo = null;

            if (!Request.HttpMethod.Equals(HTTP_METHOD_GET) && !Request.HttpMethod.Equals(HTTP_METHOD_HEAD))
            {
                //Only GET and HEAD methods are supported
                Response.StatusCode = 501; //Not Implemented                
                return status;
            }

            if (!fi.Exists)
            {
                Response.StatusCode = 404;
                return status;
            }

            if (fi.Length > Int32.MaxValue)
            {
                Response.StatusCode = 413;  // Request Entity Too Large                
                return status;
            }

            if (!IfModifiedSinceHeaderEffective(context.Request, fi))
            {
                //The entity is not modified
                Response.StatusCode = 304; //Not Modified                
                return status;
            }

            //IF THE REQUESTED FILE HAS BEEN MODIFIED SINCE THE HEADER DATE, WE
            //SHOULD IGNORE ANY RANGE SPECIFIED IN THE HEADER
            //IF FILE IS UNMODIFIED, WE SHOULD NOT IGNORE THE RANGE HEADER
            bool ignoreRangeHeader = (!IfUnmodifiedSinceHeaderEffective(context.Request, fi));
            RangeInfo rangeInfo = new RangeInfo(context, fi, ignoreRangeHeader);
            status.RangeInfo = rangeInfo;

            //VERIFY THE RANGE IS WITHIN BOUNDS
            if (!rangeInfo.RequestedRangeValid)
            {
                //the range specified was invalid, restart the request without a range
                status.RangeInfo = new RangeInfo(context, fi, true);
            }

            //If-Match, If-None-Match and If-Range not implemented
            //just ignore. Assume a match

            status.IsValid = true;
            return status;
        }

        private static void SetupHeaders(HttpContext context, RangeInfo rangeInfo, FileInfo fi, string downloadName)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;

            Int32 responseContentLength = 0;
            if (rangeInfo.IsRangeRequest)
            {
                //this is a range request.
                Response.StatusCode = 206; //Partial Response

                foreach (Range range in rangeInfo.Ranges)
                {
                    responseContentLength += Convert.ToInt32(range.end - range.begin) + 1;
                    if (rangeInfo.IsMultiPartRequest)
                    {
                        // If this is a multipart range request, calculate 
                        // the length of the intermediate headers to send
                        responseContentLength += MULTIPART_BOUNDARY.Length;                        
                        responseContentLength += DEFAULT_CONTENT_TYPE.Length;
                        responseContentLength += range.begin.ToString().Length;
                        responseContentLength += range.end.ToString().Length;
                        responseContentLength += fi.Length.ToString().Length;
                        // 49 is the length of line break and other 
                        // needed characters in one multipart header
                        responseContentLength += 49;
                    }
                }

                if (rangeInfo.IsMultiPartRequest)
                {
                    // If this is a multipart range request,  
                    // we must also calculate the length of 
                    // the last intermediate header we must send
                    responseContentLength += MULTIPART_BOUNDARY.Length;
                    // 8 is the length of dash and line break characters
                    responseContentLength += 8;
                }
                else
                {
                    // This is no multipart range request, so
                    // we must indicate the response Range of 
                    // in the initial HTTP Header 
                    Range range = rangeInfo.Ranges[0];
                    Response.AppendHeader(HTTP_HEADER_CONTENT_RANGE, "bytes " + range.begin.ToString() + "-"
                        + range.end.ToString() + "/" + fi.Length.ToString());
                }
            }
            else
            {
                //this is a new download
                // Indicate the file's complete size as content length
                responseContentLength = Convert.ToInt32(fi.Length);
                string activeFileName = downloadName;
                if (string.IsNullOrEmpty(activeFileName)) activeFileName = fi.Name;
                //IF FILE NAME HAS SPACES IT SHOULD BE QUOTED
                if (activeFileName.Contains(" ")) activeFileName = "\"" + activeFileName + "\"";
                Response.AddHeader(HTTP_HEADER_CONTENT_DISPOSITION, "attachment; filename=" + activeFileName);
                Response.StatusCode = 200; // A normal OK status
            }

            // Write the content length into the Response
            Response.AppendHeader(HTTP_HEADER_CONTENT_LENGTH, responseContentLength.ToString());

            // Write the Last-Modified Date into the Response
            Response.AppendHeader(HTTP_HEADER_LAST_MODIFIED, fi.LastWriteTimeUtc.ToString("r"));

            // Tell the client software that we accept Range request
            Response.AppendHeader(HTTP_HEADER_ACCEPT_RANGES, HTTP_HEADER_ACCEPT_RANGES_BYTES);

            // Write the file's Entity Tag into the Response (in quotes!)
            Response.AppendHeader(HTTP_HEADER_ENTITY_TAG, "\"" + DEFAULT_ENTITY_TAG + "\"");

            // Write the Content Type into the Response
            if (rangeInfo.IsMultiPartRequest)
            {
                // Multipart messages have this special Type.
                // In this case, the file's actual mime type is
                // written into the Response at a later time...
                Response.ContentType = MULTIPART_CONTENTTYPE;
            }
            else
            {
                // Single part messages have the files content type...
                Response.ContentType = DEFAULT_CONTENT_TYPE;
            }
        }

        private static bool IfModifiedSinceHeaderEffective(HttpRequest request, FileInfo fileInfo)
        {
            string sDate = RetrieveHeader(request, HTTP_HEADER_IF_MODIFIED_SINCE, string.Empty);
            if (string.IsNullOrEmpty(sDate))
            {
                //No if modified date specified. Assume modified since last request
                return true;
            }
            else
            {
                //WHEN COMPARING THE FILE WRITE TIME AND THE HEADER TIME, WE MUST ENSURE IT DOES NOT INCLUDE MILLISECONDS
                DateTime headerTime = AlwaysConvert.ToDateTime(sDate, DateTime.MinValue).ToUniversalTime();
                DateTime clientFileTime = new DateTime(headerTime.Year, headerTime.Month, headerTime.Day, headerTime.Hour, headerTime.Minute, headerTime.Second, DateTimeKind.Utc);
                DateTime lastWriteTime = fileInfo.LastWriteTimeUtc;
                DateTime serverFileTime = new DateTime(lastWriteTime.Year, lastWriteTime.Month, lastWriteTime.Day, lastWriteTime.Hour, lastWriteTime.Minute, lastWriteTime.Second, DateTimeKind.Utc);
                bool fileIsModified = (serverFileTime >= clientFileTime);
                return fileIsModified;
            }
        }

        /// <summary>
        /// Determines whether a file has been modified from the date specified in an HTTP request header
        /// </summary>
        /// <param name="request">The request that may contain an if-modified-since header</param>
        /// <param name="fileInfo">The file to check for modification</param>
        /// <returns>Returns true if the file is unmodified since the given date, false if the file has been updated.</returns>
        private static bool IfUnmodifiedSinceHeaderEffective(HttpRequest request, FileInfo fileInfo)
        {
            string sDate = RetrieveHeader(request, HTTP_HEADER_IF_UNMODIFIED_SINCE, string.Empty);
            if (sDate.Equals(String.Empty))
            {
                // If-Unmodified-Since was not sent, check Unless-Modified-Since... 
                sDate = RetrieveHeader(request, HTTP_HEADER_UNLESS_MODIFIED_SINCE, string.Empty);
            }

            if (string.IsNullOrEmpty(sDate))
            {
                //No date specified. Assume the file is not modified since last request
                return true;
            }
            else
            {
                //WHEN COMPARING THE FILE WRITE TIME AND THE HEADER TIME, WE MUST ENSURE IT DOES NOT INCLUDE MILLISECONDS
                DateTime headerTime = AlwaysConvert.ToDateTime(sDate, DateTime.MinValue).ToUniversalTime();
                DateTime clientFileTime = new DateTime(headerTime.Year, headerTime.Month, headerTime.Day, headerTime.Hour, headerTime.Minute, headerTime.Second, DateTimeKind.Utc);
                DateTime lastWriteTime = fileInfo.LastWriteTimeUtc;
                DateTime serverFileTime = new DateTime(lastWriteTime.Year, lastWriteTime.Month, lastWriteTime.Day, lastWriteTime.Hour, lastWriteTime.Minute, lastWriteTime.Second, DateTimeKind.Utc);
                bool fileIsUnmodified = (serverFileTime <= clientFileTime);
                return fileIsUnmodified;
            }
        }

        private static string RetrieveHeader(HttpRequest request, string header, string defaultValue)
        {
            string sReturn = request.Headers[header];
            if (string.IsNullOrEmpty(sReturn))
            {
                return defaultValue;
            }
            else
            {
                //strip quote characters \"
                return sReturn.Replace("\"", "");
            }
        }

        private class RangeInfo
        {
            //System.Collections.Generic.List
            List<Range> _Ranges = new List<Range>();
            bool _RequestedRangeValid = false;
            bool _RangeRequest = false;

            public RangeInfo(HttpContext context, FileInfo fileInfo, bool ignoreRangeHeader)
            {
                this.ParseRangeHeaders(context.Request, fileInfo.Length, ignoreRangeHeader);
            }

            public bool RequestedRangeValid
            {
                get { return _RequestedRangeValid; }
                set { _RequestedRangeValid = value; }
            }

            public bool IsRangeRequest
            {
                get { return _RangeRequest; }
                set { _RangeRequest = value; }
            }

            public List<Range> Ranges
            {
                get { return _Ranges; }
            }

            public void AddRange(Range range)
            {
                _Ranges.Add(range);
            }

            public void RemoveRange(Range range)
            {
                _Ranges.Remove(range);
            }

            public bool IsMultiPartRequest
            {
                get
                {
                    return _Ranges.Count > 1;
                }
            }

            /// <summary>
            /// Parses the Range header from the Request (if there is one)
            /// </summary>
            /// <param name="request">HttpRequest object</param>
            /// <param name="fileLength">Length of the file</param>
            /// <param name="ignoreRangeHeader">If true, the range specified in the header is ignored</param>
            private void ParseRangeHeaders(HttpRequest request, long fileLength, bool ignoreRangeHeader)
            {
                string sSource;
                string[] sRanges;
                // Retrieve Range Header value from Request (Empty if none is indicated)
                sSource = RetrieveHeader(request, HTTP_HEADER_RANGE, string.Empty);
                if (ignoreRangeHeader || string.IsNullOrEmpty(sSource))
                {
                    //No range requested. Return entire file range
                    Range range = new Range();
                    range.begin = 0;
                    range.end = fileLength - 1;
                    this.AddRange(range);
                    this.IsRangeRequest = false;
                    this.RequestedRangeValid = true;
                }
                else
                {
                    //A range was requested
                    this.IsRangeRequest = true;
                    // Remove "bytes=" from the beginning, and split the remaining 
                    // string by comma characters            
                    sRanges = sSource.Replace("bytes=", "").Split(",".ToCharArray());

                    bool invalidRange = false;
                    string[] sRangeItem;
                    long bRange, eRange;
                    foreach (string sRange in sRanges)
                    {
                        sRangeItem = sRange.Split("-".ToCharArray());
                        if (sRangeItem == null || sRangeItem.Length < 2)
                        {
                            invalidRange = true;
                            break;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(sRangeItem[1]))
                            {
                                //end range not specified.
                                eRange = fileLength - 1;
                            }
                            else
                            {
                                eRange = AlwaysConvert.ToLong(sRangeItem[1], long.MinValue);
                            }

                            if (string.IsNullOrEmpty(sRangeItem[0]))
                            {
                                //begin range not specified which means that the end value 
                                //indicated to return the last n bytes of the file
                                //Calculate the begin and end
                                bRange = fileLength - 1 - eRange;
                                eRange = fileLength - 1;
                            }
                            else
                            {
                                bRange = AlwaysConvert.ToLong(sRangeItem[0], long.MinValue);
                            }

                            if (!IsValidRange(bRange, eRange, fileLength))
                            {
                                invalidRange = true;
                                break;
                            }

                            Range range = new Range();
                            range.begin = bRange;
                            range.end = eRange;
                            this.AddRange(range);
                        }
                    }

                    this.RequestedRangeValid = !invalidRange;
                }
            }

            private static bool IsValidRange(long bRange, long eRange, long fileLength)
            {
                //begin and end ranges can't be negative
                if (bRange < 0 || eRange < 0)
                {
                    return false;
                }

                //begin range can't be greater than end range
                if (bRange > eRange) return false;

                //begin and end ranges must fall within file length
                if (bRange > (fileLength - 1) || eRange > (fileLength - 1))
                {
                    return false;
                }

                return true;
            }

        }

        private struct Range
        {
            public long begin;
            public long end;
        }

        private struct RequestStatus
        {
            public bool IsValid;
            public RangeInfo RangeInfo;
        }

    }
}
