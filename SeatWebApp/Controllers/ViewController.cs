using Exir.Framework.Common;
using Exir.Framework.Common.Logging;
using Exir.Framework.Security;
using Exir.Framework.Security.Cryptography;
using Exir.Framework.Uie;
using Exir.Framework.Uie.Contracts;
using Newtonsoft.Json;
using SeatDomain.Services;
using SeatWebApp.Models;
using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SeatWebApp.Controllers
{
    public class ViewController : Controller
    {
        private Exir.Framework.Common.Logging.ILogger _logger;
        public ViewController()
        {
            _logger = LogManager.Instance.GetLogger<ViewController>();
        }


        public ActionResult Index(ViewFileModel model)
        {
            ViewBag.ImageUrl = MyUrlHelper.GetUrl("View", "GetFile", model);
            IFilePropertyValueItem fileItem = getFile(model);

            if (fileItem == null) return new HttpStatusCodeResult(404, "مدرک مورد نظر شما یافت نشد!");

            if (String.Compare(fileItem.Extension, "jpg", true) == 0 ||
                String.Compare(fileItem.Extension, "png", true) == 0 ||
                String.Compare(fileItem.Extension, "jpeg", true) == 0)
            {
                try
                {
                    using (var mem = new MemoryStream(fileItem.Content))
                    {
                        var first_10 = fileItem.Content.Take(10).ToArray();
                        var text = System.Text.ASCIIEncoding.ASCII.GetString(first_10);
                        if (text.IndexOf("PDF") > 0)
                        {
                            var service = StaticServiceFactory.Create<IThumbnailPersistanceService>(model.Service);
                            var guid = Guid.Parse(model.PkValue);
                            service.ChangeExtension(guid, SeatDomain.Constants.ExtentionTypes.PDF);
                            return File(fileItem.Content, KnownMimeTypes.GetMimeType("pdf"));
                        }

                        using (var image = Image.FromStream(mem))
                        {
                            ViewBag.Width = image.Width;
                            ViewBag.Height = image.Height;
                            return View("ImageViewer");
                        }
                    }
                }
                catch (ArgumentException)
                {
                    throw;
                }
            }
            if (String.Compare(fileItem.Extension, "pdf", true) == 0)
            {
                return File(fileItem.Content, KnownMimeTypes.GetMimeType("pdf"));
            }
            throw new NotSupportedException();
        }

        public ActionResult GetFile(ViewFileModel model)
        {
            IFilePropertyValueItem fileItem = getFile(model);

            if (fileItem == null) return new HttpNotFoundResult();

            var fileName = fileItem.FileName + "." + fileItem.Extension;
            return File(fileItem.Content, KnownMimeTypes.GetMimeType(fileItem.Extension), fileName);
        }

        //[OutputCache(Duration = 3000)]
        public ActionResult GetThumbnail(ViewThumbnailModel model)
        {
            if (_logger.IsInfoEnabled)
                _logger.InfoFormat("try find thumbnail image by this criteria {0}", JsonConvert.SerializeObject(model));

            if (ConfigurationManager.AppSettings["disable-thumbnail"] == "true")
                return File(FileSystemProvider.Instance.ReadAllBytes(Server.MapPath("~/assets/img/icons/ukn.png")), KnownMimeTypes.GetMimeType(model.Extension), model.FileName);

            string path = ConfigurationManager.AppSettings["thumbnails-path"];
            var physical_path = Server.MapPath(path);

            string fn = String.Format("{0}-{1}x{2}-{3}", model.Key, model.MaxWith, model.MaxHeight, model.Audit_LastModifyDate.Ticks);
            fn += "." + model.Extension;

            string fpath = Path.Combine(physical_path, fn);

            _logger.InfoFormat("thumbnail image path for key {0} is {1}", model.Key, fpath);

            byte[] thumbnail = null;
            if (!FileSystemProvider.Instance.FileExists(fpath))
            {
                var service = StaticServiceFactory.Create<IThumbnailPersistanceService>(model.Service);
                thumbnail = service.GetThumbnail(model.Key, model.MaxWith, model.MaxHeight);
                if (thumbnail == null)
                {
                    _logger.DebugFormat("thumbnail image by key {0} not found try get file from {1}.{2}", model.Key, model.Service, model.Command);

                    var file = getFile(model);
                    if (file == null)
                    {
                        _logger.DebugFormat("sorry system cannot thumbnail image by key {0}", model.Key);

                        thumbnail = FileSystemProvider.Instance.ReadAllBytes(Server.MapPath("~/assets/img/icons/404.png"));
                    }
                    else
                        using (var mem = new MemoryStream(file.Content))
                        {
                            _logger.InfoFormat("thumbnail image by key {0} found try find extension", model.Key);

                            var first_10 = file.Content.Take(10).ToArray();
                            var text = System.Text.ASCIIEncoding.ASCII.GetString(first_10);
                            if (text.IndexOf("PDF") > 0)
                            {
                                service.ChangeExtension(model.Key, SeatDomain.Constants.ExtentionTypes.PDF);
                                thumbnail = FileSystemProvider.Instance.ReadAllBytes(Server.MapPath("~/assets/img/icons/pdf.png"));
                            }
                            else
                            {

                                try
                                {
                                    using (var img = Image.FromStream(mem))
                                    {
                                        int width = model.MaxWith, height = model.MaxHeight;
                                        NormalizeSize(ref width, ref height, img);

                                        using (var result = new MemoryStream())
                                        using (var thumbnailImg = img.GetThumbnailImage(width, height, delegate { return true; }, IntPtr.Zero))
                                        {
                                            thumbnailImg.Save(result, ImageFormat.Jpeg);
                                            thumbnail = result.ToArray();
                                        }
                                    }
                                    FileSystemProvider.Instance.WriteAllBytes(fpath, thumbnail);
                                    service.SaveThumbnail(model.Key, model.MaxWith, model.MaxHeight, thumbnail);
                                }
                                catch (Exception ex)
                                {
                                    _logger.WarnFormat("cannot convert image by key {0} to thumbnail", ex, model.Key);

                                    thumbnail = FileSystemProvider.Instance.ReadAllBytes(Server.MapPath("~/assets/img/icons/ukn.png"));
                                }
                            }
                        }
                }
            }
            else
            {
                thumbnail = FileSystemProvider.Instance.ReadAllBytes(fpath);
            }

            return File(thumbnail, KnownMimeTypes.GetMimeType(model.Extension), model.FileName);
        }

        protected void NormalizeSize(ref int width, ref int height, Image img)
        {
            if (width == 0 && height == 0)
            {
                width = img.Width;
                height = img.Height;
            }

            if ((img.Width > width || img.Height > height) && (width != 0 && height != 0))
            {
                if (img.Width > img.Height)
                {
                    height = (int)(width * 1.0f / img.Width * img.Height);
                }
                else
                {
                    width = (int)(height * 1.0f / img.Height * img.Width);
                }
            }

            if (width == 0)
            {
                width = height * img.Width / img.Height;
            }
            if (height == 0)
            {
                height = img.Height * width / img.Width;
            }
        }

        private static IFilePropertyValueItem getFile(ViewFileModel model)
        {
            var service = StaticServiceFactory.Create<IQueryService>(model.Service);
            var queryable = service.GetQuery(model.Command, new object[] { model.PkValue }, null);

            if (queryable == null) return null;

            IEntityBase entity = null;
            foreach (var entityObj in queryable)
            {
                entity = (IEntityBase)entityObj;
                break;
            }
            var data = (byte[])entity.GetValue(model.Property);
            if (model.IsRaw)
            {
                var file_name = model.FileName.Split('.');
                return new FilePropertyValueItem()
                {
                    Content = data,
                    Extension = file_name[1],
                    FileName = file_name[0],
                    Guid = Guid.NewGuid()
                };
            }
            else
            {
                var files = FilePropertyValue.Deserialize(data);
                var fileItem = files.Files[0];
                return fileItem;
            }
        }

       
        public ActionResult Gallery()
        {
            var securityProvider = ObjectRegistry.GetObject<ISecurityProvider>();
            securityProvider.CanDo(SeatDomain.Constants.KnownOperations.HlpInfrmtn_A, true);

            return View();
        }

        [HttpPost]
        public ActionResult UploadToGallery()
        {
            var securityProvider = ObjectRegistry.GetObject<ISecurityProvider>();
            securityProvider.CanDo(SeatDomain.Constants.KnownOperations.HlpInfrmtn_A, true);

            string path = ConfigurationManager.AppSettings["content-files-path"];
            path = Server.MapPath(path);

            foreach (string input in Request.Files)
            {
                var file = Request.Files[input];
                using (var mem = new MemoryStream(file.ContentLength))
                {
                    int rc = 0;
                    byte[] buffer = new byte[10480];
                    do
                    {
                        rc = file.InputStream.Read(buffer, 0, buffer.Length);
                        if (rc > 0)
                            mem.Write(buffer, 0, rc);
                    } while (rc > 0);
                    FileSystemProvider.Instance.WriteAllBytes(Path.Combine(path, file.FileName), mem.ToArray());
                }
            }

            return View("Gallery");
        }

        [HttpPost]
        public ActionResult RemoveFromGallery(string fileName)
        {
            var securityProvider = ObjectRegistry.GetObject<ISecurityProvider>();
            securityProvider.CanDo(SeatDomain.Constants.KnownOperations.HlpInfrmtn_A, true);

            string path = ConfigurationManager.AppSettings["content-files-path"];
            path = Server.MapPath(path);
            path = Path.Combine(path, fileName);

            if (FileSystemProvider.Instance.FileExists(path))
            {
                FileSystemProvider.Instance.FileDelete(path);
            }

            return Content("OK");
        }

        public ActionResult Icons()
        {
            return View();
        }
    }
}