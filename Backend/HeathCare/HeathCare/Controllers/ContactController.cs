using HeathCare.DTOs;
using HeathCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeathCare.Controllers
{
    // Controllers/ContactController.cs
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ContactDTO>> GetContact(int id)
        {
            var contact = await _contactService.GetContactByIdAsync(id);
            if (contact == null) return NotFound();
            return Ok(contact);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ContactDTO>>> GetContacts()
        {
            var contacts = await _contactService.GetAllContactsAsync();
            return Ok(contacts);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkAsRead(int id, ContactUpdateDTO contactDto)
        {
            if (id != contactDto.Id) return BadRequest();

            try
            {
                await _contactService.MarkAsReadAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("unread-count")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var count = await _contactService.GetUnreadCountAsync();
            return Ok(count);
        }


        [HttpPost]
        public async Task<ActionResult<ContactDTO>> PostContact(ContactCreateDTO contactDto)
        {
            var contact = await _contactService.CreateContactAsync(contactDto);
            return CreatedAtAction(nameof(GetContacts), new { id = contact.Id }, contact);
        }
    }
}
