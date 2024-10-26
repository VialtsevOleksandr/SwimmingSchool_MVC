using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SwimmingSchool_MVC.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SwimmingSchool_MVC.Controllers
{
    public class EventsController : Controller
    {
        private readonly DbSwimmingSchoolContext _context;
        private readonly IMemoryCache _cache;

        public EventsController(DbSwimmingSchoolContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // Клас для десеріалізації відповіді Nominatim
        private class NominatimResponse
        {
            public string lat { get; set; }
            public string lon { get; set; }
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var events = await _context.Events.ToListAsync();
            var eventCoordinates = new Dictionary<int, (string lat, string lng)>();

            foreach (var @event in events)
            {
                if (!_cache.TryGetValue($"EventCoordinates_{@event.Id}", out (string lat, string lng) coordinates))
                {
                    // Використовуємо сервіс Nominatim для отримання координат за адресою
                    var geocoderUrl = $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(@event.Locations)}&viewbox=30.2394,50.5900,30.8250,50.2133&bounded=1";

                    using (var httpClient = new HttpClient())
                    {
                        // Додаємо заголовок User-Agent
                        httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SwimmingSchoolApp", "1.0"));

                        var response = await httpClient.GetStringAsync(geocoderUrl);
                        var data = JsonSerializer.Deserialize<List<NominatimResponse>>(response);

                        if (data != null && data.Any())
                        {
                            coordinates = (data[0].lat, data[0].lon);
                            _cache.Set($"EventCoordinates_{@event.Id}", coordinates, TimeSpan.FromDays(1));
                        }
                    }
                }

                eventCoordinates[@event.Id] = coordinates;
            }

            ViewBag.EventCoordinates = eventCoordinates.ToDictionary(e => e.Key.ToString(), e => new { e.Value.lat, e.Value.lng });
            return View(events);
        }

        [HttpPost]
        public IActionResult CheckEvent()
        {
            var events = _context.Events.Where(e => !e.IsHeld && e.Date <= DateTime.Now);
            foreach (var @event in events)
            {
                @event.IsHeld = true;
            }
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetLogo(int id)
        {
            var @event = _context.Events.Find(id);

            if (@event == null || @event.Logo == null)
            {
                return NotFound();
            }

            return File(@event.Logo, "image/png");
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.PupilsEvents)
                    .ThenInclude(pe => pe.Pupils)
                    .ThenInclude(p => p.Group)
                    .ThenInclude(g => g.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Date,MaxPupilsAmount,Locations,Description,Decree")] Event @event, IFormFile? Logo)
        {
            if (ModelState.IsValid)
            {
                if (Logo != null && Logo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await Logo.CopyToAsync(memoryStream);
                        @event.Logo = memoryStream.ToArray();
                    }
                }
                if (@event.MaxPupilsAmount <= 0)
                {
                    ModelState.AddModelError("MaxPupilsAmount", "Кількість учасників повинна бути більше 0");
                    return View(@event);
                }
                if (@event.Date < DateTime.Now)
                {
                    ModelState.AddModelError("Date", "Дата не може бути в минулому");
                    return View(@event);
                }
                @event.IsHeld = false;
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Date,MaxPupilsAmount,Locations,Description,Decree,IsHeld,Logo")] Event @event, IFormFile? Logo)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (Logo != null && Logo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await Logo.CopyToAsync(memoryStream);
                        @event.Logo = memoryStream.ToArray();
                    }
                }
                else
                {
                    // Зберігаємо існуючий логотип, якщо новий не завантажено
                    _context.Entry(@event).Property(e => e.Logo).IsModified = false;
                }

                if (@event.MaxPupilsAmount <= 0)
                {
                    ModelState.AddModelError("MaxPupilsAmount", "Кількість учасників повинна бути більше 0");
                    return View(@event);
                }
                if (@event.Date < DateTime.Now)
                {
                    ModelState.AddModelError("Date", "Дата не може бути в минулому");
                    return View(@event);
                }

                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                    // Очищаємо кеш для оновлення координат
                    _cache.Remove($"EventCoordinates_{@event.Id}");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }


        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            // Підрахунок кількості записів у PupilsEvents для цього івенту
            var relatedPupilsEventsCount = _context.PupilsEvents.Count(pe => pe.EventId == id);
            ViewBag.RelatedPupilsEventsCount = relatedPupilsEventsCount;

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                // Видалення записів з PupilsEvents
                var relatedPupilsEvents = _context.PupilsEvents.Where(pe => pe.EventId == id);
                _context.PupilsEvents.RemoveRange(relatedPupilsEvents);

                // Видалення самого івенту
                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
                // Очищаємо кеш
                _cache.Remove($"EventCoordinates_{@event.Id}");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }

        public IActionResult TrainerPupilStats(int eventId)
        {
            var data = _context.Trainers
                .Where(t => t.Groups.SelectMany(g => g.Pupils)
                                    .SelectMany(p => p.PupilsEvents)
                                    .Any(pe => pe.EventId == eventId))
                .Select(t => new
                {
                    TrainerName = t.FirstName + " " + t.LastName + " " + t.MiddleName,
                    PupilCount = t.Groups.SelectMany(g => g.Pupils)
                                        .SelectMany(p => p.PupilsEvents)
                                        .Count(pe => pe.EventId == eventId)
                })
                .ToList();

            return Json(data);
        }
    }
}
