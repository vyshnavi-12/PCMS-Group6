using PCMS_Backend.DTOs;
using PCMS_Backend.Interfaces.Repositories;
using PCMS_Backend.Interfaces.Services;
using PCMS_Backend.Shared;

namespace PCMS_Backend.Services;

public class SupervisorService : ISupervisorService
{
    private readonly ISupervisorRepository _supervisorRepo;

    public SupervisorService(ISupervisorRepository supervisorRepo)
    {
        _supervisorRepo = supervisorRepo;
    }

 
}