using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Application.DTOs.Service;
using TouRest.Application.Interfaces;
using TouRest.Domain.Entities;
using TouRest.Domain.Interfaces;

namespace TouRest.Application.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IMapper _mapper;
        public ServiceService(IServiceRepository serviceRepository, IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
        }
        public async Task<ServiceDTO?> GetServiceById(Guid id)
        {
            return _mapper.Map<ServiceDTO?>(await _serviceRepository.GetByIdAsync(id));
        }
        public async Task<IEnumerable<ServiceDTO>> GetAllServices()
        {
            return _mapper.Map<IEnumerable<ServiceDTO>>(await _serviceRepository.GetAllAsync());
        }

        public async Task<IEnumerable<ServiceDTO>> GetServicesByProviderId(Guid providerId)
        {
            return _mapper.Map<IEnumerable<ServiceDTO>>(await _serviceRepository.GetByProviderIdAsync(providerId));
        }
        public async Task<ServiceDTO> CreateService(ServiceCreateRequest service)
        {
            var serviceEntity = _mapper.Map<Service>(service);
            var create = await _serviceRepository.CreateAsync(serviceEntity);
            return _mapper.Map<ServiceDTO>(create);
        }
        public async Task<ServiceDTO?> UpdateService(Guid id, ServiceUpdateRequest updatedService)
        {
            var existingService = await _serviceRepository.GetByIdAsync(id);
            if (existingService == null)
            {
                return null;
            }
            _mapper.Map(updatedService, existingService);
            await _serviceRepository.UpdateAsync(existingService);
            return _mapper.Map<ServiceDTO>(existingService);
        }
        public async Task<bool> DeleteService(Guid id)
        {
            var existingService = await _serviceRepository.GetByIdAsync(id);
            if (existingService == null)
            {
                return false;
            }
            await _serviceRepository.DeleteAsync(id);
            return true;
        }
        
    }
    }
