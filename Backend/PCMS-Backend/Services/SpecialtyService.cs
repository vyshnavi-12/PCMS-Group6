using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Services;

public class SpecialtyService : ISpecialtyService
{
    private readonly ISpecialtyRepository _specialtyRepository;

    public SpecialtyService(ISpecialtyRepository specialtyRepository)
    {
        _specialtyRepository = specialtyRepository;
    }

    public async Task<Result<IReadOnlyList<SpecialtyDto>>> GetAllSpecialtiesAsync()
    {
        var specialties = await _specialtyRepository.GetAllAsync();

        var response = specialties
            .Select(s => new SpecialtyDto
            {
                SpecialtyId = s.SpecialtyId,
                SpecialtyName = s.SpecialtyName
            })
            .ToList();

        return Result<IReadOnlyList<SpecialtyDto>>.Ok(response);
    }
}