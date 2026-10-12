using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySach.Models;
using QuanLySach.Data;

public class BooksController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public BooksController(
        ApplicationDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    // =========================
    // GET: /Books
    // =========================
    public async Task<IActionResult> Index()
    {
        return View(await _context.Books.ToListAsync());
    }

    // =========================
    // GET: /Books/Details/5
    // =========================
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .FirstOrDefaultAsync(m => m.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // =========================
    // GET: /Books/Create
    // =========================
    public IActionResult Create()
    {
        return View();
    }

    // =========================
    // POST: /Books/Create
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Book book, IFormFile? image)
    {
        if (image == null || image.Length == 0)
        {
            ModelState.AddModelError("image", "Vui lòng chọn hình ảnh cho sách.");
        }
        else ValidateImage(image);

        if (!ModelState.IsValid)
        {
            return View(book);
        }

        book.ImageUrl = await SaveImageAsync(image!);
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // =========================
    // GET: /Books/Edit/5
    // =========================
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // =========================
    // POST: /Books/Edit/5
    // =========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int? id,
        [Bind("Id,Title,Author,Publisher,Price,PublishedYear")]
        Book book,
        IFormFile? image)
    {
        if (id != book.Id)
        {
            return NotFound();
        }

        var existingBook = await _context.Books.AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == book.Id);
        if (existingBook == null)
        {
            return NotFound();
        }

        if (image is { Length: > 0 }) ValidateImage(image);

        if (ModelState.IsValid)
        {
            try
            {
                book.ImageUrl = image is { Length: > 0 }
                    ? await SaveImageAsync(image)
                    : existingBook.ImageUrl;
                _context.Update(book);
                await _context.SaveChangesAsync();

                if (image is { Length: > 0 } && !string.IsNullOrEmpty(existingBook.ImageUrl))
                    DeleteImage(existingBook.ImageUrl);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(book);
    }

    private void ValidateImage(IFormFile image)
    {
        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
        if (extension is not ".jpg" and not ".jpeg" and not ".png")
            ModelState.AddModelError("image", "Chỉ được tải ảnh JPG, JPEG hoặc PNG.");

        if (image.Length > 5 * 1024 * 1024)
            ModelState.AddModelError("image", "Kích thước ảnh không được vượt quá 5MB.");

        if (image.Length > 0 && !HasValidImageSignature(image, extension))
            ModelState.AddModelError("image", "Nội dung file không phải ảnh JPG hoặc PNG hợp lệ.");
    }

    private static bool HasValidImageSignature(IFormFile image, string extension)
    {
        Span<byte> header = stackalloc byte[8];
        using var stream = image.OpenReadStream();
        var read = stream.Read(header);
        return extension switch
        {
            ".jpg" or ".jpeg" => read >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            ".png" => read >= 8 && header.SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
            _ => false
        };
    }

    private async Task<string> SaveImageAsync(IFormFile image)
    {
        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var uploadFolder = Path.Combine(_environment.WebRootPath, "images", "books");
        Directory.CreateDirectory(uploadFolder);
        await using var stream = new FileStream(Path.Combine(uploadFolder, fileName), FileMode.CreateNew);
        await image.CopyToAsync(stream);
        return $"/images/books/{fileName}";
    }

    private void DeleteImage(string imageUrl)
    {
        var fileName = Path.GetFileName(imageUrl);
        var filePath = Path.Combine(_environment.WebRootPath, "images", "books", fileName);
        if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
    }

    // =========================
    // GET: /Books/Delete/5
    // =========================
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .FirstOrDefaultAsync(m => m.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // =========================
    // POST: /Books/Delete/5
    // =========================
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book != null)
        {
            _context.Books.Remove(book);
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // =========================
    // Kiểm tra sách tồn tại
    // =========================
    private bool BookExists(int? id)
    {
        return _context.Books.Any(e => e.Id == id);
    }
}
