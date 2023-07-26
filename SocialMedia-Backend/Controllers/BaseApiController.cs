using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SocialMedia_Backend.Impl;

namespace SocialMedia_Backend.Controllers;

public class BaseApiController : ControllerBase
{
    protected readonly IRepositoryManager _repository;
    //protected readonly ILoggerManager _logger;
    protected readonly IMapper _mapper;

    public BaseApiController(IRepositoryManager repository,  IMapper mapper)
    {
        _repository = repository;
        //_logger = logger;
        _mapper = mapper;
    }
}
