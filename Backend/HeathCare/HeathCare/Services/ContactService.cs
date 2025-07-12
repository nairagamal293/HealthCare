using AutoMapper;
using HealthCare.Data;
using HeathCare.DTOs;
using HeathCare.Models;
using Microsoft.EntityFrameworkCore;

namespace HeathCare.Services
{
    // Services/ContactService.cs
    public class ContactService : IContactService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ContactService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ContactDTO>> GetAllContactsAsync()
        {
            var contacts = await _context.Contacts
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ContactDTO>>(contacts);
        }

        public async Task<ContactDTO> CreateContactAsync(ContactCreateDTO contactDto)
        {
            // Create contact entity manually to ensure proper initialization
            var contact = new Contact
            {
                Name = contactDto.Name,
                Email = contactDto.Email,
                Message = contactDto.Message,
                CreatedAt = DateTime.UtcNow
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return _mapper.Map<ContactDTO>(contact);
        }
    }

    public interface IContactService
    {
        Task<IEnumerable<ContactDTO>> GetAllContactsAsync();
        Task<ContactDTO> CreateContactAsync(ContactCreateDTO contactDto);
    }
}
