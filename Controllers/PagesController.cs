using Humanizer;
using Microsoft.AspNetCore.Mvc;
using QLBN.Models;
//using QLBN.Models.Authentication;
using QLBN.ViewModels;

namespace QLBN.Controllers
{
    
    public class PagesController : Controller
	{
		QLBNContext db= new QLBNContext();
        private readonly ILogger<HomeController> _logger;
        public PagesController(ILogger<HomeController> logger)
        {
           _logger = logger;
        }
        public IActionResult Index()
		{
            ViewData["ActivePage"] = "Pages";
            return View("Feature");
		}
		public IActionResult Feature()
		{
            ViewData["ActivePage"] = "Pages";
            return View();
		}
		public IActionResult Team()
		{
            ViewData["ActivePage"] = "Pages";

			var lstDoctor=db.Doctors.ToList();
            var lstFaculty = db.Faculties.ToList(); // Thêm ở đây

            //var viewModel = new DoctorFacultyViewModel
            //{
            //    Doctors = lstDoctor,
            //    Faculties = lstFaculty
            //};

            //return View(viewModel);
            //ViewBag.Faculties = lstFaculty; 
            return View(lstDoctor);
		}
        public IActionResult DoctortheoKhoa(String maKhoa)
        {
            List<Doctor> lstDoctor= db.Doctors.Where(x => x.FacultyId == Convert.ToInt32(maKhoa) ).OrderBy(X=>X.DoctorDegree).ToList();
            return View(lstDoctor);
        }
        public IActionResult ChitietDoctor(String maDoctor)
        {
            var doctor=db.Doctors.SingleOrDefault(x=>x.DoctorId == Convert.ToInt32(maDoctor));
            //var anddoctor= 
            return View(doctor);
        }
        public IActionResult DoctorDetail (String maDoctor)
        {
            var doctor = db.Doctors.SingleOrDefault(x=>x.DoctorId==Convert.ToInt32(maDoctor));
            var faculty = db.Faculties.SingleOrDefault(x => x.FacultyId == doctor.FacultyId);
            var room = db.Rooms.SingleOrDefault(x => x.RoomId == doctor.RoomId);
            var service = db.Services.SingleOrDefault(x => x.ServiceId == doctor.ServiceId);
            var homeDoctorDetailViewModel = new HomeDoctorDetailViewModel
            {
                dsdoctor = doctor,
                dsfaculty = faculty,
                dsroom = room,
                dsservice = service 
                
            };
            return View(homeDoctorDetailViewModel);

        }
		public IActionResult Testimonial()
		{
            ViewData["ActivePage"] = "Pages";
            return View();
		}
		public IActionResult Appointment()
		{
            ViewData["ActivePage"] = "Pages";
            return View("~/Views/Appointment/Create.cshtml");
		}

	}
}
