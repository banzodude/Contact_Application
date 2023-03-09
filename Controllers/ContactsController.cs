using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using testing.Data;
using testing.Models;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using CsvHelper;
using System.Globalization;

namespace testing.Controllers
{
    [Authorize]
    
    public class ContactsController : Controller
    {
        private readonly testingContext _context;
      
        public ContactsController(testingContext context)
        {
            _context = context;
      
        }

        // GET: Contacts
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Contacts.ToListAsync());
        }
        private List<Contacts> lines = new List<Contacts>();



        public IActionResult Import(IFormFile file)
        {
            string fileName= $"{Directory.GetCurrentDirectory()}{@"\wwwroot"}" + "\\"+file.FileName;
            using (FileStream fileStream = System.IO.File.Create(fileName))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }

            var contacts = GetContactList(fileName);
            var newlist = new List<Contacts>();
            if (contacts!=null)
            {
                foreach(var con in contacts)
                {
                    Contacts obj = new Contacts();
                    obj.name = con.name;
                    obj.surname = con.surname;
                    obj.email = con.email;
                    obj.client = "imported";
                    obj.company = con.company;
                    _context.Add(obj);
                     _context.SaveChangesAsync();
                }
            }
            

            return View("Index", _context.Contacts.ToList());
           

            
        }
        private List<Contacts> GetContactList(string fileName)
        {
            List<Contacts> contactslist = new List<Contacts>();
            var path =fileName;
            using (var reader=new StreamReader(path))
            using (var csv=new CsvReader(reader,CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var contact = csv.GetRecord<Contacts>();
                    contactslist.Add(contact);
                }
            }

            return contactslist;
        }

       
        public IActionResult Export(int? id)
        {
            string[] columns = new string[] { "ID", "name", "surname", "email", "client", "company" };
            string csv = string.Empty;
            if (id != null)
            {
                var foundItem = _context.Contacts.Find(id);
                foreach (string s in columns)
                {
                    csv += s + ",";
                }
                csv += "\r\n";               
                csv += foundItem.ID.ToString().Replace(",", ";") + ",";
                csv += foundItem.name.Replace(",", ";") + ",";
                csv += foundItem.surname.Replace(",", ";") + ",";
                csv += foundItem.email.Replace(",", ";") + ",";
                csv += foundItem.client.Replace(",", ";") + ",";
                csv += foundItem.company.Replace(",", ";") + ",";
                csv += "\r\n";               
            }
            else
            {
                var myContact = _context.Contacts.FromSqlInterpolated($"select * from Contacts").ToList();              
                foreach (string s in columns)
                {
                    csv += s + ",";
                }
                csv += "\r\n";
                foreach (Contacts c in myContact)
                {
                    csv += c.ID.ToString().Replace(",", ";") + ",";
                    csv += c.name.Replace(",", ";") + ",";
                    csv += c.surname.Replace(",", ";") + ",";
                    csv += c.email.Replace(",", ";") + ",";
                    csv += c.client.Replace(",", ";") + ",";
                    csv += c.company.Replace(",", ";") + ",";
                    csv += "\r\n";
                }
            }
                byte[] bytes = Encoding.ASCII.GetBytes(csv);
                var countItems = _context.exportData.FromSqlInterpolated($"Select * from exportData ").Count();
                var filename = "contacts" + countItems + ".csv";
                FileResult ff = File(bytes, "text/csv", filename);

                if (ff != null)
                {
                    var exp = _context.Database.ExecuteSqlInterpolated($"INSERT into exportData (name,filename,exportnum) VALUES ({User.Identity.Name},{filename},{0})");
                }

                return ff;
            
        }

        // GET: Contacts/showSearchForm
        [Authorize]
        public async Task<IActionResult> showSearchForm()
        {
            return View("showSearchForm");
        }

        // POST: Contacts/showSearchResults
        public async Task<IActionResult> showSearchResults(String searchPhrase)
        {
            return View("Index", await _context.Contacts.Where(i => i.name.Contains(searchPhrase)).ToListAsync());
        }

        // GET: Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contacts = await _context.Contacts
                .FirstOrDefaultAsync(m => m.ID == id);
            if (contacts == null)
            {
                return NotFound();
            }

            return View(contacts);
        }

        // GET: Contacts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,name,surname,email,company")] Contacts contacts)
        {
            contacts.client = User.Identity.Name;
            if (ModelState.IsValid)
            {
                _context.Add(contacts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contacts);
        }

        public async Task<IActionResult> toImport()
        {
            var cl =await _context.Contacts.ToListAsync();
            return  View("import");
        }

        // GET: Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contacts = await _context.Contacts.FindAsync(id);
            if (contacts == null)
            {
                return NotFound();
            }
            return View(contacts);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,name,surname,email,client,company")] Contacts contacts)
        {
            if (id != contacts.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contacts);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactsExists(contacts.ID))
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
            return View(contacts);
        }

        // GET: Contacts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contacts = await _context.Contacts
                .FirstOrDefaultAsync(m => m.ID == id);
            if (contacts == null)
            {
                return NotFound();
            }

            return View(contacts);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contacts = await _context.Contacts.FindAsync(id);
            _context.Contacts.Remove(contacts);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactsExists(int id)
        {
            return _context.Contacts.Any(e => e.ID == id);
        }
    }
}
