using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;

namespace WcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    public class Service1 : IService1
    {
        #region GetService

        #region SimpleGet

        public String SimpleGet()
        {
            return "This is Simple Get Service.";
        }

        #endregion

        #region SimpleGetwithParameter

        public String SimpleGetwithParameter(string input)
        {
            return input;
        }

        #endregion

        #region SimpleGetTaskBaseAsyncService

        public async Task<string> SimpleGetTaskBaseAsyncService()
        {
            var task = Task.Factory.StartNew(() =>
            {
                return "SimpleGetTaskBaseAsyncService.";
            });
            return await task.ConfigureAwait(false);
        }

        #endregion

        #region SimpleGetTaskBaseAsyncServiceWithParameter

        public async Task<string> SimpleGetTaskBaseAsyncServiceWithParameter(string input)
        {
            var task = Task.Factory.StartNew(() =>
            {
                return input;
            });
            return await task.ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region PostService

        #region SimplePost
        public String SimplePost()
        {
            return "This is Simple Post Service.";
        }
        #endregion

        #region SimplePostWithParameter
        public String SimplePostWithParameter(string input)
        {
            return input;
        }
        #endregion

        #region SimplePostTaskBaseAsyncService

        public async Task<string> SimplePostTaskBaseAsyncService()
        {
            var task = Task.Factory.StartNew(() =>
            {
                return "SimplePostTaskBaseAsyncService.";
            });
            return await task.ConfigureAwait(false);
        }

        #endregion

        #region SimplePostTaskBaseAsyncServiceWithParameter

        public async Task<string> SimplePostTaskBaseAsyncServiceWithParameter(string input)
        {
            var task = Task.Factory.StartNew(() =>
            {
                return input;
            });
            return await task.ConfigureAwait(false);
        }

        #endregion

        #region POSTServiceWithObj
        public async Task<PostObj> PostWithObj(PostObj obj)
        {
            var t = Task.Factory.StartNew(() =>
            {
                return obj;
            });
            return await t.ConfigureAwait(false);
        }
        #endregion

        #region POSTServiceWithList
        public async Task<List<PostObj>> PostWithList(List<PostObj> list)
        {
            var t = Task.Factory.StartNew(() =>
            {
                return list;
            });
            return await t.ConfigureAwait(false);
        }
        #endregion

        #region PostUploadImageOrVideo
        /// <summary>
        /// PostUploadImageOrVideo
        /// </summary>
        /// <param name="type">image or video</param>
        /// <param name="str">stream</param>
        /// <returns></returns>
        public async Task<UploadPhoto> Upload(string type, Stream str)
        {
            var t = Task.Factory.StartNew(() =>
            {
                var x = System.Data.SqlTypes.SqlDateTime.MaxValue;
                bool flag = false;
                string message = string.Empty;
                try
                {
                    if (!string.IsNullOrEmpty(type))
                    {
                        if (str != null)
                        {
                            var recivedFile = StreamToImageorVideoWithPath(str, type, CodeLibrary.SavePath());
                            if (recivedFile != null)
                            {
                                flag = true;
                                message = CodeLibrary.SavePath() + recivedFile;
                            }
                        }
                        else
                        {
                            message = "No file uploaded.";
                        }
                    }
                    else
                    {
                        message = "provide missing parameter type.";
                    }
                }
                catch (Exception)
                {
                    flag = false;
                    message = "Some error occurred.";
                }
                return new UploadPhoto
                {
                    flag = flag,
                    Message = message,
                };
            });
            return await t.ConfigureAwait(false);
        } 

        #endregion

        #endregion

        

        #region Add Stream To Image or Video With Path
        /// <summary>
        /// Save Stream of Imageor or Video To Path
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string StreamToImageorVideoWithPath(Stream str, string type, string path)
        {
            string retPath;
            try
            {
                var bytes = ReadToEnd(str);

                if (bytes.Length == 0)
                {
                    return "";
                }

                if (type.ToLower() == "image")
                {
                    retPath = "Image_" + Guid.NewGuid() + ".jpg";
                    var filePath = path + retPath;
                    using (var ms = new MemoryStream(bytes, 0, bytes.Length))
                    {
                        var image = Image.FromStream(ms, true);
                        image.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
                else
                {
                    retPath = "Video_" + Guid.NewGuid() + ".mp4";
                    var filePath = path + retPath;
                    var ms = new MemoryStream(bytes, 0, bytes.Length);
                    FastUpload(ms, filePath, retPath, bytes.Length);
                }
            }
            catch (Exception ex)
            {
                retPath = "";
            }

            return retPath;
        }

        /// <summary>
        /// get the input stream and convert to bytes
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static byte[] ReadToEnd(Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                var readBuffer = new byte[4096];

                var totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        var nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            var temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                var buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        /// <summary>
        /// For Video faster upload
        /// </summary>
        /// <param name="str"></param>
        /// <param name="fpath"></param>
        /// <param name="fname"></param>
        /// <param name="streamlength"></param>
        public static void FastUpload(MemoryStream str, string fpath, string fname, long streamlength)
        {
            var filePath = fname;
            var offset = 0;

            var chunkSize = 65536; // 64 * 1024 kb
            var buffer = new byte[chunkSize];
            var sr = new StreamReader(str);

            try
            {
                var fileSize = streamlength; // File size of file being uploaded.
                sr.BaseStream.Position = 0;

                while (offset != fileSize) // continue uploading the file chunks until offset = file size.
                {
                    var bytesRead = sr.BaseStream.Read(buffer, 0, chunkSize);
                    if (bytesRead != buffer.Length)
                    {
                        chunkSize = bytesRead;
                        var trimmedBuffer = new byte[bytesRead];
                        Array.Copy(buffer, trimmedBuffer, bytesRead);
                        buffer = trimmedBuffer; // the trimmed buffer should become the new 'buffer'
                    }
                    var chunkAppened = UploadFile(buffer, offset, fpath);
                    if (!chunkAppened)
                    {
                        break;
                    }

                    offset += bytesRead; // save the offset position for resume
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                sr.BaseStream.Close();
            }
        }

        /// <summary>
        /// For video create new file
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="fpath"></param>
        /// <returns></returns>
        private static bool UploadFile(byte[] buffer, long offset, string fpath)
        {
            var retVal = false;
            try
            {
                var filePath = fpath;

                if (offset == 0) // new file, create an empty file
                    File.Create(filePath).Close();
                using (var fs = new FileStream(filePath, FileMode.Open,
                    FileAccess.ReadWrite, FileShare.Read))
                {
                    fs.Seek(offset, SeekOrigin.Begin);
                    fs.Write(buffer, 0, buffer.Length);
                }
                retVal = true;
            }
            catch (Exception ex)
            {

            }
            return retVal;
        }

        #endregion

        #region Save Video Thumbnail
        private static void GetThumbnail(string video, string thumbnail, string smallThumbUrl)
        {
            try
            {
                #region Scale & FrameRate
                var frameRate = 15;
                var scale = "420:-1";
                var scaleSmall = "70x70";

                try
                {
                    var ffProbe = new NReco.VideoInfo.FFProbe();
                    var videoInfo = ffProbe.GetMediaInfo(video);
                    foreach (var stream in videoInfo.Streams)
                    {
                        frameRate = Convert.ToInt16(stream.FrameRate);
                        if (stream.Width > 420 && stream.Height < 560)
                        {
                            scale = "420x560";
                        }
                        else
                        {
                            if (stream.Height > 0 && stream.Width > 0)
                            {
                                scale = stream.Width + "x" + stream.Height;
                                //scale = "1280x720";
                                break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    frameRate = 15;
                    scale = "420:-1";
                }
                #endregion

                var ffmpeg = CodeLibrary.SavePath() + @"\ffmpeg.exe";
                string pngimage = CodeLibrary.SavePath() + @"\Images\Palette\palette_" + Guid.NewGuid() + ".png";
                string pngSmallimage = CodeLibrary.SavePath() + @"\Images\iconPalette\palette_" + Guid.NewGuid() + ".png";

                var processPalette = new Process();
                var processGif = new Process();

                var processSmallPalette = new Process();
                var processSmallGif = new Process();
                try
                {
                    #region Large Thumbnail
                    var infoPalette = new ProcessStartInfo(ffmpeg, " -y -t 3 -i " + video + " -vf fps=" + frameRate + ",scale=" + scale + ":flags=lanczos,palettegen " + pngimage)
                    {
                        CreateNoWindow = false,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };
                    processPalette.StartInfo = infoPalette;
                    processPalette.EnableRaisingEvents = true;
                    processPalette.Start();
                    processPalette.BeginOutputReadLine();
                    processPalette.BeginErrorReadLine();
                    processPalette.WaitForExit();
                    processPalette.Close();

                    var infoGif = new ProcessStartInfo(ffmpeg, " -t 3 -i " + video + " -i " + pngimage + " -filter_complex \"fps=" + frameRate + ",scale=" + scale + ":flags=lanczos[x];[x][1:v]paletteuse\" " + thumbnail)
                    {
                        CreateNoWindow = false,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };
                    processGif.StartInfo = infoGif;
                    processGif.EnableRaisingEvents = true;
                    processGif.Start();
                    processGif.BeginOutputReadLine();
                    processGif.BeginErrorReadLine();
                    processGif.WaitForExit();
                    processGif.Close();
                    #endregion

                    #region Small Thumbnail
                    var ext = Path.GetExtension(video);
                    var startTime = "";
                    if (ext != ".jpg")
                    {
                        startTime = "-ss 2";
                    }
                    var infoSmallPalette = new ProcessStartInfo(ffmpeg, " -y " + startTime + " -t 3 -i " + video + " -vf fps=" + frameRate + ",scale=" + scaleSmall + ":flags=lanczos,palettegen " + pngSmallimage)
                    {
                        CreateNoWindow = false,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };
                    processSmallPalette.StartInfo = infoSmallPalette;
                    processSmallPalette.EnableRaisingEvents = true;
                    processSmallPalette.Start();
                    processSmallPalette.BeginOutputReadLine();
                    processSmallPalette.BeginErrorReadLine();
                    processSmallPalette.WaitForExit();
                    processSmallPalette.Close();

                    var infoSmallGif = new ProcessStartInfo(ffmpeg, " " + startTime + " -t 3 -i " + video + " -i " + pngSmallimage + " -filter_complex \"fps=" + frameRate + ",scale=" + scaleSmall + ":flags=lanczos[x];[x][1:v]paletteuse\" " + smallThumbUrl)
                    {
                        CreateNoWindow = false,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };
                    processSmallGif.StartInfo = infoSmallGif;
                    processSmallGif.EnableRaisingEvents = true;
                    processSmallGif.Start();
                    processSmallGif.BeginOutputReadLine();
                    processSmallGif.BeginErrorReadLine();
                    processSmallGif.WaitForExit();
                    processSmallGif.Close();
                    #endregion

                    #region Remove Image/Video

                    if ((File.Exists(video)))
                        File.Delete(video);

                    #endregion
                }
                catch (Exception ex)
                {
                    processPalette.Close();
                    processGif.Close();
                    processSmallPalette.Close();
                    processSmallGif.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private static void GetThumbnailApp(string video, string thumbnail, string smallThumbUrl, string smallThumbUrljpg, string OsType)
        {
            try
            {
                #region Scale & FrameRate
                var frameRate = 15;
                var scale = "420:-1";
                var scaleSmall = "70x70";
                var scaleSmallJPG = "";

                string height = "";
                string width = "";
                int Heightreduce = 3;//1920
                int widthreduce = 3;//1080
                try
                {
                    var ffProbe = new NReco.VideoInfo.FFProbe();
                    var videoInfo = ffProbe.GetMediaInfo(video);
                    foreach (var stream in videoInfo.Streams)
                    {
                        frameRate = Convert.ToInt16(stream.FrameRate);
                        if (stream.Width > 420 && stream.Height < 560)
                        {
                            scale = "420x560";
                            if (OsType == "android")
                            {
                                frameRate = 25;
                                height = (stream.Height / (stream.Height * Heightreduce)).ToString();
                                width = (stream.Width - (stream.Width / widthreduce)).ToString();
                                scale = stream.Height + "x" + stream.Width;
                                scaleSmallJPG = stream.Height + "x" + stream.Width;
                            }
                            else
                            {
                                //scaleSmallJPG = stream.Width + "x" + stream.Height;
                                scale = stream.Width + "x" + stream.Height;
                                scaleSmallJPG = scale;
                            }

                        }
                        else
                        {
                            if (stream.Height > 0 && stream.Width > 0)
                            {
                                scale = stream.Width + "x" + stream.Height;
                                if (OsType == "android")
                                {
                                    //height = (stream.Height - (stream.Height * Heightreduce / 100)).ToString();
                                    //width = (stream.Width - (stream.Width * widthreduce / 100)).ToString();
                                    //scale = stream.Height + "x" + stream.Width;
                                    //scaleSmallJPG = stream.Height + "x" + stream.Width;
                                    frameRate = 25;
                                    height = (stream.Height / Heightreduce).ToString();
                                    width = (stream.Width / widthreduce).ToString();
                                    scale = height + "x" + width;
                                    scaleSmallJPG = height + "x" + width;
                                }
                                else
                                {
                                    //scaleSmallJPG = stream.Width + "x" + stream.Height;
                                    scale = stream.Width + "x" + stream.Height;
                                    scaleSmallJPG = scale;
                                }
                                //scale = "1280x720";
                                break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    frameRate = 15;
                    scale = "420:-1";
                    scaleSmallJPG = "420:-1";
                }
                #endregion

                if (frameRate < 0)
                {
                    frameRate = 25;
                }

                var ffmpeg = CodeLibrary.SavePath() + @"\ffmpeg.exe";
                string pngimage = CodeLibrary.SavePath() + @"\Images\Palette\palette_" + Guid.NewGuid() + ".png";
                string pngSmallimage = CodeLibrary.SavePath() + @"\Images\iconPalette\palette_" + Guid.NewGuid() + ".png";

                var processPalette = new Process();
                var processGif = new Process();

                var processSmallPalette = new Process();
                var processSmallGif = new Process();
                var processSmallJPG = new Process();
                try
                {
                    #region Large Thumbnail
                    var infoPalette = new ProcessStartInfo(ffmpeg, " -y -t 3 -i " + video + " -vf fps=" + frameRate + ",scale=" + scale + ":flags=lanczos,palettegen " + pngimage)
                    {
                        CreateNoWindow = false,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };
                    processPalette.StartInfo = infoPalette;
                    processPalette.EnableRaisingEvents = true;
                    processPalette.Start();
                    processPalette.BeginOutputReadLine();
                    processPalette.BeginErrorReadLine();
                    processPalette.WaitForExit();
                    processPalette.Close();

                    var infoGif = new ProcessStartInfo(ffmpeg, " -t 3 -i " + video + " -i " + pngimage + " -filter_complex \"fps=" + frameRate + ",scale=" + scale + ":flags=lanczos[x];[x][1:v]paletteuse\" " + thumbnail)
                    {
                        CreateNoWindow = false,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };
                    processGif.StartInfo = infoGif;
                    processGif.EnableRaisingEvents = true;
                    processGif.Start();
                    processGif.BeginOutputReadLine();
                    processGif.BeginErrorReadLine();
                    processGif.WaitForExit();
                    processGif.Close();
                    #endregion

                    #region Small Thumbnail
                    var ext = Path.GetExtension(video);
                    var startTime = "";
                    if (ext != ".jpg")
                    {
                        startTime = "-ss 2";
                    }
                    var infoSmallPalette = new ProcessStartInfo(ffmpeg, " -y " + startTime + " -t 3 -i " + video + " -vf fps=" + frameRate + ",scale=" + scaleSmall + ":flags=lanczos,palettegen " + pngSmallimage)
                    {
                        CreateNoWindow = false,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };
                    processSmallPalette.StartInfo = infoSmallPalette;
                    processSmallPalette.EnableRaisingEvents = true;
                    processSmallPalette.Start();
                    processSmallPalette.BeginOutputReadLine();
                    processSmallPalette.BeginErrorReadLine();
                    processSmallPalette.WaitForExit();
                    processSmallPalette.Close();

                    var infoSmallGif = new ProcessStartInfo(ffmpeg, " " + startTime + " -t 3 -i " + video + " -i " + pngSmallimage + " -filter_complex \"fps=" + frameRate + ",scale=" + scaleSmall + ":flags=lanczos[x];[x][1:v]paletteuse\" " + smallThumbUrl)
                    {
                        CreateNoWindow = false,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };
                    processSmallGif.StartInfo = infoSmallGif;
                    processSmallGif.EnableRaisingEvents = true;
                    processSmallGif.Start();
                    processSmallGif.BeginOutputReadLine();
                    processSmallGif.BeginErrorReadLine();
                    processSmallGif.WaitForExit();
                    processSmallGif.Close();
                    #endregion

                    #region Small Thumbnail JPG



                    var infoSmallJPG = new ProcessStartInfo(ffmpeg, " -t 3 -i " + video + " -i " + pngimage + " -filter_complex \"fps=" + frameRate + ",scale=" + scaleSmallJPG + ":flags=lanczos[x];[x][1:v]paletteuse\" " + smallThumbUrljpg)
                    {
                        CreateNoWindow = false,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };
                    processSmallJPG.StartInfo = infoSmallJPG;
                    processSmallJPG.EnableRaisingEvents = true;
                    processSmallJPG.Start();
                    processSmallJPG.BeginOutputReadLine();
                    processSmallJPG.BeginErrorReadLine();
                    processSmallJPG.WaitForExit();
                    processSmallJPG.Close();
                    #endregion

                    #region Remove Image/Video

                    //if ((File.Exists(video)))
                    //    File.Delete(video);

                    #endregion
                }
                catch (Exception ex)
                {
                    processPalette.Close();
                    processGif.Close();

                    processSmallPalette.Close();
                    processSmallGif.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Commented
        ////Call get Service
        //private string GetService(string url)
        //{
        //    string result = string.Empty;
        //    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
        //    request.Method = "GET";
        //    request.ContentType = "application/json";
        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        //    {
        //        result = reader.ReadToEnd();
        //    }
        //    return result;
        //} 

        //private string callService()
        //{
        //    string result = string.Empty;
        //    try
        //    {
        //        string api_Key = "39ad56326186411cb14131ec4369d029";
        //        string api_secret = "5167f56d224d4ef9bd9a1704793bfc8f";
        //        string imageUrl = "https://encrypted-tbn3.gstatic.com/images?q=tbn:ANd9GcQaBK362Gt80OvVFAH_ir_yBC7rlGRNMhkRD7QMKCT20CmWgbN5khzimZR6";
        //        //"https://encrypted-tbn2.gstatic.com/images?q=tbn:ANd9GcTZwHT7ihJNlEIgisCjgk7fNIFKtCB-6mpBemK8VCD45k1FYeCH";
        //        string reqUrl = "http://api.skybiometry.com/fc/faces/detect.json?"
        //            + "api_key=" + api_Key
        //            + "&api_secret=" + api_secret
        //            + "&urls=" + imageUrl
        //            + "&attributes=gender,age_est";
        //        HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(reqUrl);
        //        req.Method = "GET";
        //        req.ContentType = "application/json";
        //        HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
        //        StreamReader reader = new StreamReader(resp.GetResponseStream());
        //        string reqResponce = reader.ReadToEnd();
        //        reader.Close();
        //        JObject JT = JObject.Parse(reqResponce);
        //        string ageVal = "";
        //        string genderVal = "";
        //        if (JT.ToString().Contains("age_est"))
        //        {
        //            JToken age = JT["photos"][0]["tags"][0]["attributes"]["age_est"]["value"];
        //            ageVal = age.ToString();
        //        }
        //        if (JT.ToString().Contains("gender"))
        //        {
        //            JToken gender = JT["photos"][0]["tags"][0]["attributes"]["gender"]["value"];
        //            genderVal = gender.ToString();
        //        }
        //        result = "Age:" + ageVal + " Gender:" + genderVal;
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    return result;
        //}

        #endregion

    }
}
