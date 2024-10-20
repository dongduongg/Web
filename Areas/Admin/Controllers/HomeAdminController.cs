using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLBN.Models;
using QLBN.Models.Authentication;
using X.PagedList;
using X.PagedList.Mvc.Core;

namespace QLBN.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [Route("Admin/homeadmin")]
    //[Authorize(Roles = "Admin")]
    //[Authorize(AuthenticationSchemes = "Admin")]
    public class HomeAdminController : Controller
    {
        QLBNContext db = new QLBNContext();
        [Route("")]
        [Route("index")] // co the se sua o day
        public IActionResult Index()
        {
            return View();
        }
        [Route("danhmucbacsi")]
        public IActionResult DanhMucBacSi(int? page)
        {
            int pageSize = 8;
            //int pageNumber = pageSize == null || pageSize < 0 ? 1 : page.Value;
            int pageNumber = page ?? 1;
            var lstsanpham = db.Doctors.AsNoTracking().OrderBy(x => x.FacultyId);
            PagedList<Doctor> lst= new PagedList<Doctor>(lstsanpham,pageNumber,pageSize);

           // var lstBacSi= db.Doctors.ToList();
            return View(lst);
        }
        [Route("ThemBacSiMoi")]
        [HttpGet]
        public IActionResult ThemBacSiMoi()
        {
            ViewBag.FacultyId = new SelectList(db.Faculties.ToList(), "FacultyId", "FacultyName");
            ViewBag.RoomId = new SelectList(db.Rooms.ToList(), "RoomId", "RoomId" );
            ViewBag.ServiceId = new SelectList(db.Services.ToList(), "ServiceId", "ServiceName");

            return View();
        }
        [Route("ThemBacSiMoi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemBacSiMoi(Doctor doctor)
        {
            if(ModelState.IsValid)
            {
                db.Doctors.Add(doctor);
                db.SaveChanges();
                return RedirectToAction("DanhMucBacSi");
            }
            return View(doctor);
        }
       [Route("SuaBacSi")]
        [HttpGet]
        public IActionResult SuaBacSi(string doctorId)
        {
            ViewBag.FacultyId = new SelectList(db.Faculties.ToList(), "FacultyId", "FacultyName");
            ViewBag.RoomId = new SelectList(db.Rooms.ToList(), "RoomId", "RoomId");
            ViewBag.ServiceId = new SelectList(db.Services.ToList(), "ServiceId", "ServiceName");
            var BacSi = db.Doctors.Find(Convert.ToInt32(doctorId));
            return View(BacSi);
        }
        [Route("SuaBacSi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaBacSi(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doctor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DanhMucBacSi","HomeAdmin");
            }
            return View(doctor);
        }
        [Route("XoaBacSi")]
        [HttpGet]
        public IActionResult XoaBacSi(string doctorId)
        {
            var lichhen = db.Appointments
            .Where(x => x.DoctorId == Convert.ToInt32(doctorId) && x.AppointmentDate > DateTime.Now)
            .ToList();
            if (lichhen.Count()>0)
            {
                TempData["Message"] = " Không thể xóa vì còn lịch hẹn";
                return RedirectToAction("DanhMucBacSi", "HomeAdmin");
            }
            db.Remove(db.Doctors.Find(Convert.ToInt32(doctorId)));
            db.SaveChanges();
            TempData["Message"] = "Bác sĩ đã được xóa";
            return RedirectToAction("DanhMucBacSi", "HomeAdmin");
        }
    }
}
