// Services/ReviewService.cs
using AutoMapper;
using HealthCare.Data;
using HeathCare.DTOs;
using HeathCare.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeathCare.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDTO>> GetReviewsByDoctorAsync(int doctorId);
        Task<ReviewDTO> GetReviewByIdAsync(int id);
        Task<ReviewDTO> CreateReviewAsync(ReviewCreateDTO reviewDto);
        Task<ReviewDTO> UpdateReviewAsync(int id, ReviewUpdateDTO reviewDto);
        Task DeleteReviewAsync(int id);
    }
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ReviewService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsByDoctorAsync(int doctorId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.Doctor)
                .Where(r => r.DoctorId == doctorId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
        }

        public async Task<ReviewDTO> GetReviewByIdAsync(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Doctor)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null) return null;

            return _mapper.Map<ReviewDTO>(review);
        }

        public async Task<ReviewDTO> CreateReviewAsync(ReviewCreateDTO reviewDto)
        {
            // Verify doctor exists
            var doctor = await _context.Doctors.FindAsync(reviewDto.DoctorId);
            if (doctor == null)
            {
                throw new ArgumentException("Doctor not found");
            }

            var review = _mapper.Map<Review>(reviewDto);
            review.CreatedAt = DateTime.UtcNow;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // Load doctor name for the response
            await _context.Entry(review)
                .Reference(r => r.Doctor)
                .LoadAsync();

            return _mapper.Map<ReviewDTO>(review);
        }

        public async Task<ReviewDTO> UpdateReviewAsync(int id, ReviewUpdateDTO reviewDto)
        {
            var review = await _context.Reviews
                .Include(r => r.Doctor)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
            {
                throw new KeyNotFoundException("Review not found");
            }

            _mapper.Map(reviewDto, review);
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();

            return _mapper.Map<ReviewDTO>(review);
        }

        public async Task DeleteReviewAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                throw new KeyNotFoundException("Review not found");
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}