using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Task.Extentions;

using Task.Models;

namespace Task.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _web;

        public SliderController(AppDbContext context,IWebHostEnvironment web)
        {
            _context = context;
            _web = web;
        }
        public IActionResult Index()
        {
            List<Slider> sliders = _context.sliders.ToList();
            return View(sliders);
        }
        public IActionResult Create()
        {
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> Create(Slider slider)
        {
            if (!ModelState.IsValid)
            {
                return View(slider);
            }
            bool IsExist = _context.sliders.Any(s => s.Name.ToLower().Trim().Contains(slider.Name.ToLower().Trim()));
            if(IsExist)
            {
                ModelState.AddModelError("Name", "Adlar eyni ola bilmez");
                return View();
            }
            if(!slider.Photo.CheckTypeImage("/image"))
            {
                ModelState.AddModelError("Photo", "Image taypinde deyil!");
                return View();
            }
            if (slider.Photo.CheckImageLength(3))
            {
                ModelState.AddModelError("Photo", "Size 3 mbni Kecir !");
                return View();
            }
            slider.Url = await slider.Photo.SaveFile(Path.Combine(_web.WebRootPath, "admin", "images", "carousel"));
            await _context.AddAsync(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Update(int id)
        {
            Slider slider = _context.sliders.Find(id);
            if (slider==null)
            {
                return NotFound();
            }
            return View(slider);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Slider slider, int id)
        {
            if (slider.Id != id)
            {
                return BadRequest();
            }
            Slider sldr = _context.sliders.Find(slider.Id);
             if (sldr == null)
            {
                return NotFound();
            }
            if (!slider.Photo.CheckTypeImage("/image"))
            {
                ModelState.AddModelError("formFile", "Format düzgün deyil !");
                return View();
            }
            if (slider.Photo.CheckImageLength(5))
            {
                ModelState.AddModelError("formFile", "Fayl həcmi 5 mbnı keçir !");
                return View();
            }
            slider.Url = await slider.Photo.SaveFile(Path.Combine(_web.WebRootPath, "admin", "images", "carousel"));
            sldr.Name = slider.Name;
            sldr.Description = slider.Description;
            sldr.Position = slider.Position;
            sldr.Url = slider.Url;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
  

        }
        public IActionResult Delete(int id)
        {
            Slider slider = _context.sliders.Find(id);
            if (slider == null) return NotFound();
            Extention.Delete(Path.Combine(_web.WebRootPath, "admin", "images", "carousel", slider.Url));
            _context.sliders.Remove(slider);
             _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
