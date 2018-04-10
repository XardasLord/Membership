using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Memberships.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        // GET: File
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var path = Server.MapPath("~/Content/Files");
            var dir = new DirectoryInfo(path);
            var files = dir.EnumerateFiles().Where(f => f.Extension != ".gitignore").Select(f => f.Name);

            return View(files);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Index(HttpPostedFileBase file)
        {
            var path = Path.Combine(Server.MapPath("~/Content/Files/"), file.FileName);
            var data = new byte[file.ContentLength];
            file.InputStream.Read(data, 0, file.ContentLength);
            using (var sw = new FileStream(path, FileMode.Create))
            {
                sw.Write(data, 0, data.Length);
            }

            return RedirectToAction("Index");
        }
    }
}