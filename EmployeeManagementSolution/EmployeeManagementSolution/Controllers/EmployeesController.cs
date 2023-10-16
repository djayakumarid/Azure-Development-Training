using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeManagement.SharedLibrary;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Newtonsoft.Json;
using System.Text;

// using EmployeeManagement.Web.Data;

namespace EmployeeManagementSolution.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeDataContext _context;
        private readonly IConfiguration _config;

        public EmployeesController(EmployeeDataContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
              return _context.Employees != null ? 
                          View(await _context.Employees.ToListAsync()) :
                          Problem("Entity set 'EmployeeDataContext.Employee'  is null.");
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmpName,Salary,ImageURL,ThumbnailURL")] Employee employee, IFormFile imageURL)
        {
            if (ModelState.IsValid)
            {
                //Code to Upload the image into Blob Container.
                String? cs = _config["StorageConnectionString"];
                BlobServiceClient blobServiceClient = new BlobServiceClient(cs);
                BlobContainerClient blobClient = blobServiceClient.GetBlobContainerClient("empimages");
                blobClient.CreateIfNotExists(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
                string blobName = Guid.NewGuid().ToString() + Path.GetExtension(imageURL.FileName);
                var blob = await blobClient.UploadBlobAsync(blobName, imageURL.OpenReadStream());

                //Inserted the Record in database
                employee.ImageURL = blobClient.Uri.ToString() + "/" + blobName;
                _context.Add(employee);
                await _context.SaveChangesAsync();

                //Posting message to Queue.
                QueueClient queueClient = new QueueClient(cs, "thumbnail-queue");
                queueClient.CreateIfNotExists();
                BlobInformation blobInformation = new BlobInformation();
                blobInformation.BlobUri = new Uri(blobClient.Uri.ToString() + "/" + blobName);
                blobInformation.EmpId = employee.Id;
                string blobString = ToBase64(blobInformation);
                Console.WriteLine(blobString);
                await queueClient.SendMessageAsync(blobString);
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }
        public string ToBase64(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            byte[] bytes = Encoding.Default.GetBytes(json);
            return Convert.ToBase64String(bytes);
        }


        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmpName,Salary,ImageURL,ThumbnailURL")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'EmployeeDataContext.Employee'  is null.");
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
          return (_context.Employees?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
