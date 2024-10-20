//using QLBN.Data;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLBN.Models;
using X.PagedList;
using X.PagedList.Mvc.Core;
using QLBN.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using QLBN.Models.Authentication;
using System.Numerics;

namespace QLBN.Controllers
{

    public class AppointmentController : Controller
    {

        QLBNContext db = new QLBNContext();
        public IActionResult Index()
        {
            var lst = db.Appointments.ToList();
            return View(lst);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var faculties = db.Faculties.ToList();
            if (faculties == null || !faculties.Any())
            {
                ModelState.AddModelError("", "No faculties available.");
                return View(); // Hoặc hiển thị thông báo phù hợp
            }
            ViewBag.Faculties = new SelectList(faculties, "FacultyId", "FacultyName");
            ViewBag.Doctors = new SelectList(db.Doctors
            .Select(d => new
            {
                d.DoctorId,
                FullName = $"{d.DoctorDegree} - {d.DoctorName}"  // Nối chuỗi ở đây
            }).ToList(), "DoctorId", "FullName");

            ViewBag.Services = new SelectList(db.Services.ToList(), "ServiceId", "ServiceName");
            return View();
            

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create( PatientAppointmentViewModel model) //[Bind("PatientName,PatientId,PatientPhone,PatientEmail,PatientGender,PatientAddress,PatientBorn,AppointmentDate,FacultyID,DoctorID,ServiceId")]
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine($"DoctorId: {model.DoctorId}"); // Debug line
                                                                  // Các đoạn mã khác...
                var patient = new Patient
                {
                    PatientName = model.PatientName,
                    PatientAddress = model.PatientAddress,
                    PatientBorn = model.PatientBorn,
                    PatientDesciption = model.PatientDesciption,
                    PatientEmail = model.PatientEmail,
                    PatientGender = model.PatientGender,
                    PatientPhone = model.PatientPhone,
                    PatientId = model.PatientId,
                };
                if (db.Patients.Find(patient.PatientId) == null)
                {
                    try
                    {
                        db.Patients.Add(patient);
                        db.SaveChanges();
                    }
                    catch (DbUpdateException ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                    }
                }

                var doctor = db.Doctors.FirstOrDefault(x => x.DoctorId == model.DoctorId); // kiểm tra xem có doctor trả về không
                var faculty = db.Faculties.FirstOrDefault(x => x.FacultyId == model.FacultyId); // kiểm tra có kết quả trả về không
                if (doctor == null)
                {
                    if(faculty == null) ModelState.AddModelError("", "Khoa không tồn tại."); // không có khoa, bác sĩ trả về

                    ViewBag.Faculties = new SelectList(db.Faculties.ToList(), "FacultyId", "FacultyName");
                    ViewBag.Doctors = new SelectList(db.Doctors
                    .Select(d => new
                    {
                        d.DoctorId,
                        FullName = $"{d.DoctorDegree} - {d.DoctorName}"  // Nối chuỗi ở đây
                    }).ToList(), "DoctorId", "FullName");
                    ViewBag.Services = new SelectList(db.Services.ToList(), "ServiceId", "ServiceName");

                    ModelState.AddModelError("", "Bác sĩ không tồn tại.");
                    return View(model); // Trả về view với thông báo lỗi
                }
                int serviceId = Convert.ToInt32(doctor.ServiceId);
                var appoinment = new Appointment
                {
                    AppointmentDate = model.AppointmentDate,
                    DoctorId = model.DoctorId,
                    PatientId = model.PatientId,
                    ServiceId = model.ServiceId,
                    AppointmentStatus = "Pending", // đang chờ


                };
                db.Appointments.Add(appoinment);
                try
                {
                    db.Appointments.Add(appoinment);
                    db.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                }
                return RedirectToAction("Index"); // sau khi create thì trả về index danh sách bác sĩ,
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            return View(model);
        }

    }
}
